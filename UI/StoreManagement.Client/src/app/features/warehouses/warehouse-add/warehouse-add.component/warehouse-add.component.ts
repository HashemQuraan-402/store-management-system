import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { WarehouseService } from '../../../../core/services/warehouse.service';


@Component({
  selector: 'app-warehouse-add.component',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './warehouse-add.component.html',
  styleUrl: './warehouse-add.component.css',
})
export class WarehouseAddComponent {
  private readonly fb = inject(FormBuilder);
  private readonly warehouseService = inject(WarehouseService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    warehouseName: ['', Validators.required],
    warehouseDescription: ['', Validators.required],
    items: this.fb.array([this.buildItemGroup()])
  });

  get items(){
    return this.form.controls.items;
  }

  private buildItemGroup(){
    return this.fb.nonNullable.group({
      itemName: ['', Validators.required],
      itemDescription: [''],
      quantity: [0, [Validators.required, Validators.min(0)]]
    });
  }

  addItemRow(): void{
    this.items.push(this.buildItemGroup());
  }

  removeItemRow(index: number): void{
    if(this.items.length>1){
      this.items.removeAt(index);
    }
  }

  submit(): void{
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);


    const raw = this.form.getRawValue();

    this.warehouseService
      .createWarehouse({
        warehouseName: raw.warehouseName,
        warehouseDescription: raw.warehouseDescription || null,
        items: raw.items.map((item) => ({
          itemName: item.itemName,
          itemDescription: item.itemDescription || null,
          quantity: item.quantity
        }))
      }).subscribe({
        next: () => this.router.navigateByUrl('/warehouses'),
        error: (err) => {
          this.submitting.set(false);
          this.errorMessage.set(
            err?.status === 409
            ? 'A warehouse with that name already exists. Choose a different name.'
            : 'Could not create the warehouse. Please try again.'
          );
        }
      });

  }



}
