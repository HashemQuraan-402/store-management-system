import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { LoginComponent } from './features/login/login.component/login.component';
import { LayoutComponent } from './features/layout/layout.component/layout.component';
import { SupplyDocumentAddComponent } from './features/supply-documents/supply-document-add.component/supply-document-add.component';
import { SupplyDocumentListComponent } from './features/supply-documents/supply-document-list/supply-document-list.component/supply-document-list.component';
import { WarehouseAddComponent } from './features/warehouses/warehouse-add/warehouse-add.component/warehouse-add.component';
import { WarehouseListComponent } from './features/warehouses/warehouse-list/warehouse-list.component/warehouse-list.component';

export const routes: Routes = [
  {
    path: 'login',component: LoginComponent
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'warehouses' },
      {
        path: 'warehouses',
        canActivate: [roleGuard('Manager')],
        component: WarehouseListComponent
      },
      {
        path: 'warehouses/add',
        canActivate: [roleGuard('Manager')],
        component: WarehouseAddComponent 
      },
      {
        path: 'supply-documents',
        component: SupplyDocumentListComponent
      },
      {
        path: 'supply-documents/add',
        canActivate: [roleGuard('Employee')],
        component: SupplyDocumentAddComponent 
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
