import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TemplateService } from './services/template-service';
import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-editor',
  template: `
    <div class="input-group">
		  <input type="text" class="form-control" [(ngModel)]="templateName" [placeholder]="'EDITOR.ENTER_TEMPLATE_NAME' | translate">
			<div class="input-group-append">
			  <button (click)="loadTemplate($event, templateName)" class="btn btn-secondary" type="button">{{ 'EDITOR.LOAD_TEMPLATE' | translate }}</button>
			</div>
		</div>
    <editor
      [(ngModel)]="templateContent"
      (ngModelChange)="onEditorChange($event)"
      apiKey="z72bhhmoeqv12t5uw13d72wdq76iimivde69bvase42k0fv2"
      [init]="tinymceInitOptions"
    ></editor>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: EditorComponent,
      multi: true
    }
  ]
})
export class EditorComponent implements ControlValueAccessor, OnInit {
  customFields: { [key: string]: any };
  editorInstance: any;
  @Input() languageCode: string;
  @Input() templateContent: string;
  @Output() templateContentChange = new EventEmitter<string>();
  templateName: string
  tinymceInitOptions: any;
  
  private onChange: (value: string) => void;
  private onTouched: () => void;

  constructor(private templateService: TemplateService, private cdr: ChangeDetectorRef, private translate: TranslateService) { } 

  createCustomFieldMenu(editor: any) {
    editor.ui.registry.addMenuButton('customfieldmenu', {
        text: this.translate.instant('EDITOR.INSERT_CUSTOM_FIELD'),
        fetch: (callback: (items: any[]) => void) => {
            const items = this.getTemplateModelFields();
            callback(items);
        }
    });
  }

  getTemplateModelFields() {
    return Object.keys(this.customFields).map((field) => {
        return {
            type: 'menuitem',
            text: field,
            onAction: () => {
                this.editorInstance.insertContent(this.customFields[field]);
            }
        };
    });
  }

  initializeTinyMCE() {
      this.tinymceInitOptions = {
        plugins: 'code',
        toolbar: 'undo redo | bold italic | alignleft aligncenter alignright | customfieldmenu | code',
        language: this.translate.currentLang,
        setup: (editor: any) => {
            this.editorInstance = editor;
            this.createCustomFieldMenu(editor);
        }
      };
  }

  loadTemplate(event: Event, templateName: string): void {
    event.preventDefault();
    this.templateService.get(templateName, this.languageCode).subscribe(template => {
      this.customFields = template.customFields.reduce((fields: { [key: string]: any }, fieldName: string) => {
        fields[fieldName] = `{{${fieldName}}}`;
        return fields;
      }, {});
      this.templateContent = template.htmlContent;
      if (this.editorInstance) {
        this.updateCustomFieldMenu();
      }      
      this.onEditorChange(this.templateContent);
      this.cdr.detectChanges();
    });
  }

  ngOnInit() {
    this.initializeTinyMCE();
  }

  onEditorChange(value: string) {
    this.templateContent = value;
    this.templateContentChange.emit(this.templateContent);
    if (this.onChange) {
      this.onChange(value);
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setValue(value: string): void {
    this.templateContent = value;
    this.onChange(value);
    this.templateContentChange.emit(value);
  }

  updateCustomFieldMenu() {
    if (this.editorInstance) {
        this.editorInstance.ui.registry.getAll().buttons.customfieldmenu.fetch = (callback: any) => {
            const items = this.getTemplateModelFields();
            callback(items);
        };
    }
  }

  writeValue(value: string): void {
    this.templateContent = value;
  }
}