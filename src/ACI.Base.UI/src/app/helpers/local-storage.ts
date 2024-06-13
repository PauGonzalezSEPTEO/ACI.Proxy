import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { AuthModel } from "../modules/auth/models/auth.model";

@Injectable()
export class LocalStorage {
  // private fields
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  constructor() { }

  public getAuthFromLocalStorage(): AuthModel | undefined {
    try {
      const lsValue = localStorage.getItem(this.authLocalStorageToken);
      if (!lsValue) {
        return undefined;
      }
      const authData = JSON.parse(lsValue);
      return authData;
    } catch (error) {
      console.error(error);
      return undefined;
    }
  }

  public removeAuthFromLocalStorage() {
    localStorage.removeItem(this.authLocalStorageToken);
  }

  public setAuthFromLocalStorage(auth: AuthModel): AuthModel | undefined {
    // store auth accessToken/refreshToken/refreshTokenExpiryTime in local storage to keep user logged in between page refreshes
    if (auth && auth.accessToken) {
      localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      return auth;
    }
    return undefined;
  }
}