angular.module("umbraco").controller("UIOMatic.Views.Date",
	function ($scope, $element, assetsService, angularHelper) {

	    var DATE_FORMAT = "YYYY-MM-DD";

	    // Remove the time part from the value
	    if ($scope.property.value)
	        $scope.property.value = moment($scope.property.value).format(DATE_FORMAT);

	    //setup the default config
	    var config = {
	        pickDate: true,
	        pickTime: false,
	        useSeconds: true,
	        format: DATE_FORMAT,
	        icons: {
	            time: "icon-time",
	            date: "icon-calendar",
	            up: "icon-chevron-up",
	            down: "icon-chevron-down"
	        }

	    };

	    $scope.config = angular.extend(config, $scope.config);

	    function applyDate(e) {
	        angularHelper.safeApply($scope, function () {
	            // when a date is changed, update the model
	            if (e.date) {
	                $scope.property.value = e.date.format(DATE_FORMAT); 
	            }
	        });
	    };

	    var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.js"];

	    assetsService.load(filesToLoad).then(
	        function() {

	            $element.find("div:first")
	                .datetimepicker($scope.config)
	                .on("dp.change", applyDate);
	        });
	    
	});