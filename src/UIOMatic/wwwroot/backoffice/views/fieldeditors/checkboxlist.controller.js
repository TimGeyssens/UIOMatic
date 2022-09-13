angular.module("umbraco").controller("UIOMatic.FieldEditors.Checkboxlist",
    function ($scope, $interpolate, uioMaticObjectResource) {

        $scope.delimiter = ",";

        if ($scope.model.config.delimiter)
            $scope.delimiter = $scope.model.config.delimiter;

        $scope.selectedValues = [];

        function updateSelected() {
            if ($scope.model.value) {
                $scope.selectedValues = $scope.model.value.toString().split($scope.delimiter);
            }
        }

        updateSelected();

        function init() {
            uioMaticObjectResource.getAll($scope.model.config.typeAlias, $scope.model.config.sortColumn, "asc").then(function (response) {
                $scope.items = response.map(function(itm) {
                    var item = {
                        value: itm[$scope.model.config.valueColumn],
                        text: $interpolate($scope.model.config.textTemplate)(itm)
                    }
                    item.selected = _.indexOf($scope.selectedValues, item.value.toString()) > -1;
                    return item;
                });
            });
        }

        $scope.setValue = function() {
            var val = [];

            angular.forEach($scope.items, function (itm) {
                if (itm.selected)
                    val.push(itm.value);
            });

            $scope.model.value = val.join($scope.delimiter);
        }

        var appScope = $scope;
        while (appScope && typeof appScope === 'object' && typeof appScope.currentSection === 'undefined') appScope = appScope.$parent;

        if (appScope.valuesLoaded) {
            updateSelected();
            init();
        } else {
            var unsubscribe = appScope.$on('valuesLoaded', function () {
                updateSelected();
                init();
                unsubscribe();
            });
        }

    });
