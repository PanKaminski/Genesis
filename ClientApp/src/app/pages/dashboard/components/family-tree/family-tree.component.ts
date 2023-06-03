import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import FamilyTree from "@balkangraph/familytree.js";
import { NotificationService } from '@shared/services/notification.service';
import { SideOverlayService } from '@shared/services/side-overlay.service';
import { Subject, takeUntil } from 'rxjs';
import { TreeNode } from '../../models/tree-node';
import { FamilyTreeService } from '../../services/family-trees.service';
import { PersonEditorService } from '../../services/person-editor-service';
import { PersonSideFormComponent } from '../person-side-form/person-side-form.component';

@Component({
  selector: 'app-family-tree',
  templateUrl: './family-tree.component.html',
  styleUrls: ['./family-tree.component.scss']
})
export class FamilyTreeComponent implements OnInit, OnDestroy {
  private treeElement;
  private familyTree: FamilyTree;
  private treeId: number;
  protected readonly destroy$ = new Subject<void>();

  constructor(
    private readonly treeService: FamilyTreeService,
    private readonly notificationService: NotificationService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly sidePanelService: SideOverlayService,
    private readonly personEditorService: PersonEditorService,
  ) { }

  get unknownPersonImage(): string {
    return 'assets/images/unknown-person.jpg';
  }

  ngOnInit() {
    this.treeId = this.activatedRoute.snapshot.params["id"];
    this.treeElement = document.getElementById('tree');

    if (this.treeElement && this.treeId) {
        this.familyTree = new FamilyTree(this.treeElement, {
            enableSearch: false,
            nodeCircleMenu: { },
            nodeBinding: {
            field_0: "name",
            img_0: "img",
          },
        });

        this.forbidDefaultForm();
        this.initListeners();
        this.treeService.configureCircleMenu(this.familyTree, this.sidePanelService);

        this.treeService.getPersons(this.treeId)
          .pipe(
            takeUntil(this.destroy$),
          ).subscribe({
            next: (item: TreeNode) => {
              this.addNode(item);
            },
            error: error => {
              this.notificationService.notifyError(error);
            }
          });
    }

    this.personEditorService.treeNodeUpdated$
      .pipe(
        takeUntil(this.destroy$),
      ).subscribe({
        next: (node: TreeNode) => {
          this.addNode(node);
        },
      });

      this.personEditorService.treeNodeDeleted$
        .pipe(
          takeUntil(this.destroy$),
        ).subscribe({
          next: (node: number) => {
            this.familyTree.removeNode(node);
          },
      });

  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initListeners(): void {
    this.familyTree.onNodeClick(({event, node}) => {
      this.sidePanelService.openSidePanel<PersonSideFormComponent>(
        PersonSideFormComponent, 
        {id: +node.id}
      );  
    });
  }

  private addNode(node: TreeNode): void {
    node.img ??= this.unknownPersonImage;
    if (this.familyTree.getNode(node.id)) {
      this.familyTree.updateNode(node);
    } else {
      this.familyTree.addNode(node, null, true);
    }
  }

  private forbidDefaultForm() {
    this.familyTree.editUI.on('hide', () =>{
      return false;
    });
    this.familyTree.editUI.on('show', () =>{
      return false;
    });
    this.familyTree.editUI.on('element-btn-click', () =>{
      return false;
    });
    this.familyTree.editUI.on('cancel', () =>{
      return false;
    });
    this.familyTree.editUI.on('button-click', () =>{
      return false;
    });
  }
}
