angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.MediaController",
    function ($scope, $routeParams, dialogService, entityResource, iconHelper) {

        function init() {

            if (!$scope.setting) {
                $scope.setting = {};
            }

            var val = parseInt($scope.property.value);

            if (!isNaN(val) && angular.isNumber(val) && val > 0) {
                $scope.showQuery = false;

                entityResource.getById(val, "Media").then(function (item) {
                    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                    $scope.node = item;
                });
            }

            $scope.openMediaPicker = function () {
                var d = dialogService.treePicker({
                    section: "media",
                    treeAlias: "media",
                    multiPicker: false,
                    callback: populate
                });
            };


            $scope.clear = function () {
                $scope.id = undefined;
                $scope.node = undefined;
                $scope.property.value = undefined;
            };

            function populate(item) {
                $scope.clear();
                item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                $scope.node = item;
                $scope.id = item.id;
                $scope.property.value = item.id;
            }
        };

        if ($scope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = $scope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }

    });