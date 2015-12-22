angular.module("umbraco").controller("Example.Picker.Person",
	function ($scope,$http) {

	    $http.get("backoffice/Example/ExampleApi/GetAll").then(function(response) {
	        $scope.persons = response.data;
	    });
	});