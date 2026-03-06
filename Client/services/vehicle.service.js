(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('vehicleService', function (apiService) {
    return {
      getAll: function () { return apiService.get('/vehicles'); },
      getById: function (id) { return apiService.get('/vehicles/' + id); },
      create: function (data) { return apiService.post('/vehicles', data); },
      update: function (id, data) { return apiService.put('/vehicles/' + id, data); },
      remove: function (id) { return apiService.delete('/vehicles/' + id); }
    };
  });
})();
