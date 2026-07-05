import api from './axios';
import { TransactionResponseDto, TransactionCreateDto, TransactionUpdateDto } from '../types/transaction';

export const getTransactionsByAccount = (accountId: string) =>
  api.get<TransactionResponseDto[]>(`/TransactionsApi/by-account/${accountId}`);

export const getTransactionById = (id: string) =>
  api.get<TransactionResponseDto>(`/TransactionsApi/${id}`);

export const createTransaction = (data: TransactionCreateDto) =>
  api.post<TransactionResponseDto>('/TransactionsApi', data);

export const updateTransaction = (id: string, data: TransactionUpdateDto) =>
  api.put<TransactionResponseDto>(`/TransactionsApi/${id}`, data);

export const getAllTransactionsForAdmin = (params?: {
  userId?: string;
  accountId?: string;
  from?: string;
  to?: string;
}) =>
  api.get<TransactionResponseDto[]>('/TransactionsApi/admin/all', { params });

export const deleteTransaction = (id: string) =>
  api.delete<void>(`/TransactionsApi/${id}`);
