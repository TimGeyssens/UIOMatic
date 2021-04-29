angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http, umbRequestHelper, localizationService) {
		let ocBaseUrl = Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl;

		var localizations = {
			failedall: 'Failed to get all',
			failedfilter: 'Failed to retrieve filter lookups',
			failedpaged: 'Failed to get paged',
			failedprops: 'Failed to get all properties',
			failedbyid: 'Failed to get by id',
			failedscaffold: 'Failed to get scaffold',
			failedtypeinfo: 'Failed to get type info',
			failedcreate: 'Failed to create entity',
			failedupdate: 'Failed to update entity',
			faileddelete: 'Failed to delete',
			failedvalidate: 'Failed to validate',
			failedsummary: 'Failed to get summary dashboard types',
			failedtotalcount: 'Failed to get total record count'
		}

		localizationService.localizeMany([
			"resource_failedall",
			"resource_failedfilter",
			"resource_failedpaged",
			"resource_failedprops",
			"resource_failedbyid",
			"resource_failedscaffold",
			"resource_failedtypeinfo",
			"resource_failedcreate",
			"resource_failedupdate",
			"resource_faileddelete",
			"resource_failedvalidate",
			"resource_failedsummary",
			"resource_failedtotalcount"
		]).then(function (data) {
			localizations.failedall = data[0];
			localizations.failedfilter = data[1];
			localizations.failedpaged = data[2];
			localizations.failedprops = data[3];
			localizations.failedbyid = data[4];
			localizations.failedscaffold = data[5];
			localizations.failedtypeinfo = data[6];
			localizations.failedcreate = data[7];
			localizations.failedupdate = data[8];
			localizations.faileddelete = data[9];
			localizations.failedvalidate = data[10];
			localizations.failedsummary = data[11];
			localizations.failedtotalcount = data[12];
		});

	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetAll?typeAlias=" + type + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder),
					localizations.failedall
                );
	        },
	        getFilterLookup: function (type, keyPropertyName, valuePropertyName) {
	            if (keyPropertyName == undefined)
	                keyPropertyName = "";
	            if (valuePropertyName == undefined)
	                valuePropertyName = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetFilterLookup?typeAlias=" + type + "&keyPropertyName=" + keyPropertyName + "&valuePropertyName=" + valuePropertyName),
					localizations.failedfilter
                );
	        },
	        getPaged: function(type, itemsPerPage, pageNumber, sortColumn, sortOrder, filters, searchTerm) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            if (filters == undefined)
	                filters = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetPaged?typeAlias=" + type + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&filters=" + filters + "&searchTerm=" + encodeURIComponent(searchTerm)),
					localizations.failedpaged
                );
	        },
	        getPagedWithNodeId: function(type,nodeId, nodeIdField, itemsPerPage, pageNumber, sortColumn, sortOrder, filters, searchTerm) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            if (filters == undefined)
	                filters = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetPagedWithNodeId?typeAlias=" + type + "&nodeId="+nodeId+ "&nodeIdField="+nodeIdField+ "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&filters=" + filters + "&searchTerm=" + searchTerm),
					localizations.failedpaged
                );
	        },
	        getAllProperties: function (type) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetAllProperties?typeAlias=" + type),
					localizations.failedprops
                );
	        },
	        getById: function (type, id) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetById?typeAlias=" + type + "&id=" + id),
					localizations.failedbyid
                );
	        },
	        getScaffold: function (type) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetScaffold?typeAlias=" + type),
					localizations.failedscaffold
                );
	        },
	        getTypeInfo: function(type, includePropertyInfo) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetTypeInfo?typeAlias=" + type + "&includePropertyInfo=" + includePropertyInfo),
					localizations.failedtypeinfo
                );
	        },
	        create: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return umbRequestHelper.resourcePromise(
                    $http.post(ocBaseUrl + "Create", angular.toJson(item)),
					localizations.failedcreate
                );
	        },
	        update: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return umbRequestHelper.resourcePromise(
                    $http.post(ocBaseUrl + "Update", angular.toJson(item)),
					localizations.failedupdate
                );
	        },
	        deleteByIds: function (type, idsArr) {
	            return umbRequestHelper.resourcePromise(
                    $http.delete(ocBaseUrl + "DeleteByIds?typeAlias=" + type + "&ids=" + idsArr.join(',')),
					localizations.faileddelete
                );
	        },
	        validate: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
                return umbRequestHelper.resourcePromise(
                    $http.post(ocBaseUrl + "Validate", angular.toJson(item)),
					localizations.failedvalidate
                );
	        },
	        getSummaryDashboardTypes: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(ocBaseUrl + "GetSummaryDashboardTypes"),
					localizations.failedsummary
                );
	        },
	        getTotalRecordCount: function (type) {
	            return umbRequestHelper.resourcePromise(
					$http.get(ocBaseUrl + "GetTotalRecordCount?typeAlias=" + type),
					localizations.failedtotalcount
                );
	        }
	    };
	});
