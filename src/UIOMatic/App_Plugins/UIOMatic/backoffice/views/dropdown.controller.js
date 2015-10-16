angular.module("umbraco").controller("UIOMatic.Views.Dropdown",
    function ($scope, uioMaticObjectResource) {
        //example config
        //"{'typeName':'Example.Models.Person, Example', 'valueColumn': 'Id', 'textColumn'='FirstName'}"

        uioMaticObjectResource.getAll($scope.property.Config.typeName, $scope.property.Config.textColumn, "asc").then(function(response) {
            $scope.objects = response.data;
        });

       


});