appWA.controller('Login', function ($scope, $rootScope, $http) {

    $scope.Login = function () {
        if (!$scope.LoginId || $scope.LoginId == '') {
            alert('Please Enter the Login Id');
        } else if (!$scope.Password || $scope.Password == '') {
            alert('Please Enter the Password');
        } else {
            $rootScope.hub.server.webLogin($scope.LoginId, $scope.Password).done(function ($r) {
                if ($r == true) {
                    $rootScope.IsWebAdminLogined = true;
                    $rootScope.$digest();
                } else {
                    alert('Invalid user');
                }
            });

        }
    };
});

