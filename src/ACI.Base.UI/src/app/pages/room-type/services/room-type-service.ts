import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { RoomType } from '../models/room-type.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_ROOM_TYPES_URL = `${environment.apiUrl}/room-types`;

@Injectable({
    providedIn: 'root'
})
export class RoomTypeService {

    constructor(private http: HttpClient) { }

    create(roomType: RoomType): Observable<RoomType> {
        return this.http.post<RoomType>(`${API_ROOM_TYPES_URL}/create`, roomType).pipe(map(roomType => new RoomType(roomType)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_ROOM_TYPES_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<RoomType> {
        return this.http.get<RoomType>(`${API_ROOM_TYPES_URL}/read-editable-by-id/${id}`).pipe(map(roomType => new RoomType(roomType)));
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_ROOM_TYPES_URL}/read-data-table`, params, options);
    }    

    update(id: number, roomType: RoomType): Observable<RoomType> {
        return this.http.put<RoomType>(`${API_ROOM_TYPES_URL}/update-by-id/${id}`, roomType).pipe(map(roomType => new RoomType(roomType)));
    }
}