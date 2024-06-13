export class RefreshTokenModel {
    accessToken: string;
    refreshToken: string;
  
    set(refreshToken: RefreshTokenModel) {
      this.accessToken = refreshToken.accessToken;
      this.refreshToken = refreshToken.refreshToken;
    }
  }  