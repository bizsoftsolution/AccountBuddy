var appWA = angular.module("WebAdmin", ['ngRoute']);
appWA.run(function ($rootScope) {
    $rootScope.hub = $.connection.aBServerHub;
    $rootScope.hub.client.ConsoleLog = function (log) {
        console.log(log);
    };
    $rootScope.IsWebAdminLogined = false;
    $.connection.hub.start().done(function () {
        console.log('SignalR Started');
    });
});



