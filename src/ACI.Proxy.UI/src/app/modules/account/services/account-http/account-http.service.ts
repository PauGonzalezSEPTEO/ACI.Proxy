import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';
import { AccountModel } from '../../models/account.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { UserApiUsageStatistics } from '../../models/api-keys.model';

const API_ACCOUNTS_URL = `${environment.apiUrl}/${environment.apiAccountsRelativeUrl}`;

@Injectable({
  providedIn: 'root',
})
export class AccountHTTPService {
  constructor(private http: HttpClient) {}

  // public methods

  deleteUserApiKey(id: number): Observable<void> {
    return this.http.delete<void>(`${API_ACCOUNTS_URL}/delete-user-api-key-by-id/${id}`);
  }

  generateUserApiKey(): Observable<string> {
    return this.http.post<string>(`${API_ACCOUNTS_URL}/generate-user-api-key`, {});
  }

  getAccount(): Observable<AccountModel> {
    return this.http.get<AccountModel>(`${API_ACCOUNTS_URL}/me`);
  }

  getLast12HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-12-hours-user-api-usage-statistics`);
  }

  getLast14DaysUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-14-days-user-api-usage-statistics`);
  }

  getLast3HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-3-hours-user-api-usage-statistics`);
  }

  getLast3MonthsUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-3-months-user-api-usage-statistics`);
  }

  getLast6HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-6-hours-user-api-usage-statistics`);
  }

  getLast6MonthsUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-6-months-user-api-usage-statistics`);
  }

  getLast7DaysUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-7-days-user-api-usage-statistics`);
  }

  getLastDayUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-day-user-api-usage-statistics`);
  }

  getLastHourUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-hour-user-api-usage-statistics`);
  }
  
  getLastMonthUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-month-user-api-usage-statistics`);
  }
  
  getLastYearUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.http.get<UserApiUsageStatistics[]>(`${API_ACCOUNTS_URL}/get-last-year-user-api-usage-statistics`);
  }
  
  readUserApiKeysDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
    const httpParams = new HttpParams()
        .append('languageCode', languageCode);
    const options = {
        params: httpParams
    };
    return this.http.post<DataTablesResponse>(`${API_ACCOUNTS_URL}/read-user-api-keys-data-table`, params, options);
  }  

  revokeUserApiKey(id: number): Observable<void> {
    return this.http.delete<void>(`${API_ACCOUNTS_URL}/revoke-user-api-key-by-id/${id}`);
  }

  updateProfileDetails(data: AccountModel): Observable<AccountModel> {
    return this.http.put<AccountModel>(`${API_ACCOUNTS_URL}/update-profile-details`, data);
  }
}
