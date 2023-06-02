import { OverlayModule } from '@angular/cdk/overlay';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NativeDateModule } from '@angular/material/core';
import { ToastrModule } from 'ngx-toastr';
import { MaterialModule } from './material.module';
import { LightboxModule } from 'ngx-lightbox';

const MODULES = [
  MaterialModule,
  FlexLayoutModule,
  CommonModule,
  FormsModule,
  ReactiveFormsModule,
  ScrollingModule,
  OverlayModule,
  NativeDateModule,
  LightboxModule,
];

@NgModule({
  declarations: [
  ],
  imports: [
    ...MODULES,
    ToastrModule.forRoot({
      timeOut: 10000,
      progressBar: true,
      positionClass: 'genesis-toast-top-right',
    }),
  ],
  exports: [
    ...MODULES,
  ],
})
export class DefaultSetModule { }