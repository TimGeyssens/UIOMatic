angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http) {
	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            //return this.getPaged(type, 1, 1, sortColumn, sortOrder);
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAll?typeName=" + type + "&sortColumn=" + sortColumn + "&sortOrder="+sortOrder);
	        },
	        getFiltered: function (type,filterColumn, filterValue, sortColumn, sortOrder) {

	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetFiltered?typeName=" + type + "&filterColumn=" + filterColumn + "&filterValue=" + filterValue + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder);
	        },
	        getPaged: function(type, itemsPerPage, pageNumber, sortColumn, sortOrder,searchTerm) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetPaged?typeName=" + type + "&itemsPerPage=" + itemsPerPage + "&pageNumber=" + pageNumber + "&sortColumn=" + sortColumn + "&sortOrder=" + sortOrder + "&searchTerm=" + searchTerm);
	        },
	        getAllProperties: function (type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetAllProperties?typeName=" + type);
	        },
	        getById: function (type, id) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetById?typeName=" + type + "&id=" + id);
	        },
	        getScaffold: function (type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetScaffold?typeName=" + type);
	        },
	        getType: function(type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "GetType?typeName=" + type);
	        },
	        create: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToCreate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "PostCreate", angular.toJson(item));
	        },
	        update: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToUpdate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "PostUpdate", angular.toJson(item));
	        },
	        deleteByIds: function (type, idsArr) {
	            return $http.delete(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "DeleteByIds?typeOfObject=" + type + "&ids=" + idsArr.join(','));
	        },
	        validate: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToValidate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ocBaseUrl + "Validate", angular.toJson(item));
	        }
	    };
	});