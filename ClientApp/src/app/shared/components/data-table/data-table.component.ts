import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { Column } from '@shared/models/tables/column';
import { ColumnType } from '@shared/models/tables/column-type';
import { Row } from '@shared/models/tables/row';
import { Table } from '@shared/models/tables/table';
import { retry, Subject } from 'rxjs';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent implements OnDestroy {
  @Input() table: Table;

  @Output() editRowClicked = new EventEmitter<number>();
  @Output() addClicked = new EventEmitter<void>();
  @Output() deleteRowsClicked = new EventEmitter<number[]>();
  @Output() copyRowsClicked = new EventEmitter<number[]>();

  private readonly destroy$= new Subject<void>;

  selection = new SelectionModel<number>(true, []);
  columnTypes = ColumnType;

  get rows(): Row[] {
    return this.table.rows;
  }

  get isAllRowsSelected(): boolean {
    return this.table.rows.length === this.selection.selected.length;
  }

  get columns(): Column[] {
    return this.table.columns;
  }

  get displayedColumns(): string[] {
    return this.columns.map(c => c.name);
  }

  get canEdit(): boolean {
    return this.selection.selected.length === 1;
  }

  get canDelete(): boolean {
    const canDeleteByRows = this.rows.filter(r => this.selection.isSelected(r.id)).every(r => r.isRemovable);

    return this.selection.hasValue() && canDeleteByRows;
  }
  
  getCellText(rowId: number, columnId: number):any {
    return this.rows.find(r => r.id === rowId)?.cells?.find(c => c.columnId === columnId)?.value;
  }

  getColumnDef(column: Column): string {
    return column.columnType === ColumnType.CheckBox ? 'select' : column.name;
  }

  getImage(rowId: number, columnId: number): any {
    var url = this.rows.find(r => r.id === rowId)?.cells?.find(c => c.columnId === columnId)?.value;

    return !!url ? url : "assets/images/unknown-person.jpg";
  }

  isChecked(rowId): boolean {
    return this.selection.isSelected(rowId);
  }

  onCheckBoxClick(rowId: number, columnId: number): void {
    if (columnId === 0) {
      this.selection.toggle(rowId);
    }
  }

  selectAllRows():void {
    if (this.isAllRowsSelected) {
      this.selection.clear();
    } else {
      this.selection.select(...this.rows.map(r => r.id));
    }
  }

  onEditClick(): void {
    this.editRowClicked.emit(this.selection.selected[0]);
  }

  onAddClick(): void {
    this.addClicked.emit();
  }

  onDeleteClick(): void {
    this.deleteRowsClicked.emit(this.selection.selected);
  }

  onCopyClick(): void {
    this.copyRowsClicked.emit(this.selection.selected);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
