angular.module("umbraco").controller("UIOMatic.FieldEditors.Link",
    function ($scope) {

        var urlExpression = /[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)?/gi;
        var urlRegex = new RegExp(urlExpression);

        function init() {
            load();
        }

        function load(value) {
            if ($scope.property.config.URL && $scope.property.config.URL.match(urlRegex)) {
                $scope.labelValue = $scope.property.config.URL
            }
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