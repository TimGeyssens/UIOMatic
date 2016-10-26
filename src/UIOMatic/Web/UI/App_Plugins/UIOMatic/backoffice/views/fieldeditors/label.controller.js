angular.module("umbraco").controller("UIOMatic.Views.Label",
    function ($scope) {

        function init() {
            if ($scope.property.config && $scope.property.config.format) {
                // TODO: Format value
                $scope.labelValue = $scope.property.value;
            } else {
                $scope.labelValue = $scope.property.value;
            }
        }

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });
        
    });