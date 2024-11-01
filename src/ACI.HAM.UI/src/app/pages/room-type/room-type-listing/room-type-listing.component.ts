import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { Observable, Subscription } from 'rxjs';
import { RoomTypeService } from '../services/room-type-service';
import { SweetAlertOptions } from 'sweetalert2';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { RoomType } from '../models/room-type.model';
import { BuildingService } from '../../building/services/building-service';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { Building } from '../../building/models/building.model';
import { AuthService } from 'src/app/modules/auth';
import { CompanyService } from '../../company/services/company-service';
import { HotelService } from '../../hotel/services/hotel-service';
import { Company } from '../../company/models/company.model';

@Component({
  selector: 'app-room-type-listing',
  templateUrl: './room-type-listing.component.html',
  styleUrls: ['./room-type-listing.component.scss']
})
export class RoomTypeListingComponent implements OnInit, OnDestroy {
  isReadOnly = false;
  isCollapsed1 = false;
  isCollapsed2 = true;
  isLoading = false;
  roomTypes: DataTablesResponse;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();
  // Single model
  aRoomType: Observable<RoomType>;
  roomTypeModel: RoomType = new RoomType();
  @ViewChild('noticeSwal')
  noticeSwal!: SwalComponent;
  swalOptions: SweetAlertOptions = {};
  buildings$: Observable<Building[]>;
  buildingsSubscription: Subscription;
  companiesList: Company[] | null = null;
  
  constructor(
    private apiService: RoomTypeService,
    private buildingService: BuildingService,    
    private companyService: CompanyService,        
    private hotelService: HotelService,  
    public authService: AuthService,
    private cdr: ChangeDetectorRef,
    private translate: TranslateService) { }

  changeLanguage(languageCode: string) {
    this.roomTypeModel.changeLanguage(languageCode);
    this.cdr.markForCheck();
  }

  create() {
    this.roomTypeModel = new RoomType();
  }

  delete(id: number) {
    this.apiService.delete(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  edit(event: { id: number, isReadOnly: boolean }) {
    this.isReadOnly = event.isReadOnly;
    this.aRoomType = this.apiService.get(event.id);
    this.aRoomType.subscribe((roomType: RoomType) => {
      this.roomTypeModel = new RoomType(roomType);
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

  loadBuildingsForHotels() {
    const hotelIds = this.roomTypeModel.getRelevantHotelIds();
    this.buildings$ = this.buildingService.getByHotelIds(hotelIds, this.translate.currentLang);
    if (this.buildingsSubscription) {
      this.buildingsSubscription.unsubscribe();
    }
    this.buildingsSubscription = this.buildings$.subscribe(newBuildings => {
      const newBuildingIds = newBuildings.map(building => building.id);
      if (this.roomTypeModel.buildings) {
        this.roomTypeModel.buildings = this.roomTypeModel.buildings.filter(buildingId => newBuildingIds.includes(buildingId));
      }
    });
  }

  loadHotelsForSelectedCompanies() {
    const selectedCompanyIds = this.roomTypeModel.companies;
    this.hotelService.getByCompanyIds(selectedCompanyIds).subscribe(hotels => {
      const selectedHotelIds = this.roomTypeModel.hotels;
      this.roomTypeModel.hotelsList = hotels
      this.roomTypeModel.hotels = hotels
        .filter(hotel => selectedHotelIds.includes(hotel.id))
        .map(hotel => hotel.id);
      this.loadBuildingsForHotels();     
    });
  }

  removeTranslations() {
    this.roomTypeModel.removeTranslations();
  }

  showAlert(swalOptions: SweetAlertOptions) {
    let style = swalOptions.icon?.toString() || 'success';
    if (swalOptions.icon === 'error') {
      style = 'danger';
    }
    this.swalOptions = Object.assign({
      buttonsStyling: false,
      confirmButtonText: this.translate.instant('ROOM_TYPES.LIST.OK'),
      customClass: {
        confirmButton: "btn btn-" + style
      }
    }, swalOptions);
    this.cdr.detectChanges();
    this.noticeSwal.fire();
  }

  ngOnDestroy(): void {
    this.reloadEvent.unsubscribe();
    if (this.buildingsSubscription) {
      this.buildingsSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
      this.loadBuildingsForHotels();
    });
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
          title: this.translate.instant('ROOM_TYPES.LIST.NAME'), name: "ROOM_TYPES.LIST.NAME", data: "name", render: function (data, _type, full) {
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
      }
    };
  }

  onChange($event: any, building: number) {
    this.roomTypeModel.updateBuilding(building, $event.target.checked);
  }

  onCompaniesChange() {
    this.loadHotelsForSelectedCompanies();
  }

  onHotelsChange() {
    this.loadBuildingsForHotels();
  }

  onSubmit(_event: Event, myForm: NgForm) {
    if (myForm && myForm.invalid) {
      return;
    }
    this.isLoading = true;
    const successAlert: SweetAlertOptions = {
      icon: 'success',
      title: this.translate.instant('ROOM_TYPES.LIST.SUCCESS'),
      text: this.roomTypeModel.id > 0 ? this.translate.instant('ROOM_TYPES.LIST.ROOM_TYPE_UPDATED_SUCCESSFULLY') : this.translate.instant('ROOM_TYPES.LIST.ROOM_TYPE_CREATED_SUCCESSFULLY'),
    };
    const errorAlert: SweetAlertOptions = {
      icon: 'error',
      title: this.translate.instant('ROOM_TYPES.LIST.ERROR'),
      text: '',
    };
    const completeFn = () => {
      this.isLoading = false;
    };
    const updateFn = () => {
      this.apiService.update(this.roomTypeModel.id, this.roomTypeModel).subscribe({
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
      this.apiService.create(this.roomTypeModel).subscribe({
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
    if (this.roomTypeModel.id > 0) {
      updateFn();
    } else {
      createFn();
    }
  }
}