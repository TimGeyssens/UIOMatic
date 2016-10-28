angular.module("umbraco").controller("UIOMatic.FieldEditors.Label",
    function ($scope, $interpolate) {

        function init() {
            if ($scope.property.config && $scope.property.config.format) {
                $scope.labelValue = $interpolate($scope.property.config.format)({ value: $scope.property.value });
            } else {
                $scope.labelValue = $scope.property.value;
            }
        }

        if ($scope.valuesLoaded || $scope.property.view.indexOf("fieldviews") > -1) {
            init();
        } else {
            var unsubscribe = $scope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }
        
    });