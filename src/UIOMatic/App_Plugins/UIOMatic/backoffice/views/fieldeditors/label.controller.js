angular.module("umbraco").controller("UIOMatic.FieldEditors.Label",
    function ($scope, $interpolate) {

        function init() {
            if ($scope.model.config && $scope.model.config.format) {
                $scope.labelValue = $interpolate($scope.model.config.format)({ value: $scope.model.value });
            } else {
                $scope.labelValue = $scope.model.value;
            }
        }

        var appScope = $scope;
        while (appScope && typeof appScope === 'object' && typeof appScope.currentSection === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded || $scope.model.view.indexOf("fieldviews") > -1) {
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }
        
    });