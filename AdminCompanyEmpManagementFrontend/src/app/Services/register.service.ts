import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {
   registerUserDetail={
    UserName:"",
    Password:"",
    Role:"",
   }
  constructor(private httpClient:HttpClient) { }
    
  create(registerUserDetail:any):Observable<any>{
          return this.httpClient.post<any>('https://localhost:7294/api/Authentication/Register',registerUserDetail);
  }

}
