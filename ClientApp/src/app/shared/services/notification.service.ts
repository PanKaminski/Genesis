import { Injectable } from "@angular/core";
import { ExceptionDetails } from "@shared/models/exception-details";
import { ToastrService } from "ngx-toastr";

@Injectable({providedIn: 'root'})
  export class NotificationService {
  
    constructor(private readonly toastr: ToastrService) { }
  
    notifySuccess(message: string, title?: string): void {
        this.toastr.success(message, title)
    }

    notifyInfo(message, title?: string): void {
        this.toastr.info(message, title)
    }

    notifyWarning(message, title?: string): void {
        this.toastr.warning(message, title)
    }

    notifyError(error: ExceptionDetails, title?: string): void {
        this.toastr.error(error.message, title)
    }

    notifyErrorMessage(errorMessage: string, title?: string): void {
        this.toastr.error(errorMessage, title)
    }
  }
  