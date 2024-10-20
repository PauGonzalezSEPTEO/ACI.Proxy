import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_TEMPLATES_URL = `${environment.apiUrl}/templates`;

@Injectable({
    providedIn: 'root'
})
export class TemplateService {

    constructor(private http: HttpClient) { }

    get(name: string): Observable<string> {
        return this.http.get<string>(`${API_TEMPLATES_URL}/get-template-by-name/${name}`);
    }
}