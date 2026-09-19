import { TestBed } from '@angular/core/testing';

import { SupplyDocumentService } from './supply-document.service';

describe('SupplyDocumentService', () => {
  let service: SupplyDocumentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SupplyDocumentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
