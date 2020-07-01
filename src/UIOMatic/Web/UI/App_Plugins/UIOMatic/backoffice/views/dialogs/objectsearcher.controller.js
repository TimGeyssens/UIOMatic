angular.module("umbraco").controller("UIOMatic.PropertyEditors.Dialogs.Searcher",
    function ($scope, $interpolate, $timeout, uioMaticObjectResource) {
        var searchTimeout;

        var vm = this;
        vm.submit = submit;
        vm.close = close;

        function submit() {
            if ($scope.model.submit) {
                $scope.model.submit($scope.model.dialogData.selectedIds);
            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };


        $scope.typeAlias = $scope.model.dialogData.typeAlias;
        $scope.selectedIds = [];

        if ($scope.model.dialogData.selectedIds) {
            $scope.selectedIds = $scope.model.dialogData.selectedIds;
        }

        $scope.actionInProgress = false;

        $scope.currentPage = 1;
        $scope.itemsPerPage = 25;
        $scope.totalPages = 1;

        $scope.searchTerm = "";

        function fetchData() {
            uioMaticObjectResource.getPaged($scope.typeAlias, $scope.itemsPerPage, $scope.currentPage, $scope.predicate, $scope.reverse ? "desc" : "asc", null, $scope.searchTerm).then(function (resp) {
                $scope.items = resp.items.map(function (itm) {
                    return {
                        id: itm[$scope.primaryKeyColumnName],
                        text: $interpolate($scope.model.dialogData.textTemplate)(itm)
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
            $scope.itemIcon = response.itemIcon;
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
            if (searchTimeout) { // if there is already a timeout in process cancel it
                $timeout.cancel(searchTimeout);
            }
            searchTimeout = $timeout(function () {
                $scope.searchTerm = searchFilter;
                $scope.currentPage = 1;
                fetchData();
                searchTimeout = null;
            }, 1000);
        };

        $scope.toggleSelection = function (item) {
            var idx = $scope.selectedIds.indexOf(item.id);
            if (idx > -1) {
                $scope.selectedIds.splice(idx, 1);
            } else if ($scope.model.dialogData.maxItems == 0 || $scope.selectedIds.length < $scope.model.dialogData.maxItems) {
                $scope.selectedIds.push(item.id);
            }
        }

        $scope.isSelected = function (item) {
            if (!$scope.selectedIds) return false;
            return $scope.selectedIds.indexOf(item.id) > -1;
        }

        $scope.returnSelection = function () {
            $scope.submit($scope.selectedIds);
        }
    });