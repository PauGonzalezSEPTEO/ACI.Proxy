import { Project } from "../../project/models/project.model";

export class UserProjectCompany {
  companyId: number;
  projectId: number | null;

  constructor(companyId: number, projectId: number | null) {
      this.companyId = companyId;
      this.projectId = projectId;
  }
}

export class User {
  companies: number[] = [];
  createDate?: Date;
  email: string;
  emailConfirmed: boolean;
  firstname: string;
  projects: number[] = [];
  id: string;
  lastLoginDate?: Date;
  lastname: string;
  roles: string[];
  projectsList: Project[] | null = null;
  userProjectsCompanies: UserProjectCompany[] = [];

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
      this.projectsList = user.projectsList;
      this.userProjectsCompanies = user.userProjectsCompanies;
      this.extractCompaniesAndProjects();
    }
  }

  extractCompaniesAndProjects() {
    this.companies = [];
    this.projects = [];
    this.userProjectsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.projectId) {
        this.projects.push(item.projectId);
      }
    });
  }

  getUserProjectsCompanies(): UserProjectCompany[] {
    const companyProjectCollection: UserProjectCompany[] = [];
    const projectToCompanyMap: { [key: number]: number } = {};
    if (this.projectsList) {
      for (const project of this.projectsList) {
        projectToCompanyMap[project.id] = project.companyId;
      }
    }
    const companiesWithProjects = new Set<number>();
    for (const projectId of this.projects) {
      const companyId = projectToCompanyMap[projectId];
      if (companyId !== undefined) {
        companiesWithProjects.add(companyId);
        companyProjectCollection.push(new UserProjectCompany(companyId, projectId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithProjects.has(companyId)) {
        companyProjectCollection.push(new UserProjectCompany(companyId, null));
      }
    }
    return companyProjectCollection;
  }

  getPayload(): any {
    return {
      email: this.email,
      firstname: this.firstname,
      id: this.id,
      lastname: this.lastname,
      roles: this.roles,
      userProjectsCompanies: this.getUserProjectsCompanies()
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