import { OnDestroy } from '@angular/core';
import { Component } from '@angular/core';
import { FormGroup, FormBuilder, AbstractControl, Validators } from '@angular/forms';
import { Gender } from '@shared/enums/gender';
import { SelectItem } from '@shared/models/forms/select-item';
import { LocationsService } from '@shared/services/locations.service';
import { NotificationService } from '@shared/services/notification.service';
import { Subject, takeUntil } from 'rxjs';
import { first } from 'rxjs';
import { RegisterRequest } from '../../models/register-request';
import { AccountService } from '../../services/account.service';
import { BaseAuthenticationComponent } from '../base-authentication.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent extends BaseAuthenticationComponent implements OnDestroy {
  form: FormGroup;
  genders: SelectItem[] = [
    {name: 'Man', value: Gender.Man},
    {name: 'Woman', value: Gender.Woman},
  ];

  countries: SelectItem[] = [];
  cities: SelectItem[] = [];
  isLinkSent: boolean = false;
  messageText: string = null;

  private destroy$ = new Subject<void>();

  constructor(
    accountService: AccountService,
    private readonly formBuilder: FormBuilder,
    private readonly notificationService: NotificationService,
    private readonly locationsService: LocationsService,
  ) { 
    super(accountService);
  }

  get email(): AbstractControl { return this.form.controls['email']; }

  get password(): AbstractControl { return this.form.controls['password']; }

  get confirmPassword(): AbstractControl { return this.form.controls['confirmPassword']; }

  get firstName(): AbstractControl { return this.form.controls['firstName']; }

  get middleName(): AbstractControl { return this.form.controls['middleName']; }

  get lastName(): AbstractControl { return this.form.controls['lastName']; }

  get gender(): AbstractControl { return this.form.controls['gender']; }

  get country(): AbstractControl { return this.form.controls['country']; }

  get city(): AbstractControl { return this.form.controls['city']; }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    confirmPassword: ['', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    middleName: [''],
    gender: ['', Validators.required],
    country: [''],
    city: [''],
    });

    this.locationsService.getCountriesItemsList()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (countriesList: SelectItem[]) => {
          this.countries = countriesList;
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
      });
  }

  onSignUp() : void {
    if (this.form.invalid) {
        return;
    }

    this.accountService.register({
      email: this.email.value,
      password: this.password.value,
      confirmPassword: this.confirmPassword.value,
      firstName: this.firstName.value,
      lastName: this.lastName.value,
      middleName: this.middleName.value,
      gender: this.gender.value,
      countryCode: this.country.value,
      cityCode: this.city.value,
    } as RegisterRequest)
      .pipe(first())
      .subscribe({
        next: (result) => {
          this.isLinkSent = true;
          this.messageText = result.message;
          this.notificationService.notifySuccess('Registration successful');
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
    });
  }

  onCountryChange(countryCode: string): void {
    console.log(countryCode)
    this.locationsService.getCitiesItemsList(countryCode)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (citiesList: SelectItem[]) => {
          this.cities = citiesList;
        },
        error: error => {
          this.notificationService.notifyError(error);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
