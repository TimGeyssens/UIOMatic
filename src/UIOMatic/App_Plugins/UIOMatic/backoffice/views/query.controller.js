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
    });