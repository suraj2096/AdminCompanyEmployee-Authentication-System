import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { CompanyService } from '../Services/company.service';
import { Subject } from 'rxjs';
import { Company } from '../Models/company';
import { Router } from '@angular/router';
import { state } from '@angular/animations';

declare var window: any;
@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss']
})
export class CompanyComponent implements OnInit,OnDestroy {
  formModal: any;
  dataTableRef:any;
  GetAllCompany:any;
  DesCheck:boolean = false;
  GetCompanyDesignation:any;
  CreateCompanyDesignation:any=[];
  dtOptions:DataTables.Settings = {};
 CreateCompany:any;
ngOnInit(): void {
  this.dtOptions = {
    responsive:true,
    pageLength:5
  };
  this.getCompanies();
  this.CreateCompany = new Company();

  this.formModal = new window.bootstrap.Modal(
    document.getElementById('CreateCompany')
  );
}  
ngOnDestroy(): void {
  this.dtTrigger.unsubscribe();
}




constructor(private companyService:CompanyService,private Router:Router){  }
dtTrigger:Subject<any> = new Subject<any>();

// get all companies here in this function..
      getCompanies(){
        this.companyService.getAllCompany().subscribe(
        (data)=>{
            this.GetAllCompany = data;
            console.log(this.GetAllCompany);
            // initiate owr data table
            this.dtTrigger.next(null);
          },
          (error:any)=>{
            console.log(error);
          }
        
        )
      }

      // here is the function of onclick of show ....... in which we will dispaly particular
      // company reputed desigantion. 
      getCompanyDesignatinOnClick(companyDesigantionList:any){
       this.GetCompanyDesignation = companyDesigantionList;
      }


      // here we add designation textbox to the frontend using this function.
      AddMoreDesignation(){
        this.CreateCompanyDesignation.push({DesType:"",Name:"",Validate:""});
       
      }

      
    // here we will remove the designation
      RemoveDesignation(index:any){
        this.CreateCompanyDesignation.splice(index,1);
        console.log(this.CreateCompanyDesignation);
      }

      // Close button par designation array koa reset
      ResetCancel(){
        this.CreateCompanyDesignation.splice(0,this.CreateCompanyDesignation.length);
      }

      // here we will save company function
      SaveCompany(form:any){
        
        if(form.invalid){
          form.control.markAllAsTouched();
          return;
        }
        this.CreateCompanyDesignation.filter((data:any)=>{
          if(data.DesType=="" && data.Name==""){
            this.DesCheck = true;
            data.Validate = "Enter the designation type and name of the person";
          }
          else if(data.Name == ""){
            this.DesCheck = true;
            data.Validate = `Enter the name of that designation on ${data.DesType}`;
          }
          else if(data.DesType==""){
            this.DesCheck = true;
            data.Validate = `Enter the designation of the person is ${data.Name}`;
          }
        });
        if(this.DesCheck) return;
        // here we will call the create company api when all validation above approved .
        this.CreateCompanyDesignation.filter((data:any,index:any)=>{
             this.CreateCompany.CompanyDesigantion[index]={DesignationType:data.DesType.toUpperCase(),Name:data.Name};
        });
      
          
          this.companyService.createCompany(this.CreateCompany).subscribe({
            next:(data)=>{
            },
            error:(err)=>{
              console.log(err);
            },
            complete:()=>{
              this.getCompanies();
              this.dtTrigger.unsubscribe();
              form.resetForm();
              this.formModal.hide();
            }
          })
         
      }
      EmployeeList(id:any){
        console.log(id);
        this.companyService.getCompanyEmployee(id).subscribe((data)=>{
               console.log(data);
        },(err)=>{
           console.log(err);
        })
        // this.Router.navigate(['/employeelist'],{state:{data:}})
      }
}
