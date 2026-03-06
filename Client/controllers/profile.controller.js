(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('ProfileController', function (profileService) {
    var vm = this;
    vm.model = {};
    vm.message = '';
    vm.error = '';
    vm.loading = true;
    vm.saving = false;

    profileService.getMe()
      .then(function (r) {
        var p = r.data;
        vm.model = {
          dateOfBirth: p.dateOfBirth ? p.dateOfBirth.split('T')[0] : null,
          addressLine1: p.addressLine1 || '',
          addressLine2: p.addressLine2 || '',
          city: p.city || '',
          state: p.state || '',
          postalCode: p.postalCode || '',
          licenseNumber: p.licenseNumber || '',
          licenseState: p.licenseState || '',
          licenseExpiry: p.licenseExpiry ? p.licenseExpiry.split('T')[0] : null
        };
      })
      .catch(function () { vm.error = 'Could not load profile.'; })
      .finally(function () { vm.loading = false; });

    vm.save = function () {
      vm.error = ''; vm.message = '';
      vm.saving = true;
      var payload = {
        dateOfBirth: vm.model.dateOfBirth || null,
        addressLine1: vm.model.addressLine1 || null,
        addressLine2: vm.model.addressLine2 || null,
        city: vm.model.city || null,
        state: vm.model.state || null,
        postalCode: vm.model.postalCode || null,
        licenseNumber: vm.model.licenseNumber || null,
        licenseState: vm.model.licenseState || null,
        licenseExpiry: vm.model.licenseExpiry || null
      };
      profileService.updateMe(payload)
        .then(function () { vm.message = 'Profile saved.'; })
        .catch(function (r) { vm.error = (r.data && r.data.message) || 'Save failed.'; })
        .finally(function () { vm.saving = false; });
    };
  });
})();
