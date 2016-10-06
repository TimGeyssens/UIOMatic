angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Column",
	function ($scope, $http, editorState) {

	    $scope.current = editorState.current;
	    $scope.typeName = _.where($scope.current.preValues, { key: "typeName" })[0].value;

	    $scope.$watch('typeName', function () {
	        getColumns();
	    });

	    function getColumns() {
	        $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllColumns?typeName=" + $scope.typeName).then(function (response) {
	            $scope.columns = response.data;
	        });
	    }

	});