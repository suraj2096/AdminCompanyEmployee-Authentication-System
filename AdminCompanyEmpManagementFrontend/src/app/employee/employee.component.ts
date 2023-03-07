import { Component, OnInit } from '@angular/core';
import { Employee } from '../Models/employee';
import { Leave } from '../Models/leave';
import { EmployeeService } from '../Services/employee.service';
import { LeaveService } from '../Services/leave.service';

declare var window: any;
@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit{
 constructor(private employeeService:EmployeeService,private leaveService:LeaveService){}
 employeeDetail :any=[];
 UpdateEmployee:any;
 formModal:any;
 LeaveData:any;
 LeaveList:any[]=[];
//  EmployeeErrorExist:boolean = false;
  ngOnInit(): void {
  this.getEmployee();
  this.UpdateEmployee = new Employee();
  this.formModal = new window.bootstrap.Modal(
    document.getElementById('UpdateEmployee')
  );
  this.LeaveData = new Leave();
  }
  getEmployee(){
    this.employeeService.getEmployee().subscribe({
      next:(data)=>{
        console.log(data.data);
        this.employeeDetail  = data.data;
      },
      error:(err)=>{
        console.log(err);
      }
    })
  }

  EditButtonSetData(employeeRecord:any){
    console.log(employeeRecord.salary);
    this.UpdateEmployee.Id = employeeRecord.id;
    this.UpdateEmployee.Name = employeeRecord.name;
    this.UpdateEmployee.Address = employeeRecord.address;
    this.UpdateEmployee.Salary = employeeRecord.salary;
    this.UpdateEmployee.Email = employeeRecord.email;
    this.UpdateEmployee.PanNum = employeeRecord.panNum;
    this.UpdateEmployee.PFNum = employeeRecord.pfNum;
    this.UpdateEmployee.AccountNum = employeeRecord.accountNum;
    this.UpdateEmployee.PhoneNum = employeeRecord.phoneNum;
    this.UpdateEmployee.ApplicationUserId = employeeRecord.applicationUserId;
     this.UpdateEmployee.CompanyId = employeeRecord.companyId;
   //console.log(this.UpdateEmployee);
   }

   EditEmployee(form:any){
   
    console.log(this.UpdateEmployee);
    if(form.invalid){
      form.control.markAllAsTouched();
      return;
    }
      this.UpdateEmployee.CompanyId = this.employeeDetail[0].companyId;
    
    
    this.employeeService.updateEmployee(this.UpdateEmployee).subscribe({
      next:(data)=>{
        console.log(data);
      },
      error:(err)=>{
         if(err.status == 400){
          // this.EmployeeErrorExist = true;
          // this.displayCreateEmployeeError = err.error.message;
         }
         setTimeout(()=>{
          this.formModal.hide();
         },4000);
      },
      complete:()=>{
        this.getEmployee();
        this.formModal.hide();
      }
    });
  }

  saveLeave(){
    debugger
    if(this.LeaveData.Reason == undefined || this.LeaveData.StartDate == undefined || this.LeaveData.EndDate== undefined){
      return;
    }
    
    this.LeaveData.EmpId = this.employeeDetail[0].id;
    this.LeaveData.Status = 2; // here leave will by default pending
    this.leaveService.createLeave(this.LeaveData).subscribe({
      next:(data:any)=>{
            alert(data.message);
      },
      error:(err)=>{
           console.log(err);
      }
    })
  }

  leaveDetail(){
    this.leaveService.getLeave(this.employeeDetail[0].id).subscribe({
      next:(data)=>{
           this.LeaveList = data.data;
            this.LeaveList = this.LeaveList.filter((item)=>{
                item.startDate = new Date(item.startDate).toDateString();
                item.endDate = new Date(item.endDate).toDateString();
                return item;
            })
      },
      error:(err)=>{
       console.log(err);
      }
    })
  }

}
