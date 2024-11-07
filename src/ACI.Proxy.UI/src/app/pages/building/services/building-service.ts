import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Building } from '../models/building.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_BUILDINGS_URL = `${environment.apiUrl}/Buildings`;

@Injectable({
    providedIn: 'root'
})
export class BuildingService {

    constructor(private http: HttpClient) { }

    create(building: Building): Observable<Building> {
        return this.http.post<Building>(`${API_BUILDINGS_URL}/create`, building).pipe(map(building => new Building(building)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_BUILDINGS_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Building> {
        return this.http.get<Building>(`${API_BUILDINGS_URL}/read-editable-by-id/${id}`).pipe(map(building => new Building(building)));
    }

    getByProjectIds(projectIds: number[], languageCode: string): Observable<Building[]> {
        const httpParams = new HttpParams()
        .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Building[]>(`${API_BUILDINGS_URL}/read-by-project-ids`, projectIds, options);        
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_BUILDINGS_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<Building[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Building[]>(`${API_BUILDINGS_URL}/search`, null, options);
    }

    update(id: number, building: Building): Observable<Building> {
        return this.http.put<Building>(`${API_BUILDINGS_URL}/update-by-id/${id}`, building).pipe(map(building => new Building(building)));
    }
}