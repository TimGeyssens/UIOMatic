angular.module("umbraco")
.controller("uioMatic.ObjectDeleteController",
	function ($scope, uioMaticObjectResource, navigationService, treeService) {
	    $scope.delete = function (type, id) {
	        var arr = [];
	        arr.push(id);
	        uioMaticObjectResource.deleteByIds(type, arr).then(function () {
	            treeService.removeNode($scope.currentNode);
	            navigationService.hideNavigation();
	            
	        });

	    };
	    $scope.cancelDelete = function () {
	        navigationService.hideNavigation();
	    };
	});