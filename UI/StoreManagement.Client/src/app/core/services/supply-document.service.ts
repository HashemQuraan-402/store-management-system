import { Injectable,inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SupplyDocumentCreate, SupplyDocumentListItem, SupplyDocumentStatus } from '../models/SupplyDocument.model';


@Injectable({providedIn: 'root'})
export class SupplyDocumentService {

    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.apiUrl}/SupplyDocument`;


    getSupplyDocuments():Observable<SupplyDocumentListItem[]>{
        return this.http.get<SupplyDocumentListItem[]>(this.baseUrl);
    }

    createSupplyDocument(supplyDocument: SupplyDocumentCreate):Observable<SupplyDocumentListItem>{
        return this.http.post<SupplyDocumentListItem>(this.baseUrl,supplyDocument);
    }


    deleteSupplyDocument(documentId: number):Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${documentId}`);
    }


    approve(id: number): Observable<{message:string;status:string}>{
        return this.http.put<{message:string;status:string}>(`${this.baseUrl}/${id}/approve`,{});
    }


    decline(id: number): Observable<{message: string; status:string}>{
        return this.http.put<{message: string; status:string}>(`${this.baseUrl}/${id}/decline`, {});
    }

}
