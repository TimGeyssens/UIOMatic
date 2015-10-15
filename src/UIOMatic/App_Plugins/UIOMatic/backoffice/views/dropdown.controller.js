angular.module("umbraco").controller("UIOMatic.Views.Dropdown",
    function ($scope, uioMaticObjectResource) {
        //example config
        //"{'typeName':'Example.Models.Person, Example', 'idColumn': 'Id', 'valueColumn'='FirstName'}"

        uioMaticObjectResource.getAll($scope.property.Config.typeName, $scope.property.Config.valueColumn, "asc").then(function(response) {
            $scope.objects = response.data;
        });

       


});