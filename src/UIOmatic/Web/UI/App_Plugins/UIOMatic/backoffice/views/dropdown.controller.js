angular.module("umbraco").controller("UIOMatic.Views.Dropdown",
    function ($scope, uioMaticObjectResource, $parse) {

        function init() {
            uioMaticObjectResource.getAll($scope.property.Config.typeAlias, $scope.property.Config.sortColumn, "asc").then(function (response) {
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