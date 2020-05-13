import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, Router, CanActivateChild } from '@angular/router';

@Injectable({providedIn: 'root'})
export class ProductListGuard implements CanActivate, CanActivateChild {
    constructor(private router: Router) { }
    canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | import("@angular/router").UrlTree | import("rxjs").Observable<boolean | import("@angular/router").UrlTree> | Promise<boolean | import("@angular/router").UrlTree> {
        return false;
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        this.router.navigate(['dashboard']);
        return false;
    }
}