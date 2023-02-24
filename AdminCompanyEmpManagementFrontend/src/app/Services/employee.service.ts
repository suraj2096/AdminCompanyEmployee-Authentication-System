import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from '../Models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  constructor(private httpClient:HttpClient) { }

  createEmployee(employeeData:Employee):Observable<any>{
          return this.httpClient.post("https://localhost:7294/api/Employee",employeeData);
  }
  updateEmployee(employeeData:Employee):Observable<any>{
    return this.httpClient.patch("https://localhost:7294/api/Employee",employeeData);
  }
  deleteEmployee(id:any):Observable<any>{
    return this.httpClient.delete(`https://localhost:7294/api/Employee/${id}`);
  }
}
