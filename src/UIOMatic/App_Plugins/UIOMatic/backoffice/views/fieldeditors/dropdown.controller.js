angular.module("umbraco").controller("UIOMatic.FieldEditors.Dropdown",
    function ($rootScope, $scope, editorState, $interpolate, uioMaticObjectResource) {

        function init() {
            load();
            $rootScope.$broadcast('dropdownChanged', $scope.property);
        }

        function load(value) {

            if ($scope.property.config.foreignKeyColumn) {
                // foreignKey Filter
                var filterValue = (value || $scope.$parent.object[$scope.property.config.foreignKeyValueAlias]);
                if (filterValue) {
                    uioMaticObjectResource.getPaged($scope.property.config.typeAlias,
                        1000,
                        1,
                        $scope.property.config.sortColumn,
                        "asc",
                        $scope.property.config.foreignKeyColumn +
                        "|" +
                        filterValue,
                        "").then(function (response) {
                            $scope.items = response.items.map(function (itm) {
                                return {
                                    value: itm[$scope.property.config.valueColumn],
                                    text: $interpolate($scope.property.config.textTemplate)(itm)
                                }
                            });
                        });
                } else {
                    $scope.items = [];
                }
            } else {
                // All
                uioMaticObjectResource
                    .getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(
                        function (response) {
                            $scope.items = response.map(function (itm) {
                                return {
                                    value: itm[$scope.property.config.valueColumn],
                                    text: $interpolate($scope.property.config.textTemplate)(itm)
                                }
                            });
                        });
            }
        }

        // TODO: There has to be a better way to do this....
        var unregisterEventListner = $rootScope.$on('dropdownChanged', function (event, data) {
            if (data.key === $scope.property.config.foreignKeyValueAlias) {
                load(data.value);
            }
        });

        $scope.onChange = function () {
            $rootScope.$broadcast('dropdownChanged', $scope.property);
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