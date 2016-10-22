angular.module("umbraco.resources")
	.factory("uioMaticFieldResource", function ($http, umbRequestHelper) {
	    return {
	        getAllUsers: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.fcBaseUrl + "GetAllUsers"),
                    'Failed to get all users'
                );
	        }
	    }
	});