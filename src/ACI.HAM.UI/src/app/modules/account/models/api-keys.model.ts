export class UserApiUsageStatistics {
    date: string;
    value: number;

    set(_userApiUsageStatistics: unknown) {
        const userApiUsageStatistics = _userApiUsageStatistics as UserApiUsageStatistics;
        this.date = userApiUsageStatistics.date;
        this.value = userApiUsageStatistics.value;
      }  
  }