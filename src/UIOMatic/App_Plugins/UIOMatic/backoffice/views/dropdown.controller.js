angular.module("umbraco").controller("UIOMatic.Views.Dropdown",
    function ($scope, uioMaticObjectResource, $parse) {
        //example config
        //{'typeName': 'Example.Models.Person, Example', 'valueColumn': 'Id', 'sortColumn': 'FirstName', 'textTemplate' : 'FirstName + \" \"+ LastName '}

        function init() {
            uioMaticObjectResource.getAll($scope.property.Config.typeName, $scope.property.Config.sortColumn, "asc").then(function (response) {
                $scope.objects = response.data;
            });
        }

        init();

        $scope.$on('ValuesLoaded', function (event, data) {
            init();
        });

        $scope.parseTemplate = function (object) {
            var template = $parse($scope.property.Config.textTemplate);
            return template(object);
        }

        
    });