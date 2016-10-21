angular.module("umbraco").controller("UIOMatic.Views.DateTimeOffset",
	function ($scope, $element, $timeout, assetsService, angularHelper) {

	    var DATE_FORMAT = 'YYYY-MM-DD HH:mm:ss';

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
	                $scope.property.value = e.date.format(DATE_FORMAT) + $scope.property.offset;
	            }
	        });
	    };

	    var filesToLoad = ["lib/moment/moment-with-locales.js",
            "lib/datetimepicker/bootstrap-datetimepicker.js"];

	    assetsService.load(filesToLoad).then(
	        function () {
	            $scope.property.viewValue = moment.parseZone($scope.property.value).format(DATE_FORMAT);
	            $scope.property.offset = moment.parseZone($scope.property.value).format("Z");

	            // It's important to remove the T from the value to make sure
	            // web api bind this as a string and not as a DateTime because
	            // when it does so, it converts the DateTimeOffset to a local
	            // DateTime.
	            $scope.property.value = $scope.property.value.replace('T', ' ');

	            // The $timeout 0 is a little hack to delay the execution of this 
	            // code on the call stack and be sure the viewValue property is 
	            // fully loaded. Without this delay, when the users select a 
	            // different date the time would change to the current time.
	            $timeout(function () {
	                $element.find("div:first")
                        .datetimepicker($scope.config)
                        .on("dp.change", applyDate);
	            }, 0);
	        });

	});