angular.module("umbraco").controller("UIOMatic.FieldEditors.Date",
	function ($scope, $element, $timeout, assetsService, angularHelper) {

	    var DATE_FORMAT = "YYYY-MM-DD";
	    var DATE_TIME_FORMAT = "YYYY-MM-DD HH:mm:ss";

	    var DATE_TIME_MIN = "0001-01-01T00:00:00";
	    var DATE_TIME_OFFSET_MIN = "0001-01-01T00:00:00+00:00";

	    //setup the default config
	    var config = {
	        pickDate: true,
	        pickTime: true,
	        useSeconds: true,
	        format: "",
	        icons: {
	            time: "icon-time",
	            date: "icon-calendar",
	            up: "icon-chevron-up",
	            down: "icon-chevron-down"
	        }
	    };

	    $scope.config = angular.extend(config, $scope.property.config);

	    if ($scope.property.view.endsWith("date.html")) {
	        $scope.config.pickTime = false;
	    }

	    //ensure the format doesn't get overwritten with an empty string
	    if ($scope.config.format === "" || $scope.config.format === undefined || $scope.config.format === null) {
	        $scope.config.format = $scope.config.pickTime ? DATE_TIME_FORMAT : DATE_FORMAT;
	    }

	    $scope.hasDatetimePickerValue = false;
	    $scope.datetimePickerValue = null;

	    $scope.clearDate = function () {
	        $scope.hasDatetimePickerValue = false;
	        $scope.datetimePickerValue = null;
	        setModelValue();
	    }

	    function applyDate(e) {
	        angularHelper.safeApply($scope, function () {
	            // when a date is changed, update the model
	            if (e.date) {
	                $scope.hasDatetimePickerValue = true;
	                $scope.datetimePickerValue = e.date.format($scope.config.format);
	            }
	            else {
	                $scope.hasDatetimePickerValue = false;
	                $scope.datetimePickerValue = null;
	            }
	            setModelValue();
	        });
	    };

	    function setModelValue() {
	        if ($scope.hasDatetimePickerValue) {
	            var elementData = $element.find("div:first").data().DateTimePicker;
	            if ($scope.config.storeUtc) {
	                $scope.property.value = elementData.getDate().utc().format(DATE_TIME_FORMAT);
	            } else {
	                $scope.property.value = elementData.getDate().format(DATE_TIME_FORMAT);
	                if ($scope.config.storeUtcOffset) {
	                    $scope.property.value += $scope.offset;
	                }
	            }
	        }
	        else {
	            if ($scope.property.type === "System.DateTime") {
	                $scope.property.value = DATE_TIME_MIN.replace('T', ' ');
	            } else if ($scope.property.type === "System.DateTimeOffset") {
	                $scope.property.value = DATE_TIME_OFFSET_MIN.replace('T', ' ');
	            } else {
	                $scope.property.value = null;
	            }
	        }
	    }

	    function init() {

	        var initValue = $scope.property.value;
	        if (initValue === DATE_TIME_MIN)
	            initValue = null;

	        $scope.hasDatetimePickerValue = initValue ? true : false;

	        assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {

	            var filesToLoad = ["lib/moment/moment-with-locales.js", "lib/datetimepicker/bootstrap-datetimepicker.js"];

	            assetsService.load(filesToLoad).then(function () {

	                var element = $element.find("div:first");

	                element
                        .datetimepicker($scope.config)
                        .on("dp.change", applyDate);

	                if ($scope.hasDatetimePickerValue) {

	                    var dateVal;
	                    if (initValue) {
	                        if ($scope.config.storeUtc) {
	                            dateVal = moment.utc(initValue, DATE_TIME_FORMAT).local();
	                        } else {
	                            dateVal = moment(initValue, DATE_TIME_FORMAT);
	                            if ($scope.config.storeUtcOffset) {
	                                dateVal = dateVal.utcOffset(initValue);
	                            }
	                        }
	                    } else {
	                        dateVal = moment();
	                    }

	                    $scope.offset = dateVal.format("Z");

	                    element.datetimepicker("setValue", dateVal);
	                    $scope.datetimePickerValue = dateVal.format($scope.config.format);
	                }

	                element.find("input").bind("blur", function () {
	                    //we need to force an apply here
	                    $scope.$apply();
	                });

	                //Ensure to remove the event handler when this instance is destroyted
	                $scope.$on('$destroy', function () {
	                    element.find("input").unbind("blur");
	                    element.datetimepicker("destroy");
	                });

	            });
	        });
	    }


	    if ($scope.valuesLoaded) {
	        init();
	    } else {
	        var unsubscribe = $scope.$on('valuesLoaded', function () {
	            init();
	            unsubscribe();
	        });
	    }
	});