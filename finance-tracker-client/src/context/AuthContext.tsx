import React, { createContext, useState, useEffect, ReactNode } from 'react';
import { login as apiLogin, register as apiRegister } from '../api/auth';
import { UserLoginDto, UserRegisterDto, UserResponseDto } from '../types/auth';
import { AxiosError } from 'axios';

interface AuthContextType {
  user: UserResponseDto | null;
  loading: boolean;
  login: (data: UserLoginDto) => Promise<void>;
  register: (data: UserRegisterDto) => Promise<void>;
  logout: () => void;
}

interface AuthContextType {
  user: UserResponseDto | null;
  loading: boolean;
  login: (data: UserLoginDto) => Promise<void>;
  register: (data: UserRegisterDto) => Promise<void>;
  logout: () => void;
  setUser: React.Dispatch<React.SetStateAction<UserResponseDto | null>>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserResponseDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // При загрузке проверяем localStorage
    const storedUser = localStorage.getItem('user');
    const token = localStorage.getItem('token');
    if (storedUser && token) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const login = async (data: UserLoginDto) => {
    try {
      const response = await apiLogin(data);
      const { token, user } = response.data;
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
      setUser(user);
    } catch (error) {
      const err = error as AxiosError;
      throw err;
    }
  };

  const register = async (data: UserRegisterDto) => {
    try {
      const response = await apiRegister(data);
      const { token, user } = response.data;
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
      setUser(user);
    } catch (error) {
      const err = error as AxiosError;
      throw err;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, setUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};