import { ChangeDetectorRef, Component, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'app-avatar',
    templateUrl: './avatar.component.html',
    styleUrls: ['./avatar.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        multi: true,
        useExisting: forwardRef(() => AvatarComponent),
    }]
})
export class AvatarComponent {
    base64Avatar: string | ArrayBuffer | null = null;    

    constructor(private cd: ChangeDetectorRef) { }

    onChange(event: Event) {
        const files = (event.target as HTMLInputElement).files;
        if (files && files.length > 0) {            
            let reader = new FileReader();
            reader.onloadend = () => {
                this.base64Avatar = reader.result;
                this.propagateChange(this.base64Avatar);
                this.cd.detectChanges();
            }
            reader.readAsDataURL(files[0]);
        }
    }

    onClear(event: Event) {
        event.stopPropagation();
        this.base64Avatar = '';
        this.propagateChange(this.base64Avatar);
        this.cd.detectChanges();
    }
    
    propagateChange = (_: any) => {};

    propagateTouched = () => {};
  
    registerOnChange(fn: any): void {
        this.propagateChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.propagateTouched = fn;
    }

    writeValue(obj: any): void {
        this.base64Avatar = obj;
    }
}