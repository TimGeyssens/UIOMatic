angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Column",
	function ($scope, editorState, uioMaticPropertyEditorResource) {

	    $scope.current = editorState.current;

	    var typeAliasPreValue = _.where($scope.current.preValues, { key: "typeAlias" })[0];
	    if (typeAliasPreValue) {
	        $scope.typeAlias = typeAliasPreValue.value;
	    }

	    $scope.$watch('typeAlias', function () {
	        getColumns();
	    });

	    function getColumns() {
	        if ($scope.typeAlias) {
	            uioMaticPropertyEditorResource.getAllColumns($scope.typeAlias).then(function (response) {
	                $scope.columns = response;
	            });
	        }
	    }

	});