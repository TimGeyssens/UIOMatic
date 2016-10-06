angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http) {
	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            //return this.getPaged(type, 1, 1, sortColumn, sortOrder);
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAll?typeAlias=" + type + "&sortColumn=" + sortColumn + "&sortOrder="+sortOrder);
	        },
	        getFiltered: function (type, filterColumn, filterValue, sortColumn, sortOrder) {

	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetFiltered?typeAlias=" + type + "&filterColumn=" + filterColumn + "&filterValue=" + filterValue + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder);
	        },
	        getPaged: function(type, itemsPerPage, pageNumber, sortColumn, sortOrder,searchTerm) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetPaged?typeAlias=" + type + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
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
	        getType: function(type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetType?typeAlias=" + type);
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