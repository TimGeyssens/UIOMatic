angular.module("umbraco").controller("UIOMatic.Views.Date",
	function ($scope, $element, assetsService, angularHelper) {

	    //setup the default config
	    var config = {
	        pickDate: true,
	        pickTime: false,
	        useSeconds: true,
	        format: "YYYY-MM-DD",
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
	               
	                    $scope.property.Value = e.date.format("YYYY-MM-DD");
	                
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