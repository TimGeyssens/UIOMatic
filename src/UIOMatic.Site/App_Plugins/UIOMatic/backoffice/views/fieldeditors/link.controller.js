angular.module("umbraco").controller("UIOMatic.FieldEditors.Link",
    function ($scope) {

        var urlExpression = /[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)?/gi;
        var urlRegex = new RegExp(urlExpression);

        function init() {
            load();
        }

        function load(value) {
            if ($scope.model.config.URL && $scope.model.config.URL.match(urlRegex)) {
                $scope.labelValue = $scope.model.config.URL
            }
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