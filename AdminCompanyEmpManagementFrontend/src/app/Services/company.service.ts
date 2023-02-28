import { HttpClient, HttpHeaders } from '@angular/common/http';
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
  updateCompany(companyData:Company):Observable<any>{
    return this.httpClient.put<Company>("https://localhost:7294/UpdateCompany",companyData);
  }
  deleteCompany(id:any):Observable<any>{
    return this.httpClient.delete<Company>(`https://localhost:7294/DeleteCompany/${id}`);
  }
  getCompanyEmployee(id:any):Observable<any>{
    return this.httpClient.get(`https://localhost:7294/api/Management/Company/Employee/${id}`);
  }
  getSpecificCompany():Observable<any>{
    debugger
  console.log(JSON.parse(localStorage.getItem('data')??"").token);
    return this.httpClient.get(`https://localhost:7294/api/Management/GetCompanyData`,{headers: new HttpHeaders({
      'Content-Type': 'application/json',
       'Authorization': "Bearer "+JSON.parse(localStorage.getItem('data')??"").token
    })});
  }
}
