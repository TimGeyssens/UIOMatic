angular.module("umbraco").controller("UIOMatic.PropertyEditors.MultiPicker",
	function ($scope, $interpolate, dialogService, uioMaticObjectResource) {

	    $scope.openDialog = function()
	    {
	        dialogService.open({
	            template: '/App_Plugins/UIOMatic/backoffice/views/PropertyEditors/objectsearcher.html',
	            show: true,
	            callback: function (selectedIds) {
	                $scope.model.value = selectedIds;
	                getFullDetails();
	            },
	            dialogData: {
	                typeAlias: $scope.model.config.typeAlias,
	                textTemplate: $scope.model.config.textTemplate,
	                selectedIds : $scope.model.value
	            }
	        });
	    }

	    $scope.remove = function (index) {
	        $scope.items.splice(index, 1);
	        $scope.model.value.splice(index, 1);
	    }

	    function getFullDetails(){

	        $scope.items = [];

	        if($scope.model.value) {
	            angular.forEach($scope.model.value, function (id) {
	                uioMaticObjectResource.getById($scope.model.config.typeAlias, id).then(function(resp) {
	                    $scope.items.push({
	                        text: $interpolate($scope.model.config.textTemplate)(resp)
	                    });
	                });
	            });
	        }
	    }

	    getFullDetails();

	});