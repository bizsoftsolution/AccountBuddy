appWA.controller('AppMaster', function ($scope,$rootScope) {
    $scope.datas = [];
    $scope.Approved_Change = function (x) {
        console.log("Approved_Change");
        console.log(x);
        $rootScope.hub.server.appApproved_Changed(x.AppId, x.IsApproved).done(function ($r) {
            console.log("Approved_Changed");
            console.log($r);
        });
    };
    if ($.connection.hub.state == 1) {
        $rootScope.hub.server.appMaster_List().done(function ($r) {
            console.log('AppMaster_List');
            console.log($r);
            $scope.datas = $r;
            $rootScope.$digest();
        });
    }
    
});