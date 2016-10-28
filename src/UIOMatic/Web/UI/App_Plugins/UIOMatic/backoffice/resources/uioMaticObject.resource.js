angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http, umbRequestHelper) {
	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAll?typeAlias=" + type + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder),
                    'Failed to get all'
                );
	        },
	        getFilterLookup: function (type, keyPropertyName, valuePropertyName) {
	            if (keyPropertyName == undefined)
	                keyPropertyName = "";
	            if (valuePropertyName == undefined)
	                valuePropertyName = "";
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetFilterLookup?typeAlias=" + type + "&keyPropertyName=" + keyPropertyName + "&valuePropertyName=" + valuePropertyName),
                    'Failed to retrieve filter lookups'
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
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetPaged?typeAlias=" + type + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&filters=" + filters + "&searchTerm=" + searchTerm),
                    'Failed to get paged'
                );
	        },
	        getAllProperties: function (type) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAllProperties?typeAlias=" + type),
                    'Failed to get all properties'
                );
	        },
	        getById: function (type, id) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetById?typeAlias=" + type + "&id=" + id),
                    'Failed to get by id'
                );
	        },
	        getScaffold: function (type) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetScaffold?typeAlias=" + type),
                    'Failed to get scaffold'
                );
	        },
	        getTypeInfo: function(type, includePropertyInfo) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetTypeInfo?typeAlias=" + type + "&includePropertyInfo=" + includePropertyInfo),
                    'Failed to get type info'
                );
	        },
	        create: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return umbRequestHelper.resourcePromise(
                    $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Create", angular.toJson(item)),
                    'Failed to create entity'
                );
	        },
	        update: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return umbRequestHelper.resourcePromise(
                    $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Update", angular.toJson(item)),
                    'Failed to update entity'
                );
	        },
	        deleteByIds: function (type, idsArr) {
	            return umbRequestHelper.resourcePromise(
                    $http.delete(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "DeleteByIds?typeAlias=" + type + "&ids=" + idsArr.join(',')),
                    'Failed to delete'
                );
	        },
	        validate: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
                return umbRequestHelper.resourcePromise(
                    $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Validate", angular.toJson(item)),
                    'Failed to validate'
                );
	        },
	        getSummaryDashboardTypes: function () {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetSummaryDashboardTypes"),
                    'Failed to get summary dashboard types'
                );
	        },
	        getTotalRecordCount: function (type) {
	            return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetTotalRecordCount?typeAlias=" + type),
                    'Failed to get total record count'
                );
	        }
	    };
	});