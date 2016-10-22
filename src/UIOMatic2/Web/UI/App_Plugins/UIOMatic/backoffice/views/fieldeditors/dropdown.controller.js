angular.module("umbraco").controller("UIOMatic.Views.Dropdown",
    function ($scope, uioMaticObjectResource, $parse) {

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.objects = response;
            });
        }

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });

        $scope.parseTemplate = function (object) {
            var template = $parse($scope.property.config.textTemplate);
            return template(object);
        }

        
    });