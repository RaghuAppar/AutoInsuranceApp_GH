(function () {
  'use strict';

  describe('apiService', function () {
    var apiService;
    var $httpBackend;
    var API_BASE;

    beforeEach(module('autoInsuranceApp'));
    beforeEach(inject(function (_apiService_, _$httpBackend_, _API_BASE_) {
      apiService = _apiService_;
      $httpBackend = _$httpBackend_;
      API_BASE = _API_BASE_;
    }));

    afterEach(function () {
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();
    });

    it('should exist', function () {
      expect(apiService).toBeDefined();
      expect(apiService.get).toBeDefined();
      expect(apiService.post).toBeDefined();
      expect(apiService.put).toBeDefined();
      expect(apiService.delete).toBeDefined();
    });

    it('get should call GET with API_BASE + url', function () {
      $httpBackend.expectGET(API_BASE + '/quotes').respond(200, []);
      apiService.get('/quotes');
      $httpBackend.flush();
    });

    it('post should call POST with API_BASE + url and data', function () {
      var payload = { email: 'a@b.com', password: 'p' };
      $httpBackend.expectPOST(API_BASE + '/auth/login', payload).respond(200, {});
      apiService.post('/auth/login', payload);
      $httpBackend.flush();
    });

    it('put should call PUT with API_BASE + url and data', function () {
      var payload = { name: 'Test' };
      $httpBackend.expectPUT(API_BASE + '/profile', payload).respond(200, {});
      apiService.put('/profile', payload);
      $httpBackend.flush();
    });

    it('delete should call DELETE with API_BASE + url', function () {
      $httpBackend.expectDELETE(API_BASE + '/quotes/1').respond(204);
      apiService.delete('/quotes/1');
      $httpBackend.flush();
    });
  });
})();
