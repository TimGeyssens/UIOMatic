angular.module("umbraco").controller("UIOMatic.FieldEditors.RTE", function ($scope) {
    function init() {
        var rowProps = $scope.property || { value: "" };
        // RTE doesn't like value being undefined, so if it is, set it to empty string
        if (rowProps.value == undefined)
            rowProps.value = "";

        $scope.property = {
            view: 'rte',
            config: {
                editor: {
                    toolbar: JSON.parse(Umbraco.Sys.ServerVariables.uioMatic.settings.rteFieldEditorButtons),
                    stylesheets: [],
                    dimensions: { height: 400 }
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
       $scope.$on('valuesLoaded', function () {
           init();
        });
    }
});
