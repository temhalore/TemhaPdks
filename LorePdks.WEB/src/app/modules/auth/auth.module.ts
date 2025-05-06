import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './components/login/login.component';
import { AuthComponent } from './auth.component';
import { BilgiPenceresiComponent } from '../../core/components/bilgi-penceresi/bilgi-penceresi.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AuthRoutingModule,
    BilgiPenceresiComponent,
    AuthComponent,
    LoginComponent
  ]
})
export class AuthModule { }