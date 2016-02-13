angular.module("umbraco").controller("UIOMatic.Views.File",
	function ($scope, dialogService) {

	    $scope.openMediaPicker = function() {
	        dialogService.mediaPicker({ callback: populateFile });
	    };

	    function populateFile(item) {
	        

            $scope.property.Value = item.properties[0].value;
	    }

        $scope.isPicture = function(path) {
            if (/\.(jpg|png|gif|jpeg)$/.test(path)) {
                return true;
            }
            return false;
        }
        $scope.isVedio = function (path) {
            if (/\.(mp4|avi)$/.test(path)) {
                return true;
            }
            return false;
        }

        $scope.isReadonly = function (path) {
            return $scope.property.IsReadOnly;
        }
	});