export class Language {
  code: string;
  name: string;
  flag: string;

  constructor(language?: Language) {
    if (language) {
      this.code = language.code;
      this.flag = language.flag;
      this.name = language.name;
    }
  }
}