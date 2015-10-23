var app = angular.module("umbraco");




angular.module("umbraco").controller("uioMatic.ObjectEditController",
	function ($scope, $routeParams, uioMaticObjectResource, notificationsService, navigationService) {

	    $scope.loaded = false;
	    $scope.editing = false;

	    $scope.id = $routeParams.id.split("?")[0];

	    $scope.typeName = "";
	    if (isNaN($routeParams.id.split("?")[0])) {
	        $scope.typeName = $routeParams.id;
	    } else {
	        $scope.typeName = $routeParams.id.split("=").slice(1).join('=');
	    }
	    uioMaticObjectResource.getType($scope.typeName).then(function (response) {
	        $scope.type = response.data;
	    });
	    uioMaticObjectResource.getAllProperties($scope.typeName).then(function (response) {
	        $scope.properties = response.data;

	        var tabsArr = [];
	        angular.forEach(response.data, function (value, key) {
	            if (this.map(function (e) { return e.label; }).indexOf(value.Tab) == -1) {
	                if (value.Tab == "") {
	                    this.push({ id: 99, label: "Misc" });
	                } else {
	                    this.push({ id: key, label: value.Tab });
	                }
	            }
	        }, tabsArr);
	        if (tabsArr.length > 1 && tabsArr[0].id != 99) {
	            $scope.content = { tabs: tabsArr };
	        }
	        

	        if (isNaN($routeParams.id.split("?")[0])) {
	            $scope.object = {};
	            $scope.loaded = true;
	        }
	        else {

	            uioMaticObjectResource.getById($routeParams.id.split("=")[1], $routeParams.id.split("?")[0]).then(function (response) {
	                $scope.object = response.data;

	                $scope.loaded = true;
	                $scope.editing = true;
	                setValues();

	                $scope.$broadcast('ValuesLoaded');
	            });
	        }
	    });

	    


	    $scope.save = function (object) {

	        if (isNaN($routeParams.id.split("?")[0])) {

	            angular.forEach($scope.properties, function (property) {
	                var key = property.Key;
	                var value = property.Value;
	                $scope.object[key] = value;

	            });

	            uioMaticObjectResource.validate($routeParams.id.split("?")[0], object).then(function(resp) {

	                if (resp.data.length > 0) {
	                    angular.forEach(resp.data, function (error) {
	                        
	                        notificationsService.error("Failed to create object", error.Message);
	                    });
	                } else {
	                    uioMaticObjectResource.create($routeParams.id.split("?")[0], object).then(function (response) {
	                        $scope.object = response.data;
	                        $scope.editing = true;
	                        $scope.objectForm.$dirty = false;
	                        navigationService.syncTree({ tree: 'uioMaticTree', path: [-1, -1], forceReload: true });
	                        notificationsService.success("Success", "Object has been created");
	                    });
	                }

	                
	            });
	        } else {

	            angular.forEach($scope.properties, function (property) {
	                var key = property.Key;
	                var value = property.Value;
	                $scope.object[key] = value;

	            });

	            uioMaticObjectResource.validate($routeParams.id.split("=")[1], object).then(function (resp) {

	                if (resp.data.length > 0) {

	                    angular.forEach(resp.data, function (error) {

	                        notificationsService.error("Failed to update object", error.Message);
	                    });

	                }else
	                {
	                    uioMaticObjectResource.update($routeParams.id.split("=")[1], object).then(function (response) {
	                        //$scope.object = response.data;
	                        $scope.objectForm.$dirty = false;
	                        navigationService.syncTree({ tree: 'uioMaticTree', path: [-1, -1], forceReload: true });
	                        notificationsService.success("Success", "Object has been saved");
	                    });
	                }
	                

	            });
	        }
	        
	    };

	    $scope.isNumber = function (n) {
	        return !isNaN(parseFloat(n)) && isFinite(n);
	    }

	    var setValues = function () {
            
	           
	            for (var theKey in $scope.object) {
	                
	                if ($scope.object.hasOwnProperty(theKey)) {

	                    if (_.where($scope.properties, { Key: theKey }).length > 0) {
	                     
	                        //_.where($scope.properties, { Key: theKey }).Value = "test";
	                        //_.where($scope.properties, { Key: theKey }).Value = $scope.object[theKey];

	                        for (var prop in $scope.properties) {
	                            if ($scope.properties[prop].Key == theKey) {
	                                if ($scope.properties[prop].Type == "System.DateTime") {
	                                    var date = moment($scope.object[theKey]).format("YYYY-MM-DD HH:mm:ss");
	                                    $scope.properties[prop].Value = date;
	                                } else {
	                                    $scope.properties[prop].Value = $scope.object[theKey];
	                                }
	                            }
	                        }
	                    }

	                }
	            }

	    };

	    
	});