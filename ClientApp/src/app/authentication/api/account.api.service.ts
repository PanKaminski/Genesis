import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { Account } from "../models/account";
import { AuthenticateRequest } from '../models/authenticate-request'
import { RegisterRequest } from "../models/register-request";
import { ResetPasswordRequest } from "../models/reset-password-request";
import { environment } from "@environments/environment";
import { ServerTextResponse } from "@shared/models/server-text-response";

@Injectable({providedIn: 'root'})
  export class AccountApiService {
    private readonly LOGIN = 'api/Account/Login';
    private readonly REGISTER = 'api/Account/Register';
    private readonly VERIFY_EMAIL = 'api/Account/VerifyEmail';
    private readonly FORGOT_PASSWORD = 'api/Account/ForgotPassword';
    private readonly VALIDATE_RESET_TOKEN = 'api/Account/ValidateResetToken';
    private readonly RESET_PASSWORD = 'api/Account/ResetPassword';
    private readonly REVOKE_TOKEN = 'api/Account/RevokeToken';
    private readonly REFRESH_TOKEN = 'api/Account/RefreshToken';

    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    login(loginModel: AuthenticateRequest): Observable<Account> {
        return this.http.post<Account>(this.apiUrl + this.LOGIN, loginModel);
    }

    logout(): Observable<ServerTextResponse> {
        return this.revokeToken();
    }

    revokeToken(refreshToken?: string): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.REVOKE_TOKEN, { refreshToken });
    }

    refreshToken(): Observable<Account> {
        return this.http.post<Account>(this.apiUrl + this.REFRESH_TOKEN, {})
    }

    register(registerModel: RegisterRequest): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.REGISTER, registerModel);
    }

    verifyEmail(token: string): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.VERIFY_EMAIL, { token });
    }
    
    forgotPassword(email: string): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.FORGOT_PASSWORD, { email });
    }
    
    validateResetToken(token: string): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.VALIDATE_RESET_TOKEN, { token });
    }
    
    resetPassword(resetTokenModel: ResetPasswordRequest): Observable<ServerTextResponse> {
        return this.http.post<ServerTextResponse>(this.apiUrl + this.RESET_PASSWORD, resetTokenModel);
    }
}