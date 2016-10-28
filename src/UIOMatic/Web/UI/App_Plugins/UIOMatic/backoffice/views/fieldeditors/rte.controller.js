angular.module("umbraco").controller("UIOMatic.FieldEditors.RTE", function ($scope) {

    var rowProps = $scope.property;

    function init() {
        $scope.property = {
            alias: 'UIOMatic.FieldEditors.RTE',
            view: 'rte',
            config: {
                editor: {
                    toolbar: JSON.parse(Umbraco.Sys.ServerVariables.uioMatic.settings.rteFieldEditorButtons),
                    stylesheets: [],
                    dimensions: { height: 400, width: '100%' }
                }
            },
            value: rowProps.value
        };

        $scope.$watch('property', function () {
            if ($scope.property != undefined) {
                var result = $.grep($scope.properties, function (e) { return e.key === rowProps.key; });
                if (result != null && result.length > 0) {
                    result[0].value = $scope.property.value;
                }
            }
        }, true);
    }

    if ($scope.valuesLoaded) {
        init();
    } else {
        var unsubscribe = $scope.$on('valuesLoaded', function () {
            init();
            unsubscribe();
        });
    }
});
