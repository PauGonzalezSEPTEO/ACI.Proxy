import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EditorService } from './services/editor-service';
import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: EditorComponent,
      multi: true
    }
  ]
})
export class EditorComponent implements ControlValueAccessor, OnInit {
  tinymceApiKey = environment.tinymceApiKey;
  customFields: { [key: string]: string };
  editorInstance: any;
  @Input() languageCode: string;
  modelsList: { [key: string]: string };
  @Input() templateContent: string;
  @Output() templateContentChange = new EventEmitter<string>();
  templateName: string
  tinymceInitOptions: any;
  
  private onChange: (value: string) => void;
  private onTouched: () => void;

  constructor(private editorService: EditorService, private cdr: ChangeDetectorRef, private translate: TranslateService) { } 

  createCustomFieldMenu() {
    if (this.editorInstance) {
      this.editorInstance.ui.registry.addMenuButton('customfieldmenu', {
          text: this.translate.instant('EDITOR.INSERT_CUSTOM_FIELD'),
          fetch: (callback: any) => {
              const items = this.getTemplateModelFields();
              callback(items);
          }
      });
    }
  }

  createCustomModelMenu() {
    if (this.editorInstance) {
      this.editorInstance.ui.registry.addMenuButton('custommodelmenu', {
          text: this.translate.instant('EDITOR.LOAD_TEMPLATE'),
          fetch: (callback: any) => {
              const items = this.getTemplateModel();
              callback(items);
          }
      });
    }
  }

  getTemplateModel() {
    return Object.keys(this.modelsList).map((field) => {
        return {
            type: 'menuitem',
            text: field,
            onAction: () => {
              this.loadTemplate(field);
            }
        };
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
        toolbar: 'custommodelmenu | customfieldmenu | code',
        language: this.translate.currentLang,
        setup: (editor: any) => {
            this.editorInstance = editor;
            this.createCustomModelMenu();
            this.createCustomFieldMenu();
            this.loadModels();
        }
      };
  }

  loadModels(): void {
    this.editorService.getModels().subscribe(models => {
      if (this.editorInstance) {
        this.modelsList = models.reduce((fields: { [key: string]: any }, fieldName: string) => {
          fields[fieldName] = `${fieldName}`;
          return fields;
        }, {});
        this.updateCustomModelsMenu();
      }       
    });
  }

  loadTemplate(templateName: string) {
    this.editorService.get(templateName, this.languageCode).subscribe(template => {
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

  updateCustomModelsMenu() {
    if (this.editorInstance) {
      this.editorInstance.ui.registry.getAll().buttons.custommodelmenu.fetch = (callback: any) => {
        const items = this.getTemplateModel();
        callback(items);
      };
    }
  }

  writeValue(value: string): void {
    this.templateContent = value;
  }
}