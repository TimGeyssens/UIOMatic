angular.module("umbraco.resources")
	.factory("uioMaticPropertyEditorResource", function ($http, umbRequestHelper, localizationService) {

        var localizations = {
            failedtypes: 'Failed to get all types',
            failedcolumns: 'Failed to get all columns'
        }

        localizationService.localizeMany(["resource_failedtypes", "resource_failedcolumns"]).then(function (data) {
            localizations.failedtypes = data[0];
            localizations.failedcolumns = data[1]
        });

	    return {
	        getAllTypes: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllTypes"),
                    localizations.failedtypes
                );
	        },
	        getAllColumns: function (typeAlias) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.pecBaseUrl + "GetAllColumns?typeAlias=" + typeAlias),
                    localizations.failedcolumns
                );
	        }
	    }
	});