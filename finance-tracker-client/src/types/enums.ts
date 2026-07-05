// ============================================================
//  Типы (строковые литералы)
// ============================================================

export type UserRole = 'User' | 'Admin';
export type Currency = 'Rub' | 'Usd' | 'Eur';
export type AccountType =
  | 'Cash'
  | 'Card'
  | 'Savings'
  | 'Deposit'
  | 'Credit'
  | 'Current'
  | 'Investment'
  | 'ForeignCurrency';
export type TransactionType = 'Income' | 'Expense' | 'Transfer';

// ============================================================
//  Человекочитаемые названия (для отображения в UI)
// ============================================================

export const UserRoleLabels: Record<UserRole, string> = {
  User: 'Пользователь',
  Admin: 'Администратор',
};

export const CurrencyLabels: Record<Currency, string> = {
  Rub: 'Рубль (RUB)',
  Usd: 'Доллар (USD)',
  Eur: 'Евро (EUR)',
};

export const AccountTypeLabels: Record<AccountType, string> = {
  Cash: 'Наличные',
  Card: 'Банковская карта',
  Savings: 'Накопительный',
  Deposit: 'Депозит',
  Credit: 'Кредитный',
  Current: 'Расчётный',
  Investment: 'Инвестиционный',
  ForeignCurrency: 'Валютный',
};

export const TransactionTypeLabels: Record<TransactionType, string> = {
  Income: 'Доход',
  Expense: 'Расход',
  Transfer: 'Перевод',
};

// ============================================================
//  Массивы всех значений (для `<select>` и других переборов)
// ============================================================

export const UserRoleValues = Object.keys(UserRoleLabels) as UserRole[];
export const CurrencyValues = Object.keys(CurrencyLabels) as Currency[];
export const AccountTypeValues = Object.keys(AccountTypeLabels) as AccountType[];
export const TransactionTypeValues = Object.keys(TransactionTypeLabels) as TransactionType[];

// ============================================================
//  (Опционально) Маппинг строкового значения → число, если бэкенд ожидает числа
//  Внимание: порядок значений должен совпадать с бэкенд-enum!
// ============================================================

export const AccountTypeNumeric: Record<AccountType, number> = {
  Cash: 0,
  Card: 1,
  Savings: 2,
  Deposit: 3,
  Credit: 4,
  Current: 5,
  Investment: 6,
  ForeignCurrency: 7,
};

export const TransactionTypeNumeric: Record<TransactionType, number> = {
  Income: 1,
  Expense: 2,
  Transfer: 3,
};

export const UserRoleNumeric: Record<UserRole, number> = {
  User: 1,
  Admin: 2,
};

export const CurrencyNumeric: Record<Currency, number> = {
  Rub: 1,
  Usd: 2,
  Eur: 3,
};