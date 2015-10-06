angular.module("umbraco").controller("uioMatic.ObjectEditController",
	function ($scope, $routeParams, uioMaticObjectResource, notificationsService, navigationService) {

	    $scope.loaded = false;
	    $scope.id = $routeParams.id.split("?")[0];

	    var typeName = "";
	    if (isNaN($routeParams.id.split("?")[0])) {
	        typeName = $routeParams.id;
	    } else {
	        typeName = $routeParams.id.split("=")[1];
	    }
	    uioMaticObjectResource.getAllProperties(typeName).then(function (response) {
	        $scope.properties = response.data;
	        

	        if (isNaN($routeParams.id.split("?")[0])) {
	            $scope.object = {};
	            $scope.loaded = true;
	        }
	        else {

	            uioMaticObjectResource.getById($routeParams.id.split("=")[1], $routeParams.id.split("?")[0]).then(function (response) {
	                $scope.object = response.data;

	                $scope.loaded = true;

	                setValues();
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
	                $scope.object[0][key] = value;

	            });

	            uioMaticObjectResource.validate($routeParams.id.split("=")[1], object[0]).then(function (resp) {

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

	        angular.forEach($scope.object[0], function (value) {
	           
	            for (var key in $scope.object[0]) {
	               
	                if ($scope.object[0].hasOwnProperty(key)) {

	                
	                    if (_.where($scope.properties, { Key: key }).length > 0) {
	                        _.where($scope.properties, { Key: key })[0].Value = $scope.object[0][key];
	                    }

	                }
	            }
	        });
	    };

	    
	});