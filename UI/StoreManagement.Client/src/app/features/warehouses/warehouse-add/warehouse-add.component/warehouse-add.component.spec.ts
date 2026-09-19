import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { WarehouseAddComponent } from './warehouse-add.component';

describe('WarehouseAddComponent', () => {
  let component: WarehouseAddComponent;
  let fixture: ComponentFixture<WarehouseAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WarehouseAddComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(WarehouseAddComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
