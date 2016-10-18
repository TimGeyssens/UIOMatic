angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dropdown",
	function ($scope, $parse, uioMaticObjectResource) {

	    uioMaticObjectResource.getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, $scope.model.config.sortOrder).then(function (response) {
	        $scope.objects = response;
	    });

	    $scope.parseTemplate = function(object) {
	        var template = $parse($scope.model.config.objectTemplate);
	        return template(object);
	    }
	});