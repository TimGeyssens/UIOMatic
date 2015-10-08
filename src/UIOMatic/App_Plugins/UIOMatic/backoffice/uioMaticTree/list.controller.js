angular.module("umbraco").controller("uioMatic.ObjectListController",
    function($scope, $routeParams, uioMaticObjectResource) {

        $scope.typeName = $routeParams.id;

        //uioMaticObjectResource.getAllProperties($scope.typeName).then(function (response) {
        //    $scope.properties = response.data;
        //});

        uioMaticObjectResource.getAll($scope.typeName).then(function (response) {
            $scope.rows = response.data;
            $scope.cols = Object.keys($scope.rows[0]);
        });

        uioMaticObjectResource.getPrimaryKeyColumnName($scope.typeName).then(function (response) {
            $scope.primaryKeyColumnName = response.data;
            $scope.predicate = response.data;
        });

        //$scope.predicate = 'Id';
        $scope.reverse = true;
        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
        };

        $scope.getObjectKey = function (object) {
            //console.log($scope.primaryKeyColumnName);
            return object["Id"];
            //return object[$scope.primaryKeyColumnName];
        }

    });