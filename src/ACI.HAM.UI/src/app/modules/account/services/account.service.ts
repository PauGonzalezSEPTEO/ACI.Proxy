import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, Subscription, of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { AccountHTTPService } from './account-http';
import { AccountModel } from '../models/account.model';
import { DataTablesResponse } from 'src/app/shared/models/data-tables-response.model';
import { UserApiUsageStatistics } from '../models/api-keys.model';

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

  getLast12HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast12HoursUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast14DaysUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast14DaysUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast3HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast3HoursUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast3MonthsUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast3MonthsUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast6HoursUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast6HoursUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast6MonthsUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast6MonthsUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLast7DaysUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLast7DaysUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLastDayUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLastDayUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLastHourUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLastHourUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLastMonthUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLastMonthUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
    );
  }

  getLastYearUserApiUsageStatistics(): Observable<UserApiUsageStatistics[]> {
    return this.accountHttpService.getLastYearUserApiUsageStatistics().pipe(
      catchError((err) => {
        console.error('err', err);
        return of(err);
      })
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
