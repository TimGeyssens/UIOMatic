angular.module("umbraco").controller("UIOMatic.FieldEditors.Label",
    function ($scope, $interpolate) {

        function init() {
            if ($scope.property.config && $scope.property.config.format) {
                $scope.labelValue = $interpolate($scope.property.config.format)({ value: $scope.property.value });
            } else {
                $scope.labelValue = $scope.property.value;
            }
        }

        var appScope = $scope;
        while (typeof appScope === 'object' && typeof appScope.activeApp === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded || $scope.property.view.indexOf("fieldviews") > -1) {
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }
        
    });