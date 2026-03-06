(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('PolicyDetailController', function (policyService, $routeParams, $location) {
    var vm = this;
    vm.policy = null;
    vm.error = '';
    vm.loading = true;
    vm.cancelling = false;

    policyService.getById($routeParams.id)
      .then(function (r) { vm.policy = r.data; })
      .catch(function () { vm.error = 'Policy not found.'; })
      .finally(function () { vm.loading = false; });

    vm.cancel = function () {
      if (!confirm('Cancel this policy? This action cannot be undone.')) return;
      vm.cancelling = true;
      vm.error = '';
      policyService.cancel(vm.policy.id)
        .then(function (r) { vm.policy = r.data; })
        .catch(function (r) { vm.error = (r.data && r.data.message) || 'Cancel failed.'; })
        .finally(function () { vm.cancelling = false; });
    };

    vm.back = function () { $location.path('/policies'); };
  });
})();
