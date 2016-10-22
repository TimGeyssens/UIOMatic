angular.module("umbraco").controller("UIOMatic.Views.Date",
	function ($scope, $element, assetsService, angularHelper) {

	    var DATE_FORMAT = "YYYY-MM-DD";

	    // Remove the time part from the value
	    $scope.property.Value = moment($scope.property.Value).format(DATE_FORMAT);

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

	                $scope.property.Value = e.date.format(DATE_FORMAT);

	            }


	        });
	    };

	    var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.js"];

	    assetsService.load(filesToLoad).then(
	        function () {

	            $element.find("div:first")
	                .datetimepicker($scope.config)
	                .on("dp.change", applyDate);
	        });


	    //Fix reset value to null if empty string
	    $scope.$watch('property.Value', function (newValue, oldValue) {
	        if (newValue === "")
	            $scope.property.Value = null;
	    });
	});