import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Button } from '@shared/models/forms/button';
import { ButtonType } from '@shared/models/forms/button-type';
import { NotificationService } from '@shared/services/notification.service';
import { Subject, takeUntil } from 'rxjs';
import { ResultCode, ServerDataResponse } from '@shared/models/server-response';
import { UpdateConnectionStatusResponse } from '../../models/update-card-status-change';
import { ConnectionCard, ConnectionStatus } from '../../models/user-card';
import { UsersToolCode } from '../../models/users-tool-code';
import { ContactsService } from '../../services/contacts.service';

@Component({
  selector: 'app-contact-card',
  templateUrl: './contact-card.component.html',
  styleUrls: ['./contact-card.component.scss']
})
export class ContactCardComponent implements OnInit, OnDestroy {
  @Input() contact: ConnectionCard;
  @Input() usersToolCode: UsersToolCode;
  protected readonly destroy$ = new Subject<void>();
  isInUpdating = false;

  constructor(
    private readonly contactsService: ContactsService,
    private readonly notifier: NotificationService,
    ) { }

  ngOnInit(): void {
  }

  get avatar(): string {
    return this.contact.avatar ?? '/assets/images/unknown-person.jpg';
  }

  get location(): string {
    let location = '';
    if (this.contact.country) location += this.contact.country;
    if (this.contact.city) location += `, ${this.contact.city}`;

    return location;
  }

  get isBlocked(): boolean {
    return this.contact.connectionStatus === ConnectionStatus.Blocked;
  }

  onClickCardButton(button: Button): void {
    const model = {
      userId: this.contact.userId,
      isRestrictionStatus: button.type === ButtonType.CardRestrictionButton,
    };

    this.isInUpdating = true;

    this.contactsService.changeConnectionStatus(model)
    .pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (result: ServerDataResponse<UpdateConnectionStatusResponse>) => {
        if (result.code === ResultCode.Done) {
          this.contact.connectionId = result.data.connectionId;
          this.contact.connectionStatus = result.data.status;
          this.contact.buttons = result.data.buttons;
        }
        else {
          this.notifier.notifyErrorMessage(result.message);
        }
        this.isInUpdating = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

