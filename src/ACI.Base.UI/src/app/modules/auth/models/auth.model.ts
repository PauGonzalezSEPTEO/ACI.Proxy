export class AuthModel {
  accessToken: string;
  refreshToken: string;
  refreshTokenExpiryTime: Date;

  setAuth(_auth: unknown) {
    const auth = _auth as AuthModel;
    this.accessToken = auth.accessToken;
    this.refreshToken = auth.refreshToken;
    this.refreshTokenExpiryTime = auth.refreshTokenExpiryTime;
  }
}
