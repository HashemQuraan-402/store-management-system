

export type UserType = 'Employee' | 'Manager';

export interface LoginRequest{
    userName: string;
    password: string;
}

export interface LoginResponse{
    userId: number;
    userFullName: string;
    userName: string;
    userType: number;
    userTypeName: UserType;
    token: string;
    expiresAt: string;
}