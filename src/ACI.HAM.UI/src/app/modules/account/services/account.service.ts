import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, Subscription, of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { AccountHTTPService } from './account-http';
import { AccountModel } from '../models/account.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = [];

  // public fields
  isLoading$: Observable<boolean>;
  isLoadingSubject: BehaviorSubject<boolean>;
  account: BehaviorSubject<AccountModel | null> = new BehaviorSubject<AccountModel | null>(null);

  constructor(
    private accountHttpService: AccountHTTPService
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.isLoading$ = this.isLoadingSubject.asObservable();
  }
  
  deleteUserApiKey(id: number): Observable<void> {
    return this.accountHttpService.deleteUserApiKey(id).pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }
  
  generateUserApiKey(): Observable<string> {
    return this.accountHttpService.generateUserApiKey().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getAccount(): Observable<AccountModel> {
    if (this.account.value) {
      return of(this.account.value);
    }
    this.isLoadingSubject.next(true);
    return this.accountHttpService.getAccount().pipe(
      tap(account => {
        this.account.next(account);
      }),
      catchError((err) => {
        console.error('err', err);
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  readUserApiKeysDataTable(params: any, languageCode: string): Observable<DataTablesResponse> {
    return this.accountHttpService.readUserApiKeysDataTable(params, languageCode).pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  revokeUserApiKey(id: number): Observable<void> {
    return this.accountHttpService.revokeUserApiKey(id).pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  updateProfileDetails(account: AccountModel): Observable<any> {
    this.isLoadingSubject.next(true);
    return this.accountHttpService.updateProfileDetails(account).pipe(
      tap(updatedAccount => {
        this.account.next(updatedAccount);
      }),
      catchError((err) => {
        console.error('err', err);
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
