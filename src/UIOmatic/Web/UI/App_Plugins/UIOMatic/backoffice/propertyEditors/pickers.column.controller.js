angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Column",
	function ($scope, $http, editorState) {

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
	            $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllColumns?typeAlias=" + $scope.typeAlias).then(function(response) {
	                $scope.columns = response.data;
	            });
	        }
	    }

	});