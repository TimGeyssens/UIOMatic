angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dropdown",
	function ($scope, $http, $parse) {
	    $http.get("backoffice/UIOMatic/PropertyEditorsApi/GetAllObjects?typeName=" + $scope.model.config.typeName).then(function (response) {
	        $scope.objects = response.data;
	    });

	    $scope.parseTemplate = function(object) {
	        var template = $parse($scope.model.config.objectTemplate);
	        return template(object);
	    }
	});