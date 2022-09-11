angular.module("umbraco").controller("uiomaticContentController",
    function ($scope, editorState) {
        
        $scope.properties = editorState.current.type.editableProperties;
        $scope.queryString = editorState.current.qs;             

        var tabsArr = [];

        angular.forEach($scope.properties, function (value, key) {
            if (this.map(function (e) { return e.label; }).indexOf(value.tab) === -1) {
                if (value.tab == "") {
                    this.push({ id: 99, label: "General" });
                } else {
                    this.push({ id: value.tabOrder > 0 ? value.tabOrder : key, label: value.tab, active: $scope.queryString["tab"] === "tab" + value.tabOrder });
                }
            }

        }, tabsArr);

        if (tabsArr.length > 1) {
            $scope.content = {
                tabs: tabsArr.sort(function (a, b) {
                    if (a.id < b.id)
                        return -1;
                    if (a.id > b.id)
                        return 1;
                    return 0;
                })
            };
        }

    });