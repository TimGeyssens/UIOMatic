angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.ContentController",
    function ($scope, $routeParams, $http, editorService, entityResource, iconHelper) {

        function init() {
            if (!$scope.setting) {
                $scope.setting = {};
            }

            var val = parseInt($scope.property.value);

            if (!isNaN(val) && angular.isNumber(val) && val >0) {
                $scope.showQuery = false;

                entityResource.getById(val, "Document").then(function (item) {
                    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                    $scope.node = item;
                });
            }

            $scope.openContentPicker = function () {
                editorService.treePicker({
                    section: "content",
                    treeAlias: "content",
                    multiPicker: false,
                    filter: function filter(i) {
                        return i.metaData.isContainer == true;
                    },
                    filterCssClass: 'not-allowed',
                    submit: function (model) {
                        populate(model.selection[0]);
                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
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