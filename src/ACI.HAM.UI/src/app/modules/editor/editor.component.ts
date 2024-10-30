import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EditorService } from './services/editor-service';
import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

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
  customFields: { [key: string]: any };
  editorInstance: any;
  @Input() languageCode: string;
  modelsList: string[];
  @Input() templateContent: string;
  @Output() templateContentChange = new EventEmitter<string>();
  templateName: string
  tinymceInitOptions: any;
  
  private onChange: (value: string) => void;
  private onTouched: () => void;

  constructor(private editorService: EditorService, private cdr: ChangeDetectorRef, private translate: TranslateService) { } 

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

  loadModels(): void {
    this.editorService.getModels().subscribe(models => {
      this.modelsList = models;
      this.cdr.detectChanges();
    });
  }

  loadTemplate(event: Event, templateName: string): void {
    event.preventDefault();
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
    this.loadModels();
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