import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";

export class BoardTranslation extends ITranslation {
  name?: string;
  shortDescription?: string;

  constructor(buildingTranslation?: BoardTranslation) {
    super(buildingTranslation);
    if (buildingTranslation) {
      this.name = buildingTranslation.name;
      this.shortDescription = buildingTranslation.shortDescription;
    }
  }
}

export class Board extends Translatable<BoardTranslation> {
  buildings?: number[];
  id: number;  
  name: string;
  shortDescription?: string;

  constructor(board?: Board) {
    super(board);
    if (board) {
      this.buildings = board.buildings;
      this.id = board.id;
      this.name = board.name;
      this.shortDescription = board.shortDescription;
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  public get translationName(): string {
    return this.getTranslation('name');
  }

  public set translationName(value: string) {
    this.setTranslation('name', value, BoardTranslation);
  }

  public get translationShortDescription(): string | undefined {
    return this.getTranslation('shortDescription');
  }

  public set translationShortDescription(value: string | undefined) {
    this.setTranslation('shortDescription', value, BoardTranslation);
  }

  public updateBuilding(building: number, isChecked: boolean) {
    if (building) {
      if (!this.buildings) {
        this.buildings = [];
      }
      if (!isChecked) {
        this.buildings = this.buildings.filter(function (id) {
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