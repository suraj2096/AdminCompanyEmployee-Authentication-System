import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../Services/login.service';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  UserName:string="";
  Password:string="";
  LoginUserData:FormGroup;
  DisplaySuccess:boolean = false;
  DisplayMessage:string = "";
  DisplayName:string="danger";
  constructor(private loginService:LoginService,private router:Router){
  this.LoginUserData = new FormGroup({
    name:new FormControl(this.UserName,[Validators.required,Validators.minLength(4)]),
    password:new FormControl(this.Password,[Validators.required])
  });
  }
  // this is a login click method 
 LoginClick(){
  // here we will check owr form is valid or not if not tnen generate the validation
  if(this.LoginUserData.invalid){
    this.LoginUserData.get('name')?.markAsTouched();
    this.LoginUserData.get('password')?.markAsTouched();
    return;
  }
    this.UserName = this.LoginUserData.get('name')?.value;
    this.Password = this.LoginUserData.get('password')?.value;
    this.loginService.loginDetail(this.UserName,this.Password).subscribe((data)=>{
     // console.log(data);
      this.DisplaySuccess=true;
      if(data.status == 1){
        this.DisplayName = "success"; 
        this.DisplayMessage = "Login Successfully!!!" 
        localStorage.setItem('data',JSON.stringify(data));
      }
      else{
        this.DisplayMessage = data.message;
      }
    },(err)=>{
    alert(JSON.stringify(err));
    },()=>{
      this.LoginUserData.reset("");
      setTimeout(()=>{
        this.DisplaySuccess = false;
        this.DisplayName = "danger";
        this.router.navigate(['/admin']);
       },3000);
    });
   }
}
