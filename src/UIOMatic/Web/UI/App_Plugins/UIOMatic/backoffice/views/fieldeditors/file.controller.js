angular.module("umbraco").controller("UIOMatic.FieldEditors.File",
	function ($scope, dialogService) {

	    $scope.openMediaPicker = function() {
	        dialogService.mediaPicker({ callback: populateFile });
	    };

	    function populateFile(item) {
            $scope.property.value = item.image;
	    }

        $scope.isPicture = function(path) {
            if (/\.(jpg|png|gif|jpeg)$/.test(path)) {
                return true;
            }
            return false;
        }
	});