(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('paymentService', function (apiService) {
    return {
      getAll: function (policyId) {
        var url = '/payments';
        if (policyId) url += '?policyId=' + policyId;
        return apiService.get(url);
      },
      getById: function (id) { return apiService.get('/payments/' + id); },
      create: function (data) { return apiService.post('/payments', data); }
    };
  });
})();
