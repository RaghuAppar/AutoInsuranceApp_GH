(function () {
  'use strict';
  angular.module('autoInsuranceApp').factory('authService', function ($http, $q, $location, API_BASE) {
    var STORAGE_KEY = 'autoInsuranceAuth';
    var currentUser = null;

    function getStored() {
      try {
        var raw = localStorage.getItem(STORAGE_KEY);
        if (raw) return JSON.parse(raw);
      } catch (e) {}
      return null;
    }

    function setStored(data) {
      try {
        if (data) localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
        else localStorage.removeItem(STORAGE_KEY);
      } catch (e) {}
    }

    function getToken() {
      var d = getStored();
      return d && d.token ? d.token : null;
    }

    function getCurrentUser() {
      if (currentUser) return currentUser;
      var d = getStored();
      if (d) {
        currentUser = { fullName: d.fullName, email: d.email, role: d.role, userId: d.userId };
        return currentUser;
      }
      return null;
    }

    function setAuth(response) {
      var data = {
        token: response.token,
        fullName: response.fullName,
        email: response.email,
        role: response.role,
        userId: response.userId,
        expiresAt: response.expiresAt
      };
      setStored(data);
      currentUser = { fullName: data.fullName, email: data.email, role: data.role, userId: data.userId };
    }

    function clearAuth() {
      setStored(null);
      currentUser = null;
    }

    function register(email, password, fullName, phone) {
      return $http.post(API_BASE + '/auth/register', {
        email: email,
        password: password,
        fullName: fullName,
        phone: phone || null
      }).then(function (r) {
        setAuth(r.data);
        return r.data;
      });
    }

    function login(email, password) {
      return $http.post(API_BASE + '/auth/login', { email: email, password: password })
        .then(function (r) {
          setAuth(r.data);
          return r.data;
        });
    }

    function requireAuth() {
      if (getToken()) return $q.resolve();
      $location.path('/login');
      return $q.reject();
    }

    return {
      getToken: getToken,
      getCurrentUser: getCurrentUser,
      setAuth: setAuth,
      clearAuth: clearAuth,
      register: register,
      login: login,
      requireAuth: requireAuth
    };
  });
})();
