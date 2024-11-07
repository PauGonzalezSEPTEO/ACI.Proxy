import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Project } from '../models/project.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_PROJECTS_URL = `${environment.apiUrl}/projects`;

@Injectable({
    providedIn: 'root'
})
export class ProjectService {

    constructor(private http: HttpClient) { }

    create(project: Project): Observable<Project> {
        return this.http.post<Project>(`${API_PROJECTS_URL}/create`, project).pipe(map(project => new Project(project)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_PROJECTS_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Project> {
        return this.http.get<Project>(`${API_PROJECTS_URL}/read-editable-by-id/${id}`).pipe(map(project => new Project(project)));
    }

    getByCompanyIds(companyIds: number[]): Observable<Project[]> {
        return this.http.post<Project[]>(`${API_PROJECTS_URL}/read-by-company-ids`, companyIds);        
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_PROJECTS_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<Project[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Project[]>(`${API_PROJECTS_URL}/search`, null, options);
    }

    update(id: number, project: Project): Observable<Project> {
        return this.http.put<Project>(`${API_PROJECTS_URL}/update-by-id/${id}`, project).pipe(map(project => new Project(project)));
    }
}