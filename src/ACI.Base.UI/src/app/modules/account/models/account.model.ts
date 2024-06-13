export class AccountModel {
    allowCommercialMail: boolean;
    avatar: string;
    communicationByMail: boolean;
    communicationByPhone: boolean;
    countryAlpha2Code?: string;
    companyName?: string;
    currencyCode?: string;
    email?: string;
    firstname: string;
    languageAlpha2Code: string;
    lastname: string;
    phoneNumber?: string;
    twoFactorEnabled: boolean;

    set(_account: unknown) {
      const account = _account as AccountModel;
      this.allowCommercialMail = account.allowCommercialMail;
      this.avatar = account.avatar;
      this.communicationByMail = account.communicationByMail;
      this.communicationByPhone = account.communicationByPhone;
      this.companyName = account.companyName;
      this.countryAlpha2Code = account.countryAlpha2Code;
      this.currencyCode = account.currencyCode;
      this.firstname = account.firstname;
      this.email = account.email;
      this.languageAlpha2Code = account.languageAlpha2Code;
      this.lastname = account.lastname;
      this.phoneNumber = account.phoneNumber;
      this.twoFactorEnabled = account.twoFactorEnabled;
    }    
  }  