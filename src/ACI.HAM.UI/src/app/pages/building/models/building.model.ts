import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class BuildingTranslation extends ITranslation {
    name?: string;
    shortDescription?: string;

    constructor(buildingTranslation?: BuildingTranslation) {
      super(buildingTranslation);
      if (buildingTranslation) {
        this.name = buildingTranslation.name;
        this.shortDescription = buildingTranslation.shortDescription;
      }
    } 
  }
  
export class Building extends Translatable<BuildingTranslation>{
    hotelId: number;
    id: number;
    name: string;
    shortDescription?: string;

    constructor(building?: Building) {
      super(building);
      if (building) {
        this.hotelId = building.hotelId;
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
      this.setTranslation('name', value, BuildingTranslation);
    }
  
    public get translationShortDescription(): string | undefined {
      return this.getTranslation('shortDescription');
    }
  
    public set translationShortDescription(value: string | undefined) {
      this.setTranslation('shortDescription', value, BuildingTranslation);
    }
}  