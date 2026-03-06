(function () {
  'use strict';

  describe('LoginController', function () {
    var $controller;
    var $location;
    var $rootScope;
    var $q;
    var authService;

    beforeEach(module('autoInsuranceApp'));
    beforeEach(inject(function (_$controller_, _$location_, _$rootScope_, _$q_, _authService_) {
      $controller = _$controller_;
      $location = _$location_;
      $rootScope = _$rootScope_;
      $q = _$q_;
      authService = _authService_;
    }));

    function createController() {
      return $controller('LoginController', { $location: $location, $rootScope: $rootScope, authService: authService }, {});
    }

    it('should set initial vm state', function () {
      var vm = createController();
      expect(vm.email).toBe('');
      expect(vm.password).toBe('');
      expect(vm.error).toBe('');
      expect(vm.loading).toBe(false);
    });

    it('submit with empty email/password should set error', function () {
      var vm = createController();
      spyOn(authService, 'login');
      vm.submit();
      expect(vm.error).toBe('Please enter email and password.');
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('submit with valid credentials should call authService.login and redirect on success', function (done) {
      var vm = createController();
      vm.email = 'u@test.com';
      vm.password = 'pass';
      spyOn(authService, 'login').and.returnValue($q.resolve({}));
      spyOn(authService, 'getCurrentUser').and.returnValue({ fullName: 'User', email: vm.email });
      spyOn($location, 'path');
      vm.submit();
      expect(authService.login).toHaveBeenCalledWith('u@test.com', 'pass');
      expect(vm.loading).toBe(true);
      $rootScope.$digest();
      expect($location.path).toHaveBeenCalledWith('/dashboard');
      expect($rootScope.currentUser).toBeDefined();
      done();
    });

    it('submit on login failure should set vm.error', function (done) {
      var vm = createController();
      vm.email = 'u@test.com';
      vm.password = 'wrong';
      spyOn(authService, 'login').and.returnValue($q.reject({ data: { message: 'Invalid credentials' } }));
      vm.submit();
      $rootScope.$digest();
      expect(vm.error).toBe('Invalid credentials');
      done();
    });
  });
})();
