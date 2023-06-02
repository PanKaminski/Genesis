import { Injectable, OnDestroy, OnInit } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({providedIn: 'root'})
  export class MenuService implements OnInit, OnDestroy {    
    private destroy$ = new Subject<void>();

    ngOnInit(): void {
        throw new Error("Method not implemented.");
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}