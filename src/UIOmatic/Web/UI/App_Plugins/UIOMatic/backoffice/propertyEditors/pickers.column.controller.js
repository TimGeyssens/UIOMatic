angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Column",
	function ($scope, $http, editorState) {

	    $scope.current = editorState.current;
	    $scope.typeAlias = _.where($scope.current.preValues, { key: "typeAlias" })[0].value;

	    $scope.$watch('typeAlias', function () {
	        getColumns();
	    });

	    function getColumns() {
	        $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllColumns?typeAlias=" + $scope.typeAlias).then(function (response) {
	            $scope.columns = response.data;
	        });
	    }

	});