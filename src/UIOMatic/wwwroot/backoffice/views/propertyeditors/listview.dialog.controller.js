var app = angular.module("umbraco");

angular.module("umbraco").controller("UIOMatic.PropertyEditors.ListViewDialog",
	function ($scope, $routeParams, $location, $timeout, uioMaticUtilityService, uioMaticObjectResource, notificationsService) {
	    $scope.loaded = false;
	    $scope.editing = false;

	    var id = $scope.dialogData.id;
	    var hasId = id != -1;

	    if (!hasId) {
            $scope.id = 0;
            $scope.typeAlias = $scope.dialogData.typeAlias;

	    } else {
	        $scope.id = id;
	        $scope.typeAlias = $scope.dialogData.typeAlias;

	    }
        
	    uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
	        $scope.type = response;
	        $scope.itemDisplayName = response.displayNameSingular;
	        $scope.readOnly = response.readOnly;
	        $scope.fromList = $scope.fromList || response.renderType == 1;
	        $scope.properties = response.editableProperties;
	        $scope.type.nameFieldIndex = $scope.type.nameFieldKey.length > 0
                ? _.indexOf(_.pluck($scope.properties, "key"), $scope.type.nameFieldKey)
	            : -1;


	        var tabsArr = [];

	        // Build items path
	        $scope.path = response.path;
	        if ($scope.id > 0 && response.renderType == 0) // Tree view so append the node id
	            $scope.path.push($scope.id);


	        angular.forEach($scope.properties, function (value, key) {
	            if (this.map(function (e) { return e.label; }).indexOf(value.tab) === -1) {
	                this.push({ id: 99, label: "General" });
	            }
	        }, tabsArr);

	        if (tabsArr.length > 1) {
	            $scope.content = {
	                tabs: tabsArr.sort(function (a, b) {
	                    if (a.id < b.id)
	                        return -1;
	                    if (a.id > b.id)
	                        return 1;
	                    return 0;
	                })
	            };
	        }


	        if (!hasId) {
	            uioMaticObjectResource.getScaffold($scope.typeAlias).then(function (response) {
	                $scope.object = response;
	                $scope.loaded = true;
	                setValues();
	                $timeout(function () {
	                    $scope.valuesLoaded = true;
	                    $scope.$broadcast('valuesLoaded');
	                });
	            });
	        }
	        else {
	            uioMaticObjectResource.getById($scope.typeAlias, $scope.id).then(function (response) {
	                $scope.object = response;
	                $scope.loaded = true;
	                $scope.editing = true;
	                setValues();
	                $timeout(function () {
	                    $scope.valuesLoaded = true;
	                    $scope.$broadcast('valuesLoaded');
	                });
	            });
	        }
	    });

	    $scope.save = function (object) {

	        angular.forEach($scope.properties, function (property) {


	            var key = property.key;
	            var value =  property.value;
	            $scope.object[key] = value;


	        });

	        //Setting nodeId on obj
	        if ($scope.dialogData == "create") {
	            $scope.object[$scope.dialogData.nodeIdSelected] = $scope.dialogData.currentNodeId;
	        }
	        
	        uioMaticObjectResource.validate($scope.typeAlias, object).then(function (resp) {

	            if (!hasId) {

	                if (resp.length > 0) {

	                    angular.forEach(resp, function (error) {

	                        notificationsService.error("Failed to create " + $scope.itemDisplayName, error.ErrorMessage);

	                    });

	                } else {
	                    
	                    uioMaticObjectResource.create($scope.typeAlias, object).then(function (response) {
	                        notificationsService.success("Success", $scope.itemDisplayName + " has been created");
	                        $scope.submit(object);

	                    });
	                  
	                }

	            } else {

	                if (resp.length > 0) {
	                    angular.forEach(resp, function (error) {
	                        notificationsService.error("Failed to update " + $scope.itemDisplayName, error.ErrorMessage);
	                    });
	                } else {
	                    uioMaticObjectResource.update($scope.typeAlias, object).then(function () {
	                        notificationsService.success("Success", $scope.itemDisplayName + " has been saved");
	                        $scope.submit(object);

	                    });
	                }
                    
	            }


	        });

	    };

	    $scope.navigate = function (url) {
	        // Because some JS seems to be translating any links starting '#'
	        $location.url(url);
	    }

	    $scope.$on("valuesLoaded", function () {
	        $timeout(function () {
	            if ($scope.content && $scope.content.tabs.length > 1 && $scope.queryString["tab"]) {
	                $("a[href='#" + $scope.queryString["tab"] + "']").trigger("click");
	            }
	        }, 202); // 202 is very specific, as tabs init code runs on a 200 timeout so gotta wait for that first
	    });

	    var setValues = function () {
	        for (var theKey in $scope.object) {
	            if ($scope.object.hasOwnProperty(theKey)) {
	                if (_.where($scope.properties, { key: theKey }).length > 0) {
	                    for (var prop in $scope.properties) {
	                        if ($scope.properties[prop].key == theKey) {
	                            $scope.properties[prop].value = $scope.object[theKey];
	                        }
	                    }
	                }
	            }
	        }
	    };


	}).filter("removeProperty", function () {
	    return function (input, namePropertyKey) {
	        if (namePropertyKey == null || namePropertyKey == "" || input == null)
	            return input;
	        return input.filter(function (property) {
	            return property.key != namePropertyKey;
	        });
	    }
	});