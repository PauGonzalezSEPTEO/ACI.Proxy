import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EditorComponent } from './editor.component';
import { EditorModule as TinyMCEEditorModule } from '@tinymce/tinymce-angular';

@NgModule({
    declarations: [EditorComponent],
    imports: [
        CommonModule,
        FormsModule,
        TinyMCEEditorModule
    ],  
    exports: [EditorComponent]
})
export class EditorModule { }
