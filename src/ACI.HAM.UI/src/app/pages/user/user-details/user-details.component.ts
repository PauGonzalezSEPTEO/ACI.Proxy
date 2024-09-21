import { AfterViewInit, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

//ToDo Pau
// import { IUserModel, UserService } from 'src/app/_fake/services/user-service';
//

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss']
})
export class UserDetailsComponent implements OnInit, AfterViewInit {

  isCollapsed: boolean;

  //ToDo Pau
  // user: IUserModel = { id: 0, name: '', email: '', };
  //

  constructor(

    //ToDo Pau
    // private userService: UserService,
    //

    private route: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    //ToDo Pau
    // this.userService.getUser(id).subscribe((res: IUserModel) => {
    //   this.user = res;
    //   this.changeDetectorRef.detectChanges();
    // });
    //

  }

  ngAfterViewInit(): void {
  }

}
