import { AccountService } from "../services/account.service";

export class BaseAuthenticationComponent {
    showPassword: boolean = false;
    showConfirmPassword: boolean = false;  

    constructor(    
        private readonly _accountService: AccountService,
    ) { }

    get isLoading(): boolean { return this.accountService.isLoading; }

    get hasError(): boolean { return this.accountService.hasError; }
  
    get errorText(): string { return this.accountService.errorText; }

    protected get accountService(): AccountService {return this._accountService; }
  
    onPasswordVisibilityChange(isConfirmField: boolean = false): void { 
        if (isConfirmField) {
          this.showConfirmPassword = !this.showConfirmPassword;
        } else {
          this.showPassword = !this.showPassword; 
        }
    }    
}