import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";
import { Hotel } from "../../hotel/models/hotel.model";

export class BoardHotelCompany {
  companyId: number;
  hotelId: number | null;

  constructor(companyId: number, hotelId: number | null) {
      this.companyId = companyId;
      this.hotelId = hotelId;
  }
}

export class BoardTranslation extends ITranslation {
  name?: string;
  shortDescription?: string;

  constructor(boardTranslation?: BoardTranslation) {
    super(boardTranslation);
    if (boardTranslation) {
      this.name = boardTranslation.name;
      this.shortDescription = boardTranslation.shortDescription;
    }
  }
}

export class Board extends Translatable<BoardTranslation> {  
  boardHotelsCompanies: BoardHotelCompany[] = [];
  buildings?: number[];
  companies: number[] = [];  
  hotels: number[] = [];
  hotelsList: Hotel[] | null = null;
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
      this.hotelsList = board.hotelsList;
      this.boardHotelsCompanies = board.boardHotelsCompanies;
      this.extractCompaniesAndHotels();
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  extractCompaniesAndHotels() {
    this.companies = [];
    this.hotels = [];
    this.boardHotelsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.hotelId) {
        this.hotels.push(item.hotelId);
      }
    });
  }

  getBoardHotelsCompanies(): BoardHotelCompany[] {
    const companyHotelCollection: BoardHotelCompany[] = [];
    const hotelToCompanyMap: { [key: number]: number } = {};
    if (this.hotelsList) {
      for (const hotel of this.hotelsList) {
        hotelToCompanyMap[hotel.id] = hotel.companyId;
      }
    }
    const companiesWithHotels = new Set<number>();
    for (const hotelId of this.hotels) {
      const companyId = hotelToCompanyMap[hotelId];
      if (companyId !== undefined) {
        companiesWithHotels.add(companyId);
        companyHotelCollection.push(new BoardHotelCompany(companyId, hotelId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithHotels.has(companyId)) {
        companyHotelCollection.push(new BoardHotelCompany(companyId, null));
      }
    }
    return companyHotelCollection;
  }

  getPayload(): any {
    return {
      buildings: this.buildings,
      id: this.id,
      name: this.name,
      shortDescription: this.shortDescription,
      boardHotelsCompanies: this.getBoardHotelsCompanies()
    };
  }

  getRelevantHotelIds(): number[] {
    const relevantHotelIds: number[] = [];
    if (this.hotelsList && this.hotelsList.length > 0) {
      const hotelToCompanyMap: { [key: number]: number } = {};
      const companiesWithHotels = new Set<number>();
      const companiesWithoutHotels = new Set<number>();
      if (this.hotelsList) {
          for (const hotel of this.hotelsList) {
              hotelToCompanyMap[hotel.id] = hotel.companyId;
          }
      }
      for (const hotelId of this.hotels) {
          const companyId = hotelToCompanyMap[hotelId];
          if (companyId !== undefined) {
              companiesWithHotels.add(companyId);
              relevantHotelIds.push(hotelId);
          }
      }
      for (const hotel of this.hotelsList) {
        if (!this.hotels.includes(hotel.id)) {
            const companyId = hotel.companyId;
            if (!companiesWithHotels.has(companyId)) {
                companiesWithoutHotels.add(companyId);
            }
        }
      }      
      for (const hotel of this.hotelsList) {
        if (companiesWithoutHotels.has(hotel.companyId)) {
            relevantHotelIds.push(hotel.id);
        }
      }
    }
    return relevantHotelIds;
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