angular.module("umbraco").controller("UIOMatic.Views.RTE",
    function ($scope, $http, $routeParams) {

        if ($routeParams.id.indexOf('?') > -1) {

            $http({ method: 'GET', url: 'backoffice/RTE/RTEApi/getDescription', params: { id: $routeParams.id.split('?')[0] } })
                    .success(function (data) {

                        var rowProps = $scope.property;

                        $scope.property = {
                            view: 'rte',
                            config: {
                                editor: {
                                    toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                                    stylesheets: [],
                                    dimensions: { height: 400, width: 640 }
                                }
                            },
                            value: JSON.parse(data)
                        };

                        $scope.property.Value = JSON.parse(data);

                        $scope.$watch('property', function () {
                            if ($scope.property != undefined) {
                                var result = $.grep($scope.properties, function (e) { return e.Key === rowProps.Key; });
                                if (result != null && result.length > 0) {
                                    result[0].Value = $scope.property.value;
                                }

                            }
                        }, true);

                    })
                    .error(function () {
                        $scope.error = "An Error has occured while loading!";
                    });
        } else {

            var rowProps = $scope.property;

            $scope.property = {
                view: 'rte',
                config: {
                    editor: {
                        toolbar: ["code", "undo", "redo", "cut", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbmacro", "table", "umbembeddialog"],
                        stylesheets: [],
                        dimensions: { height: 400, width: 640 }
                    }
                },
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


    });
