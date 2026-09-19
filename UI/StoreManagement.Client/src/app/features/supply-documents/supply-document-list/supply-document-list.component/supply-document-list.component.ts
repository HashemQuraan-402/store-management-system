import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SupplyDocumentService } from '../../../../core/services/supply-document.service';
import { AuthService } from '../../../../core/services/auth.service';
import { SupplyDocumentListItem } from '../../../../core/models/SupplyDocument.model';



@Component({
  selector: 'app-supply-document-list.component',
  imports: [CommonModule, RouterLink],
  templateUrl: './supply-document-list.component.html',
  styleUrl: './supply-document-list.component.css',
})
export class SupplyDocumentListComponent {
  readonly auth = inject(AuthService);
  private readonly supplyDocumentService = inject(SupplyDocumentService);

  readonly documents = signal<SupplyDocumentListItem[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly busyId = signal<number | null>(null);

  constructor(){
    this.load();
  }


  load(): void{
    this.loading.set(true);
    this.supplyDocumentService.getSupplyDocuments().subscribe({
      next: (documents) => {
        this.documents.set(documents);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load supply documents. Please try again.');
        this.loading.set(false);
      }
    });
  }

  deleteDocument(doc: SupplyDocumentListItem): void{
    const confirmed = confirm(`Delete "${doc.supplyDocumentName}"? This cannot be undone.`);
    if(!confirmed){
      return;
    }

    this.busyId.set(doc.supplyDocumentId);
    this.errorMessage.set(null);

    this.supplyDocumentService.deleteSupplyDocument(doc.supplyDocumentId).subscribe({
      next: () =>{
        this.documents.update((list) => list.filter((d) => d.supplyDocumentId !== doc.supplyDocumentId));
        this.busyId.set(null);
      },
      error : (err) =>{
        this.busyId.set(null);
        this.errorMessage.set(
          err?.status === 409
          ? `"${doc.supplyDocumentName}" can no longer be deleted - it's already been reviewed.`
          : `Could not delete "${doc.supplyDocumentName}". please try again.`
        );
      }
    });

  }

  approve(doc: SupplyDocumentListItem): void{
    this.busyId.set(doc.supplyDocumentId);
    this.supplyDocumentService.approve(doc.supplyDocumentId).subscribe({
      next: () => this.applyStatus(doc.supplyDocumentId, 'Approved'),
      error: () => {
        this.busyId.set(null);
        this.errorMessage.set('Could not approve this document. Please try again.');
      }
    });
  }

  decline(doc: SupplyDocumentListItem): void{
    this.busyId.set(doc.supplyDocumentId);
    this.supplyDocumentService.decline(doc.supplyDocumentId).subscribe({
      next: () => this.applyStatus(doc.supplyDocumentId, 'Declined'),
      error: () =>{
        this.busyId.set(null);
        this.errorMessage.set('Could not decline this document. Please try again.');
      }
    });
  }

  private applyStatus(id: number, status: 'Approved'| 'Declined'): void{
    this.documents.update((list)=>
    list.map((d)=>(d.supplyDocumentId === id ? {...d,status} : d)));
    this.busyId.set(null);
  }
}
