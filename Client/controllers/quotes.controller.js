(function () {
  'use strict';
  angular.module('autoInsuranceApp').controller('QuotesController', function (quoteService, $location) {
    var vm = this;
    vm.list = [];
    vm.error = '';
    vm.loading = true;

    quoteService.getAll()
      .then(function (r) { vm.list = r.data || []; })
      .catch(function () { vm.list = []; vm.error = 'Failed to load quotes.'; })
      .finally(function () { vm.loading = false; });

    vm.view = function (id) { $location.path('/quotes/' + id); };
    vm.getNew = function () { $location.path('/quote/new'); };
  });
})();
