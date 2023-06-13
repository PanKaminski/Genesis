import { Component, OnDestroy, OnInit } from '@angular/core';
import { ResultCode, ServerResponse } from '@shared/models/server-response';
import { Row } from '@shared/models/tables/row';
import { Table } from '@shared/models/tables/table';
import { NotificationService } from '@shared/services/notification.service';
import { PersonSideOverlayService } from '@shared/services/person-side-overlay-.service';
import { Subject, takeUntil } from 'rxjs';
import { DataManagerService } from '../services/data-manager.service';

@Component({
  selector: 'app-persons-data',
  templateUrl: './persons-data.component.html',
  styleUrls: ['./persons-data.component.scss']
})
export class PersonsDataComponent implements OnInit, OnDestroy {
  table: Table;
  isLoaded: boolean = false;

  private readonly destroy$ = new Subject<void>;

  constructor(
    private readonly dataManager: DataManagerService,
    private readonly personSideOverlayService: PersonSideOverlayService,
    private readonly notifier: NotificationService
  ) {
  }

  ngOnInit(): void {
    this.dataManager.loadPersonsTable()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (table: Table) => {
          this.table = table;
          this.isLoaded = true;
        },
        error: (err) => {
          this.notifier.notifyError(err);
        }
      });

      this.personSideOverlayService.rowUpdated$
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (row: Row) => {
            var oldRow = this.table.rows.find(r => r.id == row.id);

            if (oldRow) {
              oldRow.cells.forEach(c => {
                const newValue = row.cells.find(oc => oc.columnId == c.columnId)?.value;
                c.value = newValue;
              });
            } else {
              this.table.rows.push(row);
            }

            this.table.rows = [...this.table.rows];
          }
        });
  }

  onAddPerson(){
    this.personSideOverlayService.openNewPersonForm({ });
  }

  onEditPerson(personId: number): void {
    this.personSideOverlayService.openNewPersonForm({ id: personId });
  }

  onDeletePersons(rows: number[]): void {
    this.personSideOverlayService.deletePersons(rows)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result: ServerResponse) => {
          if (result.code === ResultCode.Done) {
            this.table.rows = this.table.rows.filter (r => rows.indexOf(r.id) >= 0);
          }
        }
      })
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
