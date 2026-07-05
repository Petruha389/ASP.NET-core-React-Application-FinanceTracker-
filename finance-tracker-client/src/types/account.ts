export interface AccountResponseDto {
  id: string;
  name: string;
  type: string;
  balance: number;
  userId: string;
  createdAt: string;
  userName?: string;
}

export interface AccountCreateDto {
  name: string;
  type: string;
}

export interface AccountUpdateDto {
  name: string;
  type: string;
}

export interface AccountOperationDto {
  amount: number;
  description?: string;
}