import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

  constructor(private httpClient:HttpClient) { }

  getAllCompany():Observable<any>{
    return this.httpClient.get("https://localhost:7294/Admin/Companies");
  }
}
