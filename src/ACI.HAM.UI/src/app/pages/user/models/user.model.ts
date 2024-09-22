import { Hotel } from "../../hotel/models/hotel.model";

export class UserHotelCompany {
  companyId: number;
  hotelId: number | null;

  constructor(companyId: number, hotelId: number | null) {
      this.companyId = companyId;
      this.hotelId = hotelId;
  }
}

export class User {
  companies: number[] = [];
  createDate?: Date;
  email: string;
  emailConfirmed: boolean;
  firstname: string;
  hotels: number[] = [];
  id: string;
  lastLoginDate?: Date;
  lastname: string;
  roles: string[];
  hotelsList: Hotel[] | null = null;
  userHotelsCompanies: UserHotelCompany[] = [];

  constructor(user?: User) {
    if (user) {
      this.createDate = user.createDate;
      this.email = user.email;
      this.emailConfirmed = user.emailConfirmed;
      this.firstname = user.firstname;
      this.id = user.id;
      this.lastLoginDate = user.lastLoginDate;
      this.lastname = user.lastname;
      this.roles = user.roles;
      this.hotelsList = user.hotelsList;
      this.userHotelsCompanies = user.userHotelsCompanies;
      this.extractCompaniesAndHotels();
    }
  }

  extractCompaniesAndHotels() {
    this.companies = [];
    this.hotels = [];
    this.userHotelsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.hotelId) {
        this.hotels.push(item.hotelId);
      }
    });
  }

  extractUserHotelsCompanies(): UserHotelCompany[] {
    const companyHotelCollection: UserHotelCompany[] = [];
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
        companyHotelCollection.push(new UserHotelCompany(companyId, hotelId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithHotels.has(companyId)) {
        companyHotelCollection.push(new UserHotelCompany(companyId, null));
      }
    }
    return companyHotelCollection;
  }

  getPayload(): any {
    return {
      email: this.email,
      firstname: this.firstname,
      id: this.id,
      lastname: this.lastname,
      roles: this.roles,
      userHotelsCompanies: this.extractUserHotelsCompanies()
    };
  }

  isAtLeastOneRoleSelected() {
    if (this.roles) {
      for (let role in this.roles) {
        if (this.roles[role]) {
          return true;
        }
      }
      return false;
    }
  }

  public updateRole(role: string, isChecked: boolean) {
    if (role) {
      if (!this.roles) {
        this.roles = [];
      }
      if (!isChecked) {
        this.roles = this.roles.filter(function(id) {
          return id != role;
        });
      } else {
        let index = this.roles.findIndex((id) => id === role);
        if (index === -1) {
          this.roles.push(role);
        }
      }
    }
  }
}