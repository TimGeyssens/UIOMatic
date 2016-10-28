angular.module("umbraco").controller("UIOMatic.FieldEditors.Map",
	function ($scope, $timeout, assetsService, notificationsService) {

	    var map, marker, input, tabLinkSelector, markerZoom = 15;

	    $scope.config = $scope.property.config;
	    $scope.center = $scope.config.center;

	    $scope.setValue = function (lat, lng) {
	        var val = { lat: lat, lng: lng };
	        if (!marker) {
	            marker = new google.maps.Marker({
	                position: val,
	                map: map,
	                title: "Right click me to clear value",
	                draggable: true
	            });
	            marker.addListener('dragend', function (evt) {
	                $scope.setValue(evt.latLng.lat(), evt.latLng.lng());
	            });
	            marker.addListener('rightclick', function (evt) {
	                $scope.clearValue();
	            });
	        } else {
	            marker.setMap(map);
	            marker.setPosition(val);
	        }
	        $scope.property.value = ($scope.property.type === "System.String") ? JSON.stringify(val) : val;
	    }

	    $scope.clearValue = function () {
	        if (marker) {
	            marker.setMap(null);
	        }
	        map.setCenter($scope.center);
	        map.setZoom($scope.center.zoom);
	        $scope.property.value = "";
	    }

	    $scope.initMap = function () {

	        var value, startPos = $scope.center || { lat: 55.37805, lng: -3.4359, zoom: 5 };
	        if ($scope.property.value) {
	            startPos = value = ($scope.property.type === "System.String" && typeof $scope.property.value === "string") ? JSON.parse($scope.property.value) : $scope.property.value;
	            startPos.zoom = markerZoom;
	        }

	        map = new google.maps.Map(document.getElementsByClassName($scope.property.key.toLowerCase() + "_map")[0], {
	            center: startPos,
	            zoom: startPos.zoom
	        });

	        input = document.getElementsByClassName($scope.property.key.toLowerCase() + "_search")[0];
	        google.maps.event.addDomListener(input, 'keydown', function (e) {
	            if (e.keyCode == 13) {
	                e.preventDefault();
	            }
	        });

	        var searchBox = new google.maps.places.SearchBox(input);
	        map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

	        searchBox.addListener('places_changed', function () {
	            var places = searchBox.getPlaces();

	            if (places.length == 0) {
	                $scope.clearValue();
	                notificationsService.error("Place not found", "Please try another location");
	                return;
	            }
	            else {
	                var location = places[0].geometry.location;
	                $scope.setValue(location.lat(), location.lng());
	                map.setCenter(location);
	                map.setZoom(markerZoom);
	            }
	        });

	        if (value) {
	            $scope.setValue(value.lat, value.lng);
	        }

	        tabLinkSelector = 'a[data-toggle="tab"]:contains("' + ($scope.property.tab == "" ? "General" : $scope.property.tab) + '")';
	        $(tabLinkSelector).on('shown', function (e) {
	            var center = map.getCenter();
	            google.maps.event.trigger(map, 'resize');
	            map.setCenter(center);
	        });
	    }

	    function init() {
	        if (typeof google === 'undefined') {
	            assetsService.loadJs("https://maps.googleapis.com/maps/api/js?key=" + $scope.config.apiKey + "&libraries=places").then(function() {
	                $scope.initMap();
	            });
	        } else {
	            $timeout(function() {
	                $scope.initMap();
	            }, 100);
	        }
	    }

	    if ($scope.valuesLoaded) {
	        init();
	    } else {
	        var unsubscribe = $scope.$on('valuesLoaded', function () {
	            init();
	            unsubscribe();
	        });
	    }

	    $scope.$on("$destroy", function () {
	        if (marker) {
	            google.maps.event.clearInstanceListeners(marker);
	        }
	        google.maps.event.clearInstanceListeners(map);
	        google.maps.event.clearInstanceListeners(input);
	        $(tabLinkSelector).off("shown");
	    });
	});