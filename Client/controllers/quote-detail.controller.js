(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('QuoteDetailController', function (quoteService, policyService, $routeParams, $location) {
    var vm = this;
    vm.quote = null;
    vm.error = '';
    vm.loading = true;
    vm.purchasing = false;
    vm.paymentPlan = 'Full';

    quoteService.getById($routeParams.id)
      .then(function (r) { vm.quote = r.data; })
      .catch(function () { vm.error = 'Quote not found.'; })
      .finally(function () { vm.loading = false; });

    vm.purchase = function () {
      if (!vm.quote || vm.quote.status !== 'Completed') return;
      vm.purchasing = true;
      vm.error = '';
      policyService.purchase(vm.quote.id, vm.paymentPlan, null)
        .then(function (r) { $location.path('/policies/' + r.data.id); })
        .catch(function (r) { vm.error = (r.data && r.data.message) || 'Purchase failed.'; })
        .finally(function () { vm.purchasing = false; });
    };

    vm.back = function () { $location.path('/quotes'); };
  });
})();
