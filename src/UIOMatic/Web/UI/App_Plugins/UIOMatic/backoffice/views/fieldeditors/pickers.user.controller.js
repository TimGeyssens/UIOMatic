angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.UserController",
    function ($scope, uioMaticFieldResource) {

        function init() {
            uioMaticFieldResource.getAllUsers().then(function (resp) {
                $scope.users = resp;
            });
        };

        var appScope = $scope;
        while (typeof appScope === 'object' && typeof appScope.activeApp === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }

    });