angular.module("umbraco").controller("UIOMatic.Views.List",
    function ($scope, uioMaticObjectResource,$routeParams) {

        
        $scope.selectedIds = [];
        $scope.actionInProgress = false;

        $scope.canEdit = $scope.property.Config.canEdit != undefined ? $scope.property.Config.canEdit : true;


        function fetchData() {
            uioMaticObjectResource.getFiltered($scope.property.Config.typeName, $scope.property.Config.foreignKeyColumn, $routeParams.id.split("?")[0], "", "").then(function (response) {
                $scope.rows = response.data;

                if ($scope.rows.length > 0) {
                    $scope.cols = Object.keys($scope.rows[0]).filter(function (c) {
                        return $scope.ignoreColumnsFromListView.indexOf(c) == -1;
                    });
                }
            });
        }
        function init() {
            
            $scope.typeName = $scope.property.Config.typeName;

            uioMaticObjectResource.getType($scope.typeName).then(function (response) {
                //.replace(' ', '_') nasty hack to allow columns with a space
                $scope.primaryKeyColumnName = response.data.PrimaryKeyColumnName.replace(' ', '_');
                $scope.predicate = response.data.PrimaryKeyColumnName.replace(' ', '_');
                $scope.ignoreColumnsFromListView = response.data.IgnoreColumnsFromListView;
                $scope.nameField = response.data.NameField.replace(' ', '_');

                if ($routeParams.id.split("?").length == 2)
                    fetchData();

            });

            
        }

        init();

        $scope.$on('ValuesLoaded', function (event, data) {
            init();
        });

        $scope.getObjectKey = function (object) {
            var keyPropName = $scope.primaryKeyColumnName;
            return object[keyPropName];

        }

        $scope.isColumnLinkable = function (column, index) {

            if ($scope.nameField.length > 0) {
                return column == $scope.nameField;
            } else {

                return index == 0
                || (index == 1 && $scope.cols[0] == $scope.primaryKeyColumnName)
            }
        }

        $scope.toggleSelection = function (val) {
            var idx = $scope.selectedIds.indexOf(val);
            if (idx > -1) {
                $scope.selectedIds.splice(idx, 1);
            } else {
                $scope.selectedIds.push(val);
            }
        }

        $scope.isRowSelected = function (row) {
            var id = $scope.getObjectKey(row);
            return $scope.selectedIds.indexOf(id) > -1;
        }

        $scope.isAnythingSelected = function () {
            return $scope.selectedIds.length > 0;
        }

        $scope.delete = function (object) {
            if (confirm("Are you sure you want to delete " + $scope.selectedIds.length + " object" + ($scope.selectedIds.length > 1 ? "s" : "") + "?")) {
                $scope.actionInProgress = true;
                var keyPropName = $scope.primaryKeyColumnName;
                uioMaticObjectResource.deleteByIds($scope.typeName, $scope.selectedIds).then(function () {
                    $scope.rows = _.reject($scope.rows, function (el) { return $scope.selectedIds.indexOf(el[keyPropName]) > -1; });
                    $scope.selectedIds = [];
                    $scope.actionInProgress = false;
                });
            }
        }
 });