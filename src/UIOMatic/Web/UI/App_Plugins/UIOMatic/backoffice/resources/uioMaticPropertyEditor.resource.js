angular.module("umbraco.resources")
	.factory("uioMaticPropertyEditorResource", function ($http, umbRequestHelper) {
	    return {
	        getAllTypes: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllTypes"),
                    'Failed to get all types'
                );
	        },
	        getAllColumns: function (typeAlias) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllColumns?typeAlias=" + typeAlias),
                    'Failed to get all columns'
                );
	        }
	    }
	});