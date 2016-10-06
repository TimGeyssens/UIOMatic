angular.module("umbraco").controller("UIOMatic.PropertyEditors.MultiPicker",
	function ($scope, dialogService, uioMaticObjectResource, $parse) {

	    $scope.objects = [];

	    function done(data){
	        $scope.model.value = data;
	        getFullDetails();
	    }     
	    $scope.openDialog = function()
	    {
	        var dialog = dialogService.open({
	            template: '/App_Plugins/UIOMatic/backoffice/PropertyEditors/objectsearcher.html',
	            show: true,
	            callback: done,
	            dialogData: {
	                typeName: $scope.model.config.typeName,
	                objectTemplate: $scope.model.config.objectTemplate,
	                selectedIds : $scope.model.value
	            }
	        });
	    }
	    $scope.remove = function (index) {
	        $scope.objects.splice(index, 1);
	        $scope.model.value.splice(index, 1)
	    }
	    $scope.parseTemplate = function (object) {
	        var template = $parse($scope.model.config.objectTemplate);
	        return template(object);

	    }
	    function getFullDetails()
	    {

	        $scope.objects = [];
	        if($scope.model.value)
	        {
	            angular.forEach($scope.model.value, function (id) {
	                uioMaticObjectResource.getById($scope.model.config.typeName, id).then(function (resp) {
	                    $scope.objects.push(resp.data);
	                })
	            });
	        }
	    }
	    getFullDetails();

	});