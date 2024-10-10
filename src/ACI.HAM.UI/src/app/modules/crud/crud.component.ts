import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal, NgbModalOptions, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { DataTableDirective } from 'angular-datatables';
import { Subject, fromEvent } from 'rxjs';
import { debounceTime, map } from 'rxjs/operators';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { SweetAlertOptions } from 'sweetalert2';
import datatables_net_es_ES from 'datatables.net-plugins/i18n/es-ES.json';
import datatables_net_en_GB from 'datatables.net-plugins/i18n/en-GB.json';

@Component({
  selector: 'app-crud',
  templateUrl: './crud.component.html',
  styleUrls: ['./crud.component.scss'],
})
export class CrudComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input() datatableConfig: DataTables.Settings = {};
  @Input() route: string = '/';
  // Reload emitter inside datatable
  @Input() reload: EventEmitter<boolean>;
  @Input() modal: TemplateRef<any>;
  @Input() isReadOnly: boolean;
  @Output() deleteEvent = new EventEmitter<any>();
  @Output() emailEvent = new EventEmitter<any>();
  @Output() unlinkEvent = new EventEmitter<any>();
  @Output() editEvent = new EventEmitter<any>();
  @Output() createEvent = new EventEmitter<boolean>();
  @ViewChild(DataTableDirective, { static: false })
  private datatableElement: DataTableDirective;
  dtTrigger: Subject<any> = new Subject<void>();
  dtOptions: DataTables.Settings = {};
  @ViewChild('deleteSwal')
  public readonly deleteSwal!: SwalComponent;
  @ViewChild('unlinkSwal')
  public readonly unlinkSwal!: SwalComponent;
  @ViewChild('emailSwal')
  public readonly emailSwal!: SwalComponent;
  @ViewChild('successDeleteSwal')
  public readonly successDeleteSwal!: SwalComponent;
  @ViewChild('successUnlinkSwal')
  public readonly successUnlinkSwal!: SwalComponent;
  @ViewChild('successEmailSwal')
  public readonly successEmailSwal!: SwalComponent;
  private emailInAction: any;
  private idInAction: any;
  modalConfig: NgbModalOptions = {
    modalDialogClass: 'modal-dialog modal-dialog-centered mw-650px',
  };
  swalOptions: SweetAlertOptions = { buttonsStyling: false };
  private modalRef: NgbModalRef;
  private filterValue: string;
  private clickListener: () => void;

  constructor(private renderer: Renderer2, private router: Router, private modalService: NgbModal, private translate: TranslateService) {
    this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        var that = this;
        this.datatableElement.dtInstance.then((_dtInstance: DataTables.Api) => {
          this.dtOptions.language = that.setLanguage(event.lang);
          this.dtOptions.columns?.forEach(function (value, _key) {
            if (value.name) {
              value.title = that.translate.instant(value.name || [""]);
            }
          });
          this.dtTrigger.next(null);
          this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => dtInstance.search(this.filterValue).draw());
        });
      });
  }

  ngAfterViewInit(): void {
    var that = this;
    this.clickListener = this.renderer.listen(document, 'click', (event) => {
      const closestBtn = event.target.closest('.btn');
      if (closestBtn) {
        const { action, id, email } = closestBtn.dataset;
        this.idInAction = id;
        this.emailInAction = email
        switch (action) {
          case 'view':
            this.router.navigate([`${this.route}/${id}`]);
            break;
          case 'create':
            this.createEvent.emit(true);
            if (this.modal) {
              that.modalRef = this.modalService.open(this.modal, this.modalConfig);
            }
            break;
          case 'edit':
            this.editEvent.emit({ id: this.idInAction, isReadOnly: this.isReadOnly });
            that.modalRef = this.modalService.open(this.modal, this.modalConfig);
            break;
          case 'delete':
            this.deleteSwal.fire().then((clicked) => {
              if (clicked.isConfirmed) {
                this.successDeleteSwal.fire();
              }
            });
            break;
          case 'email':
              this.emailSwal.fire().then((clicked) => {
                if (clicked.isConfirmed) {
                  this.successEmailSwal.fire();
                }
              });
              break;            
          case 'unlink':
            this.unlinkSwal.fire().then((clicked) => {
              if (clicked.isConfirmed) {
                this.successUnlinkSwal.fire();
              }
            });
            break;
          }
      }
    });
    this.triggerFilter();
    this.dtTrigger.next(null);
  }

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
    this.reload.unsubscribe();
    if (this.clickListener) {
      this.clickListener();
    }
    this.modalService.dismissAll();
  }

  ngOnInit(): void {
    this.dtOptions = {
      destroy: true,
      dom: "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
      processing: true,
      language: this.setLanguage(this.translate.currentLang),
      ...this.datatableConfig
    };
    this.renderActionColumn();
    this.setupSweetAlert();
    if (this.reload) {
      this.reload.subscribe(_data => {
        this.modalService.dismissAll();
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => dtInstance.ajax.reload());
      });
    }
  }

  renderActionColumn(): void {
    const actionColumn = {
      name: "CRUD.ACTIONS",
      data: 'actions',
      sortable: false,
      title: this.translate.instant('CRUD.ACTIONS'),
      render: (_data: any, _type: any, full: any) => {
        let editIcon: string;
        if (this.isReadOnly) {
          editIcon = 'magnifier';
        } else {
          editIcon = 'pencil';
        }
        const editButton = `
          <button class="btn btn-icon btn-active-light-primary w-30px h-30px me-3" data-action="edit" data-id="${full.id}" title="${this.translate.instant('CRUD.EDIT')}">
            <i class="ki-duotone ki-${editIcon} fs-3"><span class="path1"></span><span class="path2"></span></i>
          </button>`;
        const deleteButton = `
          <button class="btn btn-icon btn-active-light-primary w-30px h-30px" data-action="delete" data-id="${full.id}" title="${this.translate.instant('CRUD.DELETE')}">
            <i class="ki-duotone ki-trash fs-3">
              <span class="path1"></span><span class="path2"></span>
              <span class="path3"></span><span class="path4"></span><span class="path5"></span>
            </i>
          </button>`;
        const unlinkButton = `
        <button class="btn btn-icon btn-active-light-primary w-30px h-30px" data-action="unlink" data-id="${full.id}" title="${this.translate.instant('CRUD.UNLINK')}">
          <i class="ki-duotone ki-disconnect fs-3">
            <span class="path1"></span><span class="path2"></span>
            <span class="path3"></span><span class="path4"></span><span class="path5"></span>
          </i>
        </button>`;
        const emailButton = `
        <button class="btn btn-icon btn-active-light-primary w-30px h-30px" data-action="email" data-email="${full.email}" title="${this.translate.instant('CRUD.SEND_EMAIL')}">
          <i class="ki-duotone ki-sms fs-3">
            <span class="path1"></span><span class="path2"></span>
            <span class="path3"></span><span class="path4"></span><span class="path5"></span>
          </i>
        </button>`;        
        const buttons = [];
        if (this.editEvent.observed) {
          buttons.push(editButton);
        }
        if (this.deleteEvent.observed) {
          buttons.push(deleteButton);
        }
        if (this.unlinkEvent.observed) {
          buttons.push(unlinkButton);
        }
        if (this.emailEvent.observed && !full.emailConfirmed) {
          buttons.push(emailButton);
        }
        return buttons.join('');
      },
    };
    if (this.dtOptions.columns) {
      this.dtOptions.columns.push(actionColumn);
    }
  }

  setLanguage(lang: string): any | undefined {
    var url;
    switch (lang) {
      case 'es': {
        url =  datatables_net_es_ES;
        break;
      }
      default: {
        url = datatables_net_en_GB;
      }
    }
    return url;
  }

  setupSweetAlert() {
    this.swalOptions = {
      buttonsStyling: false,
    };
  }

  triggerDelete() {
    this.deleteEvent.emit(this.idInAction);
  }

  triggerEmail() {
    this.emailEvent.emit(this.emailInAction);
  }

  triggerFilter() {
    fromEvent<KeyboardEvent>(document, 'keyup')
      .pipe(
        debounceTime(50),
        map(event => {
          const target = event.target as HTMLElement;
          const action = target.getAttribute('data-action');
          const value = (target as HTMLInputElement).value?.trim().toLowerCase();
          return { action, value };
        })
      )
      .subscribe(({ action, value }) => {
        if (action === 'filter') {
          this.filterValue = value;
          this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => dtInstance.search(value).draw());
        }
      });
  }

  triggerUnlink() {
    this.unlinkEvent.emit(this.idInAction);
  }
}

