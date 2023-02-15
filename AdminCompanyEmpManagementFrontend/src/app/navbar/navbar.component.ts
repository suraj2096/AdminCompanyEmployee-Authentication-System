import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../Services/login.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  constructor(public loginService:LoginService,private router:Router){
 
  }
  ngOnInit(): void {
    try{
      const checkTokenExist = localStorage.getItem('data')!=null?JSON.parse(localStorage.getItem('data')?.toString()??""):"";
      if(checkTokenExist!="" && checkTokenExist.token!=""){
        this.loginService.loginSuccess = true;
      }
      else{
        this.router.navigate(['/login']);
      }
    }
    catch(err){
      this.router.navigate(['/login']);
    }   
  }
   
   Logout(){
     this.loginService.loginSuccess = false;
    localStorage.clear();
    this.router.navigate(['/login']);
   }
}
