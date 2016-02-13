angular.module('umbraco')
    .controller('UIOMatic.Views.Query.DialogController',
    function ($scope) {
    	$scope.Operators =
        [
            { id: 1, name: "=" },
            { id: 2, name: "≠" },
            { id: 3, name: ">" },
            { id: 4, name: "<" },
            { id: 5, name: "≥" },
            { id: 6, name: "≤" }
        ];
    	$scope.filterproperties = $scope.dialogData;

    	$scope.isInteger = function (n) {
    	    if (n.toLowerCase().indexOf("int")>0) {
    	        return true;
    	    }
    	    return false;
    	}
    });