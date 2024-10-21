import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class TemplateTranslation extends ITranslation {
  name?: string;

  constructor(templateTranslation?: TemplateTranslation) {
    super(templateTranslation);
    if (templateTranslation) {
      this.name = templateTranslation.name;
    }
  }
}

export class Template extends Translatable<TemplateTranslation> {
  id: number;
  name: string;

  constructor(template?: Template) {
    super(template);
    if (template) {
      this.id = template.id;
      this.name = template.name;
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  getPayload(): any {
    return {
      id: this.id,
      name: this.name,
      translations: this.translations
    };
  }

  public get translationName(): string {
    return this.getTranslation('name');
  }

  public set translationName(value: string) {
    this.setTranslation('name', value, TemplateTranslation);
  }
}