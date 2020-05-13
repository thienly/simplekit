import { Component, OnInit } from '@angular/core';
import { ProductModel } from './product.model';
import { getStyle } from '@coreui/coreui/dist/js/coreui-utilities';
import { CustomTooltips } from '@coreui/coreui-plugin-chartjs-custom-tooltips';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  products: ProductModel[]=[
  {
    id:1,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  },
  {
    id:2,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  },
  {
    id:3,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  },
  {
    id:4,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  },
  {
    id:5,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  },
  {
    id:6,
    name: "Iphone",
    imageSrc: "https://salt.tikicdn.com/cache/200x200/ts/product/ae/81/46/9107148ffeb43027b3e2082c8d279ae0.jpg",
     description:"This is a new generation iphone",
     inventory:100   
  }];

  constructor() { 
  }

  ngOnInit(): void {
  }
  

}
