angular.module("umbraco").controller("UIOMatic.FieldEditors.Radiobuttonlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.objects = response;
            });
        }

        init();

        $scope.$on('valuesLoaded', function () {
            init();
        });

        $scope.setValue = function (val) {

            $scope.property.value = val;
        }

        $scope.parseTemplate = function (object) {
            return $interpolate($scope.property.config.textTemplate)(object);
        }
    });