import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Role } from '../models/role.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_ROLES_URL = `${environment.apiUrl}/roles`;

@Injectable({
    providedIn: 'root'
})
export class RoleService {

    constructor(private http: HttpClient) { }

    create(role: Role): Observable<Role> {
        return this.http.post<Role>(`${API_ROLES_URL}/create`, role).pipe(map(role => new Role(role)));;
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${API_ROLES_URL}/delete-by-id/${id}`);
    }

    deleteUser(id: string, userId: string): Observable<void> {
        return this.http.delete<void>(`${API_ROLES_URL}/delete-user-by-id/${id}/${userId}`);
    }

    get(id: string): Observable<Role> {
        return this.http.get<Role>(`${API_ROLES_URL}/read-editable-by-id/${id}`).pipe(map(role => new Role(role)));
    }

    read(languageCode: string): Observable<Role[]> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Role[]>(`${API_ROLES_URL}/read`, null, options);
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_ROLES_URL}/read-data-table`, params, options);
    }

    readUsersDataTable(id: string, params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('id', id)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_ROLES_URL}/read-users-data-table`, params, options);
    }   

    search(search: string, ordering: string, languageCode: string): Observable<Role[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Role[]>(`${API_ROLES_URL}/search`, null, options);
    }

    update(id: string, role: Role): Observable<Role> {
        return this.http.put<Role>(`${API_ROLES_URL}/update-by-id/${id}`, role).pipe(map(role => new Role(role)));
    }
}