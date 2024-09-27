import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';
import { AccountModel } from '../../models/account.model';

const API_ACCOUNTS_URL = `${environment.apiUrl}/${environment.apiAccountsRelativeUrl}`;

@Injectable({
  providedIn: 'root',
})
export class AccountHTTPService {
  constructor(private http: HttpClient) {}

  // public methods
  generateApiKey(): Observable<string> {
    return this.http.get<string>(`${API_ACCOUNTS_URL}/generate-api-key`);
  }

  getAccount(): Observable<AccountModel> {
    return this.http.get<AccountModel>(`${API_ACCOUNTS_URL}/me`);
  }

  updateProfileDetails(data: AccountModel): Observable<AccountModel> {
    return this.http.put<AccountModel>(`${API_ACCOUNTS_URL}/update-profile-details`, data);
  }
}
