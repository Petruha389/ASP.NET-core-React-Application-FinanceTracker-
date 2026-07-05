import api from './axios';
import { AccountResponseDto, AccountCreateDto, AccountUpdateDto, AccountOperationDto } from '../types/account';

export const getAllAccounts = () =>
  api.get<AccountResponseDto[]>('/AccountsApi');

export const getAccountById = (id: string) =>
  api.get<AccountResponseDto>(`/AccountsApi/${id}`);

export const getAllAccountsForAdmin = () =>
  api.get<AccountResponseDto[]>('/AccountsApi/admin/all');

export const createAccount = (data: AccountCreateDto) =>
  api.post<AccountResponseDto>('/AccountsApi', data);

export const updateAccount = (id: string, data: AccountUpdateDto) =>
  api.put<AccountResponseDto>(`/AccountsApi/${id}`, data);

export const deleteAccount = (id: string) =>
  api.delete<void>(`/AccountsApi/${id}`);

export const editAccount = (id: string, data: AccountUpdateDto) =>
  api.put<AccountResponseDto>(`/AccountsApi/${id}`, data);

export const deposit = (id: string, data: AccountOperationDto) =>
  api.post<AccountResponseDto>(`/AccountsApi/${id}/deposit`, data);

export const withdraw = (id: string, data: AccountOperationDto) =>
  api.post<AccountResponseDto>(`/AccountsApi/${id}/withdraw`, data);