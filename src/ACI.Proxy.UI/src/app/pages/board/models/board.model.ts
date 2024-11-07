import { ITranslation, Translatable } from "src/app/modules/language/models/translatable.model";
import { Project } from "../../project/models/project.model";

export class BoardProjectCompany {
  companyId: number;
  projectId: number | null;

  constructor(companyId: number, projectId: number | null) {
      this.companyId = companyId;
      this.projectId = projectId;
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
  boardProjectsCompanies: BoardProjectCompany[] = [];
  buildings?: number[];
  companies: number[] = [];  
  projects: number[] = [];
  projectsList: Project[] | null = null;
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
      this.projectsList = board.projectsList;
      this.boardProjectsCompanies = board.boardProjectsCompanies;
      this.extractCompaniesAndProjects();
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  extractCompaniesAndProjects() {
    this.companies = [];
    this.projects = [];
    this.boardProjectsCompanies.forEach((item: any) => {
      if (item.companyId) {
        this.companies.push(item.companyId);
      }
      if (item.projectId) {
        this.projects.push(item.projectId);
      }
    });
  }

  getBoardProjectsCompanies(): BoardProjectCompany[] {
    const companyProjectCollection: BoardProjectCompany[] = [];
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
        companyProjectCollection.push(new BoardProjectCompany(companyId, projectId));
      }
    }
    for (const companyId of this.companies) {
      if (!companiesWithProjects.has(companyId)) {
        companyProjectCollection.push(new BoardProjectCompany(companyId, null));
      }
    }
    return companyProjectCollection;
  }

  getPayload(): any {
    return {
      buildings: this.buildings,
      id: this.id,
      name: this.name,
      shortDescription: this.shortDescription,
      boardProjectsCompanies: this.getBoardProjectsCompanies(),
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