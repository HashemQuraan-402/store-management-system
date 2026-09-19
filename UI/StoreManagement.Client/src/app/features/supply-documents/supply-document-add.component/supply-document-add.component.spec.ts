import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { SupplyDocumentAddComponent } from './supply-document-add.component';

describe('SupplyDocumentAddComponent', () => {
  let component: SupplyDocumentAddComponent;
  let fixture: ComponentFixture<SupplyDocumentAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupplyDocumentAddComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(SupplyDocumentAddComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
