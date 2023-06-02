import { Role } from "@shared/enums/role";

export interface Account {
    id: number;
    firstName: string;
    secondName: string;
    email: string;
    picture?: string;
    roles: Role[];
    isVerified: boolean;
    jwtToken: string;
}