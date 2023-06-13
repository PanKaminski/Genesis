import { Component, Inject, OnInit } from '@angular/core';
import { BaseSideFormComponent } from '@shared/components/base-side-form.component';
import { ExceptionDetails } from '@shared/models/exception-details';
import { ControlValue } from '@shared/models/forms/control-value';
import { Form } from '@shared/models/forms/form';
import { ResultCode, ServerDataResponse, ServerResponse } from '@shared/models/server-response';
import { SideOverlayRef } from '@shared/models/side-overlay-ref';
import { SIDE_OVERLAY_DATA } from '@shared/models/side-overlay.tokens';
import { NotificationService } from '@shared/services/notification.service';
import { takeUntil } from 'rxjs';
import { TreeCardInfo } from '../../models/tree-card-info';
import { TreeFormData } from '../../models/tree-form-data';
import { TreeNode } from '../../models/tree-node';
import { FamilyTreeService } from '../../services/family-trees.service';

@Component({
  selector: 'app-family-tree-side-form',
  templateUrl: './family-tree-side-form.component.html',
  styleUrls: ['./family-tree-side-form.component.scss']
})
export class FamilyTreeSideFormComponent extends BaseSideFormComponent<TreeFormData> implements OnInit {
  formLoaded: boolean = false;
  updating: boolean = false;
  
  constructor(
    @Inject(SIDE_OVERLAY_DATA) private treeId: number,
    dialogRef: SideOverlayRef,
    private readonly familyTreeService: FamilyTreeService,
    private readonly notificationService: NotificationService,
  ) {
    super(dialogRef);
  }

  get emptyAvatarTemplate(): string {
    return '/assets/images/default-tree.jpg';
  }

  get isNewTree(): boolean {
    return !this.treeId;
  }

  override ngOnInit() {
    super.ngOnInit();

    this.fillItemWithDefault();

    this.familyTreeService.loadTreeForm(this.treeId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (form: Form) => {
        this.form = form;
        this.formLoaded = true;
      },
      error: (error: ExceptionDetails) => {
        this.notificationService.notifyError(error);
        this.onClose();
      }
    });
  }

  onSave(formValues: ControlValue[]): void {
    this.item.values = formValues;

    this.updating = true;
    this.familyTreeService.saveTree(this.item)
      .pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: (result: ServerDataResponse<TreeCardInfo>) => {
          this.updating = false;
          if (result.code === ResultCode.Done)
            this.onClose();
        }
      });
  }

  onDelete(): void {
    /*this.familyTreeService.deleteTree(this.treeId)
      .pipe(takeUntil(this.destroy$),)
      .subscribe({
        next: (result: ServerResponse) => {
          if (result.code === ResultCode.Done)
            this.onClose();
        }
    });*/
  }

  private fillItemWithDefault(): void {
    this.item = {
      treeId: this.treeId,
      values: [],
    }
  }
}
