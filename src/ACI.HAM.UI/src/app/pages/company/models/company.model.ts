import { Hotel } from "../../hotel/models/hotel.model";

export class Company {
  id: number;
  name: string;
  hotels?: Hotel[];

  constructor(company?: Company) {
    if (company) {
      this.id = company.id;
      this.name = company.name;
      this.hotels = company.hotels;
    }
  }
}