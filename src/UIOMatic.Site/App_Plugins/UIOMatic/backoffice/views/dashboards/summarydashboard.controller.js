angular.module("umbraco").controller("UIOMatic.Dashboards.SummaryDashboardController",
	function ($scope, uioMaticObjectResource) {

	    // Get summary types
	    uioMaticObjectResource.getSummaryDashboardTypes().then(function (types) {
	        $scope.types = types.map(function(itm) {
	            itm.recordCount = "-";
	            return itm;
	        });

	        if ($scope.types.length > 0) {
	            loadNextRecordCount(0);
	        }
	    });

	    // Get total record counts
	    function loadNextRecordCount(idx) {
	        uioMaticObjectResource.getTotalRecordCount($scope.types[idx].alias).then(function (count) {
	            $scope.types[idx].recordCount = count;
                if (idx < $scope.types.length - 1) {
                    loadNextRecordCount(++idx);
                }
	        });
	    }

	});