import { Component } from '@angular/core';
import { AccountService } from '../../../authentication/services/account.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {

  constructor(private accountService: AccountService){

  }

  get logo(): string {
    return '/assets/images/logo.svg';
  }

  get avatar(): string {
    return this.accountService.account?.picture;
  }

  onLogout() {
    this.accountService.logout();
  }
}
