(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('PayController', function (paymentService, policyService, $routeParams, $location) {
    var vm = this;
    vm.policy = null;
    vm.error = '';
    vm.loading = true;
    vm.submitting = false;
    vm.done = false;
    vm.paymentResult = null;

    vm.method = 'Card';
    vm.cardLast4 = '';
    vm.cardExpiry = '';
    vm.cardCvv = '';
    vm.bankCode = '';
    vm.upiId = '';

    vm.banks = [
      { code: 'HDFC', name: 'HDFC Bank' },
      { code: 'ICICI', name: 'ICICI Bank' },
      { code: 'SBI', name: 'State Bank of India' },
      { code: 'AXIS', name: 'Axis Bank' },
      { code: 'KOTAK', name: 'Kotak Mahindra Bank' },
      { code: 'PNB', name: 'Punjab National Bank' }
    ];

    policyService.getById($routeParams.policyId)
      .then(function (r) {
        vm.policy = r.data;
        if (!vm.policy || vm.policy.status !== 'Active') vm.error = 'Policy not found or not active.';
      })
      .catch(function () { vm.error = 'Policy not found.'; })
      .finally(function () { vm.loading = false; });

    vm.submit = function () {
      vm.error = '';
      vm.submitting = true;
      var payload = {
        policyId: vm.policy.id,
        amount: vm.policy.totalPremium,
        paymentMethod: vm.method
      };
      if (vm.method === 'Card') {
        payload.cardLast4 = (vm.cardLast4 || '').trim();
        payload.cardExpiry = (vm.cardExpiry || '').trim();
        payload.cardCvv = (vm.cardCvv || '').trim();
      } else if (vm.method === 'NetBanking') {
        payload.bankCode = vm.bankCode;
      } else if (vm.method === 'UPI') {
        payload.upiId = (vm.upiId || '').trim();
      }

      paymentService.process(payload)
        .then(function (r) {
          vm.paymentResult = r.data;
          vm.done = true;
        })
        .catch(function (r) {
          vm.error = (r.data && r.data.message) || 'Payment failed.';
        })
        .finally(function () { vm.submitting = false; });
    };

    vm.backToPolicy = function () { $location.path('/policies/' + vm.policy.id); };
    vm.goToPayments = function () { $location.path('/payments'); };
  });
})();
