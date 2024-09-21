import { Directive, Input } from '@angular/core';
import { AbstractControl, Validator, NG_VALIDATORS } from '@angular/forms';

@Directive({
  selector: '[appTranslationsValidator]',
  providers: [{
    provide: NG_VALIDATORS,
    useExisting: TranslationsValidatorDirective,
    multi: true
  }]
})
export class TranslationsValidatorDirective implements Validator {
    @Input('translations')
    translations: any;
    @Input('validators')
    validators: string;
    
  validate(_control: AbstractControl) : { [key: string]: any } | null {
    var validations = JSON.parse(this.validators)
    if (validations) {
        if (validations.hasOwnProperty('property')) {
            var property = validations['property'];
            if (property) {
                var required: boolean = false;
                if (validations.hasOwnProperty('required')) {
                    required = validations['required'];
                }
                var minlength: number | undefined = undefined;
                if (validations.hasOwnProperty('minlength')) {
                    minlength = validations['minlength'];
                }
                var maxlength: number | undefined = undefined;
                if (validations.hasOwnProperty('maxlength')) {
                    maxlength = validations['maxlength'];
                }
                if (this.translations) {
                    var errorMaxlength: boolean = false,
                        errorMinlength: boolean = false,
                        errorRequired: boolean = false;
                    this.translations.forEach(function (translation: any) {
                        if (translation && translation.hasOwnProperty(property)) {
                            var value = translation[property];
                            if (!errorRequired && required && !value) {
                                errorRequired = true;
                            }
                            if (!errorMinlength && minlength && value && value.length < minlength) {
                                errorMinlength = true;
                            }
                            if (!errorMaxlength && maxlength && value && value.length > maxlength) {
                                errorMaxlength = true;
                            }
                        }
                    });
                    var result: { [key: string]: any } = {};
                    if (errorRequired) {
                        result = Object.assign(result, { required: true });
                    }
                    if (errorMinlength) {
                        result = Object.assign(result, { minlength: true });
                        return { minlength: true };
                    }
                    if (errorMaxlength) {
                        result = Object.assign(result, { maxlength: true });
                    }
                    return result;
                }
            }
        }
    }
    return null;
  }
}