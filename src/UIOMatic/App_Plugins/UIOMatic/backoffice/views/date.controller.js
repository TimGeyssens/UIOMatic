angular.module("umbraco").controller("UIOMatic.Views.Date",
	function ($scope) {

	    angular.element(document).ready(function () {

	        setTimeout(function() {

	            if ($scope.property.Value) {
	                $scope.property.picker.setMoment(moment($scope.property.Value, 'YYYY-MM-DD'));
	            }
	        }, 100);
	    });
	});