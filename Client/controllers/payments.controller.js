(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('PaymentsController', function (paymentService) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;

    paymentService.getAll()
      .then(function (r) { vm.list = r.data || []; })
      .catch(function () { vm.list = []; vm.error = 'Failed to load payments.'; })
      .finally(function () { vm.loading = false; });
  });
})();
