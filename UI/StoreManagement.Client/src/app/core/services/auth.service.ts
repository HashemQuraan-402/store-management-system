
import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, UserType } from '../models/Auth.model';


const STORAGE_KEY = 'sm.session';

interface StoredSession extends LoginResponse{}

@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly router = inject(Router);

    private readonly session = signal<StoredSession | null>(this.readSession());


    readonly currentUser = computed(() => this.session());
    readonly isLoggedIn = computed(() => this.session() !== null);
    readonly userType = computed<UserType | null>(() => this.session()?.userTypeName ?? null);
    readonly isManager = computed(() => this.userType() === 'Manager');
    readonly isEmployee = computed(() => this.userType() === 'Employee');
    readonly fullName = computed(() => this.session()?.userFullName ?? '');



    login(request: LoginRequest):Observable<LoginResponse>{
        return this.http.post<LoginResponse>(`${environment.apiUrl}/Auth/login`,request).pipe(
            tap((response) => {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
                this.session.set(response);
            })
        );
    }


    clearSession():void{
        localStorage.removeItem(STORAGE_KEY);
        this.session.set(null);
    }

    logout():void{
        this.clearSession();
        this.router.navigateByUrl('/login');
    }


    get token(): string | null{
        return this.session()?.token ?? null;
    }




    isExpired(): boolean{
        const expiresAt = this.session()?.expiresAt;
        if(!expiresAt){
            return true;
        }
        return new Date(expiresAt).getTime() <= Date.now();
    }



    private readSession(): StoredSession | null{
        const raw = localStorage.getItem(STORAGE_KEY);
        if(!raw){
            return null;
        }
        try{
            return JSON.parse(raw) as StoredSession;
        }catch{
            return null;
        }
    }
}



