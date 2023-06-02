import { Button } from "@shared/models/forms/button";

export interface ConnectionCard {
    connectionId?: number;
    connectionStatus?: ConnectionStatus;
    avatar: string;
    userName: string;
    userId: number;
    connectionsCount: number,
    country?: string;
    city?: string;
    buttons: Button[];
}

export enum ConnectionStatus {
    Pending = 1,
    Accepted,
    Blocked,
    Declined
}