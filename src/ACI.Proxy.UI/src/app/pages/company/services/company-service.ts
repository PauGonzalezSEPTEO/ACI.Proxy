import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Company } from '../models/company.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_COMPANIES_URL = `${environment.apiUrl}/companies`;

@Injectable({
    providedIn: 'root'
})
export class CompanyService {

    constructor(private http: HttpClient) { }

    create(company: Company): Observable<Company> {
        return this.http.post<Company>(`${API_COMPANIES_URL}/create`, company).pipe(map(company => new Company(company)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_COMPANIES_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Company> {
        return this.http.get<Company>(`${API_COMPANIES_URL}/read-editable-by-id/${id}`).pipe(map(company => new Company(company)));
    }

    read(languageCode: string): Observable<Company[]> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Company[]>(`${API_COMPANIES_URL}/read`, null, options);
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_COMPANIES_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<Company[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Company[]>(`${API_COMPANIES_URL}/search`, null, options);
    }

    update(id: number, company: Company): Observable<Company> {
        return this.http.put<Company>(`${API_COMPANIES_URL}/update-by-id/${id}`, company).pipe(map(company => new Company(company)));
    }
}