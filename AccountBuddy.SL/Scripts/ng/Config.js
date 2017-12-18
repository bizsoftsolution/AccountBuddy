appWA.config(function ($routeProvider) {
    $routeProvider
        .when('/', { templateUrl: './Home/Dashboard', controller: 'Dashboard' })
        .when('/AppMaster', { templateUrl: './Home/AppMaster', controller: 'AppMaster' })
        .when('/Connection', { templateUrl: './Home/Connection', controller: 'Connection' })
        .otherwise({template:'Request Not Valid'})
    ;
    
});