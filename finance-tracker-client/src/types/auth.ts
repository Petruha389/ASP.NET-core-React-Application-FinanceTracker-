// ============ DTO для аутентификации ============

export interface UserLoginDto {
  email: string;
  password: string;
}

export interface UserRegisterDto {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

// ============ DTO для управления пользователями (CRUD) ============

export interface UserCreateDto {
  name: string;
  email: string;
  password: string;
  // role не включаем – назначается автоматически (User)
}

export interface UserUpdateDto {
  name: string;
  email: string;
  password?: string;       // опционально – если не указан, не меняем
  role?: 'User' | 'Admin'; // только администратор может менять роль
}

// ============ DTO для ответов от сервера ============

export interface UserResponseDto {
  id: string;
  name: string;
  email: string;
  role: 'User' | 'Admin';
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: UserResponseDto;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
