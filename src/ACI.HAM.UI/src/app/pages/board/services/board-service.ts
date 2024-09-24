import { environment } from '../../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Board } from '../models/board.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

const API_BOARDS_URL = `${environment.apiUrl}/boards`;

@Injectable({
    providedIn: 'root'
})
export class BoardService {

    constructor(private http: HttpClient) { }

    create(roomType: Board): Observable<Board> {
        return this.http.post<Board>(`${API_BOARDS_URL}/create`, roomType.getPayload()).pipe(map(roomType => new Board(roomType)));
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${API_BOARDS_URL}/delete-by-id/${id}`);
    }

    get(id: number): Observable<Board> {
        return this.http.get<Board>(`${API_BOARDS_URL}/read-editable-by-id/${id}`).pipe(map(roomType => new Board(roomType)));
    }

    readDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
        const httpParams = new HttpParams()
            .append('languageCode', languageCode);
        const options = {
            params: httpParams
        };
        return this.http.post<DataTablesResponse>(`${API_BOARDS_URL}/read-data-table`, params, options);
    }    

    update(id: number, roomType: Board): Observable<Board> {
        return this.http.put<Board>(`${API_BOARDS_URL}/update-by-id/${id}`, roomType.getPayload()).pipe(map(roomType => new Board(roomType)));
    }
}