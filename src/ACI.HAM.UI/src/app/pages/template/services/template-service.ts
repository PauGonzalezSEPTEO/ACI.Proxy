import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Template } from '../models/template.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_TEMPLATES_URL = `${environment.apiUrl}/boards`;

@Injectable({
    providedIn: 'root'
})
export class TemplateService {

    constructor(private http: HttpClient) { }

    create(template: Template): Observable<Template> {
        return this.http.post<Template>(`${API_TEMPLATES_URL}/create`, template.getPayload()).pipe(map(template => new Template(template)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_TEMPLATES_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Template> {
        return this.http.get<Template>(`${API_TEMPLATES_URL}/read-editable-by-id/${id}`).pipe(map(template => new Template(template)));
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_TEMPLATES_URL}/read-data-table`, params, options);
    }    

    update(id: number, template: Template): Observable<Template> {
        return this.http.put<Template>(`${API_TEMPLATES_URL}/update-by-id/${id}`, template.getPayload()).pipe(map(template => new Template(template)));
    }
}