import { Component, Input } from '@angular/core';
import { SideOverlayService } from '@shared/services/side-overlay.service';
import { TreeCardInfo } from '../../models/tree-card-info';
import { FamilyTreeSideFormComponent } from '../family-tree-side-form/family-tree-side-form.component';

@Component({
  selector: 'app-tree-card',
  templateUrl: './tree-card.component.html',
  styleUrls: ['./tree-card.component.scss']
})
export class TreeCardComponent {
  @Input() treeCard: TreeCardInfo;

  constructor(
    private readonly sidePanelService: SideOverlayService
  ) { }

  get defaultCardPicture(): string {
    return 'assets/images/default-tree.jpg';
  }

  onEdit() {
    this.sidePanelService.openSidePanel<FamilyTreeSideFormComponent>(
      FamilyTreeSideFormComponent, 
      this.treeCard.id
    );
  }
}
