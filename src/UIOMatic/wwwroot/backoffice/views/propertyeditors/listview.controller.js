angular.module("umbraco").controller("UIOMatic.PropertyEditors.ListView",
    function ($scope, $routeParams, $location, uioMaticUtilityService, uioMaticObjectResource, navigationService, editorService, notificationsService) {

	    //$scope.model.config.typeAlias
	    //$scope.model.config.sortColumn
	    //$scope.model.config.sortOrder
	    //$scope.model.config.nodeIdSelected 
	    //$scope.model.config.numberOfItems
	    //$scope.model.config.filters
	    //$scope.primaryKeyColumnName
	    $scope.primaryKeyColumnName = "Id";
	    $scope.typeAlias = $scope.model.config.typeAlias;
	    $scope.selectedIds = [];
	    $scope.actionInProgress = false;
	    $scope.hideCreate = $scope.model.config.hideCreate;
	    $scope.hideEdit = $scope.model.config.hideEdit;
	    $scope.hideDelete = $scope.model.config.hideDelete;
	    $scope.hideSearch = $scope.model.config.hideSearch;

	    $scope.currentPage = 1;
	    $scope.itemsPerPage = $scope.model.config.numberOfItems;
	    $scope.totalPages = 1;

	    $scope.legacyPagination = uioMaticUtilityService.useLegacyPaginationControl();

	    $scope.reverse = false;
	  
	    $scope.filtersStr = "";
	    $scope.searchTerm = "";
	    if ($scope.model.config.filters !== undefined) {
	        for (var i = 0; i < $scope.model.config.filters.length; i++) {
	            if ($scope.model.config.filters[i].value.includes("|")) {
	                if (i != 0 && i != $scope.model.config.filters.length) {
	                    $scope.filtersStr += "|";
	                }
	                 $scope.filtersStr += $scope.model.config.filters[i].value;
	             }
	        }

	    }
	      $scope.initialFetch = true;
	    function fetchData() {
	        uioMaticObjectResource.getPagedWithNodeId($scope.typeAlias, $routeParams.id,$scope.model.config.nodeIdSelected, $scope.itemsPerPage, $scope.currentPage,
                $scope.initialFetch ? $scope.model.config.sortColumn : $scope.predicate,
                $scope.initialFetch ? $scope.model.config.sortOrder : ($scope.reverse ? "desc" : "asc"),
                $scope.filtersStr,
                $scope.searchTerm).then(function (resp) {
                    $scope.initialFetch = false;
                    $scope.rows = resp.items;
                    $scope.totalPages = resp.totalPages;
                });
	    }

	    uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {

	        $scope.title = response.displayNamePlural;
	        $scope.primaryKeyColumnName = response.primaryKeyColumnName.replace(' ', '_');
	        $scope.itemDisplayName = response.displayNameSingular;
	        $scope.predicate = response.primaryKeyColumnName.replace(' ', '_');
	        $scope.properties = response.listViewProperties;
	        $scope.nameField = response.nameFieldKey.replace(' ', '_');
	        $scope.readOnly = response.readOnly;
	        $scope.listViewActions = response.listViewActions;
	        $scope.predicate = response.sortColumn;
	        $scope.reverse = response.sortOrder == "desc";
            
	     
	        fetchData();
	    });


	    $scope.order = function (predicate) {
	        $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
	        $scope.predicate = predicate;
	        $scope.currentPage = 1;
	        fetchData();
	    };

	    $scope.getObjectKey = function (object) {
	        //console.log(object);
	        return object[$scope.primaryKeyColumnName];
	    }

	    $scope.delete = function (object) {
	        if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
	            $scope.actionInProgress = true;
	            var keyPropName = $scope.primaryKeyColumnName;
	            uioMaticObjectResource.deleteByIds($scope.typeAlias, $scope.selectedIds).then(function () {
	                $scope.rows = _.reject($scope.rows, function (el) { return $scope.selectedIds.indexOf(el[keyPropName]) > -1; });
	                $scope.selectedIds = [];
	                $scope.actionInProgress = false;
	            });
	        }
	    }

	    $scope.toggleSelection = function (val) {
	        var idx = $scope.selectedIds.indexOf(val);
	        if (idx > -1) {
	            $scope.selectedIds.splice(idx, 1);
	        } else {
	            $scope.selectedIds.push(val);
	        }
	    }

	    $scope.isRowSelected = function (row) {
	        var id = $scope.getObjectKey(row);
	        return $scope.selectedIds.indexOf(id) > -1;
	    }

	    $scope.isAnythingSelected = function () {
	        return $scope.selectedIds.length > 0;
	    }

	    $scope.getNumber = function (num) {
	        return new Array(num);
	    }

	    $scope.prevPage = function () {
	        if ($scope.currentPage > 1) {
	            $scope.currentPage--;
	            fetchData();
	        }
	    };

	    $scope.nextPage = function () {
	        if ($scope.currentPage < $scope.totalPages) {
	            $scope.currentPage++;
	            fetchData();
	        }
	    };

	    $scope.setPage = function (pageNumber) {
	        $scope.currentPage = pageNumber;
	        fetchData();
	    };

	    $scope.search = function (searchFilter) {
	        $scope.searchTerm = searchFilter;
	        $scope.currentPage = 1;
	        fetchData();
	    };

	    $scope.isColumnLinkable = function (prop, index) {
	        if ($scope.hideEdit) {
	            return false;
	        }
	        if ($scope.nameField.length > 0) {
	            return prop.key == $scope.nameField;
	        } else {
	            return index == 0 || (index == 1 && prop.key == $scope.primaryKeyColumnName);
	        }
	    }

	    $scope.unCamelCase = function (str) {
	        return str
                // insert a space between lower & upper
                .replace(/([a-z])([A-Z])/g, '$1 $2')
                // space before last upper in a sequence followed by lower
                .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
                // uppercase the first character
                .replace(/^./, function (str) { return str.toUpperCase(); })
	    }

	    $scope.navigate = function (url) {
	        // Because some JS seems to be translating any links starting '#'
	        $location.url(url);
	    }

	    $scope.openAction = function (action) {
            editorService.open({
	            template: action.view,
	            show: true,
	            dialogData: {
	                typeAlias: $scope.typeAlias
	            }
	        });
	    }

	    $scope.addItem = function () {
            editorService.open({
	            template: '/App_Plugins//UIOMatic/backoffice/views/PropertyEditors/Listview.Dialog.html',
	            dialogData: {
	                id: -1,
	                currentNodeId:$routeParams.id,
	                typeAlias: $scope.model.config.typeAlias,
	                nodeIdSelected: $scope.model.config.nodeIdSelected,
                    mode:"create"
	            },
                callback: function (data) {
	                console.log(data);
	                fetchData();
	            
	            }
	        });
	    };
	    $scope.editItem = function (object) {
            editorService.open({
	            template: '/App_Plugins//UIOMatic/backoffice/views/PropertyEditors/Listview.Dialog.html',
	            dialogData: {
	                id: object,
	                currentNodeId: $routeParams.id,
	                typeAlias: $scope.model.config.typeAlias,
	                nodeIdSelected: $scope.model.config.nodeIdSelected,
	                mode: "edit"
                   
	            },
	            callback: function (data) {
	                console.log(data);
	                fetchData();
	              
	            }
	        });
	    };
     
	});