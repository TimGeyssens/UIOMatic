angular.module("umbraco").controller("UIOMatic.Views.File",
	function ($scope, dialogService) {

	    $scope.openMediaPicker = function() {
	        dialogService.mediaPicker({ callback: populateFile });
	    };

	    function populateFile(item) {


            $scope.property.Value = item.image;
	    }

        $scope.isPicture = function(path) {
            if (/\.(jpg|png|gif|jpeg)$/.test(path)) {
                return true;
            }
            return false;
        }
	});