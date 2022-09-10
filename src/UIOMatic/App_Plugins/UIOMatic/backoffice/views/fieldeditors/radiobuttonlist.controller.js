angular.module("umbraco").controller("UIOMatic.FieldEditors.Radiobuttonlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        function init() {
            uioMaticObjectResource.getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, "asc").then(function (response) {
                $scope.items = response.map(function(itm) {
                    return {
                        value: itm[$scope.model.config.valueColumn],
                        text: $interpolate($scope.model.config.textTemplate)(itm)
                    }
                });
            });
        }

        $scope.setValue = function (val) {
            $scope.model.value = val;
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