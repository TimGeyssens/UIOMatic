angular.module("umbraco").controller("uioMatic.ObjectListController",
    function($scope, $routeParams, uioMaticObjectResource) {

        $scope.typeName = $routeParams.id;
        $scope.selectedIds = [];
        $scope.actionInProgress = false;


        uioMaticObjectResource.getType($scope.typeName).then(function (response) {
            //.replace(' ', '_') nasty hack to allow columns with a space
            $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
            $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;

            uioMaticObjectResource.getAll($scope.typeName).then(function (resp) {
                $scope.rows = resp.data;
                if ($scope.rows.length > 0) {
                    $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
                        return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
                    });
                }
            });

        });


        $scope.reverse = true;
        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
        };

        $scope.getObjectKey = function (object) {
            var keyPropName = $scope.primaryKeyColumnName;
            return object[keyPropName];

        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.deleteByIds($routeParams.id, $scope.selectedIds).then(function() {
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

    });