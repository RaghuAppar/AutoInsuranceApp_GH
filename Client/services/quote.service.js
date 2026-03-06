(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('quoteService', function (apiService) {
    return {
      getAll: function () { return apiService.get('/quotes'); },
      getById: function (id) { return apiService.get('/quotes/' + id); },
      create: function (data) { return apiService.post('/quotes', data); }
    };
  });
})();
