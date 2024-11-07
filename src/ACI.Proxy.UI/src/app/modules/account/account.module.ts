import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { AccountRoutingModule } from './account-routing.module';
import { AccountComponent } from './account.component';
import { OverviewComponent } from './overview/overview.component';
import { SettingsComponent } from './settings/settings.component';
import { ApiKeysComponent } from './api-keys/api-keys.component';
import { ProfileDetailsComponent } from './settings/forms/profile-details/profile-details.component';
import { DeactivateAccountComponent } from './settings/forms/deactivate-account/deactivate-account.component';
import { SignInMethodComponent } from './settings/forms/sign-in-method/sign-in-method.component';
import { TranslationModule } from '../i18n/translation.module';
import { DropdownMenusModule, WidgetsModule } from '../../_metronic/partials';
import { CrudModule } from 'src/app/modules/crud/crud.module';
import { SharedModule } from "../../_metronic/shared/shared.module";
import { AvatarModule } from '../../modules/avatar/avatar.module';

@NgModule({
  declarations: [
    AccountComponent,
    OverviewComponent,
    SettingsComponent,
    ApiKeysComponent,
    ProfileDetailsComponent,    
    DeactivateAccountComponent,
    SignInMethodComponent
  ],
  imports: [
    CommonModule,
    TranslationModule,
    AccountRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    InlineSVGModule,
    DropdownMenusModule,
    WidgetsModule,
    AvatarModule,
    CrudModule,
    SharedModule
  ],
})
export class AccountModule {}
