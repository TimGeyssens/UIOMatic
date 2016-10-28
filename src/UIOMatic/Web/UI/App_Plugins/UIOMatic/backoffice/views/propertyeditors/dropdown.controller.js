angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dropdown",
	function ($scope, $interpolate, uioMaticObjectResource) {

	    uioMaticObjectResource.getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, $scope.model.config.sortOrder).then(function (response) {

	        var items = response.map(function (itm) {
	            return {
	                value: itm[$scope.model.config.valueColumn],
	                text: $interpolate($scope.model.config.textTemplate)(itm)
	            }
	        });

	        // Make sure model value is same type as value column
            // otherwise we get empty items in select list
	        if ($scope.model.value && items.length > 0) {
	            var valueType = typeof items[0].value;
	            if (valueType !== typeof $scope.model.value) {
	                if (valueType === "number") {
	                    $scope.model.value = parseInt($scope.model.value);
	                }
	            }
	        }

	        $scope.items = items;
	    });

	});