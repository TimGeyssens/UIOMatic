angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http) {
	    return {
	        getAll: function (type, sortColumn, sortOrder) {
	            if (sortColumn == undefined)
	                sortColumn = "";
	            if (sortOrder == undefined)
	                sortOrder = "";
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "GetAll?typeName=" + type + "&sortColumn=" + sortColumn + "&sortOrder="+sortOrder);
	        },
	        getAllProperties: function (type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "GetAllProperties?typeName=" + type);
	        },
	        getById: function (type, id) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "GetById?typeName=" + type + "&id=" + id);
	        },
	        getPrimaryKeyColumnName: function(type) {
	            return $http.get(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "GetPrimaryKeyColumnName?typeName=" + type);
	        },
	        create: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToCreate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "PostCreate", angular.toJson(item));
	        },
	        update: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToUpdate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "PostUpdate", angular.toJson(item));
	        },
	        deleteById: function (type, id) {
	            return $http.delete(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "DeleteById?typeOfObject=" + type + "&id=" + id);
	        },
	        validate: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToValidate = object;
	            return $http.post(Umbraco.Sys.ServerVariables.uioMatic.ppcBaseUrl + "Validate", angular.toJson(item));
	        }
	    };
	});