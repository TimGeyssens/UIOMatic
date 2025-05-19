angular.module("umbraco").controller("UIOMatic.FieldEditors.Dropdown",
    function ($rootScope, $scope, editorState, $interpolate, uioMaticObjectResource) {

        function init() {
            load();
            $rootScope.$broadcast('dropdownChanged', $scope.model);
        }

        function load(value) {

            if ($scope.model.config.foreignKeyColumn) {
                // foreignKey Filter
                var filterValue = (value || $scope.$parent.object[$scope.model.config.foreignKeyValueAlias]);
                if (filterValue) {
                    uioMaticObjectResource.getPaged($scope.model.config.typeAlias,
                        1000,
                        1,
                        $scope.model.config.sortColumn,
                        "asc",
                        $scope.model.config.foreignKeyColumn +
                        "|" +
                        filterValue,
                        "").then(function (response) {
                            $scope.items = response.items.map(function (itm) {
                                return {
                                    value: itm[$scope.model.config.valueColumn],
                                    text: $interpolate($scope.model.config.textTemplate)(itm)
                                }
                            });
                        });
                } else {
                    $scope.items = [];
                }
            } else {
                // All
                uioMaticObjectResource
                    .getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, "asc").then(
                        function (response) {
                            $scope.items = response.map(function (itm) {
                                return {
                                    value: itm[$scope.model.config.valueColumn],
                                    text: $interpolate($scope.model.config.textTemplate)(itm)
                                }
                            });
                        });
            }
        }

        // TODO: There has to be a better way to do this....
        var unregisterEventListner = $rootScope.$on('dropdownChanged', function (event, data) {
            if (data.key === $scope.model.config.foreignKeyValueAlias) {
                load(data.value);
            }
        });

        $scope.onChange = function () {
            $rootScope.$broadcast('dropdownChanged', $scope.model);
        }

        var appScope = $scope;
        while (appScope && typeof appScope === 'object' && typeof appScope.currentSection === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }

        // Unregister
        $scope.$on('$destroy', function () {
            unregisterEventListner();
        });
    });