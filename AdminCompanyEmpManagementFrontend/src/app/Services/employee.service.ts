import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from '../Models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private httpClient:HttpClient) { }

  getEmployee():Observable<any>{
    return this.httpClient.get("https://localhost:7294/api/Employee",{headers: new HttpHeaders({
      'Content-Type': 'application/json',
       'Authorization': "Bearer "+JSON.parse(localStorage.getItem('data')??"").token
    })});
  }
  createEmployee(employeeData:Employee):Observable<any>{
          return this.httpClient.post("https://localhost:7294/api/Employee",employeeData);
  }
  updateEmployee(employeeData:Employee):Observable<any>{
    debugger
    return this.httpClient.put("https://localhost:7294/api/Employee",employeeData);
  }
  deleteEmployee(id:any):Observable<any>{
    return this.httpClient.delete(`https://localhost:7294/api/Employee/${id}`);
  }
}
