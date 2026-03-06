(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('claimService', function (apiService) {
    return {
      getAll: function () { return apiService.get('/claims'); },
      getById: function (id) { return apiService.get('/claims/' + id); },
      create: function (data) { return apiService.post('/claims', data); },
      updateStatus: function (id, status, notes) {
        return apiService.put('/claims/' + id + '/status', { status: status, notes: notes || null });
      }
    };
  });
})();
