angular.module("umbraco").controller("UIOMatic.FieldFilters.Dropdown",
    function ($scope, uioMaticObjectResource) {

        function init() {
            uioMaticObjectResource.getFilterLookup($scope.property.typeAlias, $scope.property.keyPropertyName, $scope.property.key).then(function (response) {
                $scope.items = response;
            });
        }

        init();

        $scope.$on('valuesLoaded', function () {
            init();
        });
        
    });