export interface WarehouseListItem{
    warehouseId: number;
    warehouseName: string;
    warehouseDescription: string;
    CreatedByName: string;
    createdDateAndTime: Date;
    itemsCount: number;
}

export interface Item{
    itemId: number;
    itemName: string;
    itemDescription: string;
    quantity: number;
    warehouseId: number;
}

export interface ItemCreate{
    itemName: string;
    itemDescription: string | null;
    quantity: number;
}

export interface WarehouseCreate{
    warehouseName: string;
    warehouseDescription: string | null;
    items: ItemCreate[];
}

export interface DropdownOption{
    id: number;
    name: string;
}

