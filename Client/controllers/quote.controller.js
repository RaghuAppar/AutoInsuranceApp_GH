(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('QuoteController', function (vehicleService, quoteService, $location) {
    var vm = this;
    vm.vehicles = [];
    vm.selectedIds = [];
    vm.deductible = 500;
    vm.liabilityLimit = 100000;
    vm.includeCollision = true;
    vm.includeComprehensive = true;
    vm.quote = null;
    vm.error = '';
    vm.loading = true;
    vm.submitting = false;

    vehicleService.getAll()
      .then(function (r) { vm.vehicles = r.data || []; })
      .catch(function () { vm.vehicles = []; })
      .finally(function () { vm.loading = false; });

    vm.toggleVehicle = function (id) {
      var i = vm.selectedIds.indexOf(id);
      if (i >= 0) vm.selectedIds.splice(i, 1);
      else vm.selectedIds.push(id);
    };

    vm.isSelected = function (id) { return vm.selectedIds.indexOf(id) >= 0; };

    vm.submit = function () {
      vm.error = '';
      if (!vm.selectedIds.length) { vm.error = 'Select at least one vehicle.'; return; }
      vm.submitting = true;
      quoteService.create({
        vehicleIds: vm.selectedIds,
        deductible: vm.deductible,
        liabilityLimit: vm.liabilityLimit,
        includeCollision: vm.includeCollision,
        includeComprehensive: vm.includeComprehensive
      }).then(function (r) {
        vm.quote = r.data;
        vm.quote.created = true;
      }).catch(function (r) { vm.error = (r.data && r.data.message) || 'Failed to get quote.'; })
        .finally(function () { vm.submitting = false; });
    };

    vm.viewQuote = function (id) { $location.path('/quotes/' + id); };
    vm.back = function () { vm.quote = null; };
  });
})();
