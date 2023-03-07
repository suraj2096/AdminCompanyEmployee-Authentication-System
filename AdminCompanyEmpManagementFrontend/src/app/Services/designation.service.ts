import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DesignationService {

  constructor(private httpClient:HttpClient) { }

 getDesigantion(id:any):Observable<any>{
 return this.httpClient.get("https://localhost:7294/api/Designation/"+id)
 }


  createDesigantion(data:any):Observable<any>{
    return  this.httpClient.post("https://localhost:7294/api/Designation",data);

  }

  deleteDesignation(id:any):Observable<any>{
    return this.httpClient.delete("https://localhost:7294/api/Designation/"+id);
  }


}
