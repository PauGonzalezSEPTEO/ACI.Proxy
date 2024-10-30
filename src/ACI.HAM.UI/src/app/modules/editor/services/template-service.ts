import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Template } from '../models/editor.model';

const API_TEMPLATES_URL = `${environment.apiUrl}/mail`;

@Injectable({
    providedIn: 'root'
})
export class TemplateService {

    constructor(private http: HttpClient) { }

    get(name: string, languageCode: string): Observable<Template> {
        return this.http.get<Template>(`${API_TEMPLATES_URL}/get-template-by-name/${name}`, {
            params: { languageCode }
        });
    }
}