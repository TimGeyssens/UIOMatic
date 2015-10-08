angular.module("umbraco").controller("UIOMatic.Views.Date",
	function ($scope) {

	    angular.element(document).ready(function () {
	        
            //doesn't work without the timeout :(
	        setTimeout(function() {
	          
	            var picker = new Pikaday(
	            {
	                field: document.getElementById("UIOMaticProperty" + $scope.property.Name),
	                format: 'DD/MMM/YYYY',
	                firstDay: 1,
	                
	            });

	        }, 100);

	        
	    });

	});