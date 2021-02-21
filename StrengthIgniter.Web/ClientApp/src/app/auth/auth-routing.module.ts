import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthComponent } from './auth.component';
import { ForgotComponent } from './forgot/forgot.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  {
    path: '', component: AuthComponent, children: [
      { path: '', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot', component: ForgotComponent }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
