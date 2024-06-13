import { Translatable } from "src/app/modules/language/models/translatable.model";

export class RoleTranslation {
  languageCode: string;
  name?: string;
  shortDescription?: string;

  constructor(roleTranslation?: RoleTranslation) {
    if (roleTranslation) {
      this.languageCode = roleTranslation.languageCode;
      this.name = roleTranslation.name;
      this.shortDescription = roleTranslation.shortDescription;
    }
  }  
}

export class Role extends Translatable<RoleTranslation>{
  id: string;
  name: string;
  shortDescription?: string;
  totalUsers: number;

  constructor(role?: Role) {
    super(role);
    if (role) {
      this.id = role.id;
      this.name = role.name;
      this.shortDescription = role.shortDescription;
      this.totalUsers = role.totalUsers;
    }
  }

  changeLanguage(languageCode: string) {
    this.updateLanguage(languageCode);
  }

  public get translationName(): string {
    return this.getTranslation('name');
  }

  public set translationName(value: string) {
    this.setTranslation('name', value, RoleTranslation);
  }

  public get translationShortDescription(): string | undefined {
    return this.getTranslation('shortDescription');
  }

  public set translationShortDescription(value: string | undefined) {
    this.setTranslation('shortDescription', value, RoleTranslation);
  }
}