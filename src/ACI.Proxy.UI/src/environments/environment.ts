// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  appVersion: 'v8.1.8',
  USERDATA_KEY: 'authf649fc9a5f55',
  apiUrl: 'http://localhost:5000/api/v1',
  apiAccountsRelativeUrl: 'accounts',
  apiAuthenticationRelativeUrl: 'authentication',
  appAPIUrl:
    'http://localhost:5000/swagger/index.html',
  register: true,
  tinymceApiKey: 'z72bhhmoeqv12t5uw13d72wdq76iimivde69bvase42k0fv2'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.