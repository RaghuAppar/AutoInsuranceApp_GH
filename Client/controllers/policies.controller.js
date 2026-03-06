(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('PoliciesController', function (policyService, $location) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;

    policyService.getAll()
      .then(function (r) { vm.list = r.data || []; })
      .catch(function () { vm.list = []; vm.error = 'Failed to load policies.'; })
      .finally(function () { vm.loading = false; });

    vm.view = function (id) { $location.path('/policies/' + id); };
  });
})();
