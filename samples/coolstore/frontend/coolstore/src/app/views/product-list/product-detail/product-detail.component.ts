import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  id: number;
  constructor(private route: ActivatedRoute) { 
    this.id = +route.snapshot.params['id'];
  }

  ngOnInit(): void {
  }

}
