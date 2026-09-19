import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { WarehouseListComponent } from './warehouse-list.component';

describe('WarehouseListComponent', () => {
  let component: WarehouseListComponent;
  let fixture: ComponentFixture<WarehouseListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WarehouseListComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(WarehouseListComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
