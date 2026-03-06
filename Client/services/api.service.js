(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('apiService', function ($http, API_BASE) {
    function get(url) { return $http.get(API_BASE + url); }
    function post(url, data) { return $http.post(API_BASE + url, data); }
    function put(url, data) { return $http.put(API_BASE + url, data); }
    function del(url) { return $http.delete(API_BASE + url); }
    return { get: get, post: post, put: put, delete: del };
  });
})();
