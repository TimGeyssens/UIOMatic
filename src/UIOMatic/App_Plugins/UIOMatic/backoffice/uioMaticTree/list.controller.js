angular.module("umbraco").controller("uioMatic.ObjectListController",
    function($scope, $routeParams, uioMaticObjectResource) {

        $scope.typeName = $routeParams.id;

        //uioMaticObjectResource.getAllProperties($scope.typeName).then(function (response) {
        //    $scope.properties = response.data;
        //});

        uioMaticObjectResource.getType($scope.typeName).then(function (response) {
            $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName;
            $scope.predicate = response.data.PrimaryKeyColumnName;
            $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;

            uioMaticObjectResource.getAll($scope.typeName).then(function (resp) {
                $scope.rows = resp.data;
                $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
                    return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
                });
            });

        });

        

        

        //$scope.predicate = 'Id';
        $scope.reverse = true;
        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
        };

        $scope.getObjectKey = function (object) {
            var keyPropName = $scope.primaryKeyColumnName.replace('"', '').replace('"', '').replace(' ', '_');
            return object[keyPropName];

        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete this object?")) {
                var keyPropName = $scope.primaryKeyColumnName.replace('"', '').replace('"', '').replace(' ', '_');
                uioMaticObjectResource.deleteById($routeParams.id, object[keyPropName]).then(function() {
                    $scope.rows = _.reject($scope.rows, function(el) { return el[keyPropName] === object[keyPropName]; });
                });
            }
        }

    });