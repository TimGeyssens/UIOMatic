angular.module("umbraco")
.controller("uioMatic.ObjectDeleteController",
	function ($scope, uioMaticObjectResource, navigationService, treeService) {
	    $scope.delete = function (type, id) {
	        uioMaticObjectResource.deleteById(type, id).then(function () {
	            treeService.removeNode($scope.currentNode);
	            navigationService.hideNavigation();
	            
	        });

	    };
	    $scope.cancelDelete = function () {
	        navigationService.hideNavigation();
	    };
	});