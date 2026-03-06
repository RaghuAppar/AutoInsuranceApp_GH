(function () {
  'use strict';
  // When served from API (port 5000), use relative /api. Otherwise point to API.
  var API_BASE = (window.location.port === '5000' && (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'))
    ? '/api'
    : 'http://localhost:5000/api';

  angular.module('autoInsuranceApp', ['ngRoute'])
    .constant('API_BASE', API_BASE)
    .config(function ($routeProvider, $httpProvider) {
      $routeProvider
        .when('/login', { templateUrl: 'views/login.html', controller: 'LoginController', controllerAs: 'vm' })
        .when('/register', { templateUrl: 'views/register.html', controller: 'RegisterController', controllerAs: 'vm' })
        .when('/dashboard', { templateUrl: 'views/dashboard.html', controller: 'DashboardController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/profile', { templateUrl: 'views/profile.html', controller: 'ProfileController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/vehicles', { templateUrl: 'views/vehicles.html', controller: 'VehiclesController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/drivers', { templateUrl: 'views/drivers.html', controller: 'DriversController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/quote/new', { templateUrl: 'views/quote-new.html', controller: 'QuoteController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/quotes', { templateUrl: 'views/quotes.html', controller: 'QuotesController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/quotes/:id', { templateUrl: 'views/quote-detail.html', controller: 'QuoteDetailController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/policies', { templateUrl: 'views/policies.html', controller: 'PoliciesController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/policies/:id', { templateUrl: 'views/policy-detail.html', controller: 'PolicyDetailController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/claims', { templateUrl: 'views/claims.html', controller: 'ClaimsController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/claims/new', { templateUrl: 'views/claim-new.html', controller: 'ClaimNewController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/claims/:id', { templateUrl: 'views/claim-detail.html', controller: 'ClaimDetailController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .when('/payments', { templateUrl: 'views/payments.html', controller: 'PaymentsController', controllerAs: 'vm', resolve: { auth: ['authService', function (authService) { return authService.requireAuth(); }] } })
        .otherwise({ redirectTo: '/dashboard' });

      $httpProvider.interceptors.push('authInterceptor');
    })
    .factory('authInterceptor', function ($location) {
      var STORAGE_KEY = 'autoInsuranceAuth';
      return {
        request: function (config) {
          try {
            var raw = localStorage.getItem(STORAGE_KEY);
            if (raw) {
              var d = JSON.parse(raw);
              if (d && d.token) {
                config.headers = config.headers || {};
                config.headers.Authorization = 'Bearer ' + d.token;
              }
            }
          } catch (e) {}
          return config;
        },
        responseError: function (response) {
          if (response.status === 401) {
            try { localStorage.removeItem(STORAGE_KEY); } catch (e) {}
            $location.path('/login');
          }
          return Promise.reject(response);
        }
      };
    })
    .run(function ($rootScope, $location, authService) {
      $rootScope.currentUser = authService.getCurrentUser();
      $rootScope.isPublicRoute = function () {
        var path = $location.path();
        return path === '/login' || path === '/register';
      };
      $rootScope.isActive = function (path) {
        var p = $location.path();
        if (path === '/dashboard') return p === '/dashboard';
        if (path === '/quote') return p.indexOf('/quote') === 0;
        return p && p.indexOf(path) === 0;
      };
      $rootScope.logout = function () {
        authService.clearAuth();
        $location.path('/login');
      };
    });
})();
