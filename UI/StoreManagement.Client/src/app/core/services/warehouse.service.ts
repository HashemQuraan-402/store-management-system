
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DropdownOption, Item, WarehouseCreate, WarehouseListItem } from '../models/Warehouse.model'



@Injectable({providedIn: 'root'})
export class WarehouseService {

    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/Warehouses`;

    getMyWarehouses():Observable<WarehouseListItem[]>{
        return this.http.get<WarehouseListItem[]>(this.baseUrl);
    }

    getWarehouseItems(warehouseId: number): Observable<Item[]>{
        return this.http.get<Item[]>(`${this.baseUrl}/${warehouseId}/items`);
    }


    createWarehouse(warehouse: WarehouseCreate):Observable<WarehouseListItem>{
        return this.http.post<WarehouseListItem>(this.baseUrl,warehouse);
    }


    deleteWarehouse(warehouseId: number): Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${warehouseId}`);
    }

    exportWarehouses(): Observable<Blob>{
        return this.http.get(`${this.baseUrl}/export`, { responseType: 'blob'});
    }

    getWarehousesDropdown():Observable<DropdownOption[]>{
        return this.http.get<DropdownOption[]>(`${this.baseUrl}/dropdown`);
    }


    getItemsDropdown(warehouseId: number):Observable<DropdownOption[]>{
        return this.http.get<DropdownOption[]>(`${this.baseUrl}/${warehouseId}/items/dropdown`);
    }

}
