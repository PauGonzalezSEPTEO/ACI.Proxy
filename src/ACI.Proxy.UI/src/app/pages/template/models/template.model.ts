import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";
import { Project } from "../../project/models/project.model";

export class TemplateProjectCompany {
  companyId: number;
  projectId: number | null;

  constructor(companyId: number, projectId: number | null) {
      this.companyId = companyId;
      this.projectId = projectId;
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
  buildings?: number[];
  companies: number[] = [];  
  projects: number[] = [];
  projectsList: Project[] | null = null;
  id: number;
  name: string;
  content: string;
  shortDescription?: string;
  templateProjectsCompanies: TemplateProjectCompany[] = [];

  constructor(template?: Template) {
    super(template);
    if (template) {
      this.buildings = template.buildings;
      this.content = template.content;
      this.id = template.id;
      this.name = template.name;
      this.shortDescription = template.shortDescription;
      this.projectsList = template.projectsList;
      this.templateProjectsCompanies = template.templateProjectsCompanies;
      this.extractCompaniesAndProjects();
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  extractCompaniesAndProjects() {
    this.companies = [];
    this.projects = [];
    this.templateProjectsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.projectId) {
        this.projects.push(item.projectId);
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
      templateProjectsCompanies: this.getTemplateProjectsCompanies(),
      translations: this.translations
    };
  }

  getRelevantProjectIds(): number[] {
    const relevantProjectIds: number[] = [];
    if (this.projectsList && this.projectsList.length > 0) {
      const projectToCompanyMap: { [key: number]: number } = {};
      const companiesWithProjects = new Set<number>();
      const companiesWithoutProjects = new Set<number>();
      if (this.projectsList) {
          for (const project of this.projectsList) {
              projectToCompanyMap[project.id] = project.companyId;
          }
      }
      for (const projectId of this.projects) {
          const companyId = projectToCompanyMap[projectId];
          if (companyId !== undefined) {
              companiesWithProjects.add(companyId);
              relevantProjectIds.push(projectId);
          }
      }
      for (const project of this.projectsList) {
        if (!this.projects.includes(project.id)) {
            const companyId = project.companyId;
            if (!companiesWithProjects.has(companyId)) {
                companiesWithoutProjects.add(companyId);
            }
        }
      }      
      for (const project of this.projectsList) {
        if (companiesWithoutProjects.has(project.companyId)) {
            relevantProjectIds.push(project.id);
        }
      }
    }
    return relevantProjectIds;
  }

  getTemplateProjectsCompanies(): TemplateProjectCompany[] {
    const companyProjectCollection: TemplateProjectCompany[] = [];
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
        companyProjectCollection.push(new TemplateProjectCompany(companyId, projectId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithProjects.has(companyId)) {
        companyProjectCollection.push(new TemplateProjectCompany(companyId, null));
      }
    }
    return companyProjectCollection;
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