angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Type",
	function ($scope, uioMaticPropertyEditorResource) {

	    uioMaticPropertyEditorResource.getAllTypes().then(function (response) {
	        $scope.types = response;
	    });

	});