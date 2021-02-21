import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  //TODO: get security questions from api
  frmRegister: FormGroup
  submitted: boolean;

  ngOnInit(): void {
    this.frmRegister = new FormGroup({
      txtName: new FormControl('', [
        Validators.required
      ]),
      txtEmail: new FormControl('', [
        Validators.required
      ]),
      txtPassword: new FormControl('', [
        Validators.required
      ]),
      txtConfirmPassword: new FormControl(''),
    }, { validators: this.checkPasswords });
  }

  get txtName() {
    return this.frmRegister.get('txtName');
  }
  get txtEmail() {
    return this.frmRegister.get('txtEmail');
  }
  get txtPassword() {
    return this.frmRegister.get('txtPassword');
  }
  get txtConfirmPassword() {
    return this.frmRegister.get('txtConfirmPassword');
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.frmRegister.valid) {
      console.log('form submitted');
      //TODO: send login request
    } else {
      console.log('form invalid');
    }
  }

  checkPasswords(group: FormGroup) { // here we have the 'passwords' group
    const password = group.get('txtPassword').value;
    const confirmPassword = group.get('txtConfirmPassword').value;

    return password === confirmPassword ? null : { notSame: true }
  }

} 
