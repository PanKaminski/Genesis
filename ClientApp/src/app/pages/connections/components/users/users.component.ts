import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PagedModel } from '@shared/models/paged-model';
import { NotificationService } from '@shared/services/notification.service';
import { Subject, takeUntil } from 'rxjs';
import { ConnectionCard } from '../../models/user-card';
import { UsersToolCode } from '../../models/users-tool-code';
import { ContactsService } from '../../services/contacts.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  users: ConnectionCard[] = [];

  protected readonly destroy$ = new Subject<void>();
  private usersToolCode: UsersToolCode;
  isLoaded: boolean = false;
  currentPage: number = 1;
  pageSize: number = 40;
  totalItemsCount: number = 0;

  constructor(
    private readonly contactsService: ContactsService,
    private readonly notificationsService: NotificationService,
    private readonly activatedRoute: ActivatedRoute,
    ) {    
  }

  ngOnInit(): void {
    this.usersToolCode = this.activatedRoute.snapshot.data['toolCode'] as UsersToolCode;
    this.loadUsers();
  }

  get hasUsers(): boolean {
    return !!this.users?.length
  }

  onPageChange($pageEvent) {
    this.currentPage = $pageEvent.pageIndex + 1;
    this.pageSize = $pageEvent.pageSize;

    this.loadUsers();
  }

  loadUsers(): void {
    this.contactsService.getUsers(this.usersToolCode, this.currentPage, this.pageSize)
      .pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: (connectionsPage: PagedModel<ConnectionCard>) => {
          this.users = connectionsPage.items;
          this.currentPage = connectionsPage.currentPage;
          this.totalItemsCount = connectionsPage.totalCount;
          this.isLoaded = true;
        },
        error: (err) => {
          this.notificationsService.notifyError(err);
          this.isLoaded = true;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
