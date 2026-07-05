import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getTransactionsByAccount, deleteTransaction } from '../../api/transactions';
import { getAccountById } from '../../api/accounts';
import { TransactionResponseDto } from '../../types/transaction';
import { TransactionTypeLabels } from '../../types/enums';

const TransactionList: React.FC = () => {
  const { accountId } = useParams<{ accountId: string }>();
  const navigate = useNavigate();
  const [transactions, setTransactions] = useState<TransactionResponseDto[]>([]);
  const [accountName, setAccountName] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!accountId) return;
    const fetchData = async () => {
      try {
        const [transactionsRes, accountRes] = await Promise.all([
          getTransactionsByAccount(accountId),
          getAccountById(accountId),
        ]);
        setTransactions(transactionsRes.data);
        setAccountName(accountRes.data.name);
      } catch (err) {
        setError('Не удалось загрузить транзакции');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [accountId]);

  const handleDelete = async (id: string) => {
    if (!confirm('Удалить транзакцию?')) return;
    try {
      await deleteTransaction(id);
      setTransactions(transactions.filter((t) => t.id !== id));
    } catch (err) {
      alert('Ошибка удаления');
    }
  };

  if (loading) return <div className="text-center mt-5">Загрузка...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div className="container mt-4">
      <h1>Транзакции по счёту: {accountName}</h1>
      <div className="mb-3">
        {/* <Link to={`/transactions/create?accountId=${accountId}`} className="btn btn-primary">
          Добавить транзакцию
        </Link> */}
        <Link to="/accounts" className="btn btn-secondary ms-2">Назад к счетам</Link>
      </div>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Тип</th>
            <th>Сумма</th>
            <th>Описание</th>
            {/* <th>Действия</th> */}
          </tr>
        </thead>
        <tbody>
          {transactions.map((t) => (
            <tr key={t.id}>
              <td>{new Date(t.date).toLocaleDateString()}</td>
              <td>{TransactionTypeLabels[t.type as keyof typeof TransactionTypeLabels] || t.type}</td>
              <td className={t.type === 'Income' ? 'text-success' : 'text-danger'}>
                {t.type === 'Income' ? '+' : '-'} {t.amount} ₽
              </td>
              <td>{t.description}</td>
              {/* <td>
                <button className="btn btn-sm btn-danger" onClick={() => handleDelete(t.id)}>
                  Удалить
                </button>
              </td> */}
            </tr>
          ))}
          {transactions.length === 0 && (
            <tr>
              <td colSpan={5} className="text-center">Нет транзакций</td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default TransactionList;