(function () {
  'use strict';

  describe('authService', function () {
    var authService;
    var $httpBackend;
    var $location;
    var $q;
    var API_BASE;

    beforeEach(module('autoInsuranceApp'));
    beforeEach(inject(function (_authService_, _$httpBackend_, _$location_, _$q_, _API_BASE_) {
      authService = _authService_;
      $httpBackend = _$httpBackend_;
      $location = _$location_;
      $q = _$q_;
      API_BASE = _API_BASE_;
    }));

    afterEach(function () {
      authService.clearAuth();
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();
    });

    it('should exist and expose methods', function () {
      expect(authService).toBeDefined();
      expect(authService.getToken).toBeDefined();
      expect(authService.getCurrentUser).toBeDefined();
      expect(authService.setAuth).toBeDefined();
      expect(authService.clearAuth).toBeDefined();
      expect(authService.register).toBeDefined();
      expect(authService.login).toBeDefined();
      expect(authService.requireAuth).toBeDefined();
    });

    it('getToken should return null when no stored auth', function () {
      authService.clearAuth();
      expect(authService.getToken()).toBeNull();
    });

    it('getCurrentUser should return null when no stored auth', function () {
      authService.clearAuth();
      expect(authService.getCurrentUser()).toBeNull();
    });

    it('setAuth should store token and user; getToken and getCurrentUser return values', function () {
      var response = {
        token: 'jwt123',
        fullName: 'Test User',
        email: 'test@example.com',
        role: 'Customer',
        userId: 1,
        expiresAt: new Date().toISOString()
      };
      authService.setAuth(response);
      expect(authService.getToken()).toBe('jwt123');
      var user = authService.getCurrentUser();
      expect(user).toBeDefined();
      expect(user.fullName).toBe('Test User');
      expect(user.email).toBe('test@example.com');
      expect(user.role).toBe('Customer');
      expect(user.userId).toBe(1);
    });

    it('clearAuth should clear stored auth', function () {
      authService.setAuth({ token: 'x', fullName: 'U', email: 'u@u.com', role: 'Customer', userId: 1, expiresAt: new Date().toISOString() });
      authService.clearAuth();
      expect(authService.getToken()).toBeNull();
      expect(authService.getCurrentUser()).toBeNull();
    });

    it('register should POST to /auth/register and set auth on success', function () {
      var payload = { email: 'r@test.com', password: 'Pass1!', fullName: 'Register User', phone: null };
      var response = { token: 'jwt', fullName: payload.fullName, email: payload.email, role: 'Customer', userId: 1, expiresAt: new Date().toISOString() };
      $httpBackend.expectPOST(API_BASE + '/auth/register', payload).respond(200, response);
      var result;
      authService.register(payload.email, payload.password, payload.fullName, payload.phone).then(function (data) {
        result = data;
      });
      $httpBackend.flush();
      expect(result).toEqual(response);
      expect(authService.getToken()).toBe('jwt');
      expect(authService.getCurrentUser().email).toBe(payload.email);
    });

    it('login should POST to /auth/login and set auth on success', function () {
      var payload = { email: 'l@test.com', password: 'Pass1!' };
      var response = { token: 'jwt', fullName: 'User', email: payload.email, role: 'Customer', userId: 1, expiresAt: new Date().toISOString() };
      $httpBackend.expectPOST(API_BASE + '/auth/login', payload).respond(200, response);
      var result;
      authService.login(payload.email, payload.password).then(function (data) {
        result = data;
      });
      $httpBackend.flush();
      expect(result).toEqual(response);
      expect(authService.getToken()).toBe('jwt');
    });

    it('requireAuth should resolve when token exists', function (done) {
      authService.setAuth({ token: 'x', fullName: 'U', email: 'u@u.com', role: 'Customer', userId: 1, expiresAt: new Date().toISOString() });
      spyOn($location, 'path');
      authService.requireAuth().then(function () {
        expect($location.path).not.toHaveBeenCalled();
        done();
      });
    });

    it('requireAuth should reject and redirect to /login when no token', function (done) {
      authService.clearAuth();
      spyOn($location, 'path');
      authService.requireAuth().catch(function () {
        expect($location.path).toHaveBeenCalledWith('/login');
        done();
      });
    });
  });
})();
