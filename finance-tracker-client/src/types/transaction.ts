export interface TransactionResponseDto {
  id: string;
  amount: number;
  type: string;
  description: string;
  date: string;
  accountId: string;
  accountName?: string;
  userName?: string; // добавлено
}

export interface TransactionCreateDto {
  amount: number;
  type: 'Income' | 'Expense';
  description: string;
  accountId: string;
}

export interface TransactionUpdateDto {
  description: string;
}