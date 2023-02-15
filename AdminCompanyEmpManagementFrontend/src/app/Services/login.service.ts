import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService{
  loginSuccess:boolean = false;
  constructor(private httpClient:HttpClient) { 
  
  }
  loginDetail(userName:string,password:string):Observable<any>{
    return this.httpClient.post('https://localhost:7294/api/Authentication/Login',{UserName:userName,Password:password});
  };
  
}
