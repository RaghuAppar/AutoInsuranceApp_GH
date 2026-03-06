(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('ClaimsController', function (claimService, $location) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;

    claimService.getAll()
      .then(function (r) { vm.list = r.data || []; })
      .catch(function () { vm.list = []; vm.error = 'Failed to load claims.'; })
      .finally(function () { vm.loading = false; });

    vm.view = function (id) { $location.path('/claims/' + id); };
    vm.newClaim = function () { $location.path('/claims/new'); };
  });
})();
