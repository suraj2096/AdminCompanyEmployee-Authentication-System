import { Component,Input, OnDestroy, OnInit } from '@angular/core';
import { Employee } from '../Models/employee';
import { Location } from '@angular/common';
import { EmployeeService } from '../Services/employee.service';
import { CompanyService } from '../Services/company.service';

declare var window: any;
@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})
export class EmployeeListComponent implements OnInit {
  empList:any;
  formModal:any;
  saveUpdatButton:boolean = false;
  CreateEmployee:any;
  EmployeeErrorExist:boolean=false;
  displayCreateEmployeeError:any;
  ngOnInit(): void {
    this.empList = history.state.data;
    console.log(this.empList);
    this.CreateEmployee = new Employee();
    this.formModal = new window.bootstrap.Modal(
      document.getElementById('CreateEmployee')
    );
  }
  constructor(private employeeService:EmployeeService,private location:Location,private companyService:CompanyService){}
  
  addEmployee():void{
    this.saveUpdatButton = false;
  }
  
  SaveEmployee(form:any){
    
    if(form.invalid){
      form.control.markAllAsTouched();
      return;
    }
    if(this.empList.length!=0){
      this.CreateEmployee.CompanyId = this.empList[0].companyId;
    }
    else{
      this.CreateEmployee.CompanyId = history.state.cmpId;
    }
    this.employeeService.createEmployee(this.CreateEmployee).subscribe({
      next:(data)=>{
        console.log(data);
        
      },
      error:(err)=>{
         if(err.status == 400){
          this.EmployeeErrorExist = true;
          this.displayCreateEmployeeError = err.error.message;
         }
         setTimeout(()=>{
          this.formModal.hide();
         },4000);
      },
      complete:()=>{
        this.formModal.hide;
        this.companyService.getCompanyEmployee(history.state.cmpId).subscribe(
          (data)=>{
            this.empList = data.data;
            
          },
          (err)=>{
            console.log(err);
          }
        )
      }
    });

  }

  EditButtonSetData(employeeRecord:any){
    
   this.CreateEmployee.Name = employeeRecord.name;
   this.CreateEmployee.Address = employeeRecord.address;
   this.CreateEmployee.Salary = employeeRecord.salary;
   this.CreateEmployee.PanNum = employeeRecord.panNum;
   this.CreateEmployee.PFNum = employeeRecord.pfNum;
   this.CreateEmployee.AccountNum = employeeRecord.accountNum;
   this.CreateEmployee.PhoneNum = employeeRecord.phoneNum;
   this.saveUpdatButton = true;
  }

  EditEmployee(form:any){
    if(form.invalid){
      form.control.markAllAsTouched();
      return;
    }
    if(this.empList.length!=0){
      this.CreateEmployee.CompanyId = this.empList[0].companyId;
    }
    else{
      this.CreateEmployee.CompanyId = history.state.cmpId;
    }
    this.employeeService.updateEmployee(this.CreateEmployee).subscribe({
      next:(data)=>{
        console.log(data);
      },
      error:(err)=>{
         if(err.status == 400){
          this.EmployeeErrorExist = true;
          this.displayCreateEmployeeError = err.error.message;
         }
         setTimeout(()=>{
          this.formModal.hide();
         },4000);
      },
      complete:()=>{
        this.formModal.hide;
        this.companyService.getCompanyEmployee(this.empList[0].companyId).subscribe(
          (data)=>{
            this.empList = data.data;
            
          },
          (err)=>{
            console.log(err);
          }
        )
      }
    });
  }


  DeleteEmployee(id:any){
    console.log(id);
    this.employeeService.deleteEmployee(id).subscribe({
      next:(data)=>{
        console.log(data);
      },
      error:(err)=>{
      },
      complete:()=>{
        this.companyService.getCompanyEmployee(this.empList[0].companyId).subscribe(
          (data)=>{
            this.empList = data.data;
            
          },
          (err)=>{
            console.log(err);
          }
        )
      }
    });
  }

  BackToList(){
     this.location.back();
    // history.back();
  }
  

 
}
