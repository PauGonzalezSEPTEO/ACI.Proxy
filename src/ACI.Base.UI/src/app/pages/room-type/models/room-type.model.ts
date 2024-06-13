import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class RoomTypeTranslation extends ITranslation {
  name?: string;
  shortDescription?: string;

  constructor(buildingTranslation?: RoomTypeTranslation) {
    super(buildingTranslation);
    if (buildingTranslation) {
      this.name = buildingTranslation.name;
      this.shortDescription = buildingTranslation.shortDescription;
    }
  }   
}

export class RoomType extends Translatable<RoomTypeTranslation>{
  buildings?: number[];
  id: number;
  name: string;  
  shortDescription?: string;

  constructor(roomType?: RoomType) {
    super(roomType);
    if (roomType) {
      this.buildings = roomType.buildings;
      this.id = roomType.id;
      this.name = roomType.name;
      this.shortDescription = roomType.shortDescription;
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  public get translationName(): string {
    return this.getTranslation('name');
  }

  public set translationName(value: string) {
    this.setTranslation('name', value, RoomTypeTranslation);
  }

  public get translationShortDescription(): string | undefined {
    return this.getTranslation('shortDescription');
  }

  public set translationShortDescription(value: string | undefined) {
    this.setTranslation('shortDescription', value, RoomTypeTranslation);
  }

  public updateBuilding(building: number, isChecked: boolean) {
    if (building) {
      if (!this.buildings) {
        this.buildings = [];
      }
      if (!isChecked) {
        this.buildings = this.buildings.filter(function(id) {
          return id != building;
        });
      } else {
        let index = this.buildings.findIndex((id) => id === building);
        if (index === -1) {
          this.buildings.push(building);
        }
      }
    }
  }
}