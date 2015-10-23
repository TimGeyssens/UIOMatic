angular.module("umbraco").controller("UIOMatic.Views.Radiobuttonlist",
    function ($scope, uioMaticObjectResource) {
        //example config
        //"{'typeName':'Example.Models.Person, Example', 'valueColumn': 'Id', 'textColumn'='FirstName'}"

       

        function init() {
            uioMaticObjectResource.getAll($scope.property.Config.typeName, $scope.property.Config.textColumn, "asc").then(function (response) {
                $scope.objects = response.data;

            });
        }

        init();

        $scope.$on('ValuesLoaded', function (event, data) {
            init();
        });

        $scope.setValue = function (val) {

            $scope.property.Value = val;
        }
    });