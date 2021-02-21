import { Component } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  frmSignIn = new FormGroup({
    txtEmail: new FormControl(''),
    txtPassword: new FormControl(''),
  });

  onSubmit(): void {
    //TODO: send login request
    console.log(this.frmSignIn.value);
  }

}
