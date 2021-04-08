angular.module("umbraco").controller("uioMatic.ObjectEditController",
    function ($scope, $routeParams, $location, $timeout, editorState, uioMaticObjectResource, notificationsService, navigationService, localizationService) {

        var localizations = {
            failedcreate: 'Failed to create',
            failedupdate: 'Failed to update',
            success: 'Success',
            created: 'has been created',
            saved: 'has been saved',
            update: 'Update',
            create: 'Create'
        }

        localizationService.localizeMany(["edit_failedcreate", "edit_failedupdate", "edit_success", "edit_created", "edit_saved", "edit_update", "edit_create"]).then(function (data) {
            localizations.failedcreate = data[0];
            localizations.failedupdate = data[1];
            localizations.success = data[2];
            localizations.created = data[3];
            localizations.saved = data[4];
            localizations.update = data[5];
            localizations.create = data[6];
        });

        $scope.loaded = false;
        $scope.editing = false;
        $scope.saveButtonState = "init";
        $scope.currentSection = $routeParams.section || 'uiomatic';
        $scope.activeApp = null;


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

        // If we have a ta querystring, it must be an edit
        // and the first part of the URL must be the ID
        var hasId = !!qs["ta"];

        if (!hasId) {
            $scope.id = 0;
            $scope.typeAlias = id;
        } else {
            $scope.id = id;
            $scope.typeAlias = qs["ta"];
        }

        $scope.queryString = qs;
        $scope.returnUrl = qs["returnUrl"] || "/" + $scope.currentSection + "/uiomatic/list/" + $scope.typeAlias;
        $scope.fromList = !!qs["returnUrl"]; // Assumes a return URL means you've come from a list field view
        $scope.syncTree = !$scope.fromList; // Don't sync the tree if we are a nested type


        uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
            $scope.type = response;
            $scope.itemDisplayName = response.displayNameSingular;
            $scope.headerCaption = (hasId ? (response.readOnly ? "" : localizations.update + " ") : localizations.create + " ") + response.displayNameSingular;
            $scope.readOnly = response.readOnly;
            $scope.fromList = $scope.fromList || response.renderType == 1;
            $scope.properties = response.editableProperties;
            $scope.type.nameFieldIndex = $scope.type.nameFieldKey.length > 0
                ? _.indexOf(_.pluck($scope.properties, "key"), $scope.type.nameFieldKey)
                : -1;            

            editorState.set({                
                qs,                
                type: response,
                id: $scope.id
            });

            var tabsArr = [];

            // Build items path
            $scope.path = response.path;
            if ($scope.id > 0 && response.renderType == 0) // Tree view so append the node id
                $scope.path.push($scope.id);

            // Sync the tree
            if ($scope.syncTree) {
                navigationService.syncTree({ tree: 'uiomatic', path: $scope.path, forceReload: false, activate: true });
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

        function initApps() {

            // we need to check wether an app is present in the current data, if not we will present the default app.
            var isAppPresent = false;

            // on first init, we dont have any apps. but if we are re-initializing, we do, but ...
            if ($scope.activeApp) {

                _.forEach($scope.type.apps, function (app) {
                    if (app.alias === $scope.activeApp.alias) {
                        isAppPresent = true;
                        $scope.appChanged(app);
                    }
                });

                if (isAppPresent === false) {
                    // active app does not exist anymore.
                    $scope.activeApp = null;
                }
            }

            // if we still dont have a app, lets show the first one:
            if ($scope.activeApp === null && $scope.type.apps.length) {
                $scope.appChanged($scope.type.apps[0]);
            }
        }


        /**
         * Call back when a content app changes
         * @param {any} app
         */
        $scope.appChanged = function (activeApp) {

            $scope.activeApp = activeApp;

            _.forEach($scope.type.apps, function (app) {
                app.active = false;
                if (app.alias === $scope.activeApp.alias) {
                    app.active = true;
                }
            });

            $scope.$broadcast("editors.apps.appChanged", { app: activeApp });            
        };


        $scope.save = function (object) {

            $scope.saveButtonState = "busy";

            angular.forEach($scope.properties, function (property) {
                var key = property.key;
                var value = $scope.queryString[property.columnName] || property.value;
                $scope.object[key] = value;

            });

            uioMaticObjectResource.validate($scope.typeAlias, object).then(function (resp) {

                if (!hasId) {

                    if (resp.length > 0) {
                        angular.forEach(resp, function (error) {
                            notificationsService.error(localizations.failedcreate + " " + $scope.itemDisplayName, error.ErrorMessage);
                            $scope.saveButtonState = "error";
                        });
                    } else {
                        uioMaticObjectResource.create($scope.typeAlias, object).then(function (response) {
                            $scope.objectForm.$dirty = false;
                            var redirectUrl = "/" + $scope.currentSection + "/uiomatic/edit/" + response[$scope.type.primaryKeyColumnName] + "%3Fta=" + $scope.typeAlias;
                            for (var k in $scope.queryString) {
                                if ($scope.queryString.hasOwnProperty(k) && k != "ta") {
                                    redirectUrl += "%26" + encodeURIComponent(k) + "=" + encodeURIComponent(encodeURIComponent($scope.queryString[k]));
                                }
                            };
                            $location.url(redirectUrl);
                            notificationsService.success(localizations.success, $scope.itemDisplayName + " " + localizations.created);
                            $scope.saveButtonState = "success";
                        });
                    }

                } else {

                    if (resp.length > 0) {
                        angular.forEach(resp, function (error) {
                            notificationsService.error(localizations.failedupdate + " " + $scope.itemDisplayName, error.ErrorMessage);
                            $scope.saveButtonState = "error";
                        });
                    } else {
                        uioMaticObjectResource.update($scope.typeAlias, object).then(function () {
                            $scope.objectForm.$dirty = false;

                            // Sync the tree
                            if ($scope.syncTree) {
                                navigationService.syncTree({ tree: 'uiomatic', path: $scope.path, forceReload: true, activate: true });
                            }
                            notificationsService.success(localizations.success, $scope.itemDisplayName + " " + localizations.saved);
                            $scope.saveButtonState = "success";
                        });
                    }

                }


            });

        };

        $scope.navigate = function (url) {
            // Because some JS seems to be translating any links starting '#'
            $location.url(url);
        };


        $scope.$on("valuesLoaded", function () {

            initApps();

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
        };
    });