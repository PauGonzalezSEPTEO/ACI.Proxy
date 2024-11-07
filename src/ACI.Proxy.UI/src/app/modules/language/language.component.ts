import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Language } from './models/language.model';
import { NgSelectComponent } from '@ng-select/ng-select';

@Component({
  selector: 'app-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss']
})
export class LanguageComponent implements OnInit {

  @Input() isReadOnly: boolean = false;
  @Input() hasTranslations: boolean = false;
  @Output() changeEvent = new EventEmitter<string>();
  @Output() clearEvent = new EventEmitter();
  languages: Language[]
  selectedLanguage?: Language;
  @ViewChild(NgSelectComponent)
  ngSelectComponent: NgSelectComponent;

  constructor(private translate: TranslateService) { }

  changeLanguage(language: Language) {
    this.changeEvent.emit(language?.code);
  }

  clearSelection() {
    this.ngSelectComponent.clearModel();
  }
 
  ngOnInit(): void {
    this.languages = [new Language({
        code: 'en',
        name: this.translate.instant('LANGUAGES.ENGLISH'),
        flag: './assets/media/flags/united-kingdom.svg'
      }), new Language({
        code: 'es',
        name: this.translate.instant('LANGUAGES.SPANISH'),
        flag: './assets/media/flags/spain.svg'
      })];    
  }

  removeTranslations() {
    this.clearEvent.emit();
    this.ngSelectComponent.clearModel();
  }  
}