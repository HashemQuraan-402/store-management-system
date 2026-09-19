import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplyDocumentListComponent } from './supply-document-list.component';

describe('SupplyDocumentListComponent', () => {
  let component: SupplyDocumentListComponent;
  let fixture: ComponentFixture<SupplyDocumentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupplyDocumentListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SupplyDocumentListComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
