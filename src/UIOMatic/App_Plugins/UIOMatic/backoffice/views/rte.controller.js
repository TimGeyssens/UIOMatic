var app = angular.module("umbraco");

app.controller("UIOMatic.Views.RTE", function ($scope, $http, $routeParams) {

    var rowProps = $scope.property;

    function init() {
        $scope.property = {
            alias: 'UIOMatic.Views.RTE',
            view: 'rte',
            config: {
                editor: {
                    toolbar: ['preview', '|', 'undo', 'redo', '|', 'copy', 'cut', 'paste', '|', 'bold', 'italic', '|', 'link', 'unlink'],
                    stylesheets: [],
                    dimensions: { height: 400, width: '100%' }
                }
            },
            value: rowProps.Value
        };

        $scope.$watch('property', function () {
            if ($scope.property != undefined) {
                var result = $.grep($scope.properties, function (e) { return e.Key === rowProps.Key; });
                if (result != null && result.length > 0) {
                    result[0].Value = $scope.property.value;
                }
            }
        }, true);
    }

    init();

    $scope.$on('ValuesLoaded', function (event, data) {
        init();
    });
});
