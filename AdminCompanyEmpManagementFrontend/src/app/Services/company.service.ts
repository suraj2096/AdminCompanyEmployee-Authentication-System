import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Company } from '../Models/company';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

  constructor(private httpClient:HttpClient) { }

  getAllCompany():Observable<any>{
    return this.httpClient.get("https://localhost:7294/Admin/Companies");
  }
  createCompany(companyData:Company):Observable<any>{
   return this.httpClient.post<Company>("https://localhost:7294/CreateCompany",companyData);
  }
  getCompanyEmployee(id:any):Observable<any>{
    return this.httpClient.get(`https://localhost:7294/api/Management/Company/Employee/${id}`);
  }
}
