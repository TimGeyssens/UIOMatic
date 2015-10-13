angular.module("umbraco").controller("uioMatic.ObjectListController",
    function($scope, $routeParams, uioMaticObjectResource) {

        $scope.typeName = $routeParams.id;

        //uioMaticObjectResource.getAllProperties($scope.typeName).then(function (response) {
        //    $scope.properties = response.data;
        //});

        uioMaticObjectResource.getPrimaryKeyColumnName($scope.typeName).then(function (response) {
            $scope.primaryKeyColumnName = response.data;
            $scope.predicate = response.data;

            uioMaticObjectResource.getAll($scope.typeName).then(function (resp) {
                $scope.rows = resp.data;
                $scope.cols = Object.keys($scope.rows[0]);
            });

        });

        

        

        //$scope.predicate = 'Id';
        $scope.reverse = true;
        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
        };

        $scope.getObjectKey = function (object) {
            keyPropName = $scope.primaryKeyColumnName.replace('"', '').replace('"', '').replace(' ', '_');
            return object[keyPropName];

        }

    });