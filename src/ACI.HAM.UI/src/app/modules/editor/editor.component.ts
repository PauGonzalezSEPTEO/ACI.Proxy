import { ChangeDetectorRef, Component } from '@angular/core';
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
      [init]="{
        toolbar: 'undo redo | bold italic | alignleft aligncenter alignright | code',
        language: this.translate.currentLang,
      }"
    ></editor>
  `
})
export class EditorComponent {
  templateContent: string;
  templateName: string

  constructor(private templateService: TemplateService, private cdr: ChangeDetectorRef, public translate: TranslateService) { } 

  loadTemplate(event: Event, templateName: string): void {
    event.preventDefault();
    this.templateService.get(templateName).subscribe(content => {
      this.templateContent = content;
      this.cdr.detectChanges();
    });
  }  
}