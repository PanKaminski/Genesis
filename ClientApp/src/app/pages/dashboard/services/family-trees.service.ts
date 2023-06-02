import { Injectable, OnDestroy } from "@angular/core";
import FamilyTree from "@balkangraph/familytree.js";
import { Gender } from "@shared/enums/gender";
import { Form } from "@shared/models/forms/form";
import { SideOverlayService } from "@shared/services/side-overlay.service";
import { concatMap, Observable, Subject } from "rxjs";
import { FamilyTreesApiService } from "../api/family-trees.api.service";
import { PersonSideFormComponent } from "../components/person-side-form/person-side-form.component";
import { PersonEditorData } from "../models/person-editor-data";
import { PersonRelationType } from "../models/person-relation-type";
import { TreeCardInfo } from "../models/tree-card-info";
import { TreeNode } from "../models/tree-node";
import { TreesList } from "../models/trees-list";

@Injectable({
  providedIn: 'any'
})
export class FamilyTreeService implements OnDestroy {
  private destroy$ = new Subject<void>();

  constructor(
    private readonly api: FamilyTreesApiService,
  ) {
    this.configureBalkanTreeMenuUi();
  }
  
  getTreesList(): Observable<TreesList> {
      return this.api.getTreesList();
  }

  getPersons(treeId: number): Observable<TreeNode> {
    return this.api.getPersonsAsync(treeId)
    .pipe(
      concatMap((data: TreeNode[]) => data) // convert the response to a stream
    );
  }

  loadTreeForm(treeId: number): Observable<Form> {
    return this.api.loadTreeForm(treeId);
  }

  canLoadTree(treeId): Observable<boolean> {
    return this.api.canLoadTree(treeId);
  }

  private configureBalkanTreeMenuUi(): void {
    FamilyTree.templates['tommy'].nodeCircleMenuButton = 
    FamilyTree.templates['tommy_female'].nodeCircleMenuButton = 
    FamilyTree.templates['tommy_male'].nodeCircleMenuButton = {
        radius: 20,
        x: 0,
        y: 0,
        color: '#fff',
        stroke: '#aeaeae'
    };
  }

  configureCircleMenu(familyTree: FamilyTree, sidePanelService: SideOverlayService): void {
    familyTree.nodeCircleMenuUI.on('show', function (sender, args) {
      delete args.menu.father;
      delete args.menu.mother;
      delete args.menu.wife;
      delete args.menu.husband;
      
      var node = familyTree.getNode(args.nodeId) as TreeNode;
      const icons = FamilyTree.icon as any;
      if (!node.mid) {
          args.menu.mother = {
              icon: icons.mother(30, 30, '#F57C00'),
              text: "Add mother",
              color: "white"
          };
      }

      if (!node.fid) {
          args.menu.father = {
              icon: icons.father(30, 30, '#039BE5'),
              text: "Add father",
              color: "white"
          };
      }

      if (node.gender == 'male') {
          args.menu.wife = {
              icon: icons.wife(30, 30, '#F57C00'),
              text: "Add wife",
              color: "white"
          };
      } else if (node.gender == 'female') {
          args.menu.husband = {
              icon: icons.husband(30, 30, '#F57C00'),
              text: "Add husband",
              color: "white"
          };
      }

      args.menu.son = {
        icon: icons.son(30, 30, '#F57C00'),
        text: "Add son",
        color: "white",
      }

      args.menu.daughter = {
        icon: icons.daughter(30, 30, '#F57C00'),
        text: "Add daughter",
        color: "white",
      }
    });

    this.resolveMenuItemsActions(familyTree, sidePanelService);
  }

  resolveMenuItemsActions(
    family: FamilyTree, 
    sidePanelService: SideOverlayService
    ): void {
    family.nodeCircleMenuUI.on('click', function (sender, args) {
      let node = family.getNode(args.nodeId);
      let params = { } as PersonEditorData;
      switch (args.menuItemName) {
          case "husband":
            params.gender = Gender.Man;
            params.newRelation = PersonRelationType.Partners;
            params.personRelationTo = +node.id;
            break;
          case "wife":
            params.gender = Gender.Woman;
            params.newRelation = PersonRelationType.Partners;
            params.personRelationTo = +node.id;
            break;
          case "mother":
            params.gender = Gender.Woman;
            params.newRelation = PersonRelationType.ChildToParent;
            params.personRelationFrom = +node.id;
            break;
          case "father":
            params.gender = Gender.Man;
            params.newRelation = PersonRelationType.ChildToParent;
            params.personRelationFrom = +node.id;
            break;
          case "son":
            params.gender = Gender.Man;
            params.newRelation = PersonRelationType.ChildToParent;
            params.personRelationTo = +node.id;
            break;
          case "daughter":
            params.gender = Gender.Woman;
            params.newRelation = PersonRelationType.ChildToParent;
            params.personRelationTo = +node.id;
            break;
          default:
            return;
      };

      sidePanelService.openSidePanel<PersonSideFormComponent>(
        PersonSideFormComponent, 
        params
      );  
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}