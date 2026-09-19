import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";
import { UserType } from "../models/Auth.model";

export function roleGuard(requiredRole: UserType): CanActivateFn {
    return () => {
        const auth = inject(AuthService);
        const router = inject(Router);

        if(auth.userType() === requiredRole){
            return true;
        }
        return router.parseUrl(auth.isManager() ? '/warehouses' : '/supply-documents');
    };
}