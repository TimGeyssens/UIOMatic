var app = angular.module("umbraco");
app.requires.push('ngSanitize');
app.requires.push('ngCsv');
app.controller("uioMatic.ObjectListController",
    function ($scope, $routeParams, uioMaticObjectResource, dialogService) {

        $scope.typeName = encodeURI($routeParams.id);
        $scope.selectedIds = [];
        $scope.actionInProgress = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.totalPages = 1;
        $scope.object = {};

        $scope.reverse = false;

        $scope.isFilterForm = false;

        $scope.searchTerm = "";

        $scope.displayname = "";

        $scope.IsCanExport = false;

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
                    $scope.csvheader = Object.keys($scope.rows[0]).filter(function (c) {
                        return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
                    });
                    $scope.cols = $scope.properties.filter(function (c) {
                        return $scope.ignoreColumnsFromListView.indexOf(c.Key) == -1;
                    });
                }
            });
        }
        uioMaticObjectResource.getType($scope.typeName).then(function (response) {
            //.replace(' ', '_') nasty hack to allow columns with a space
            $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
            //$scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;
            $scope.nameField = response.data.NameField.replace(' ', '_');
            $scope.displayname = response.data.DisplayName;
            $scope.QueryTemplate = response.data.QueryTemplate;
            $scope.IsCanExport = response.data.IsCanExport;
            $scope.IsReadOnly = response.data.IsReadOnly;
            if ($scope.IsCanExport) {
                $scope.itemsPerPage = 40000;
            }

            uioMaticObjectResource.getAllProperties($scope.typeName, true).then(function (response) {
                $scope.properties = response.data;
            });
            uioMaticObjectResource.getFilterProperties($scope.typeName).then(function (response) {
                $scope.filterproperties = response.data;
                if ($scope.filterproperties.length > 0) {
                    $scope.isFilterForm = true;
                }
                fetchData();
            });
            

        });


        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate.Key) ? !$scope.reverse : false;
            $scope.predicate = predicate.Key;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.IsOrder = function (col) {
            return $scope.predicate === col.Key
        };

        $scope.getObjectKey = function (object) {
            var keyPropName = $scope.primaryKeyColumnName;
            return object[keyPropName];

        }

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
        $scope.openQueryDialog = function () {
            // open a custom dialog
            dialogService.open({
                // set the location of the view
                template: $scope.QueryTemplate,
                //"/App_Plugins/UIOMatic/backoffice/views/query.html",
                // pass in data used in dialog
                dialogData: $scope.filterproperties,
                // function called when dialog is closed
                callback: function (value) {
                        fetchData();
                }
            });
        };
        $scope.export = function (object) {
            if (confirm("Are you sure you want to export?")) {
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.exportcsv($routeParams.id, $scope.searchTerm).then(function () {
                    $scope.rows = _.reject($scope.rows, function (el) { return $scope.selectedIds.indexOf(el[keyPropName]) > -1; });

                });
            }
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
            if ($scope.IsReadOnly) {
                return false;
            }
            if ($scope.nameField.length > 0) {
                return column.Key == $scope.nameField;
            } else {

                return index == 0
                || (index == 1 && $scope.cols[0].Key == $scope.primaryKeyColumnName)
            }
        }

        $scope.isDateTime = function (column) {
            
                return column.Type.indexOf('DateTime') > 0;
        }

        var toUTCDate = function (date) {
            var _utc = new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
            return _utc;
        };

        var millisToUTCDate = function (millis) {
            return toUTCDate(new Date(millis));
        };

        $scope.toUTCDate = toUTCDate;
        $scope.millisToUTCDate = millisToUTCDate;
    })