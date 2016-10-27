angular.module("umbraco").controller("UIOMatic.FieldEditors.Checkboxlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        $scope.delimiter = ",";

        if ($scope.property.config.delimiter)
            $scope.delimiter = $scope.property.config.delimiter;

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.objects = response;

                angular.forEach($scope.objects, function (object) {
                    if ($scope.property.value && _.indexOf($scope.property.value.toString().split($scope.delimiter), object[$scope.property.config.valueColumn].toString()) > -1)
                        object.selected = true;
                    else
                        object.selected = false;

                });
            });
        }

        init();

        $scope.$on('valuesLoaded', function () {
            init();
        });

        $scope.setValue = function() {
            var val = [];
            angular.forEach($scope.objects, function (object) {
                if (object.selected)
                    val.push(object[$scope.property.config.valueColumn]);
            });

            $scope.property.value = val.join($scope.delimiter);
        }

        $scope.parseTemplate = function (object) {
            return $interpolate($scope.property.config.textTemplate)(object);
        }
    });