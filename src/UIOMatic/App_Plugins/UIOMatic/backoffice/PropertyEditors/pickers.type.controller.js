angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Type",
	function ($scope, $http) {
	    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllTypes").then(function (response) {
	        $scope.types = response.data;
	    });

	});