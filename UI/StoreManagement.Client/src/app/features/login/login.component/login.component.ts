import { Component,inject,signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder,ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';


@Component({
  selector: 'app-login.component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {

  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);


  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });


  submit():void{

    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.auth.login(this.form.getRawValue()).subscribe({
      next: (Response) => {
        this.submitting.set(false);
        this.router.navigateByUrl(Response.userTypeName === 'Manager' ? '/warehouses' : '/supply-documents');
      },
      error: (err) => {
        this.submitting.set(false);
        this.errorMessage.set(err?.status === 401 ? 'Wrong user name or password' : 'Could not reach the server. please try again.');
      }
    });

  }


}
