angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dialogs.Searcher",
	function ($scope, $interpolate, uioMaticObjectResource) {

	    $scope.typeAlias = $scope.dialogData.typeAlias;
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
	        uioMaticObjectResource.getPaged($scope.typeAlias, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", null, $scope.searchTerm).then(function (resp) {
	            $scope.items = resp.items.map(function(itm) {
                    return {
                        id: itm[$scope.primaryKeyColumnName],
                        text: $interpolate($scope.dialogData.textTemplate)(itm)
                    }
	            });
	            $scope.totalPages = resp.totalPages;
	        });
	    }

	    uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
	        //.replace(' ', '_') nasty hack to allow columns with a space
	        $scope.primaryKeyColumnName = response.primaryKeyColumnName.replace(' ', '_');
	        $scope.predicate = response.primaryKeyColumnName.replace(' ', '_');
	        $scope.nameField = response.nameFieldKey.replace(' ', '_');
	        $scope.readOnly = response.readOnly;
	        fetchData();

	    });

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

	    $scope.toggleSelection = function (item) {
	        var idx = $scope.selectedIds.indexOf(item.id);
	        if (idx > -1) {
	            $scope.selectedIds.splice(idx, 1);
	        } else {
	            $scope.selectedIds.push(item.id);
	        }
	    }

	    $scope.isSelected = function (item) {
	        return $scope.selectedIds.indexOf(item.id) > -1;
	    }

	    $scope.returnSelection = function () {
	        $scope.submit($scope.selectedIds);
	    }
	});