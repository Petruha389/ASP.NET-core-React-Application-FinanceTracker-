import api from './axios';
import { UserResponseDto, UserCreateDto, UserUpdateDto, ChangePasswordDto } from '../types/auth';

export const getAllUsers = () =>
  api.get<UserResponseDto[]>('/UsersApi');

export const getUserById = (id: string) =>
  api.get<UserResponseDto>(`/UsersApi/${id}`);

export const createUser = (data: UserCreateDto) =>
  api.post<UserResponseDto>('/UsersApi', data);

export const updateUser = (id: string, data: UserUpdateDto) =>
  api.put<UserResponseDto>(`/UsersApi/${id}`, data);

export const deleteUser = (id: string) =>
  api.delete<void>(`/UsersApi/${id}`);

export const changePassword = (id: string, data: ChangePasswordDto) =>
  api.post<void>(`/UsersApi/${id}/change-password`, data);