import api from './axios';
import { UserLoginDto, UserRegisterDto, AuthResponse } from '../types/auth';

export const login = (data: UserLoginDto) =>
  api.post<AuthResponse>('/AuthApi/login', data);

export const register = (data: UserRegisterDto) =>
  api.post<AuthResponse>('/AuthApi/register', data);

export const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
};