import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Integration } from '../models/integration.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_INTEGRATIONS_URL = `${environment.apiUrl}/Integrations`;

@Injectable({
    providedIn: 'root'
})
export class IntegrationService {

    constructor(private http: HttpClient) { }

    create(integration: Integration): Observable<Integration> {
        return this.http.post<Integration>(`${API_INTEGRATIONS_URL}/create`, integration).pipe(map(integration => new Integration(integration)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_INTEGRATIONS_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Integration> {
        return this.http.get<Integration>(`${API_INTEGRATIONS_URL}/read-editable-by-id/${id}`).pipe(map(integration => new Integration(integration)));
    }

    getByProjectIds(projectIds: number[], languageCode: string): Observable<Integration[]> {
        const httpParams = new HttpParams()
        .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Integration[]>(`${API_INTEGRATIONS_URL}/read-by-project-ids`, projectIds, options);        
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_INTEGRATIONS_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<Integration[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Integration[]>(`${API_INTEGRATIONS_URL}/search`, null, options);
    }

    update(id: number, integration: Integration): Observable<Integration> {
        return this.http.put<Integration>(`${API_INTEGRATIONS_URL}/update-by-id/${id}`, integration).pipe(map(integration => new Integration(integration)));
    }
}