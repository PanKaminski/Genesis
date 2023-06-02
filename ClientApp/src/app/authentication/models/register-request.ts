import { Gender } from "@shared/enums/gender";
import { AuthenticateRequest } from "./authenticate-request";

export interface RegisterRequest extends AuthenticateRequest {
    confirmPassword: string,
    firstName: string;
    lastName: string;
    middleName?: string;
    gender: Gender;
    countryCode: string;
    cityCode?: string;
}