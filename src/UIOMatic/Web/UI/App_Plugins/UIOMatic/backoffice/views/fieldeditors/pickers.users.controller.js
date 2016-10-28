angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.UsersController",
    function ($scope, uioMaticFieldResource) {

        $scope.userIds = [];

        function init() {
            console.log("init");
            uioMaticFieldResource.getAllUsers().then(function (resp) {
                $scope.users = resp;
                $scope.selectedUsers = [];
                $scope.selectedUsers2 = [];
                $scope.users2 = [];

                var ids = $scope.property.value.split(",");

                angular.forEach(ids, function (value) {

                    var id = Number(value);
                    var user = _.findWhere($scope.users, { Id: id });

                    if (user != null) {

                        if (!_.contains($scope.users2, user)) {
                            $scope.users2.push(user);

                            var index = $scope.users.indexOf(user);
                            $scope.users.splice(index, 1);

                            $scope.userIds.push(user.Id);
                        }
                    }
                });
            });
        };

        uioMaticFieldResource.getAllUsers().then(function (resp) {
            $scope.users = resp;
            $scope.selectedUsers = [];
            $scope.selectedUsers2 = [];
            $scope.users2 = [];
        });

        $scope.removeUsersFromSelection = function () {
            angular.forEach($scope.selectedUsers2, function (value, key) {

                if (!_.contains($scope.users, value)) {
                    $scope.users.push(value);

                    var index = $scope.users2.indexOf(value);
                    $scope.users2.splice(index, 1);

                    index = $scope.userIds.indexOf(value.Id);
                    $scope.userIds.splice(index, 1);
                        
                    $scope.property.value = $scope.userIds.join();
                }

            });
        }
        $scope.addUsersToSelection = function () {
            angular.forEach($scope.selectedUsers, function (value, key) {

                if (!_.contains($scope.users2, value)) {
                    $scope.users2.push(value);

                    var index = $scope.users.indexOf(value);
                    $scope.users.splice(index, 1);

                    $scope.userIds.push(value.Id);

                    $scope.property.value = $scope.userIds.join();
                }

            });
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