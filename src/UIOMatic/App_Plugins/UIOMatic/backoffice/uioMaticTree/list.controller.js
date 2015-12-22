angular.module("umbraco").controller("uioMatic.ObjectListController",
    function ($scope, $routeParams, uioMaticObjectResource, dialogService) {

        $scope.typeName = $routeParams.id;
        $scope.selectedIds = [];
        $scope.actionInProgress = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.totalPages = 1;

        $scope.reverse = false;

        $scope.isFilterForm = false;

        $scope.searchTerm = "";

        $scope.displayname = "";

        $scope.Operators =
        [
            { id: 1, name: "=" },
            { id: 2, name: "≠" },
            { id: 3, name: ">" },
            { id: 4, name: "<" },
            { id: 5, name: "≥" },
            { id: 6, name: "≤" }
        ];

        function fetchData() {

            uioMaticObjectResource.getQuery($routeParams.id, $scope.filterproperties, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", $scope.searchTerm).then(function (resp) {

                $scope.rows = resp.data.Items;
                $scope.totalPages = resp.data.TotalPages;

                if ($scope.rows.length > 0) {
                    $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
                        return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
                    });
                }
            });
        }
        uioMaticObjectResource.getType($scope.typeName).then(function (response) {
            //.replace(' ', '_') nasty hack to allow columns with a space
            $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;
            $scope.nameField = response.data.NameField.replace(' ', '_');
            $scope.readOnly = response.data.ReadOnly;
            fetchData();
            uioMaticObjectResource.getFilterProperties($scope.typeName).then(function (response) {
                $scope.filterproperties = response.data;
                if ($scope.filterproperties.length > 0) {
                    $scope.isFilterForm = true;
                }
            });

        });


        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.getObjectKey = function (object) {
            var keyPropName = $scope.primaryKeyColumnName;
            return object[keyPropName];

        }

        $scope.openQueryDialog = function () {
            // open a custom dialog
            dialogService.open({
                // set the location of the view
                template: "/App_Plugins/UIOMatic/backoffice/views/query.html",
                // pass in data used in dialog
                dialogData: $scope.filterproperties,
                // function called when dialog is closed
                callback: function (value) {
                    fetchData();
                }
            });
        };

        $scope.filter = function (object) {
            fetchData();
            //if (confirm("Are you sure you want to query data?")) {
            //    uioMaticObjectResource.getQuery($routeParams.id, $scope.filterproperties, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc",$scope.searchTerm).then(function (resp) {
            //        $scope.rows = resp.data.Items;
            //        $scope.totalPages = resp.data.TotalPages;

            //        if ($scope.rows.length > 0) {
            //            $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
            //                return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
            //            });
            //        }

            //    });
            //}
        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.deleteByIds($routeParams.id, $scope.selectedIds).then(function () {
                    $scope.rows = _.reject($scope.rows, function (el) { return $scope.selectedIds.indexOf(el[keyPropName]) > -1; });
                    $scope.selectedIds = [];
                    $scope.actionInProgress = false;
                });
            }
        }

        $scope.toggleSelection = function (val) {
            var idx = $scope.selectedIds.indexOf(val);
            if (idx > -1) {
                $scope.selectedIds.splice(idx, 1);
            } else {
                $scope.selectedIds.push(val);
            }
        }

        $scope.isRowSelected = function (row) {
            var id = $scope.getObjectKey(row);
            return $scope.selectedIds.indexOf(id) > -1;
        }

        $scope.isAnythingSelected = function () {
            return $scope.selectedIds.length > 0;
        }

        $scope.getNumber = function (num) {
            return new Array(num);
        }

        $scope.prevPage = function () {
            if ($scope.currentPage > 1) {
                $scope.currentPage--;
                fetchData();
            }
        };

        $scope.nextPage = function () {
            if ($scope.currentPage < $scope.totalPages) {
                $scope.currentPage++;
                fetchData();
            }
        };

        $scope.setPage = function (pageNumber) {
            console.log(pageNumber);
            $scope.currentPage = pageNumber;
            fetchData();
        };

        $scope.search = function (searchFilter) {
            $scope.searchTerm = searchFilter;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.isColumnLinkable = function (column, index) {

            if ($scope.nameField.length > 0) {
                return column == $scope.nameField;
            } else {

                return index == 0
                || (index == 1 && $scope.cols[0] == $scope.primaryKeyColumnName)
            }
        }
    });