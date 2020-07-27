angular.module("umbraco").controller("uioMatic.ObjectListController",
    function ($scope, $routeParams, $location, $timeout, uioMaticUtilityService, uioMaticObjectResource, navigationService, editorService) {

        var searchTimeout;

        $scope.dropdown = {};
        $scope.currentSection = $routeParams.section || 'uiomatic';
        $scope.dropdown.isOpen = false;

        $scope.typeAlias = $routeParams.id;
        $scope.selectedIds = [];
        $scope.actionInProgress = false;
        $scope.loading = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = JSON.parse(Umbraco.Sys.ServerVariables.uioMatic.settings.defaultListViewPageSize);
        $scope.totalPages = 1;
        $scope.totalItems = 0;

        $scope.legacyPagination = uioMaticUtilityService.useLegacyPaginationControl();

        $scope.reverse = false;

        $scope.filtersStr = "";
        $scope.searchTerm = "";

        $scope.initialFetch = true;

        function startFilterWatch() {
            $scope.$watch("filterProperties", function () {

                if (!$scope.filterProperties)
                    return;

                var str = buildFilterStr();
                if (str != $scope.filtersStr) {
                    $scope.filtersStr = str;
                    fetchData();
                }

            }, true);
        }

        function buildFilterStr() {
            return _.filter($scope.filterProperties, function (itm) { return itm.value }).map(function (itm) {
                return itm.keyColumnName + "|" + itm.value;
            }).join("|");
        }

        function fetchData(firstLoad) {
            
            $scope.loading = true;
            $scope.rows = 0;
            $scope.totalPages = 0;

            if (firstLoad) {
                saveState();

            } else {
                loadState();
            }

            uioMaticObjectResource.getPaged($scope.typeAlias, $scope.itemsPerPage, $scope.currentPage,
                $scope.initialFetch ? "" : $scope.predicate,
                $scope.initialFetch ? "" : ($scope.reverse ? "desc" : "asc"),
                $scope.filtersStr,
                $scope.searchTerm).then(function (resp) {
                    $scope.initialFetch = false;
                    $scope.rows = resp.items;
                    $scope.totalPages = resp.totalPages;
                    $scope.totalItems = resp.totalItems;
                    $scope.loading = false;
                });

          
        }

        function saveState() {
            localStorage.setItem('uioMatic.ObjectListController.' + $scope.typeAlias, JSON.stringify({
                itemsPerPage: $scope.itemsPerPage,
                currentPage: $scope.currentPage,
                searchTerm: $scope.searchTerm,
                predicate: $scope.predicate,
                reverse: $scope.reverse,
                filtersStr: $scope.filtersStr,
                initialFetch: $scope.initialFetch,
                filterProperties: $scope.filterProperties.map((fp) => {
                    return {
                        key: fp.key,
                        value: fp.value
                    };
                })
            }));
        }

        function loadState() {
            try {
                var state = JSON.parse(localStorage.getItem('uioMatic.ObjectListController.' + $scope.typeAlias));
                $scope.itemsPerPage = state.itemsPerPage;
                $scope.currentPage = state.currentPage;
                $scope.searchTerm = state.searchTerm;
                $scope.searchFilter = state.searchTerm;
                $scope.predicate = state.predicate;
                $scope.reverse = state.reverse;
                $scope.filtersStr = state.filtersStr;
                $scope.initialFetch = state.initialFetch;

                if (state.filterProperties) {
                    state.filterProperties.forEach(function (savedFp) {

                        var filter = $scope.filterProperties.find((fp) => {
                            return fp.key === savedFp.key
                        });

                        if (filter) {
                            filter.value = savedFp.value;
                        }
                    });
                }
            } catch (e) {
                console.warn("ui-o-matic list state restore failed => " + e);
              
            }
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
                if (itm.config && itm.config.defaultValue) {
                    itm.value = itm.config.defaultValue;
                }

                return itm;
            });

            // Build an initial filter string
            $scope.filtersStr = buildFilterStr();

            // Sync the tree
            navigationService.syncTree({ tree: 'uiomatic', path: response.path, forceReload: false, activate: true });

            fetchData(true);

            startFilterWatch();
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

        $scope.toggleSelectAll = function (event) {
            var doSelect = event.target.checked;
            $scope.rows.forEach(function (row) {
                var rowSelected = $scope.isRowSelected(row);
                if ((doSelect && !rowSelected) || (!doSelect && rowSelected)) {
                    $scope.toggleSelection($scope.getObjectKey(row));
                }
            });
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

        $scope.search = function (searchFilter) {
            if (searchTimeout) { // if there is already a timeout in process cancel it
                $timeout.cancel(searchTimeout);
            }
            searchTimeout = $timeout(function () {
                $scope.searchTerm = searchFilter;
                $scope.currentPage = 1;
                fetchData();
                searchTimeout = null;
            }, 1000);
        };

        $scope.isColumnLinkable = function (prop, index) {
            if ($scope.nameField.length > 0) {
                return prop.key == $scope.nameField;
            } else {
                return index == 0 || (index == 1 && prop.key == $scope.primaryKeyColumnName);
            }
        }

        $scope.unCamelCase = function (str) {
            return str
                // insert a space between lower & upper
                .replace(/([a-z])([A-Z])/g, '$1 $2')
                // space before last upper in a sequence followed by lower
                .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
                // uppercase the first character
                .replace(/^./, function (str) { return str.toUpperCase(); })
        }

        $scope.navigate = function (url) {
            // Because some JS seems to be translating any links starting '#'
            $location.url(url);
        }

        $scope.openAction = function (action) {
            editorService.open({
                view: action.view,
                dialogData: {
                    typeAlias: $scope.typeAlias,
                    config: action.config
                },
                submit: function () {
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        }
    });
