angular.module("umbraco").controller("uioMatic.ObjectListController",
    function($scope, $routeParams, uioMaticObjectResource) {

        $scope.typeAlias = $routeParams.id;
        $scope.selectedIds = [];
        $scope.actionInProgress = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.totalPages = 1;
        
        $scope.reverse = false;

        $scope.searchTerm = "";

        $scope.initialFetch = true;

        function fetchData() {
            uioMaticObjectResource.getPaged($scope.typeAlias, $scope.itemsPerPage, $scope.currentPage, $scope.initialFetch ? "" : $scope.predicate, $scope.initialFetch ? "" : ($scope.reverse ? "desc" : "asc"), $scope.searchTerm).then(function (resp) {
                $scope.rows = resp.data.Items;
                $scope.totalPages = resp.data.TotalPages;
            });
        }

        uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
            //.replace(' ', '_') nasty hack to allow columns with a space
            $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.properties = response.data.ListViewProperties;
            $scope.nameField = response.data.NameFieldKey.replace(' ', '_');
            $scope.readOnly = response.data.ReadOnly;
            fetchData();
        });


        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
            $scope.currentPage = 1;
            $scope.initialFetch = false;
            fetchData();
        };

        $scope.getObjectKey = function (object) {
            return object[$scope.primaryKeyColumnName];
        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.deleteByIds($routeParams.id, $scope.selectedIds).then(function() {
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
            $scope.currentPage = pageNumber;
            fetchData();
        };

        $scope.search = function(searchFilter) {
            $scope.searchTerm = searchFilter;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.isColumnLinkable = function (prop, index) {
            if ($scope.nameField.length > 0) {
                return prop.Key == $scope.nameField;
            } else {
                return index == 0 || (index == 1 && prop.Key == $scope.primaryKeyColumnName);
            }
        }

        $scope.unCamelCase = function(str)
        {
            return str
                // insert a space between lower & upper
                .replace(/([a-z])([A-Z])/g, '$1 $2')
                // space before last upper in a sequence followed by lower
                .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
                // uppercase the first character
                .replace(/^./, function(str){ return str.toUpperCase(); })
        }
    });