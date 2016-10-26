angular.module("umbraco").controller("UIOMatic.Views.Label",
    function ($scope, $interpolate) {

        function init() {
            if ($scope.property.config && $scope.property.config.format) {
                $scope.labelValue = $interpolate($scope.property.config.format)({ value: $scope.property.value });
            } else {
                $scope.labelValue = $scope.property.value;
            }
        }

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });
        
    });