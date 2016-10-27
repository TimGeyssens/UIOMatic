angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dropdown",
	function ($scope, $interpolate, uioMaticObjectResource) {

	    uioMaticObjectResource.getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, $scope.model.config.sortOrder).then(function (response) {
	        $scope.objects = response;
	    });

	    $scope.parseTemplate = function(object) {
	        return $interpolate($scope.model.config.textTemplate)(object);
	    }
	});