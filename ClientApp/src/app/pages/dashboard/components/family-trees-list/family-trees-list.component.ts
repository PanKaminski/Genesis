import { Component, OnDestroy, OnInit } from '@angular/core';
import { NotificationService } from '@shared/services/notification.service';
import { SideOverlayService } from '@shared/services/side-overlay.service';
import { Subject, takeUntil } from 'rxjs';
import { TreeCardInfo } from '../../models/tree-card-info';
import { TreesList } from '../../models/trees-list';
import { FamilyTreeService } from '../../services/family-trees.service';
import { FamilyTreeSideFormComponent } from '../family-tree-side-form/family-tree-side-form.component';

@Component({
  selector: 'app-family-trees-list',
  templateUrl: './family-trees-list.component.html',
  styleUrls: ['./family-trees-list.component.scss']
})
export class FamilyTreesListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  treesList: TreesList;

  constructor(
    private treeService: FamilyTreeService,
    private notificationService: NotificationService,
    private readonly sidePanelService: SideOverlayService
    ) {
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

    this.treeService.treeCardUpdated$
      .pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: (cardInfo: TreeCardInfo) => {
          const cardIndex = this.trees.findIndex(c => c.id == cardInfo.id);
          if (cardIndex > 0) {
            this.trees[cardIndex] = cardInfo;
          } else {
            this.trees.push(cardInfo);
          }
        }
    });
  }

  get trees(): TreeCardInfo[] { return this.treesList?.trees }

  onAddTree(): void {
    const treeId = 0;
    this.sidePanelService.openSidePanel<FamilyTreeSideFormComponent>(
      FamilyTreeSideFormComponent, 
      treeId
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
