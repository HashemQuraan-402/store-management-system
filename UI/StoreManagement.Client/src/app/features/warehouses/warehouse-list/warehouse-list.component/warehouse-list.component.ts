
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { WarehouseService } from '../../../../core/services/warehouse.service';
import { Item, WarehouseListItem} from './../../../../core/models/Warehouse.model';


@Component({
  selector: 'app-warehouse-list.component',
  imports: [CommonModule, RouterLink],
  templateUrl: './warehouse-list.component.html',
  styleUrl: './warehouse-list.component.css',
})
export class WarehouseListComponent {
  private readonly warehouseService = inject(WarehouseService);

  readonly warehouses = signal<WarehouseListItem[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly exporting = signal(false);

  // Per-row expand state: which warehouse's items are currently shown, and the cached items.
  readonly expandedId = signal<number | null>(null);
  readonly itemsByWarehouse = signal<Record<number, Item[]>>({});
  readonly itemsLoading = signal<number | null>(null);

  // Per-row delete-in-progress, so only the clicked row's button shows a busy state.
  readonly deletingId = signal<number | null>(null);
  readonly deleteError = signal<string | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.warehouseService.getMyWarehouses().subscribe({
      next: (warehouses) => {
        this.warehouses.set(warehouses);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load warehouses. Please try again.');
        this.loading.set(false);
      }
    });
  }

  toggleView(warehouse: WarehouseListItem): void {
    if (this.expandedId() === warehouse.warehouseId) {
      this.expandedId.set(null);
      return;
    }

    this.expandedId.set(warehouse.warehouseId);

    if (this.itemsByWarehouse()[warehouse.warehouseId]) {
      return; // already cached
    }

    this.itemsLoading.set(warehouse.warehouseId);
    this.warehouseService.getWarehouseItems(warehouse.warehouseId).subscribe({
      next: (items) => {
        this.itemsByWarehouse.update((map) => ({ ...map, [warehouse.warehouseId]: items }));
        this.itemsLoading.set(null);
      },
      error: () => {
        this.itemsLoading.set(null);
      }
    });
  }

  deleteWarehouse(warehouse: WarehouseListItem): void {
    const confirmed = confirm(`Delete "${warehouse.warehouseName}"? This cannot be undone.`);
    if (!confirmed) {
      return;
    }

    this.deletingId.set(warehouse.warehouseId);
    this.deleteError.set(null);

    this.warehouseService.deleteWarehouse(warehouse.warehouseId).subscribe({
      next: () => {
        this.warehouses.update((list) => list.filter((w) => w.warehouseId !== warehouse.warehouseId));
        this.deletingId.set(null);
      },
      error: (err) => {
        this.deletingId.set(null);
        this.deleteError.set(
          err?.status === 409
            ? `"${warehouse.warehouseName}" can't be deleted while it's referenced by a Supply Document.`
            : `Could not delete "${warehouse.warehouseName}". Please try again.`
        );
      }
    });
  }

  export(): void {
    this.exporting.set(true);
    this.warehouseService.exportWarehouses().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `Warehouses_${new Date().toISOString().slice(0, 10)}.xlsx`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.exporting.set(false);
      },
      error: () => {
        this.exporting.set(false);
        this.errorMessage.set('Could not export warehouses. Please try again.');
      }
    });
  }  
}
