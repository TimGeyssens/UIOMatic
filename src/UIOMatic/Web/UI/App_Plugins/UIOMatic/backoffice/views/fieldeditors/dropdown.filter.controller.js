angular.module("umbraco").controller("UIOMatic.Views.DropdownFilter",
    function ($scope, uioMaticObjectResource, $parse) {

        function init() {
            uioMaticObjectResource.getFilterLookup($scope.property.typeAlias, $scope.property.keyPropertyName, $scope.property.key).then(function (response) {
                $scope.objects = response;
            });
        }

        init();

        $scope.$on('valuesLoaded', function (event, data) {
            init();
        });

        //$scope.parseTemplate = function (object) {
        //    var template = $parse($scope.property.config.textTemplate);
        //    return template(object);
        //}

        
    });