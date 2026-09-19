import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";

export const authInterceptor: HttpInterceptorFn = (req, next) =>{
    const auth = inject(AuthService);
    const token = auth.token;

    const authorizedReq = token
        ? req.clone({ setHeaders: {Authorization: `Bearer ${token}`}})
        : req;

    return next(authorizedReq).pipe(
        catchError((error) => {
            if(error?.status === 401){
                auth.logout();
            }
            return throwError(() => error);
        })
    );


};