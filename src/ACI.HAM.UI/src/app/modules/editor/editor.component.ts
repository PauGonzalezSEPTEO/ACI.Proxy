import { ChangeDetectorRef, Component } from '@angular/core';
import { TemplateService } from './services/template-service';

@Component({
  selector: 'app-editor',
  template: `
    <editor
      [(ngModel)]="templateContent"
      apiKey="z72bhhmoeqv12t5uw13d72wdq76iimivde69bvase42k0fv2"
      [init]="{
        toolbar: 'undo redo | bold italic | alignleft aligncenter alignright | code'
      }"
    ></editor>
  `
})
export class EditorComponent {
  templateContent: string;

  constructor(private templateService: TemplateService, private cdr: ChangeDetectorRef) { } 

  loadTemplate(): void {
    this.templateService.get('ChangeEmail').subscribe(content => {
      this.templateContent = content;
      this.cdr.detectChanges();
    });
  }  
}