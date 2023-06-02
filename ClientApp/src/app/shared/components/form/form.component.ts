import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSelectChange } from '@angular/material/select';
import { SafeUrl } from '@angular/platform-browser';
import { ButtonType } from '@shared/models/forms/button-type';
import { ControlType } from '@shared/models/forms/control-type';
import { ControlValue } from '@shared/models/forms/control-value';
import { Form } from '@shared/models/forms/form';
import { FormControl } from '@shared/models/forms/form-control';
import { FormTab } from '@shared/models/forms/form-tab';
import { AlbumImage } from '@shared/models/pictures/album-image';
import { Picture } from '@shared/models/pictures/picture';
import { NotificationService } from '@shared/services/notification.service';
import { PhotosService } from '@shared/services/photos.service';
import { Lightbox, IAlbum } from 'ngx-lightbox';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.scss']
})
export class FormComponent implements OnInit {
  @Input() form: Form;
  @Input() hasAvatar: boolean;
  @Input() avatarTemplate: string;
  @Input() isNew: boolean;
  @Input() updating: boolean;

  @Output() controlChange = new EventEmitter<boolean>();
  @Output() updateAvatar = new EventEmitter<number>();
  @Output() removeImage = new EventEmitter<number>();
  @Output() addImages = new EventEmitter<Picture[]>();
  @Output() closeForm = new EventEmitter<void>();
  @Output() deleteFormItem = new EventEmitter<void>();
  @Output() saveForm = new EventEmitter<ControlValue[]>();

  private _avatarImgId: number;
  private isAlbumChanged: boolean = false;
  private destroy$ = new Subject<void>();
  formGroup: FormGroup;
  imagesAlbum: AlbumImage[] = [];
  activeTabIndex: number = 0;
  controlTypes = ControlType;
  buttonTypes = ButtonType;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly lightbox: Lightbox,
    private readonly photosService: PhotosService,
    private readonly notificationService: NotificationService,
    ) {}

  ngOnInit(): void {
    this.formGroup = this.formBuilder.group({});
    this.form.controls.filter(control => control.type != ControlType.Images)
      .forEach(control => {
        const validators = [];
        if (control.isRequired) {
          validators.push(Validators.required);
        }

        const formControl = this.formBuilder.control(
          {value: control.value, disabled: control.isReadonly}, 
          validators
        );
        this.formGroup.addControl(control.name, formControl);
    });

    this.fillAlbum();
  }

  get avatar(): string | SafeUrl {
    return this.imagesAlbum?.find(i => i.id === this._avatarImgId)?.src ?? this.avatarTemplate;
  }

  get tabs(): FormTab[] {
    return this.form.tabs;
  }

  get hasButtons(): boolean {
    return !!this.form.buttonTypes;
  }

  get canSave(): boolean {
    return this.formGroup.touched 
      && !this.formGroup.invalid 
      && this.formGroup.dirty 
      || this.isAlbumChanged;
  }

  get leftButtonsGroup(): ButtonType[] {
    return this.form?.buttonTypes?.filter(t => t == ButtonType.Close);
  }

  get rightButtonsGroup(): ButtonType[] {
    return this.form?.buttonTypes?.filter(t => t == ButtonType.Save || t == ButtonType.Delete);
  }

  getDatePickerId(control: FormControl): string {
    return `picker${control.tabId}_${control.name}`;
  }

  showAvatar(tabName: string): boolean {
    return this.hasAvatar && this.avatar && tabName === 'Common';
  }

  onSave(): void {
    const formValues = [];

    for (let i = 0; i < this.form.controls.length; i++) {
      const serverControl = this.form.controls[i];
      const controlValue = this.formGroup.controls[serverControl.name]?.value;
      formValues.push({ entityType: serverControl.entityType, value: controlValue });
    }

    this.saveForm.next(formValues);
  }

  onDelete(): void {
    this.deleteFormItem.next();
  }

  onClose(): void {
    this.closeForm.next();
  }

  onTabChange(event: MatSelectChange) {
    this.activeTabIndex = event.value;
  }

  getTabControls(tabId: number): FormControl[] {
    return this.form.controls.filter(control => control.tabId === tabId);
  }

  isMultiSelect(control: FormControl) {
    return control.isMulty && control.items && control.items.length > 0;
  }

  onImageOpen(index: number): void {
    this.lightbox.open(this.imagesAlbum as IAlbum[], index);
  }

  onFileSelected(fileSelectedEvent): void {
    const files = fileSelectedEvent.target.files;
    this.updating = true;
    this.photosService.uploadPhotos(files).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (pictures: Picture[]) => {
        let id = this.getNextGeneratedPictureId();
        for (let i = 0; i < pictures.length; i++) {
          const pic = pictures[i];
          pic.id = id;
          const albumImage = {
            id: id++,
            src: pic.url,
            caption: pic.publicId,
            thumb: pic.publicId,
          };
    
          this.imagesAlbum.unshift(albumImage);
        }

        this.isAlbumChanged = true;
        this.updating = false;
        this.addImages.next(pictures);    
      },
      error: () => {
        this.notificationService.notifyErrorMessage('Failed to upload photos.');
      }
    });
  }

  onRemoveImage(id: number): void {
    this.imagesAlbum = this.imagesAlbum.filter(i => i.id !== id);
    this.isAlbumChanged = true;
    this.removeImage.next(id);
  }

  onAvatarChange(id: number): void {
    if (this._avatarImgId !== id) {
      this._avatarImgId = id;
      this.isAlbumChanged = true;
      this.updateAvatar.next(id);  
    }
  }

  private fillAlbum(): void {
    const photos = this.form.controls.find(c => c.type == ControlType.Images)?.value as Picture[];
    
    if (!photos) return;

    for (let i = 0; i < photos.length; i++) {
      const photo = photos[i];
      const albumImage = {
        id: photo.id,       
        src: photo.url,
        caption: photo.publicId,
        isMain: photo.isMain,
        thumb: photo.publicId,
      };

      this.imagesAlbum.push(albumImage);
    }
  }

  private getNextGeneratedPictureId(): number {
    let maxId = 1;
    if (!this.imagesAlbum?.length) return maxId;
    
    maxId = Math.max(...this.imagesAlbum.map(i => i.id));
    return maxId ? maxId + 1 : 1;
  }
}
