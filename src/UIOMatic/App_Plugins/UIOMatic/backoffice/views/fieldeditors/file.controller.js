angular.module("umbraco").controller("UIOMatic.FieldEditors.File",
    function ($scope, editorService) {

	    $scope.openMediaPicker = function() {
            editorService.mediaPicker({
                submit: function (model) {
                    populateFile(model.selection[0]);
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
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