import React, { useEffect, useState } from 'react';
import { getAllAccounts, deleteAccount } from '../../api/accounts';
import { AccountResponseDto } from '../../types/account';
import { Link } from 'react-router-dom';

const AccountList: React.FC = () => {
  const [accounts, setAccounts] = useState<AccountResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const res = await getAllAccounts();
        setAccounts(res.data);
      } catch (err) {
        setError('Не удалось загрузить счета');
      } finally {
        setLoading(false);
      }
    };
    fetchAccounts();
  }, []);

  const handleDelete = async (id: string) => {
    if (!confirm('Удалить счёт?')) return;
    try {
      await deleteAccount(id);
      setAccounts(accounts.filter(a => a.id !== id));
    } catch (err) {
      alert('Ошибка удаления');
    }
  };

  if (loading) return <div>Загрузка...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div>
      <h1>Мои счета</h1>
      <Link to="/accounts/create" className="btn btn-primary mb-3">Создать счёт</Link>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Название</th>
            <th>Тип</th>
            <th>Баланс</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          {accounts.map(acc => (
            <tr key={acc.id}>
              <td>{acc.name}</td>
              <td>{acc.type}</td>
              <td>{acc.balance} ₽</td>
              <td>
                <Link to={`/accounts/${acc.id}/deposit`} className="btn btn-sm btn-success me-1">Пополнить</Link>
                <Link to={`/accounts/${acc.id}/withdraw`} className="btn btn-sm btn-warning me-1">Снять</Link>
                <Link to={`/accounts/${acc.id}/edit`} className="btn btn-sm btn-primary me-1">Изменить</Link>
                <Link to={`/transactions/${acc.id}`} className="btn btn-sm btn-info me-1">Транзакции</Link>
                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(acc.id)}>Удалить</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AccountList;