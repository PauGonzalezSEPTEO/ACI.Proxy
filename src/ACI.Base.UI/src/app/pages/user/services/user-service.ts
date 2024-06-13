import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { User } from '../models/user.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_USERS_URL = `${environment.apiUrl}/users`;

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private http: HttpClient) { }

    createUser(user: User): Observable<User> {
        return this.http.post<User>(`${API_USERS_URL}/create-user`, user).pipe(map(user => new User(user)));
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${API_USERS_URL}/delete-by-id/${id}`);
    }

    get(id: string): Observable<User> {
        return this.http.get<User>(`${API_USERS_URL}/read-editable-by-id/${id}`).pipe(map(user => new User(user)));;
    }

    read(languageCode: string): Observable<User[]> {
        const params = new HttpParams()
            .append('languageCode', languageCode);
        return this.http.post<User[]>(`${API_USERS_URL}/read`, null, {
            params: params
        });
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_USERS_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<User[]> {
        const params = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        return this.http.post<User[]>(`${API_USERS_URL}/search`, null, {
            params: params
        });
    }

    update(id: string, user: User): Observable<User> {
        return this.http.put<User>(`${API_USERS_URL}/update-by-id/${id}`, user).pipe(map(user => new User(user)));
    }
}
