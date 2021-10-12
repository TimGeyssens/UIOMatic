angular.module("umbraco")
	.controller("uioMatic.ObjectDeleteController",
		function ($scope, uioMaticObjectResource, navigationService, treeService) {

			$scope.deleteButtonState = "init";

			$scope.delete = function (type, id) {
				$scope.deleteButtonState = "busy";
				var arr = [];
				arr.push(id);
				uioMaticObjectResource.deleteByIds(type, arr).then(function () {
					$scope.deleteButtonState = "success";
					treeService.removeNode($scope.currentNode);
					navigationService.hideNavigation();
				});

			};
			$scope.cancelDelete = function () {
				navigationService.hideNavigation();
			};
		});