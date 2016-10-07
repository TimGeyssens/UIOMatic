angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Type",
	function ($scope, $http) {
	    console.log($scope);
	    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllTypes").then(function (response) {
	        $scope.types = response.data;
	    });

	});