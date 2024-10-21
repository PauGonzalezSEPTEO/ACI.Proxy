import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";
import { Hotel } from "../../hotel/models/hotel.model";

export class RoomTypeHotelCompany {
  companyId: number;
  hotelId: number | null;

  constructor(companyId: number, hotelId: number | null) {
      this.companyId = companyId;
      this.hotelId = hotelId;
  }
}

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
  companies: number[] = [];  
  hotels: number[] = [];  
  hotelsList: Hotel[] | null = null;
  id: number;
  name: string; 
  roomTypeHotelsCompanies: RoomTypeHotelCompany[] = []; 
  shortDescription?: string;

  constructor(roomType?: RoomType) {
    super(roomType);
    if (roomType) {
      this.buildings = roomType.buildings;
      this.id = roomType.id;
      this.name = roomType.name;
      this.shortDescription = roomType.shortDescription;
      this.hotelsList = roomType.hotelsList;
      this.roomTypeHotelsCompanies = roomType.roomTypeHotelsCompanies;
      this.extractCompaniesAndHotels();
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  extractCompaniesAndHotels() {
    this.companies = [];
    this.hotels = [];
    this.roomTypeHotelsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.hotelId) {
        this.hotels.push(item.hotelId);
      }
    });
  }

  getRoomTypeHotelsCompanies(): RoomTypeHotelCompany[] {
    const companyHotelCollection: RoomTypeHotelCompany[] = [];
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
        companyHotelCollection.push(new RoomTypeHotelCompany(companyId, hotelId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithHotels.has(companyId)) {
        companyHotelCollection.push(new RoomTypeHotelCompany(companyId, null));
      }
    }
    return companyHotelCollection;
  }

  getPayload(): any {
    return {
      buildings: this.buildings,
      id: this.id,
      name: this.name,
      roomTypeHotelsCompanies: this.getRoomTypeHotelsCompanies(),
      shortDescription: this.shortDescription,
      translations: this.translations
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