export interface ResetPasswordRequest {
    token: string;
    confirmPassword: string;
    password: string;
}