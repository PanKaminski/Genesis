import { Component, OnDestroy, OnInit } from '@angular/core';
import { Form } from '@shared/models/forms/form';
import { SideOverlayRef } from '@shared/models/side-overlay-ref';
import { Subject } from 'rxjs';

@Component({
    template: ''
})
export abstract class BaseSideFormComponent<T> implements OnInit, OnDestroy {   
    item: T;
    form: Form;
    protected readonly destroy$ = new Subject<void>();

    constructor(
        private dialogRef: SideOverlayRef,
    ) {}

    ngOnInit() {
    }

    onClose() {
        this.dialogRef.close();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}