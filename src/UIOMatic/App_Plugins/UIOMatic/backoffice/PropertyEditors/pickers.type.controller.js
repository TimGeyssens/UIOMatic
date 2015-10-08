angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Type",
	function ($scope, $http) {
	    $http.get("backoffice/UIOMatic/PropertyEditorsApi/GetAllTypes").then(function (response) {
	        $scope.types = response.data;
	    });

	});