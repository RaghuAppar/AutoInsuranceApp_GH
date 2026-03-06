(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('DriversController', function (driverService) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;
    vm.showForm = false;
    vm.model = { fullName: '', dateOfBirth: null, licenseNumber: '', licenseState: '', licenseExpiry: null, isPrimary: false };

    function load() {
      driverService.getAll()
        .then(function (r) { vm.list = r.data || []; })
        .catch(function () { vm.list = []; vm.error = 'Failed to load drivers.'; })
        .finally(function () { vm.loading = false; });
    }
    load();

    vm.add = function () {
      vm.error = '';
      if (!vm.model.fullName) { vm.error = 'Full name is required.'; return; }
      driverService.create({
        fullName: vm.model.fullName,
        dateOfBirth: vm.model.dateOfBirth || null,
        licenseNumber: vm.model.licenseNumber || null,
        licenseState: vm.model.licenseState || null,
        licenseExpiry: vm.model.licenseExpiry || null,
        isPrimary: !!vm.model.isPrimary
      }).then(function () {
        vm.showForm = false;
        vm.model = { fullName: '', dateOfBirth: null, licenseNumber: '', licenseState: '', licenseExpiry: null, isPrimary: false };
        load();
      }).catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to add driver.'; });
    };

    vm.remove = function (id) {
      if (!confirm('Remove this driver?')) return;
      driverService.remove(id).then(function () { load(); }).catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to remove.'; });
    };
  });
})();
