angular.module("umbraco").controller("UIOMatic.PropertyEditors.Pickers.Property",
	function ($scope, $http, editorState) {

	    $scope.current = editorState.current;

	    console.log($scope.current.preValues);

	    var typeAliasPreValue = _.where($scope.current.preValues, { key: "typeAlias" })[0];
	    if (typeAliasPreValue) {
	        $scope.typeAlias = typeAliasPreValue.value;
	    }

	    $scope.$watch('typeAlias', function () {
	        getProperties();
	    });

	    function getProperties() {
	        if ($scope.typeAlias) {
	            $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetTypeInfo?typeAlias=" + $scope.typeAlias + "&includePropertyInfo=true").then(function(response) {
	                $scope.properties = response.data.RawProperties;
	            });
	        }
	    }

	});