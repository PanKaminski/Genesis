import { Button } from "@shared/models/forms/button";
import { ConnectionStatus } from "./user-card";

export interface UpdateConnectionStatusResponse {
    userId: number;
    connectionId?: number;
    status?: ConnectionStatus
    buttons: Button[];
}