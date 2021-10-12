angular.module("umbraco").controller("UIOMatic.FieldFilters.Dropdown",
    function ($scope, uioMaticObjectResource) {

        uioMaticObjectResource.getFilterLookup($scope.property.typeAlias, $scope.property.keyPropertyName, $scope.property.key).then(function (response) {
            $scope.items = response;
        });
        
    });