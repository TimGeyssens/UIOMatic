angular.module("umbraco").controller("UIOMatic.FieldEditors.Radiobuttonlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.items = response.map(function(itm) {
                    return {
                        value: itm[$scope.property.config.valueColumn],
                        text: $interpolate($scope.property.config.textTemplate)(itm)
                    }
                });
            });
        }

        $scope.setValue = function (val) {
            $scope.property.value = val;
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
    });