import { Project } from "../../project/models/project.model";

export class Company {
  id: number;
  name: string;
  projects?: Project[];

  constructor(company?: Company) {
    if (company) {
      this.id = company.id;
      this.name = company.name;
      this.projects = company.projects;
    }
  }
}