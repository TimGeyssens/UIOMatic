angular.module("umbraco").controller("UIOMatic.Views.Pickers.ContentController",
    function ($scope, $routeParams, $http, dialogService, entityResource, iconHelper) {

        function init() {
            if (!$scope.setting) {
                $scope.setting = {};
            }

            var val = parseInt($scope.property.Value);

            if (!isNaN(val) && angular.isNumber(val)) {
                $scope.showQuery = false;

                entityResource.getById(val, "Document").then(function (item) {
                    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                    $scope.node = item;
                });
            }

            $scope.openContentPicker = function () {
                var d = dialogService.treePicker({
                    section: "content",
                    treeAlias: "content",
                    multiPicker: false,
                    callback: populate
                });
            };

            $scope.clear = function () {
                $scope.id = undefined;
                $scope.node = undefined;
                $scope.property.Value = undefined;
            };

            function populate(item) {
                $scope.clear();
                item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                $scope.node = item;
                $scope.id = item.id;
                $scope.property.Value = item.id;
            }
        };

        init();

        $scope.$on('ValuesLoaded', function (event, data) {
            init();
        });
    });