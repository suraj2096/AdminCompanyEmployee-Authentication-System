import { Component,Input, OnDestroy, OnInit } from '@angular/core';
import { Employee } from '../Models/employee';
import { Location } from '@angular/common';
import { EmployeeService } from '../Services/employee.service';
import { CompanyService } from '../Services/company.service';
import { DesignationService } from '../Services/designation.service';
import { Leave } from '../Models/leave';
import { LeaveService } from '../Services/leave.service';

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
  CreateCompanyDesignation:any=[];
  DesCheck:boolean = false;
  SendCompanyDesignation:any=[];
  GetCompanyDesignation:any[]=[];
  LeaveList:any[]=[];
  disRejAppButton:boolean = false;
  disLeaveListHead:any = "";
  ngOnInit(): void {
    // this.empList = history.state.data;
    this.getCompanyEmployee();
    console.log(this.empList);
    this.CreateEmployee = new Employee();
    
    this.formModal = new window.bootstrap.Modal(
      document.getElementById('CreateEmployee')
    );
  }
  constructor(private employeeService:EmployeeService,private location:Location,private companyService:CompanyService,private designationService:DesignationService
    ,private leaveService:LeaveService){}
  
  getCompanyEmployee(){
    this.companyService.getCompanyEmployee(history.state.cmpId).subscribe(
      (data)=>{
        this.empList = data.data;
        
      },
      (err)=>{
        console.log(err);
      }
    )
  }
  



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
        alert(`userName:${data.data.user} and password is ${data.data.password}`);
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
        // this.companyService.getCompanyEmployee(history.state.cmpId).subscribe(
        //   (data)=>{
        //     this.empList = data.data;
            
        //   },
        //   (err)=>{
        //     console.log(err);
        //   }
        // )
        this.getCompanyEmployee();
      }
    });

  }

  EditButtonSetData(employeeRecord:any){
    this.CreateEmployee.Id = employeeRecord.id;
   this.CreateEmployee.Name = employeeRecord.name;
   this.CreateEmployee.Address = employeeRecord.address;
   this.CreateEmployee.Salary = employeeRecord.salary;
   this.CreateEmployee.PanNum = employeeRecord.panNum;
   this.CreateEmployee.PFNum = employeeRecord.pfNum;
   this.CreateEmployee.AccountNum = employeeRecord.accountNum;
   this.CreateEmployee.PhoneNum = employeeRecord.phoneNum;
   this.CreateEmployee.ApplicationUserId = employeeRecord.applicationUserId;
   this.CreateEmployee.Email = employeeRecord.email;
   this.saveUpdatButton = true;
  }

  EditEmployee(form:any){
    debugger
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
    console.log(this.CreateEmployee);
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
        // this.companyService.getCompanyEmployee(this.empList[0].companyId).subscribe(
        //   (data)=>{
        //     this.empList = data.data;
            
        //   },
        //   (err)=>{
        //     console.log(err);
        //   }
        // )
        this.getCompanyEmployee();
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
  resetValue(){
    this.CreateEmployee= new Employee();
  }

   // here we add designation textbox to the frontend using this function.
   AddMoreDesignation(){
   if(this.CreateCompanyDesignation.length>=1 && this.CreateCompanyDesignation[this.CreateCompanyDesignation.length-1].Name == ""){
    return;
   }

    if(this.CreateCompanyDesignation.length<this.empList.length ){
      this.CreateCompanyDesignation.push({DesType:"",Name:"",Validate:"",EmpId:""});
    }
    else{
      return;
    }
    // for(let i=0;i<this.CreateCompanyDesignation.length;i++){
    // }
    if(this.CreateCompanyDesignation.length>1){
      setTimeout(()=>{
        let data = document.getElementById("desigantionParent")?.children[this.CreateCompanyDesignation.length-1].children[0].children[1].children[0];
        for(let i=0;i<=this.CreateCompanyDesignation.length-1;i++){
          for(let j=0;j<=this.empList.length;j++){
            if(data?.children[j].innerHTML == this.CreateCompanyDesignation[i].Name ){
              data?.children[j].setAttribute('style',"display:none;");
            }
          }
        }
        console.log(data);
  
      },1000)

    }

   
  }




  onChangeEmployeeName(event:any){
    this.CreateCompanyDesignation[this.CreateCompanyDesignation.length-1].Name = event.target.value;
    for(let i=0;i<event.target.children.length;i++){
      if(event.target.children[i].value == this.CreateCompanyDesignation[this.CreateCompanyDesignation.length-1].Name){
        this.CreateCompanyDesignation[this.CreateCompanyDesignation.length-1].EmpId = event.target.children[i].id;       
        break;
      }
    }
  }




  saveDesignation(){
    this.CreateCompanyDesignation.filter((data:any)=>{
      if(data.DesType=="" && data.Name==""){
        this.DesCheck = true;
        data.Validate = "Enter the designation type and name of the person";
      }
      else if(data.Name == ""){
        this.DesCheck = true;
        data.Validate = `Select the name of the employee in designation  ${data.DesType}`;
      }
      else if(data.DesType==""){
        this.DesCheck = true;
        data.Validate = `Enter the designation of the person is ${data.Name}`;
      }
    });
    if(this.DesCheck) return;

    this.CreateCompanyDesignation.filter((data:any,index:any)=>{
      this.SendCompanyDesignation[index]={DesignationType:data.DesType.toUpperCase(),Name:data.Name,EmpId:data.EmpId,CmpId:history
      .state.cmpId};
 });

 this.designationService.createDesigantion(this.SendCompanyDesignation).subscribe({
  next:(data)=>{
    alert(data.message);
  },
  error:(err)=>{
    console.log(err);
  },
  complete:()=>{
   
  }
})

  }

  showDesignation(){
    this.designationService.getDesigantion(history.state.cmpId).subscribe((data)=>{
      this.GetCompanyDesignation = data.data;
    },()=>{

    })
  }
  removeDesignation(data:any){
  this.designationService.deleteDesignation(data.id).subscribe((data)=>{
   alert(data.message);
  })
  }


getleaveSpecific(status:any){
  this.leaveService.getPendingLeave(status,history.state.cmpId).subscribe((data)=>{
    this.LeaveList = data.data;
    this.LeaveList = this.LeaveList.filter((item)=>{
        item.startDate = new Date(item.startDate).toDateString();
        item.endDate = new Date(item.endDate).toDateString();
        return item;
    })
  },(err)=>{
  console.log(err);
  })
}


  getPendingLeaves(){
    this.disLeaveListHead = "Pending Leaves"
    this.disRejAppButton = true;
        this.getleaveSpecific(2);
  }

  getApprovedLeaves(){
    this.disLeaveListHead = "Approved Leaves"
    this.disRejAppButton = false;
        this.getleaveSpecific(1);
  }

  getRejectedLeaves(){
    this.disLeaveListHead = "Reject Leaves"
    this.disRejAppButton = false;
        this.getleaveSpecific(3);
  }

  leaveRejected(empId:any){
    let leave = new Leave();
    leave.Status = 3;
    leave.EmpId = empId;
    this.leaveService.leaveStatusChange(leave).subscribe((data:any)=>{
         alert(data.message);
    },(err)=>{
        console.log(err);
    })
  }

  leaveApproved(empId:any){
    // this.disLeaveListHead = "Approved Leaves";
    // this.disRejAppButton = false;
    let leave = new Leave();
    leave.Status = 1;
    leave.EmpId = empId;
    this.leaveService.leaveStatusChange(leave).subscribe((data:any)=>{
         alert(data.message);
    },(err)=>{
        console.log(err);
    })
  }
  


 
}
