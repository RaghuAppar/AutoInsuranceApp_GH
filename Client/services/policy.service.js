(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('policyService', function (apiService) {
    return {
      getAll: function () { return apiService.get('/policies'); },
      getById: function (id) { return apiService.get('/policies/' + id); },
      purchase: function (quoteId, paymentPlan, paymentToken) {
        return apiService.post('/policies/purchase', {
          quoteId: quoteId,
          paymentPlan: paymentPlan || 'Full',
          paymentToken: paymentToken || null
        });
      },
      cancel: function (id) { return apiService.post('/policies/' + id + '/cancel'); }
    };
  });
})();
