import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class TemplateTranslation extends ITranslation {
  content?: string;
  name?: string;
  shortDescription?: string;

  constructor(templateTranslation?: TemplateTranslation) {
    super(templateTranslation);
    if (templateTranslation) {
      this.content = templateTranslation.content;
      this.name = templateTranslation.name;
      this.shortDescription = templateTranslation.shortDescription;
    }
  }
}

export class Template extends Translatable<TemplateTranslation> {
  id: number;
  name: string;
  content: string;
  shortDescription?: string;

  constructor(template?: Template) {
    super(template);
    if (template) {
      this.content = template.content;
      this.id = template.id;
      this.name = template.name;
      this.shortDescription = template.shortDescription;
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  getPayload(): any {
    return {
      id: this.id,
      name: this.name,
      content: this.content,
      translations: this.translations
    };
  }

  public get translationContent(): string {
    return this.getTranslation('content');
  }

  public set translationContent(value: string) {
    this.setTranslation('content', value, TemplateTranslation);
  }

  public get translationName(): string {
    return this.getTranslation('name');
  }

  public set translationName(value: string) {
    this.setTranslation('name', value, TemplateTranslation);
  }

  public get translationShortDescription(): string | undefined {
    return this.getTranslation('shortDescription');
  }

  public set translationShortDescription(value: string | undefined) {
    this.setTranslation('shortDescription', value, TemplateTranslation);
  }
}