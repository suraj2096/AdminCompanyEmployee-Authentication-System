import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
 RegisterUserData:any ={};
 RegisterFormData:FormGroup;
 PasswordValidatePattern="^[a-zA-Z0-9@]{8,15}";
 constructor(){
   this.RegisterUserData = {
     UserName:"",
     Password:"",
     ConfirmPassword:"",
     Role:""
    };
    this.RegisterFormData  = new FormGroup({
      'UserName':new FormControl(this.RegisterUserData.UserName,[Validators.required,Validators.minLength(4)]),
      'Password':new FormControl(this.RegisterUserData.Password,[Validators.required,Validators.pattern(this.PasswordValidatePattern)]),
      'ConfirmPassword':new FormControl(this.RegisterUserData.ConfirmPassword,[Validators.required]),
      'Role':new FormControl(this.RegisterUserData.Role,[Validators.required])
    },passwordMatchValidator);
    //abstractcontrol it will provide all the control value and also child control value.
    function passwordMatchValidator(control:AbstractControl){
        return control.get('Password')?.value === control.get('ConfirmPassword')?.value?null:{'mismatch':true};
      }
    
  }
 // this is a regiter click method 
 RegisterClick(){
     if(this.RegisterFormData.invalid){
      this.RegisterFormData.get('UserName')?.markAsTouched();
      this.RegisterFormData.get('Password')?.markAsTouched();
      this.RegisterFormData.get('ConfirmPassword')?.markAsTouched();
      this.RegisterFormData.get('Role')?.markAsTouched();
     }

  
 }
}
