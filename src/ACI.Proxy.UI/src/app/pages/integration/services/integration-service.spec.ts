import { TestBed } from '@angular/core/testing';

import { IntegrationService } from '../services/integration-service';

describe('IntegrationService', () => {
  let service: IntegrationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IntegrationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
