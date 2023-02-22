import { Component, OnDestroy, OnInit } from '@angular/core';
import { CompanyService } from '../Services/company.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss']
})
export class CompanyComponent implements OnInit,OnDestroy {
  GetAllCompany:any;
  GetCompanyDesignation:any;
  CreateCompanyDesignation:[{}]=[{DesType:"",name:""}];
  dtOptions:DataTables.Settings = {};
ngOnInit(): void {
  this.dtOptions = {
    pagingType: 'full_numbers',
    responsive:true
  };
  this.getCompanies();
}  
ngOnDestroy(): void {
  this.dtTrigger.unsubscribe();
}
constructor(private companyService:CompanyService){  }
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
      getCompanyDesignatinOnClick(companyDesigantionList:any){
       this.GetCompanyDesignation = companyDesigantionList;
      }
      AddMoreDesignation(){

      }
      RemoveDesignation(){
        
      }
}
