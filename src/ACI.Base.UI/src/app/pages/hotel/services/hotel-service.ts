import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Hotel } from '../models/hotel.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_HOTELS_URL = `${environment.apiUrl}/hotels`;

@Injectable({
    providedIn: 'root'
})
export class HotelService {

    constructor(private http: HttpClient) { }

    create(hotel: Hotel): Observable<Hotel> {
        return this.http.post<Hotel>(`${API_HOTELS_URL}/create`, hotel).pipe(map(hotel => new Hotel(hotel)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_HOTELS_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Hotel> {
        return this.http.get<Hotel>(`${API_HOTELS_URL}/read-editable-by-id/${id}`).pipe(map(hotel => new Hotel(hotel)));
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_HOTELS_URL}/read-data-table`, params, options);
    }

    search(search: string, ordering: string, languageCode: string): Observable<Hotel[]> {
        const httpParams = new HttpParams()
            .append('search', search)
            .append('ordering', ordering)
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<Hotel[]>(`${API_HOTELS_URL}/search`, null, options);
    }

    update(id: number, hotel: Hotel): Observable<Hotel> {
        return this.http.put<Hotel>(`${API_HOTELS_URL}/update-by-id/${id}`, hotel).pipe(map(hotel => new Hotel(hotel)));
    }
}