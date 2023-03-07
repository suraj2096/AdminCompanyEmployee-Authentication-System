import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {

  constructor(private httpClient:HttpClient) { }
  
  getLeave(empId:any):Observable<any>{
   return this.httpClient.get("https://localhost:7294/api/Leave/"+empId);
  }
  
  createLeave(leaveData:any){
  return this.httpClient.post("https://localhost:7294/api/Leave",leaveData);
  }


  getPendingLeave(status:any,cmpId:any):Observable<any>{
    return this.httpClient.get("https://localhost:7294/api/Leave/"+status+"/"+cmpId)
  }

  leaveStatusChange(leave:any):Observable<any>{
    return this.httpClient.put("https://localhost:7294/api/Leave",leave);
  }
}
