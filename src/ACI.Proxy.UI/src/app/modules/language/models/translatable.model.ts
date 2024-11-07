export class ITranslation {
    languageCode: string;

    constructor(translation?: ITranslation) {
        if (translation) {
            this.languageCode = translation.languageCode;
        }
    }
}

export class Translatable<T extends ITranslation> {
    languageCode: string = '';
    translations?: T[];

    constructor(translatable?: Translatable<T>) {
        if (translatable) {
            this.translations = translatable.translations;
        }
    }

    private get = <T, K extends keyof T>(obj: T, key: K) => obj[key];

    public getTranslation(property: any) {
        if (this.languageCode) {
            var that = this;
            var translation = this.translations?.find(function (item) {
                return item.languageCode === that.languageCode;
            });
            if (translation) {
                return this.get(translation, property) ?? '';
            }
            return '';
        }
        return this.get(this, property);
    }

    public hasTranslation(property: any): boolean {
        var that = this;
        var exists = this.translations?.some(function (item) {
            return that.get(item, property) !== undefined;
        });
        if (exists) {
            return true;
        }
        return false;
    }

    public hasTranslations(): boolean {
        var that = this;
        var exists = this.translations?.some(function (item) {
            return item.languageCode === that.languageCode;
        });
        if (exists) {
            return true;
        }
        return false;
    }

    public removeTranslations() {
        var that = this;
        this.translations = this.translations?.filter(function (item) {
            return item.languageCode != that.languageCode;
        });
    }

    private set = <T, K extends keyof T>(obj: T, key: K, value: T[K]) => obj[key] = value;

    public setTranslation(property: any, value: string | undefined, ctr: new (data: ITranslation) => T) {
        if (this.languageCode) {
            var _translationName: string | undefined;
            if (typeof value === "string" && value.trim() === "") {
                _translationName = undefined;
            } else {
                _translationName = value;
            }
            var that = this;
            var translation = this.translations?.find(function (item) {
                return item.languageCode === that.languageCode;
            });
            if (translation) {
                this.set(translation, property, _translationName);
            } else {
                if (!this.translations) {
                    this.translations = [];
                }
                let newTranlation = new ctr({
                    languageCode: this.languageCode
                });
                this.set(newTranlation, property, _translationName);
                this.translations.push(newTranlation);
            }
        } else {
            this.set(this, property, value);
        }
    }

    protected updateLanguage(languageCode: string)
    {
        this.languageCode = languageCode;
    }
}