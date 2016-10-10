angular.module("umbraco").controller("UIOMatic.Views.Radiobuttonlist",
    function ($scope, uioMaticObjectResource,$parse) {

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.objects = response.data;
            });
        }

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });

        $scope.setValue = function (val) {

            $scope.property.value = val;
        }

        $scope.parseTemplate = function (object) {
            var template = $parse($scope.property.config.textTemplate);
            return template(object);
        }
    });