angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http) {
	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAll?typeAlias=" + type + "&sortColumn=" + sortColumn + "&sortOrder="+sortOrder);
	        },
	        getFilterLookup: function (type, keyPropertyName, valuePropertyName) {
	            if (keyPropertyName == undefined)
	                keyPropertyName = "";
	            if (valuePropertyName == undefined)
	                valuePropertyName = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetFilterLookup?typeAlias=" + type + "&keyPropertyName=" + keyPropertyName + "&valuePropertyName=" + valuePropertyName);
	        },
	        getPaged: function(type, itemsPerPage, pageNumber, sortColumn, sortOrder, filters, searchTerm) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            if (filters == undefined)
	                filters = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetPaged?typeAlias=" + type + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&filters=" + filters + "&searchTerm=" + searchTerm);
	        },
	        getAllProperties: function (type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAllProperties?typeAlias=" + type);
	        },
	        getById: function (type, id) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetById?typeAlias=" + type + "&id=" + id);
	        },
	        getScaffold: function (type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetScaffold?typeAlias=" + type);
	        },
	        getTypeInfo: function(type, includePropertyInfo) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetTypeInfo?typeAlias=" + type + "&includePropertyInfo=" + includePropertyInfo);
	        },
	        create: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Create", angular.toJson(item));
	        },
	        update: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Update", angular.toJson(item));
	        },
	        deleteByIds: function (type, idsArr) {
	            return $http.delete(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "DeleteByIds?typeAlias=" + type + "&ids=" + idsArr.join(','));
	        },
	        validate: function (type, object) {
	            var item = {
	                typeAlias: type,
	                value: object
	            };
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Validate", angular.toJson(item));
	        }
	    };
	});