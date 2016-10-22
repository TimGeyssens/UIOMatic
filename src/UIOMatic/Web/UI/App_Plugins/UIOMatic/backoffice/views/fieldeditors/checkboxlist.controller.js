angular.module("umbraco").controller("UIOMatic.Views.Checkboxlist",
    function ($scope, uioMaticObjectResource, $parse) {

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

        $scope.$on('valuesLoaded', function (event, data) {
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
            var template = $parse($scope.property.config.textTemplate);
            return template(object);
        }
    });