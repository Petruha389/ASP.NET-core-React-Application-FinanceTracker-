import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllAccountsForAdmin } from '../../api/accounts';
import { AccountResponseDto } from '../../types/account';

const AdminAccountList: React.FC = () => {
  const [accounts, setAccounts] = useState<AccountResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const response = await getAllAccountsForAdmin();
        setAccounts(response.data);
      } catch (err) {
        setError('Не удалось загрузить счета');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchAccounts();
  }, []);

  if (loading) return <div className="text-center mt-5">Загрузка...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div className="container mt-4">
      <h1>Все счета (администрирование)</h1>
      <p className="text-muted">Список всех счетов всех пользователей</p>
      <table className="table table-striped table-hover">
        <thead>
          <tr>
            <th>Название</th>
            <th>Тип</th>
            <th>Баланс</th>
            <th>Владелец</th>
            <th>Дата создания</th>
          </tr>
        </thead>
        <tbody>
          {accounts.map((acc) => (
            <tr key={acc.id}>
              <td>{acc.name}</td>
              <td>{acc.type}</td>
              <td>{acc.balance.toFixed(2)} ₽</td>
              <td>{acc.userName || acc.userId}</td>
              <td>{new Date(acc.createdAt).toLocaleDateString()}</td>
              
            </tr>
          ))}
          {accounts.length === 0 && (
            <tr>
              <td colSpan={6} className="text-center">Нет счетов</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default AdminAccountList;