angular.module("umbraco").controller("uioMatic.ObjectListController",
    function ($scope, $routeParams, $location, uioMaticUtilityService, uioMaticObjectResource, navigationService, dialogService) {

        $scope.typeAlias = $routeParams.id;
        $scope.selectedIds = [];
        $scope.actionInProgress = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = JSON.parse(Umbraco.Sys.ServerVariables.uioMatic.settings.defaultListViewPageSize);
        $scope.totalPages = 1;

        $scope.legacyPagination = uioMaticUtilityService.useLegacyPaginationControl();
        
        $scope.reverse = false;

        $scope.filtersStr = "";
        $scope.searchTerm = ""; 

        $scope.initialFetch = true;

        function fetchData() {
            uioMaticObjectResource.getPaged($scope.typeAlias, $scope.itemsPerPage, $scope.currentPage,
                $scope.initialFetch ? "" : $scope.predicate,
                $scope.initialFetch ? "" : ($scope.reverse ? "desc" : "asc"),
                $scope.initialFetch ? "" : $scope.filtersStr,
                $scope.searchTerm).then(function (resp) {
                $scope.initialFetch = false;
                $scope.rows = resp.items;
                $scope.totalPages = resp.totalPages;
            });
        }

        uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
            $scope.title = response.displayNamePlural;
            $scope.primaryKeyColumnName = response.primaryKeyColumnName.replace(' ', '_');
            $scope.predicate = response.primaryKeyColumnName.replace(' ', '_');
            $scope.properties = response.listViewProperties;
            $scope.nameField = response.nameFieldKey.replace(' ', '_');
            $scope.readOnly = response.readOnly;
            $scope.listViewActions = response.listViewActions;
            $scope.predicate = response.sortColumn;
            $scope.reverse = response.sortOrder == "desc";
            // Pass extra meta data into filter properties
            $scope.filterProperties = response.listViewFilterProperties.map(function (itm) {
                itm.typeAlias = $scope.typeAlias;
                return itm;
            });

            // Sync the tree
            navigationService.syncTree({ tree: 'uiomatic', path: response.path, forceReload: false, activate: true });

            fetchData();
        });


        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
            $scope.currentPage = 1;
            fetchData();
        };

        $scope.getObjectKey = function (object) {
            return object[$scope.primaryKeyColumnName];
        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.deleteByIds($scope.typeAlias, $scope.selectedIds).then(function () {
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
                return prop.key == $scope.nameField;
            } else {
                return index == 0 || (index == 1 && prop.key == $scope.primaryKeyColumnName);
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

        $scope.navigate = function (url) {
            // Because some JS seems to be translating any links starting '#'
            $location.url(url);
        }

        $scope.openAction = function (action) {
            dialogService.open({
                template: action.view,
                show: true,
                dialogData: {
                    typeAlias: $scope.typeAlias
                }
            });
        }

        $scope.$watch("filterProperties", function() {

            if (!$scope.filterProperties)
                return;

            var str = _.filter($scope.filterProperties, function (itm) { return itm.value }).map(function (itm) {
                return itm.keyColumnName + "|" + itm.value;
            }).join("|");

            if (str != $scope.filtersStr) {
                $scope.filtersStr = str;
                fetchData();
            }

        }, true);
    });