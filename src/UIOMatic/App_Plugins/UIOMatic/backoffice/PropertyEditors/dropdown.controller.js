angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dropdown",
	function ($scope, $http, $parse) {
	    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllObjects?typeName=" + $scope.model.config.typeName + "&sortColumn=" + $scope.model.config.sortColumn + "&sortOrder=" + $scope.model.config.sortOrder).then(function (response) {
	        $scope.objects = response.data;
	    });

	    $scope.parseTemplate = function(object) {
	        var template = $parse($scope.model.config.objectTemplate);
	        return template(object);
	    }
	});