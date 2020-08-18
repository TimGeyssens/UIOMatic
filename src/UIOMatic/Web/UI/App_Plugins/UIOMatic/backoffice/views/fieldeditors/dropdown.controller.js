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

        if ($scope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = $scope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }

        // Unregister
        $scope.$on('$destroy', function () {
            unregisterEventListner();
        });
    });