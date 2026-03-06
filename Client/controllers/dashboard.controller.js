(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('DashboardController', function (policyService, quoteService, claimService) {
    var vm = this;
    vm.policies = [];
    vm.quotes = [];
    vm.claims = [];
    vm.loading = true;

    policyService.getAll().then(function (r) { vm.policies = r.data || []; }).catch(function () { vm.policies = []; });
    quoteService.getAll().then(function (r) { vm.quotes = r.data || []; }).catch(function () { vm.quotes = []; });
    claimService.getAll().then(function (r) { vm.claims = r.data || []; }).catch(function () { vm.claims = []; })
      .finally(function () { vm.loading = false; });
  });
})();
