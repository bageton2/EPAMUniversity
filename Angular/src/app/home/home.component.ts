import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: []
})
export class HomeComponent implements OnInit {
  userDetails;  
  constructor(private router: Router, private userService: UserService) { }

  ngOnInit() {
    this.userService.getUserProfile().subscribe(
      res => { this.userDetails = res },
      err => {
        localStorage.removeItem('token');
        this.router.navigateByUrl('/user/login')
        console.log(err);
      }
    )
  }

  onLogoutClick() {
    localStorage.removeItem('token');
    this.router.navigate(['/user/login'])
  }

  onHomeClick(){
    this.router.navigate(['/home'])
  }
}
