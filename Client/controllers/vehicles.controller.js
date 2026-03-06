(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('VehiclesController', function (vehicleService, $location) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;
    vm.showForm = false;
    vm.model = { vin: '', make: '', model: '', year: new Date().getFullYear(), usage: 'Personal', annualMileage: null };

    function load() {
      vehicleService.getAll()
        .then(function (r) { vm.list = r.data || []; })
        .catch(function () { vm.list = []; vm.error = 'Failed to load vehicles.'; })
        .finally(function () { vm.loading = false; });
    }
    load();

    vm.add = function () {
      vm.error = '';
      if (!vm.model.vin || !vm.model.make || !vm.model.model || !vm.model.year) {
        vm.error = 'VIN, Make, Model, and Year are required.';
        return;
      }
      vehicleService.create({
        vin: vm.model.vin,
        make: vm.model.make,
        model: vm.model.model,
        year: parseInt(vm.model.year, 10),
        usage: vm.model.usage || 'Personal',
        annualMileage: vm.model.annualMileage ? parseInt(vm.model.annualMileage, 10) : null
      }).then(function () {
        vm.showForm = false;
        vm.model = { vin: '', make: '', model: '', year: new Date().getFullYear(), usage: 'Personal', annualMileage: null };
        load();
      }).catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to add vehicle.'; });
    };

    vm.remove = function (id) {
      if (!confirm('Remove this vehicle?')) return;
      vehicleService.remove(id).then(function () { load(); }).catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to remove.'; });
    };

    vm.goQuote = function () { $location.path('/quote/new'); };
  });
})();
