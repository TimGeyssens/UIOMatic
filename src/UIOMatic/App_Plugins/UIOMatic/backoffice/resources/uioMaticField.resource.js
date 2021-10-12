angular.module("umbraco.resources")
	.factory("uioMaticFieldResource", function ($http, umbRequestHelper, localizationService) {

        var localizations = {
            failed: 'Failed to get all users'
        }

        localizationService.localizeMany(["resource_failed"]).then(function (data) {
            localizations.failed = data[0];
        });

	    return {
	        getAllUsers: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.fcBaseUrl + "GetAllUsers"),
                    localizations.failed
                );
	        }
	    }
	});