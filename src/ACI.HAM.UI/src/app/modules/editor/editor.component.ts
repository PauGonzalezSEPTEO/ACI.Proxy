import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { TemplateService } from './services/template-service';
import { TranslateService } from '@ngx-translate/core';

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
      apiKey="z72bhhmoeqv12t5uw13d72wdq76iimivde69bvase42k0fv2"
      [init]="tinymceInitOptions"
    ></editor>
  `
})
export class EditorComponent implements OnInit {
  customFields: { [key: string]: any };
  editorInstance: any;
  templateContent: string;
  templateName: string
  tinymceInitOptions: any;

  constructor(private templateService: TemplateService, private cdr: ChangeDetectorRef, private translate: TranslateService) { } 

  createCustomFieldMenu(editor: any) {
    editor.ui.registry.addMenuButton('customFieldMenu', {
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
        toolbar: 'undo redo | bold italic | alignleft aligncenter alignright | code | customFieldMenu',
        language: this.translate.currentLang,
        setup: (editor: any) => {
            this.editorInstance = editor;
            this.createCustomFieldMenu(editor);
        }
      };
  }

  loadTemplate(event: Event, templateName: string): void {
    event.preventDefault();
    this.templateService.get(templateName).subscribe(template => {
      this.customFields = template.customFields.reduce((fields: { [key: string]: any }, fieldName: string) => {
        fields[fieldName] = `{{${fieldName}}}`;
        return fields;
      }, {});
      this.templateContent = template.htmlContent;
      if (this.editorInstance) {
        this.updateCustomFieldMenu();
      }      
      this.cdr.detectChanges();
    });
  }

  ngOnInit() {
    this.initializeTinyMCE();
  }

  updateCustomFieldMenu() {
    if (this.editorInstance) {
        this.editorInstance.ui.registry.getAll().buttons.customFieldMenu.fetch = (callback: any) => {
            const items = this.getTemplateModelFields();
            callback(items);
        };
    }
  }
}