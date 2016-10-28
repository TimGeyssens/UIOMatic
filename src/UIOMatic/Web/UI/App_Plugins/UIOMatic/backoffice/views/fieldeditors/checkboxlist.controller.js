angular.module("umbraco").controller("UIOMatic.FieldEditors.Checkboxlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        $scope.delimiter = ",";

        if ($scope.property.config.delimiter)
            $scope.delimiter = $scope.property.config.delimiter;

        $scope.selectedValues = [];
        if ($scope.property.value)
            $scope.selectedValues = $scope.property.value.toString().split($scope.delimiter);

        function init() {
            uioMaticObjectResource.getAll($scope.property.config.typeAlias, $scope.property.config.sortColumn, "asc").then(function (response) {
                $scope.items = response.map(function(itm) {
                    var item = {
                        value: itm[$scope.property.config.valueColumn],
                        text: $interpolate($scope.property.config.textTemplate)(itm)
                    }
                    item.selected = _.indexOf($scope.selectedValues, item.value.toString()) > -1;
                    return item;
                });
            });
        }

        init();

        $scope.$on('valuesLoaded', function () {
            init();
        });

        $scope.setValue = function() {
            var val = [];

            angular.forEach($scope.items, function (itm) {
                if (itm.selected)
                    val.push(itm.value);
            });

            $scope.property.value = val.join($scope.delimiter);
        }

    });