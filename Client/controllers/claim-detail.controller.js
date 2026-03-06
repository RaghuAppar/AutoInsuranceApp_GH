(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('ClaimDetailController', function (claimService, $routeParams, $location) {
    var vm = this;
    vm.claim = null;
    vm.error = '';
    vm.loading = true;

    claimService.getById($routeParams.id)
      .then(function (r) { vm.claim = r.data; })
      .catch(function () { vm.error = 'Claim not found.'; })
      .finally(function () { vm.loading = false; });

    vm.back = function () { $location.path('/claims'); };
  });
})();
