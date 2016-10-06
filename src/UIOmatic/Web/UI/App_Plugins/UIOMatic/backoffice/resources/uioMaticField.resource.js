angular.module("umbraco.resources")
	.factory("uioMaticFieldResource", function ($http) {
	    return {
	        getAllUsers: function () {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.fcBaseUrl + "GetAllUsers");
	        }
	    }
	    });