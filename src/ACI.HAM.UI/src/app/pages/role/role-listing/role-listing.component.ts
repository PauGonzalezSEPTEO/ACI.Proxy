import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { RoleService } from '../../role/services/role-service';
import { Role } from '../../role/models/role.model';
import { TranslateService } from '@ngx-translate/core';
import { RoleEditComponent } from '../role-edit/role-edit.component';

@Component({
  selector: 'app-role-listing',
  templateUrl: './role-listing.component.html',
  styleUrls: ['./role-listing.component.scss']
})
export class RoleListingComponent implements OnInit, AfterViewInit, OnDestroy {

  isCollapsed1 = false;
  isLoading = false;
  roles$: Observable<Role[]>;
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  role$: Observable<Role>;
  roleModel: Role = new Role();
  private clickListener: () => void;
  private idInAction: any;
  @ViewChild(RoleEditComponent) roleEditComponent!: RoleEditComponent;
  
  constructor(private apiService: RoleService, private cdr: ChangeDetectorRef, private renderer: Renderer2, private translate: TranslateService) { }

  changeLanguage(languageCode: string) {
    this.roleModel.changeLanguage(languageCode);
    this.cdr.markForCheck();
  }

  ngAfterViewInit(): void {
    this.clickListener = this.renderer.listen(document, 'click', (event) => {
      const closestBtn = event.target.closest('.btn');
      if (closestBtn) {
        const { action, id } = closestBtn.dataset;
        this.idInAction = id;
        switch (action) {
          case 'create':
            if (this.roleEditComponent) {
              this.roleEditComponent.create();
            }
            break;
          case 'edit':
            if (this.roleEditComponent) {
              this.roleEditComponent.edit(this.idInAction);
            }
            break;          
          case 'delete':
            if (this.roleEditComponent) {
              this.roleEditComponent.delete(this.idInAction);
            }
            break;            
        }
      }
    });
  }

  ngOnDestroy(): void {
    if (this.clickListener) {
      this.clickListener();
    }
  }

  ngOnInit(): void {
    this.roles$ = this.apiService.read(this.translate.currentLang);
    this.translate.onLangChange.subscribe(() => {
      this.roles$ = this.apiService.read(this.translate.currentLang);
    });
  }

  onActionCompleted() {
    this.roles$ = this.apiService.read(this.translate.currentLang);
    this.cdr.detectChanges();
  }
}