import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from "@angular/router";
import { StorageKey } from "@shared/enums/storage-key";
import { Observable } from "rxjs";
import { FamilyTreeService } from "../services/family-trees.service";

@Injectable({
    providedIn: 'root'
  })
  export class AuthGuard implements CanActivate {
    constructor(
      private readonly router: Router,
      private readonly familyTreeService: FamilyTreeService
  ) { }
    canActivate(
      route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot
      ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        const treeId = route.params["id"] as number;
        if (treeId && this.familyTreeService.canLoadTree(treeId))
          return true;
      
      this.router.navigate(['/404']);
      return false;
    }
  }