import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginService } from '../Services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
 // LoginUserData:any ={};
  UserName:string="";
  Password:string="";
  LoginUserData:FormGroup;
  constructor(private loginService:LoginService){
  this.LoginUserData = new FormGroup({
    name:new FormControl(this.UserName,[Validators.required,Validators.minLength(4)]),
    password:new FormControl(this.Password,[Validators.required])
  });
  }
  // this is a login click method 
 LoginClick(){
  // here we will check owr form in valid or not if not tnen generate the validation
  if(this.LoginUserData.invalid){
    this.LoginUserData.get('name')?.markAsTouched();
    this.LoginUserData.get('password')?.markAsTouched();
    return;
  }
    this.UserName = this.LoginUserData.get('name')?.value;
    this.Password = this.LoginUserData.get('password')?.value;
    this.loginService.loginDetail(this.UserName,this.Password).subscribe((data)=>{
    console.log(data);
    },(err)=>{
    alert(JSON.stringify(err));
    });

   }
}
