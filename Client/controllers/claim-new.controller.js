(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('ClaimNewController', function (claimService, policyService, $location) {
    var vm = this;
    vm.policies = [];
    vm.model = {
      policyId: null,
      incidentDate: new Date().toISOString().split('T')[0],
      description: '',
      location: '',
      involvedParties: '',
      claimType: 'FirstParty'
    };
    vm.error = '';
    vm.loading = true;
    vm.submitting = false;

    policyService.getAll()
      .then(function (r) { vm.policies = (r.data || []).filter(function (p) { return p.status === 'Active'; }); })
      .catch(function () { vm.policies = []; })
      .finally(function () { vm.loading = false; });

    vm.submit = function () {
      vm.error = '';
      if (!vm.model.description) { vm.error = 'Description is required.'; return; }
      vm.submitting = true;
      claimService.create({
        policyId: vm.model.policyId || null,
        incidentDate: vm.model.incidentDate,
        description: vm.model.description,
        location: vm.model.location || null,
        involvedParties: vm.model.involvedParties || null,
        claimType: vm.model.claimType || 'FirstParty'
      }).then(function (r) { $location.path('/claims/' + r.data.id); })
        .catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to submit claim.'; })
        .finally(function () { vm.submitting = false; });
    };

    vm.back = function () { $location.path('/claims'); };
  });
})();
