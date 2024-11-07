import { Building } from "../../building/models/building.model";
import { RoomType } from "../../room-type/models/room-type.model";

export class Project {
  buildings?: Building[];
  companyId: number;
  id: number;
  name: string;
  roomTypes?: RoomType[];

  constructor(project?: Project) {
    if (project) {
      this.buildings = project.buildings;
      this.companyId = project.companyId;
      this.id = project.id;
      this.name = project.name;
      this.roomTypes = project.roomTypes;
    }
  }
}