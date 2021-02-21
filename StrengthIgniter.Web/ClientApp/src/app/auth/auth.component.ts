import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent {
  title: string = 'auth';
  currentYear: number = (new Date()).getFullYear(); 
  
}
