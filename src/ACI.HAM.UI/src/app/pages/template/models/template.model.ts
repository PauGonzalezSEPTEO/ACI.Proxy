import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";
import { Hotel } from "../../hotel/models/hotel.model";

export class TemplateHotelCompany {
  companyId: number;
  hotelId: number | null;

  constructor(companyId: number, hotelId: number | null) {
      this.companyId = companyId;
      this.hotelId = hotelId;
  }
}

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
  boardHotelsCompanies: TemplateHotelCompany[] = [];
  buildings?: number[];
  companies: number[] = [];  
  hotels: number[] = [];
  hotelsList: Hotel[] | null = null;
  id: number;
  name: string;
  content: string;
  shortDescription?: string;

  constructor(template?: Template) {
    super(template);
    if (template) {
      this.buildings = template.buildings;
      this.content = template.content;
      this.id = template.id;
      this.name = template.name;
      this.shortDescription = template.shortDescription;
      this.hotelsList = template.hotelsList;
      this.boardHotelsCompanies = template.boardHotelsCompanies;
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

  getPayload(): any {
    return {
      buildings: this.buildings,
      id: this.id,
      name: this.name,
      content: this.content,
      shortDescription: this.shortDescription,
      templateHotelsCompanies: this.getTemplateHotelsCompanies(),
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

  getTemplateHotelsCompanies(): TemplateHotelCompany[] {
    const companyHotelCollection: TemplateHotelCompany[] = [];
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
        companyHotelCollection.push(new TemplateHotelCompany(companyId, hotelId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithHotels.has(companyId)) {
        companyHotelCollection.push(new TemplateHotelCompany(companyId, null));
      }
    }
    return companyHotelCollection;
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