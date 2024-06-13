import { Building } from "../../building/models/building.model";
import { RoomType } from "../../room-type/models/room-type.model";

export class Hotel {
  buildings?: Building[];
  companyId: number;
  id: number;
  name: string;
  roomTypes?: RoomType[];

  constructor(hotel?: Hotel) {
    if (hotel) {
      this.buildings = hotel.buildings;
      this.companyId = hotel.companyId;
      this.id = hotel.id;
      this.name = hotel.name;
      this.roomTypes = hotel.roomTypes;
    }
  }
}