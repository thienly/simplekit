import { NgModule } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { ProductListComponent } from './product-list.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { ProductListGuard } from './product-list.guard';
export const routes: Routes = [
    {
        path: '',
        component: ProductListComponent,
        data: {
            title: 'Products'
        }        
    },
    {
        path: ':id',
        component: ProductDetailComponent
    }];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProductRoutingModule { };
