angular.module("umbraco").controller("UIOMatic.FieldEditors.List",
    function ($scope, $location, uioMaticObjectResource, $routeParams) {

        $scope.filterId = $routeParams.id.split("?")[0];
        $scope.typeAlias = $scope.property.config.typeAlias;
        $scope.foreignKeyColumn = $scope.property.config.foreignKeyColumn;

        $scope.selectedIds = [];
        $scope.actionInProgress = false;
        $scope.canEdit = $scope.property.config.canEdit != undefined ? $scope.property.config.canEdit : true;

        $scope.reverse = false;
        $scope.initialFetch = true;

        function fetchData() {
            if (!isNaN($scope.filterId)) { // Only fetch if we have an ID (ie, editing)
                uioMaticObjectResource.getPaged($scope.typeAlias, 1000, 1,
                    $scope.initialFetch ? "" : $scope.predicate,
                    $scope.initialFetch ? "" : ($scope.reverse ? "desc" : "asc"),
                    $scope.foreignKeyColumn + "|" + $scope.filterId,
                    "").then(function(resp) {
                    $scope.rows = resp.items;
                    $scope.initialFetch = false;
                });
            }
        }

        function init() {
            uioMaticObjectResource.getTypeInfo($scope.typeAlias, true).then(function (response) {
                //.replace(' ', '_') nasty hack to allow columns with a space
                $scope.primaryKeyColumnName = response.primaryKeyColumnName.replace(' ', '_');
                $scope.predicate = response.primaryKeyColumnName.replace(' ', '_');
                $scope.properties = response.listViewProperties;
                $scope.nameField = response.nameFieldKey.replace(' ', '_');
                $scope.readOnly = response.readOnly;

                if ($scope.filterId)
                    fetchData();

            });
        }

        $scope.order = function (predicate) {
            $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
            $scope.predicate = predicate;
            fetchData();
        };

        $scope.getObjectKey = function (object) {
            return object[$scope.primaryKeyColumnName];
        }

        $scope.isColumnLinkable = function (prop, index) {
            if ($scope.nameField.length > 0) {
                return prop.key == $scope.nameField;
            } else {
                return index == 0 || (index == 1 && prop.key == $scope.primaryKeyColumnName);
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
                uioMaticObjectResource.deleteByIds($scope.typeAlias, $scope.selectedIds).then(function () {
                    $scope.rows = _.reject($scope.rows, function (el) { return $scope.selectedIds.indexOf(el[keyPropName]) > -1; });
                    $scope.selectedIds = [];
                    $scope.actionInProgress = false;
                });
            }
        }

        $scope.getReturnUrl = function () {
            var activeTab = $("li[val-tab].active > a");
            var tabId = activeTab.length === 1 ? activeTab.attr("href").replace('#', '') : "";
            var url = $location.url();
            if (url.indexOf("tab=") === -1) {
                url += (url.indexOf("%3F") === -1 ? "%3F" : "%26") + "tab=" + tabId;
            } else {
                url = url.replace(/(tab=)(tab[0-9]*)/g, "$1" + tabId);
            }
            return encodeURIComponent(encodeURIComponent(url));
        }

        $scope.navigate = function (url) {
            // Because some JS seems to be translating any links starting '#'
            $location.url(url);
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