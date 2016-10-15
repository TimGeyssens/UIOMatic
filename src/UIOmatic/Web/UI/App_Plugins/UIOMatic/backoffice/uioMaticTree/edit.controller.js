var app = angular.module("umbraco");

angular.module("umbraco").controller("uioMatic.ObjectEditController",
	function ($scope, $routeParams, uioMaticObjectResource, notificationsService, navigationService) {

	    $scope.loaded = false;
	    $scope.editing = false;

	    var urlParts = $routeParams.id.split("?");
	    var id = urlParts[0];
	    var qs = {};

	    if (urlParts.length > 1) {
	        var vars = urlParts[1].split('&');
	        for (var i = 0; i < vars.length; i++) {
	            var pair = vars[i].split('=');
	            qs[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
	        }
	    }

	    var hasId = !isNaN(id) && id > 0;

	    if (!hasId) {
	        $scope.id = 0;
	        $scope.typeAlias = id;
	    } else {
	        $scope.id = id;
	        $scope.typeAlias = qs["ta"];
	    }

	    $scope.queryString = qs;
	    $scope.returnUrl = qs["returnUrl"] || "/uiomatic/uioMaticTree/list/" + $scope.typeAlias;

	    uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response)
	    {
	        $scope.type = response.data;
	        $scope.readOnly = response.data.readOnly;
	        $scope.properties = response.data.editableProperties;
	        $scope.type.nameFieldIndex = $scope.type.nameFieldKey.length > 0
                ? _.indexOf(_.pluck($scope.properties, "key"), $scope.type.nameFieldKey)
	            : -1;

	        var tabsArr = [];

	        angular.forEach($scope.properties, function (value, key) {
                if (this.map(function (e) { return e.label; }).indexOf(value.tab) === -1) {
	                if (value.tab == "") {
	                    this.push({ id: 99, label: "General" });
	                } else {
	                    this.push({ id: value.tabOrder > 0 ? value.tabOrder : key, label: value.tab });
	                }
	            }

	        }, tabsArr);

	        if (tabsArr.length > 1) {
	            $scope.content = {
	                 tabs: tabsArr.sort(function(a, b) {
	                     if (a.id < b.id)
	                         return -1;
	                     if (a.id > b.id)
	                         return 1;
	                     return 0;
	                 })
	            };
	        }


	        if (!hasId)
	        {
	            uioMaticObjectResource.getScaffold($scope.typeAlias).then(function (response) {
	                $scope.object = response.data;
	                $scope.loaded = true;
	                setValues();
	                $scope.$broadcast('valuesLoaded');
	            });
	        }
	        else
	        {
	            uioMaticObjectResource.getById($scope.typeAlias, $scope.id).then(function (response) {
	                $scope.object = response.data;
	                $scope.loaded = true;
	                $scope.editing = true;
	                setValues();
	                $scope.$broadcast('valuesLoaded');
	            });
	        }
	    });


	    $scope.save = function (object) {

	        angular.forEach($scope.properties, function (property) {
	            var key = property.key;
	            var value = $scope.queryString[property.columnName] || property.value;
	            $scope.object[key] = value;

	        });

	        uioMaticObjectResource.validate($scope.typeAlias, object).then(function (resp) {

	            if (!hasId) {
	                if (resp.data.length > 0) {
	                    angular.forEach(resp.data, function (error) {
	                        notificationsService.error("Failed to create object", error.Message);
	                    });
	                } else {
	                    uioMaticObjectResource.create($scope.typeAlias, object).then(function (response) {
	                        $scope.object = response.data;
	                        $scope.editing = true;
	                        $scope.objectForm.$dirty = false;
	                        navigationService.syncTree({ tree: 'uioMaticTree', path: [-1, -1], forceReload: true });
	                        notificationsService.success("Success", "Object has been created");
	                    });
	                }

	            } else {
	                if (resp.data.length > 0) {
	                    angular.forEach(resp.data, function (error) {
	                        notificationsService.error("Failed to update object", error.Message);
	                    });
	                } else {
	                    uioMaticObjectResource.update($scope.typeAlias, object).then(function (response) {
	                        $scope.objectForm.$dirty = false;
	                        navigationService.syncTree({ tree: 'uioMaticTree', path: [-1, -1], forceReload: true });
	                        notificationsService.success("Success", "Object has been saved");
	                    });
	                }
	            }


	        });

	    };

	    $scope.delete = function (object) {

	        if (hasId) {

	            //var arr = [];
	            //arr.push($scope.id);
	            //uioMaticObjectResource.deleteByIds($scope.typeAlias, arr).then(function () {
	            //    treeService.removeNode($scope.currentNode);
	            //    navigationService.hideNavigation();
	            //});
	        }

	    };

	    var setValues = function () {

	        for (var theKey in $scope.object) {

	            if ($scope.object.hasOwnProperty(theKey)) {

	                if (_.where($scope.properties, { key: theKey }).length > 0) {

	                    for (var prop in $scope.properties) {
	                        if ($scope.properties[prop].key == theKey) {
	                            if ($scope.properties[prop].type == "System.DateTime") {
	                                var date = moment($scope.object[theKey]).add(new Date().getTimezoneOffset(), "minutes").format("YYYY-MM-DD HH:mm:ss");
	                                $scope.properties[prop].value = date;
	                            } else {
	                                $scope.properties[prop].value = $scope.object[theKey];
	                            }
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
	});;