angular.module("umbraco").controller("UIOMatic.Views.Pickers.UserController",
    function ($scope, uioMaticFieldResource) {

        function init() {

            uioMaticFieldResource.getAllUsers().then(function (resp) {
                $scope.users = resp.data;
            });

        };

        init();

        $scope.$on('ValuesLoaded', function (event, data) {
            init();
        });

    });