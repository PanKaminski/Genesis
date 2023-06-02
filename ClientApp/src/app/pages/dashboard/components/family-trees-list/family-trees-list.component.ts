import { Component, OnInit } from '@angular/core';
import { NotificationService } from '@shared/services/notification.service';
import { Subject, takeUntil } from 'rxjs';
import { TreeCardInfo } from '../../models/tree-card-info';
import { TreesList } from '../../models/trees-list';
import { FamilyTreeService } from '../../services/family-trees.service';

@Component({
  selector: 'app-family-trees-list',
  templateUrl: './family-trees-list.component.html',
  styleUrls: ['./family-trees-list.component.scss']
})
export class FamilyTreesListComponent implements OnInit {
  private destroy$ = new Subject<void>();

  treesList: TreesList;

  constructor(
    private treeService: FamilyTreeService,
    private notificationService: NotificationService) {
  }

  ngOnInit(): void {
    this.treeService.getTreesList()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (treesList: TreesList) => {
          this.treesList = treesList;
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
      });
  }

  get trees(): TreeCardInfo[] { return this.treesList?.trees }
}
