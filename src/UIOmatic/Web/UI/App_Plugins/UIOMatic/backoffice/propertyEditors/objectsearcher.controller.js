angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dialogs.Searcher",
	function ($scope, $http, uioMaticObjectResource, $parse) {

	    $scope.typeName = $scope.dialogData.typeName;
	    $scope.selectedIds = [];

	    if ($scope.dialogData.selectedIds) {
	        $scope.selectedIds = $scope.dialogData.selectedIds;
	    }

	    $scope.actionInProgress = false;

	    $scope.currentPage = 1;
	    $scope.itemsPerPage = 25;
	    $scope.totalPages = 1;

	    $scope.searchTerm = "";

	    function fetchData() {

	        uioMaticObjectResource.getPaged($scope.typeName, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", $scope.searchTerm).then(function (resp) {

	            $scope.rows = resp.data.Items;
	            $scope.totalPages = resp.data.TotalPages;
	           
	            if ($scope.rows.length > 0) {
	                $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
	                    return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
	                });
	            }
	        });
	    }

	    uioMaticObjectResource.getType($scope.typeName).then(function (response) {
	        //.replace(' ', '_') nasty hack to allow columns with a space
	        $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
	        $scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
	        $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;
	        $scope.nameField = response.data.NameField.replace(' ', '_');
	        $scope.readOnly = response.data.ReadOnly;
	        fetchData();

	    });

	    $scope.parseTemplate = function (object) {
	        var template = $parse($scope.dialogData.objectTemplate);
	        return template(object);
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
	        console.log(pageNumber);
	        $scope.currentPage = pageNumber;
	        fetchData();
	    };

	    $scope.search = function (searchFilter) {
	        $scope.searchTerm = searchFilter;
	        $scope.currentPage = 1;
	        fetchData();
	    };


	    $scope.getObjectKey = function (object) {
	        var keyPropName = $scope.primaryKeyColumnName;
	        return object[keyPropName];

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

	    $scope.returnSelection = function()
	    {
	        $scope.submit($scope.selectedIds);
	    }
});