angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Property",
	function ($scope, editorState, uioMaticObjectResource) {

	    $scope.current = editorState.current;

	    var typeAliasPreValue = _.where($scope.current.preValues, { key: "typeAlias" })[0];
	    if (typeAliasPreValue) {
	        $scope.typeAlias = typeAliasPreValue.value;
	    }

	    $scope.$watch('typeAlias', function () {
	        getProperties();
	    });

	    function getProperties() {
	        if ($scope.typeAlias) {
	            uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
	                $scope.properties = response.rawProperties;
	            });
	        }
	    }

	});