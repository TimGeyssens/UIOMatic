angular.module("umbraco").controller("UIOMatic.FieldEditors.Pickers.ObjectController",
    function ($scope, $routeParams, $interpolate, $http, editorService, uioMaticObjectResource) {

        $scope.maxItems = $scope.model.config.maxItems || 0;

        $scope.openDialog = function () {
            editorService.open({
                view: '/App_Plugins/UIOMatic/backoffice/views/dialogs/objectsearcher.html',
                show: true,
                callback: function (selectedIds) {
                    $scope.selectedIds = selectedIds;
                    getFullDetails();
                },
                dialogData: {
                    maxItems: $scope.maxItems > 0 ? $scope.maxItems - $scope.selectedIds.length : 0,
                    typeAlias: $scope.model.config.typeAlias,
                    textTemplate: $scope.model.config.textTemplate,
                    selectedIds: $scope.selectedIds
                },
                submit: function (selectedIds) {
                    $scope.selectedIds = selectedIds;
                    getFullDetails();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        }

        $scope.remove = function (index) {
            $scope.items.splice(index, 1);
            $scope.selectedIds.splice(index, 1);
        }

        function getFullDetails() {
            $scope.items = [];
            if ($scope.selectedIds.length > 0) {
                angular.forEach($scope.selectedIds, function (id, idx) {
                    uioMaticObjectResource.getById($scope.model.config.typeAlias, id).then(function (resp) {
                        $scope.items.splice(idx, 0, {
                            id: id,
                            text: $interpolate($scope.model.config.textTemplate)(resp)
                        });
                    });
                });
            }
        }

        function init() {
            $scope.selectedIds = $scope.model.value ? $scope.model.value.toString().split(',') : [];

            getFullDetails();

            $scope.$watch("selectedIds", function () {
                $scope.model.value = $scope.selectedIds ? $scope.selectedIds.join() : undefined;
            }, true);

            // Watch items for sorting
            $scope.$watch(function () {
                return _.map($scope.items, function (i) {
                    return i.id;
                }).join();
            }, function () {
                $scope.selectedIds = _.map($scope.items, function (i) {
                    return i.id;
                });
            });
        }

        var appScope = $scope;
        while (appScope && typeof appScope === 'object' && typeof appScope.currentSection === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded) {
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                init();
                unsubscribe();
            });
        }
    });