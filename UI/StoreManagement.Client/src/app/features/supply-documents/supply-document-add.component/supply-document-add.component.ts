
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder,ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { SupplyDocumentService } from '../../../core/services/supply-document.service';
import { DropdownOption } from '../../../core/models/Warehouse.model';

@Component({
  selector: 'app-supply-document-add.component',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './supply-document-add.component.html',
  styleUrl: './supply-document-add.component.css',
})
export class SupplyDocumentAddComponent {
  private readonly fb = inject(FormBuilder);
  private readonly warehouseService = inject(WarehouseService);
  private readonly supplyDocumentService = inject(SupplyDocumentService);
  private readonly router = inject(Router);

  readonly warehouses = signal<DropdownOption[]>([]);
  readonly items = signal<DropdownOption[]>([]);
  readonly loadingWarehouses = signal(true);
  readonly loadingItems = signal(false);
  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    supplyDocumentName: ['', Validators.required],
    supplyDocumentSubject: ['', Validators.required],
    warehouseId: [null as number | null, Validators.required],
    itemId: [null as number | null, Validators.required]
  });

  constructor(){
    this.warehouseService.getWarehousesDropdown().subscribe({
      next : (warehouses) => {
        this.warehouses.set(warehouses);
        this.loadingWarehouses.set(false);
      },
      error: () =>{
        this.loadingWarehouses.set(false);
        this.errorMessage.set('Could not load warehouses. please try again.');
      }
    });

    this.form.controls.warehouseId.valueChanges.subscribe((warehouseId) => {
      this.items.set([]);
      this.form.controls.itemId.setValue(null);

      if(warehouseId === null){
        return;
      }

      this.loadingItems.set(true);
      this.warehouseService.getItemsDropdown(warehouseId).subscribe({
        next: (items) => {
          this.items.set(items);
          this.loadingItems.set(false);
        },
        error: () => {
          this.loadingItems.set(false);
          this.errorMessage.set('Could not load items for that warehouse. Please try again.');
        }
      });
    });
  }

  submit(): void {
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();

    this.supplyDocumentService.createSupplyDocument({
      supplyDocumentName: raw.supplyDocumentName,
      supplyDocumentSubject: raw.supplyDocumentSubject,
      warehouseId: raw.warehouseId!,
      itemId: raw.itemId!
    }).subscribe({
      next: () => this.router.navigateByUrl('/supply-documents'),
      error: () =>{
        this.submitting.set(false);
        this.errorMessage.set('Could not submit the request. Please try again.');
      }
    });
  }


}
