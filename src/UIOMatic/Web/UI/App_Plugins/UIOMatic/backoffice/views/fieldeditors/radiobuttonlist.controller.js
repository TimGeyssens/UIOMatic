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

        if ($scope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = $scope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }
    });