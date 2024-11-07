import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class IntegrationTranslation extends ITranslation {
    name?: string;
    shortDescription?: string;

    constructor(buildingTranslation?: IntegrationTranslation) {
      super(buildingTranslation);
      if (buildingTranslation) {
        this.name = buildingTranslation.name;
        this.shortDescription = buildingTranslation.shortDescription;
      }
    } 
  }
  
export class Integration extends Translatable<IntegrationTranslation>{
    projectId: number;
    id: number;
    name: string;
    shortDescription?: string;

    constructor(building?: Integration) {
      super(building);
      if (building) {
        this.projectId = building.projectId;
        this.id = building.id;
        this.name = building.name;
        this.shortDescription = building.shortDescription;
      }
    }

    changeLanguage(languageCode: string) {
      this.updateLanguage(languageCode);
    }
  
    public get translationName(): string {
      return this.getTranslation('name');
    }
  
    public set translationName(value: string) {
      this.setTranslation('name', value, IntegrationTranslation);
    }
  
    public get translationShortDescription(): string | undefined {
      return this.getTranslation('shortDescription');
    }
  
    public set translationShortDescription(value: string | undefined) {
      this.setTranslation('shortDescription', value, IntegrationTranslation);
    }
}