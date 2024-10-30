import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EditorComponent } from './editor.component';
import { EditorModule as TinyMCEEditorModule } from '@tinymce/tinymce-angular';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslationModule } from '../../modules/i18n/translation.module';

@NgModule({
    declarations: [EditorComponent],
    imports: [
        CommonModule,
        TranslationModule,
        NgSelectModule,
        FormsModule,
        TinyMCEEditorModule
    ],  
    exports: [EditorComponent]
})
export class EditorModule { }
