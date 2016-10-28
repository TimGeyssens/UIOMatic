angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.UserController",
    function ($scope, uioMaticFieldResource) {

        function init() {
            uioMaticFieldResource.getAllUsers().then(function (resp) {
                $scope.users = resp;
            });
        };

        if ($scope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = $scope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }

    });