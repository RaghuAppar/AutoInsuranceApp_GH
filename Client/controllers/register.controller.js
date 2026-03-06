(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('RegisterController', function ($location, $rootScope, authService) {
    var vm = this;
    vm.email = '';
    vm.password = '';
    vm.fullName = '';
    vm.phone = '';
    vm.error = '';
    vm.loading = false;

    vm.submit = function () {
      vm.error = '';
      if (!vm.email || !vm.password || !vm.fullName) {
        vm.error = 'Please enter email, password, and full name.';
        return;
      }
      vm.loading = true;
      authService.register(vm.email, vm.password, vm.fullName, vm.phone)
        .then(function () {
          if ($rootScope) $rootScope.currentUser = authService.getCurrentUser();
          $location.path('/dashboard');
        })
        .catch(function (r) {
          vm.error = (r.data && r.data.message) ? r.data.message : 'Registration failed.';
        })
        .finally(function () { vm.loading = false; });
    };
  });
})();
