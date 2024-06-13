import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { RoleService } from '../../role/services/role-service';
import { SweetAlertOptions } from 'sweetalert2';
import { Role } from '../../role/models/role.model';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-role-edit',
  templateUrl: './role-edit.component.html',
  styleUrls: ['./role-edit.component.scss']
})
export class RoleEditComponent implements OnInit, OnDestroy {

  isCollapsed1 = false;
  isLoading = false;
  roles$: Observable<Role[]>;
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  role$: Observable<Role>;
  roleModel: Role = new Role();
  @ViewChild('formModal')
  formModal: TemplateRef<any>;
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  modalConfig: NgbModalOptions = {
    modalDialogClass: 'modal-dialog modal-dialog-centered mw-650px',
  };
  @Output() actionCompleted = new EventEmitter<string>();
  
  constructor(private apiService: RoleService, private cdr: ChangeDetectorRef, private renderer: Renderer2, private modalService: NgbModal, private translate: TranslateService) { }

  changeLanguage(languageCode: string) {
    this.roleModel.changeLanguage(languageCode);
    this.cdr.markForCheck();
  }

  complete(modal: boolean) {
    this.isLoading = false;
    if (modal) {
      this.modalService.dismissAll();
    }
    this.actionCompleted.emit();
  };

  create() {
    this.roleModel = new Role();
    this.modalService.open(this.formModal, this.modalConfig);
  }

  delete(id: string) {
    const deleteFn = () => {
      this.isLoading = true;
      const successAlert: SweetAlertOptions = {
        icon: 'success',
        title: this.translate.instant('ROLES.EDIT.SUCCESS'),
        text: this.translate.instant('ROLES.EDIT.DELETE_SUCCESSFULLY'),
      };
      const errorAlert: SweetAlertOptions = {
        icon: 'error',
        title: this.translate.instant('ROLES.EDIT.ERROR'),
        text: '',
      };
      this.apiService.delete(id).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: () => {
          this.complete(false);;
        }
      });
    };
    this.swalOptions = {
      cancelButtonText: this.translate.instant('ROLES.EDIT.CANCEL'),
      confirmButtonText: this.translate.instant('ROLES.EDIT.OK'),
      customClass: {
        cancelButton: "btn btn-active-light",
        confirmButton: "btn btn-danger"
      },
      icon: 'warning',
      showCancelButton: true,
      text: this.translate.instant('ROLES.EDIT.THIS_CANNOT_BE_UNDONE'),
      title: this.translate.instant('ROLES.EDIT.ARE_YOU_SURE_TO_DELETE_IT')
    };
    this.cdr.detectChanges();
    this.noticeSwal.fire().then((result) => {
      if (result.isConfirmed) {
        deleteFn();
      }
    });
  }

  edit(id: string) {
    this.role$ = this.apiService.get(id);
    this.role$.subscribe((role: Role) => {
      this.roleModel = role;
      this.modalService.open(this.formModal, this.modalConfig);
    });
  }

  extractText(obj: any): string {
    var textArray: string[] = [];
    for (var key in obj) {
      if (typeof obj[key] === 'string') {
        // If the value is a string, add it to the 'textArray'
        textArray.push(obj[key]);
      } else if (typeof obj[key] === 'object') {
        // If the value is an object, recursively call the function and concatenate the results
        textArray = textArray.concat(this.extractText(obj[key]));
      }
    }
    // Use a Set to remove duplicates and convert back to an array
    var uniqueTextArray = Array.from(new Set(textArray));
    // Convert the uniqueTextArray to a single string with line breaks
    var text = uniqueTextArray.join('\n');
    return text;
  }
  
  removeTranslations() {
    this.roleModel.removeTranslations();
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('ROLES.EDIT.OK'),
      customClass: {
        confirmButton: "btn btn-" + style
      },
      showCancelButton: false
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }

  ngOnDestroy(): void {
    this.modalService.dismissAll();
  }

  ngOnInit(): void {
    this.roles$ = this.apiService.read(this.translate.currentLang);
    this.translate.onLangChange.subscribe((_event: LangChangeEvent) => {
      this.roles$ = this.apiService.read(this.translate.currentLang);
    });
  }

  onSubmit(_event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }
    this.isLoading = true;
    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: this.translate.instant('ROLES.EDIT.SUCCESS'),
      text: this.roleModel?.id ? this.translate.instant('ROLES.EDIT.ROLE_UPDATED_SUCCESSFULLY') : this.translate.instant('ROLES.EDIT.ROLE_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: this.translate.instant('ROLES.EDIT.ERROR'),
      text: '',
    };
    const updateFn = () => {
      this.apiService.update(this.roleModel.id, this.roleModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: () => {
          this.complete(true);
        }
      });
    };
    const createFn = () => {
      this.apiService.create(this.roleModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: () => {
          this.complete(true);
        }
      });
    };
    if (this.roleModel.id) {
      updateFn();
    } else {
      createFn();
    }
  }
}