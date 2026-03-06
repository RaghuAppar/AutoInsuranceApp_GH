(function () {
  'use strict';

  describe('quoteService', function () {
    var quoteService;
    var $httpBackend;
    var API_BASE;

    beforeEach(module('autoInsuranceApp'));
    beforeEach(inject(function (_quoteService_, _$httpBackend_, _API_BASE_) {
      quoteService = _quoteService_;
      $httpBackend = _$httpBackend_;
      API_BASE = _API_BASE_;
    }));

    afterEach(function () {
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();
    });

    it('should exist and expose getAll, getById, create', function () {
      expect(quoteService).toBeDefined();
      expect(quoteService.getAll).toBeDefined();
      expect(quoteService.getById).toBeDefined();
      expect(quoteService.create).toBeDefined();
    });

    it('getAll should GET /quotes', function () {
      $httpBackend.expectGET(API_BASE + '/quotes').respond(200, []);
      quoteService.getAll();
      $httpBackend.flush();
    });

    it('getById should GET /quotes/:id', function () {
      var id = 42;
      $httpBackend.expectGET(API_BASE + '/quotes/' + id).respond(200, { id: id });
      quoteService.getById(id);
      $httpBackend.flush();
    });

    it('create should POST /quotes with payload', function () {
      var payload = { vehicleIds: [1], deductible: 500, liabilityLimit: 100000 };
      $httpBackend.expectPOST(API_BASE + '/quotes', payload).respond(201, { id: 1 });
      quoteService.create(payload);
      $httpBackend.flush();
    });
  });
})();
