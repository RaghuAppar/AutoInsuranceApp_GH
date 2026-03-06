(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('LoginController', function ($location, $rootScope, authService) {
    var vm = this;
    vm.email = '';
    vm.password = '';
    vm.error = '';
    vm.loading = false;

    vm.submit = function () {
      vm.error = '';
      if (!vm.email || !vm.password) { vm.error = 'Please enter email and password.'; return; }
      vm.loading = true;
      authService.login(vm.email, vm.password)
        .then(function () {
          if ($rootScope) $rootScope.currentUser = authService.getCurrentUser();
          $location.path('/dashboard');
        })
        .catch(function (r) {
          vm.error = (r.data && r.data.message) ? r.data.message : 'Login failed.';
        })
        .finally(function () { vm.loading = false; });
    };
  });
})();
