(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('profileService', function (apiService) {
    return {
      getMe: function () { return apiService.get('/profiles/me'); },
      updateMe: function (data) { return apiService.put('/profiles/me', data); }
    };
  });
})();
