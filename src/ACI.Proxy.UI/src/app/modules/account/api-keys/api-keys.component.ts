import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import * as am4themes_animated from "@amcharts/amcharts4/themes/animated";
import am4lang_en_US from "@amcharts/amcharts4/lang/en_US";
import am4lang_es_ES from "@amcharts/amcharts4/lang/es_ES";
import moment from 'moment';
import { Observable } from 'rxjs';
import { UserApiUsageStatistics } from '../models/api-keys.model';

am4core.useTheme(am4themes_animated.default);

@Component({
  selector: 'app-api-keys',
  templateUrl: './api-keys.component.html',
})
export class ApiKeysComponent implements OnInit, OnDestroy {
  apiKey?: string;
  private chart: am4charts.XYChart;
  datatableConfig: DataTables.Settings = {};
  // Reload emitter inside datatable
  reloadEvent: EventEmitter<boolean> = new EventEmitter();  
  isReadOnly = false;
  totalCalls = 0;

  constructor(
    private accountService: AccountService,
    private cdr: ChangeDetectorRef,
    private translate: TranslateService
  )
  {
    moment.locale(this.translate.currentLang);
    this.translate.onLangChange
      .subscribe((event: LangChangeEvent) => {
        moment.locale(event.lang);
        if (this.chart) {
          this.chart.language.locale = this.setLanguage(this.translate.currentLang);
        }
    });    
  }
  
  create(): void {
    this.accountService.generateUserApiKey().subscribe((apiKey: string) => {
      this.apiKey = apiKey;
      this.reloadEvent.emit(true);
      this.cdr.detectChanges();
    });
  }

  private createChart() {
    const buttonGroup = document.querySelector('.nav-group');
    if (buttonGroup) {
      buttonGroup.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        if (target.tagName === 'BUTTON') {
          const buttons = buttonGroup.querySelectorAll('button');
          buttons.forEach(button => button.classList.remove('active'));
          target.classList.add('active');
          this.updateChart(target.getAttribute('data-period'));
        }
      });
    }
    this.chart = am4core.create("chart", am4charts.XYChart);
    this.chart.language.locale = this.setLanguage(this.translate.currentLang);
    this.chart.xAxes.push(new am4charts.DateAxis());
    this.chart.yAxes.push(new am4charts.ValueAxis());
    let series = this.chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "value";
    series.dataFields.dateX = "date";
    series.tooltipText = "{value}"
    this.chart.scrollbarX = new am4core.Scrollbar();
    this.chart.cursor = new am4charts.XYCursor();
    this.updateChart(null);
  }

  delete(id: number) {
    this.accountService.deleteUserApiKey(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  ngOnInit(): void {
    this.createChart();
    var that = this;
    const capitalizeFirstLetter = (string: string) => {
      return string.charAt(0).toUpperCase() + string.slice(1);
    }
    this.datatableConfig = {
      serverSide: true,
      ajax: (dataTablesParameters: any, callback) => {
        this.accountService.readUserApiKeysDataTable(dataTablesParameters, this.translate.currentLang).subscribe(resp => {
          callback(resp);
        });
      },      
      columns: [
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.API_KEY'), name: "ACCOUNT.API_KEYS.API_KEY", data: "apiKeyLast6", render: function (data, _type, full) {
            return `
              <div class="d-flex flex-column">
                <span>${'*'.repeat(5) + full.apiKeyLast6}</span>
              </div>
            `;
          }
        },
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.EXPIRATION'), name: "ACCOUNT.API_KEYS.EXPIRATION", data: 'expiration', render: (data, _type, full) => {
            const date = data || full.expiration;
            let dateString = moment(date).fromNow();
            dateString = capitalizeFirstLetter(dateString);
            return `<div class="badge badge-light fw-bold">${dateString}</div>`;
          }
        },        
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.IS_ACTIVE'), name: "ACCOUNT.API_KEYS.IS_ACTIVE", data: 'isActive', 
          render: function (_data, _type, full) {            
            if (_data) {
              return '<span class="badge badge-light-success fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.ACTIVE') + '</span>';
            } else {
              return '<span class="badge badge-light-danger fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.INACTIVE') + '</span>';
            }
          },
          type: 'boolean'
        },        
        {
          title: this.translate.instant('ACCOUNT.API_KEYS.IS_VALID'), name: "ACCOUNT.API_KEYS.IS_VALID", data: 'isValid', 
          render: function (_data, _type, full) {            
            if (_data) {
              return '<span class="badge badge-light-success fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.VALID') + '</span>';
            } else {
              return '<span class="badge badge-light-danger fs-7 fw-semibold">' + that.translate.instant('ACCOUNT.API_KEYS.INVALID') + '</span>';
            }
          },
          type: 'boolean',
          orderable: false 
        }
      ],
      order: [[2, 'desc']]
    };       
  };

  ngOnDestroy() {
    if (this.chart) {
      this.chart.dispose();
    }
  }

  revoke(id: number) {
    this.accountService.revokeUserApiKey(id).subscribe(() => {
      this.reloadEvent.emit(true);
    });
  }

  setLanguage(lang: string): any | undefined {    
    switch (lang) {
      case 'es': {
        return am4lang_es_ES;
        break;
      }
      default: {
        return am4lang_en_US;
      }
    }
  }

  private updateChart(action: string | null) {
    switch (action) {
      case 'lasthour':
        this.updateChartData(() => this.accountService.getLastHourUserApiUsageStatistics());
        break;
      case 'last3hours':
        this.updateChartData(() => this.accountService.getLast3HoursUserApiUsageStatistics());
        break;
      case 'last12hours':
        this.updateChartData(() => this.accountService.getLast12HoursUserApiUsageStatistics());
        break;
      case 'lastday':
        this.updateChartData(() => this.accountService.getLastDayUserApiUsageStatistics());
        break;
      case 'last7days':
        this.updateChartData(() => this.accountService.getLast7DaysUserApiUsageStatistics());
        break;
      case 'last14days':
        this.updateChartData(() => this.accountService.getLast14DaysUserApiUsageStatistics());
        break;
      case 'lastmonth':
        this.updateChartData(() => this.accountService.getLastMonthUserApiUsageStatistics());
        break;
      case 'last3months':
        this.updateChartData(() => this.accountService.getLast3MonthsUserApiUsageStatistics());
        break;
      case 'last6months':
        this.updateChartData(() => this.accountService.getLast6MonthsUserApiUsageStatistics());
        break;
      case 'lastyear':
        this.updateChartData(() => this.accountService.getLastYearUserApiUsageStatistics());
        break;
      default:
        this.updateChartData(() => this.accountService.getLast6HoursUserApiUsageStatistics());
        break;
        this.accountService.getLast6HoursUserApiUsageStatistics().subscribe(data => {
          this.chart.data = data.map(item => ({
            date: new Date(item.date),
            value: item.value
          }));
        });
        break;  
    }      
  }

  private updateChartData(serviceMethod: () => Observable<UserApiUsageStatistics[]>) {
    this.totalCalls = 0;
    serviceMethod().subscribe(data => {
      const formattedData = data.map(item => ({
        date: new Date(item.date),
        value: item.value
      }));
      formattedData.sort((a, b) => a.date.getTime() - b.date.getTime());
      this.totalCalls = formattedData.reduce((sum, item) => sum + item.value, 0);
      this.chart.data = formattedData;
      this.cdr.detectChanges();
    });
  }
}