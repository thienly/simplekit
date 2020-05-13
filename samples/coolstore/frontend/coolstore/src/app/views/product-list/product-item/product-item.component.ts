import { Component, OnInit, Input } from '@angular/core';
import { ProductModel } from '../product.model';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent implements OnInit {

  @Input() Item: ProductModel;
  constructor() { }

  ngOnInit(): void {
  }

}
