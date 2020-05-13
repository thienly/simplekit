import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartsModule } from 'ng2-charts';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { ProductListComponent } from './product-list.component';
import { ProductRoutingModule } from './product-list.routing';
import { ProductItemComponent } from './product-item/product-item.component';
import { CommonModule } from '@angular/common';


@NgModule({
  imports: [
    CommonModule,
    ProductRoutingModule,
    ChartsModule,
    BsDropdownModule,
    ButtonsModule.forRoot(),
    FormsModule
  ],
  declarations: [ProductListComponent, ProductItemComponent]
})
export class ProductListModule { }