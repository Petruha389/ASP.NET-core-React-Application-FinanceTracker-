import React, { useEffect, useState } from 'react';
import { getAllTransactionsForAdmin } from '../../api/transactions';
import { getAllUsers } from '../../api/users';
import { getAllAccountsForAdmin } from '../../api/accounts';
import { TransactionResponseDto } from '../../types/transaction';
import { UserResponseDto } from '../../types/auth';
import { AccountResponseDto } from '../../types/account';
import { TransactionTypeLabels } from '../../types/enums';

const AdminTransactionList: React.FC = () => {
  const [transactions, setTransactions] = useState<TransactionResponseDto[]>([]);
  const [users, setUsers] = useState<UserResponseDto[]>([]);
  const [accounts, setAccounts] = useState<AccountResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Фильтры
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedAccountId, setSelectedAccountId] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [transactionsRes, usersRes, accountsRes] = await Promise.all([
          getAllTransactionsForAdmin(),
          getAllUsers(),
          getAllAccountsForAdmin(),
        ]);
        setTransactions(transactionsRes.data);
        setUsers(usersRes.data);
        setAccounts(accountsRes.data);
      } catch (err) {
        setError('Не удалось загрузить данные');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const applyFilters = async () => {
    setLoading(true);
    try {
      const params: any = {};
      if (selectedUserId) params.userId = selectedUserId;
      if (selectedAccountId) params.accountId = selectedAccountId;
      if (fromDate) params.from = new Date(fromDate).toISOString();
      if (toDate) params.to = new Date(toDate).toISOString();

      const res = await getAllTransactionsForAdmin(params);
      setTransactions(res.data);
    } catch (err) {
      setError('Ошибка фильтрации');
    } finally {
      setLoading(false);
    }
  };

  const resetFilters = () => {
    setSelectedUserId('');
    setSelectedAccountId('');
    setFromDate('');
    setToDate('');
    // повторно загрузить без фильтров
    applyFilters();
  };

  if (loading) return <div className="text-center mt-5">Загрузка...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div className="container mt-4">
      <h1>Все транзакции (администрирование)</h1>
      <div className="row mb-3">
        <div className="col-md-3">
          <label className="form-label">Пользователь</label>
          <select
            className="form-select"
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(e.target.value)}
          >
            <option value="">Все</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>{u.name} ({u.email})</option>
            ))}
          </select>
        </div>
        <div className="col-md-3">
          <label className="form-label">Счёт</label>
          <select
            className="form-select"
            value={selectedAccountId}
            onChange={(e) => setSelectedAccountId(e.target.value)}
          >
            <option value="">Все</option>
            {accounts.map((a) => (
              <option key={a.id} value={a.id}>{a.name} ({a.userName})</option>
            ))}
          </select>
        </div>
        <div className="col-md-2">
          <label className="form-label">С</label>
          <input
            type="date"
            className="form-control"
            value={fromDate}
            onChange={(e) => setFromDate(e.target.value)}
          />
        </div>
        <div className="col-md-2">
          <label className="form-label">По</label>
          <input
            type="date"
            className="form-control"
            value={toDate}
            onChange={(e) => setToDate(e.target.value)}
          />
        </div>
        <div className="col-md-2 d-flex align-items-end">
          <button className="btn btn-primary me-2" onClick={applyFilters}>Применить</button>
          <button className="btn btn-secondary" onClick={resetFilters}>Сбросить</button>
        </div>
      </div>

      <table className="table table-striped table-hover">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Пользователь</th>
            <th>Счёт</th>
            <th>Тип</th>
            <th>Сумма</th>
            <th>Описание</th>
          </tr>
        </thead>
        <tbody>
          {transactions.map((t) => (
            <tr key={t.id}>
              <td>{new Date(t.date).toLocaleString()}</td>
              <td>{t.userName || t.accountId}</td>
              <td>{t.accountName || t.accountId}</td>
              <td>{TransactionTypeLabels[t.type as keyof typeof TransactionTypeLabels] || t.type}</td>
              <td className={t.type === 'Income' ? 'text-success' : 'text-danger'}>
                {t.type === 'Income' ? '+' : '-'} {t.amount.toFixed(2)} ₽
              </td>
              <td>{t.description}</td>
            </tr>
          ))}
          {transactions.length === 0 && (
            <tr>
              <td colSpan={6} className="text-center">Нет транзакций</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default AdminTransactionList;