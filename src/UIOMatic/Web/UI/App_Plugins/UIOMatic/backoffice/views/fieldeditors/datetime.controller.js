angular.module("umbraco").controller("UIOMatic.Views.DateTime",
	function ($scope, $element, assetsService, angularHelper) {

	    var DATE_FORMAT = "YYYY-MM-DD HH:mm:ss";

	    //setup the default config
	    var config = {
	        pickDate: true,
	        pickTime: true,
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

	    var filesToLoad = ["lib/moment/moment-with-locales.js", "lib/datetimepicker/bootstrap-datetimepicker.js"];

	    assetsService.load(filesToLoad).then(
	        function () {

	            $element.find("div:first")
	                .datetimepicker($scope.config)
	                .on("dp.change", applyDate);
	        });


	    //Fix reset value to null if empty string
	    $scope.$watch('property.value', function (newValue, oldValue) {
	        if (newValue === "")
	            $scope.property.value = null;

	       
	    });

	   
	});