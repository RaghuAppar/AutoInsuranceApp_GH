(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('driverService', function (apiService) {
    return {
      getAll: function () { return apiService.get('/drivers'); },
      getById: function (id) { return apiService.get('/drivers/' + id); },
      create: function (data) { return apiService.post('/drivers', data); },
      update: function (id, data) { return apiService.put('/drivers/' + id, data); },
      remove: function (id) { return apiService.delete('/drivers/' + id); }
    };
  });
})();
