import { Component, Inject, OnInit } from '@angular/core';
import { BaseSideFormComponent } from '@shared/components/base-side-form.component';
import { SideOverlayRef } from '@shared/models/side-overlay-ref';
import { SIDE_OVERLAY_DATA } from '@shared/models/side-overlay.tokens';
import { PersonEditorData } from '../../models/person-editor-data';
import { PersonSaveFormData } from '../../models/person-save-from-data';
import { PersonEditorService } from '../../services/person-editor-service';
import { takeUntil } from 'rxjs';
import { Form } from '@shared/models/forms/form';
import { ControlValue } from '@shared/models/forms/control-value';
import { ExceptionDetails } from '@shared/models/exception-details';
import { NotificationService } from '@shared/services/notification.service';
import { ResultCode, ServerDataResponse, ServerResponse } from '@shared/models/server-response';
import { Picture } from '@shared/models/pictures/picture';
import { ControlType } from '@shared/models/forms/control-type';
import { PersonSaveResponse } from '../../models/person-save-response';

@Component({
  selector: 'app-person-side-form',
  templateUrl: './person-side-form.component.html',
  styleUrls: ['./person-side-form.component.scss']
})
export class PersonSideFormComponent extends BaseSideFormComponent<PersonSaveFormData> implements OnInit {
  
  formLoaded: boolean = false;
  updating: boolean = false;
  
  constructor(
    @Inject(SIDE_OVERLAY_DATA) private data: PersonEditorData,
    dialogRef: SideOverlayRef,
    private readonly personEditorService: PersonEditorService,
    private readonly notificationService: NotificationService,
  ) {
    super(dialogRef);
  }

  get emptyAvatarTemplate(): string {
    return '/assets/images/unknown-person.jpg';
  }

  get isNewPerson(): boolean {
    return !this.data.id;
  }

  override ngOnInit() {
    super.ngOnInit();

    this.fillItemWithDefault();

    this.personEditorService.loadPersonForm(this.data)
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

  onAddImages(images: Picture[]): void {
    for (let i = 0; i < images.length; i++) {
      this.item.addedPhotos.push(images[i]);
    }
  }

  onAvatarChange(id: number): void {
    for (let i = 0; i < this.item.addedPhotos.length; i++) {
      const picture = this.item.addedPhotos[i];
      picture.isMain = picture.id === id;
    }

    this.item.updatedPhotos = this.item.updatedPhotos.filter(ph => !ph.isMain);

    const photos = this.form.controls.find(c => c.type == ControlType.Images)?.value as Picture[];
    
    if (photos) {
      const photo = photos.find(ph => ph.id === id);
      if (photo) {
        photo.isMain = true;
        this.item.updatedPhotos.push(photo);
      }
    }
  }

  onRemoveImage(id: number): void {
    let index = this.item.addedPhotos.findIndex(img => img.id === id);
    this.item.updatedPhotos = this.item.updatedPhotos.filter(img => img.id === id);

    if (index >= 0) {
      this.item.addedPhotos.splice(index, 1);
    } else {
      this.item.removedPhotos.push(id);
    }
  }

  onSave(formValues: ControlValue[]): void {
    this.item.formValues = formValues;

    this.updating = true;
    this.personEditorService.savePerson(this.item)
      .pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: (result: ServerDataResponse<PersonSaveResponse>) => {
          this.updating = false;
          if (result.code === ResultCode.Done)
            this.onClose();
        }
      });
  }

  onDelete(): void {
    this.personEditorService.deletePerson(this.data.id)
      .pipe(takeUntil(this.destroy$),)
      .subscribe({
        next: (result: ServerResponse) => {
          if (result.code === ResultCode.Done)
            this.onClose();
        }
    });
  }

  private fillItemWithDefault(): void {
    this.item = {
      personEditorInfo: this.data,
      formValues: [],
      updatedPhotos: [],
      removedPhotos: [],
      addedPhotos: [],
    }
  }
}