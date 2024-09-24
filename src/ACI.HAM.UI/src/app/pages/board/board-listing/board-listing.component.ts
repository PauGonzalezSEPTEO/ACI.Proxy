import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable } from 'rxjs';
import { BoardService } from '../services/board-service';
import { SweetAlertOptions } from 'sweetalert2';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Board } from '../models/board.model';
import { BuildingService } from '../../building/services/building-service';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { Building } from '../../building/models/building.model';
import { AuthService } from 'src/app/modules/auth';
import { Company } from '../../company/models/company.model';
import { HotelService } from '../../hotel/services/hotel-service';
import { CompanyService } from '../../company/services/company-service';

@Component({
  selector: 'app-board-listing',
  templateUrl: './board-listing.component.html',
  styleUrls: ['./board-listing.component.scss']
})
export class BoardListingComponent implements OnInit, AfterViewInit, OnDestroy {
  
  isReadOnly = false;
  isCollapsed1 = false;
  isCollapsed2 = true;
  isLoading = false;
  boards: DataTablesResponse;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  aBoard: Observable<Board>;
  boardModel: Board = new Board();
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  buildings$: Observable<Building[]>;
  companiesList: Company[] | null = null;

  constructor(
    private apiService: BoardService,
    private buildingService: BuildingService,
    public authService: AuthService,
    private companyService: CompanyService,        
    private hotelService: HotelService,    
    private cdr: ChangeDetectorRef,
    private translate: TranslateService
  ) { }

  changeLanguage(languageCode: string) {
    this.boardModel.changeLanguage(languageCode);
    this.cdr.markForCheck();
  }

  create() {
    this.boardModel = new Board();
  }

  delete(id: number) {
    this.apiService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(event: { id: number, isReadOnly: boolean }) {
    this.isReadOnly = event.isReadOnly;
    this.aBoard = this.apiService.get(event.id);
    this.aBoard.subscribe((board: Board) => {
      this.boardModel = new Board(board);
      this.loadHotelsForSelectedCompanies();      
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

  loadHotelsForSelectedCompanies() {
    const selectedCompanyIds = this.boardModel.companies;
    this.hotelService.getByCompanyIds(selectedCompanyIds).subscribe(hotels => {
      const selectedHotelIds = this.boardModel.hotels;
      this.boardModel.hotelsList = hotels
      this.boardModel.hotels = hotels
        .filter(hotel => selectedHotelIds.includes(hotel.id))
        .map(hotel => hotel.id);
    });
  }

  removeTranslations() {
    this.boardModel.removeTranslations();
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('BOARDS.LIST.OK'),
      customClass: {
        confirmButton: "btn btn-" + style
      }
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }
  
  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {
    this.reloadEvent.unsubscribe();
  }

  ngOnInit(): void {
    const getBuildingsFn = () => {
      this.buildings$ = this.buildingService.search('', '', this.translate.currentLang);
    };
    this.translate.onLangChange.subscribe((_event: LangChangeEvent) => {
      getBuildingsFn();
    });
    getBuildingsFn();
    this.companyService.search('', '', this.translate.currentLang).subscribe((companies: Company[]) => {
      this.companiesList = companies;
    });    
    this.datatableConfig = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.readDataTable(dataTablesParameters, this.translate.currentLang).subscribe(resp => {
          callback(resp);
        });
      },
      columns: [
        {
          title: this.translate.instant('BOARDS.LIST.NAME'), name: "BOARDS.LIST.NAME", data: "name", render: function (data, _type, full) {
            const colorClasses = ['success', 'info', 'warning', 'danger'];
            const randomColorClass = colorClasses[Math.floor(Math.random() * colorClasses.length)];
            const initials = data[0].toUpperCase();
            const symbolLabel = `
              <div class="symbol-label fs-3 bg-light-${randomColorClass} text-${randomColorClass}">
                ${initials}
              </div>
            `;
            const name = `
              <div class="d-flex flex-column" data-action="view" data-id="${full.id}">
                <a href="javascript:;" class="text-gray-800 text-hover-primary mb-1">${data}</a>
                <span>${full.name}</span>
              </div>
            `;
            return `
              <div class="symbol symbol-circle symbol-50px overflow-hidden me-3" data-action="view" data-id="${full.id}">
                <a href="javascript:;">
                  ${symbolLabel}
                </a>
              </div>
              ${name}
            `;
          }
        }
      ],
      createdRow: function (row, _data, _dataIndex) {
        $('td:eq(0)', row).addClass('d-flex align-items-center');
      },
    };   
  }

  onChange($event: any, building: number) {
    this.boardModel.updateBuilding(building, $event.target.checked);
  }

  onCompaniesChange() {
    this.loadHotelsForSelectedCompanies();
  }

  onSubmit(_event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }
    this.isLoading = true;
    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: this.translate.instant('BOARDS.LIST.SUCCESS'),
      text: this.boardModel.id > 0 ? this.translate.instant('BOARDS.LIST.BOARD_UPDATED_SUCCESSFULLY') : this.translate.instant('BOARDS.LIST.BOARD_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: this.translate.instant('BOARDS.LIST.ERROR'),
      text: '',
    };
    const completeFn = () => {
      this.isLoading = false;
    };
    const updateFn = () => {
      this.apiService.update(this.boardModel.id, this.boardModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: completeFn,
      });
    };
    const createFn = () => {
      this.apiService.create(this.boardModel).subscribe({
        next: () => {
          this.showAlert(successAlert);
          this.reloadEvent.emit(true);
        },
        error: (error) => {
          errorAlert.text = this.extractText(error.error);
          this.showAlert(errorAlert);
          this.isLoading = false;
        },
        complete: completeFn,
      });
    };
    if (this.boardModel.id > 0) {
      updateFn();
    } else {
      createFn();
    }
  }
}