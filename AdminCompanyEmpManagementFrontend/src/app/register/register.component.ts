import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterService } from '../Services/register.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
 RegisterUserData:any ={};
 RegisterFormData:FormGroup;
 PasswordValidatePattern="^[a-zA-Z0-9@]{8,15}";
 DisplaySuccess:boolean = false;
 DisplayName:string="danger";
 DisplayMessage:string ="";
 DisplayRole=["Company","Employee"];
 constructor(private registerService:RegisterService,private router:Router){
   this.RegisterUserData = {
     UserName:"",
     Password:"",
     ConfirmPassword:"",
     Role:"Select your Role"
    };
    this.RegisterFormData  = new FormGroup({
      'UserName':new FormControl(this.RegisterUserData.UserName,[Validators.required,Validators.minLength(4),Validators.pattern(/^[a-zA-Z_]/)]),
      'Password':new FormControl(this.RegisterUserData.Password,[Validators.required,Validators.pattern(this.PasswordValidatePattern)]),
      'ConfirmPassword':new FormControl(this.RegisterUserData.ConfirmPassword,[Validators.required]),
      'Role':new FormControl(this.RegisterUserData.Role,[Validators.required])
    },passwordMatchValidator);

    //abstractcontrol it will provide all the control value and also child control value.
    function passwordMatchValidator(control:AbstractControl){
        return control.get('Password')?.value === control.get('ConfirmPassword')?.value?null:{'mismatch':true};
      }
    
  }
 
  SelectRoleChange(event:any){
     this.RegisterFormData.setValue({UserName:this.RegisterFormData.get('UserName')?.value,
      Password:this.RegisterFormData.get('Password')?.value,ConfirmPassword:this.RegisterFormData.get('ConfirmPassword')?.value,
      Role:event.target.value});
  }

 // this is a regiter click method 
 RegisterClick(){ debugger
  if(this.RegisterFormData.invalid){
      this.RegisterFormData.get('UserName')?.markAsTouched();
      this.RegisterFormData.get('Password')?.markAsTouched();
      this.RegisterFormData.get('ConfirmPassword')?.markAsTouched();
      this.RegisterFormData.get('Role')?.markAsTouched();
      return;
     }
     this.RegisterUserData.UserName = this.RegisterFormData.get('UserName')?.value;
     this.RegisterUserData.Password = this.RegisterFormData.get('Password')?.value;
     this.RegisterUserData.Role = this.RegisterFormData.get('Role')?.value;
   
     // here we will call the register api .......
    this.registerService.create(this.RegisterUserData).subscribe({
      next:(data)=>{
        this.DisplaySuccess=true;
        if(data.status == 1){
          this.DisplayName = "success"; 
        }
          this.DisplayMessage = data.message; 
      },
      error:(err)=>{debugger
       if(err.status == 500){
        this.DisplaySuccess = true;
        this.DisplayMessage = "You Choose the wrong Role";
        setTimeout(()=>{
        // this.RegisterFormData.setValue({UserName:this.RegisterUserData.UserName,
        //   Password:this.RegisterUserData.Password,ConfirmPassword:this.RegisterUserData.Password,
        // Role:this.DisplayRole[1]});
        this.DisplaySuccess = false;
        this.DisplayName = "danger";
       },3000);
       }  
      
      },
      complete:()=>{
        this.RegisterFormData.reset("");
        alert(this.RegisterUserData.Role = this.RegisterFormData.get('Role')?.value);
        setTimeout(()=>{
        this.DisplaySuccess = false;
        this.DisplayName = "danger";
        this.router.navigate(['/login']);
       },2000);
      }
    })
  
 }
}
