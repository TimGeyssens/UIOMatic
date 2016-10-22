angular.module("umbraco").controller("UIOMatic.Views.Pickers.UserController",
    function ($scope, uioMaticFieldResource) {

        function init() {
            uioMaticFieldResource.getAllUsers().then(function (resp) {
                $scope.users = resp;
            });
        };

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });

    });