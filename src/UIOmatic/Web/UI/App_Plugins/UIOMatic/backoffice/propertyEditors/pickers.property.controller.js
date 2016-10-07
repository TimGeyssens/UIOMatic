angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Property",
	function ($scope, $http, editorState) {

	    $scope.current = editorState.current;
	    $scope.typeAlias = _.where($scope.current.preValues, { key: "typeAlias" })[0].value;

	    $scope.$watch('typeAlias', function () {
	        getProperties();
	    });

	    function getProperties() {
	        $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetTypeInfo?typeAlias=" + $scope.typeAlias + "&includePropertyInfo=true").then(function (response) {
	            $scope.properties = response.data.Properties;
	        });
	    }

	});