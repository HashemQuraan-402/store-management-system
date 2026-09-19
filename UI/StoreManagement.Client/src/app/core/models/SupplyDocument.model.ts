export type SupplyDocumentStatus = 'Pending' | 'Approved' | 'Declined';

export interface SupplyDocumentListItem{
    supplyDocumentId: number;
    supplyDocumentName: string;
    supplyDocumentSubject: string;
    createdByName: string;
    createdDate: string;
    warehouseName: string;
    itemName: string;
    status: SupplyDocumentStatus;
}

export interface SupplyDocumentCreate{
    supplyDocumentName: string;
    supplyDocumentSubject: string;
    warehouseId: number;
    itemId: number;
}
