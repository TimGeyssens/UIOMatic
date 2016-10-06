angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Property",
	function ($scope, $http, editorState) {

	    $scope.current = editorState.current;
	    $scope.typeName = _.where($scope.current.preValues, { key: "typeName" })[0].value;

	    $scope.$watch('typeName', function () {
	        getProperties();
	    });

	    function getProperties() {
	        $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllProperties?typeName=" + $scope.typeName).then(function(response) {
	            $scope.properties = response.data;
	        });
	    }

	});