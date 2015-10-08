angular.module("umbraco.resources")
	.factory("uioMaticObjectResource", function ($http) {
	    return {
	        getAll: function (type) {
	            return $http.get("backoffice/UIOMatic/PetaPocoObject/GetAll?typeName=" + type);
	        },
	        getAllProperties: function (type) {
	            return $http.get("backoffice/UIOMatic/PetaPocoObject/GetAllProperties?typeName=" + type);
	        },
	        getById: function (type, id) {
	            return $http.get("backoffice/UIOMatic/PetaPocoObject/GetById?typeName="+type+"&id=" + id);
	        },
	        create: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToCreate = object;
	            return $http.post("backoffice/UIOMatic/PetaPocoObject/PostCreate", angular.toJson(item));
	        },
	        update: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToUpdate = object[0];
	            return $http.post("backoffice/UIOMatic/PetaPocoObject/PostUpdate", angular.toJson(item));
	        },
	        deleteById: function (type, id) {
                return $http.delete("backoffice/UIOMatic/PetaPocoObject/DeleteById?typeOfObject=" + type + "&id=" + id);
	        },
	        validate: function (type, object) {
	            var item = {};
	            item.typeOfObject = type;
	            item.objectToValidate = object;
	            return $http.post("backoffice/UIOMatic/PetaPocoObject/Validate", angular.toJson(item));
	        }
	    };
	});