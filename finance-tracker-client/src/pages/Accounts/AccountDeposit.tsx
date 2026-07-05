import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { deposit, getAccountById } from '../../api/accounts';
import { AccountOperationDto } from '../../types/account';

const AccountDeposit: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [amount, setAmount] = useState<string>(''); // строка, не число
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [accountName, setAccountName] = useState('');

  useEffect(() => {
    const fetchAccount = async () => {
      if (!id) return;
      try {
        const response = await getAccountById(id);
        setAccountName(response.data.name);
      } catch (err) {
        setError('Не удалось загрузить счёт');
      }
    };
    fetchAccount();
  }, [id]);

  // Фильтр ввода: только цифры и одна точка
  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    // Разрешаем: цифры, точку (только одну), пустую строку
    const regex = /^\d*\.?\d*$/;
    if (value === '' || regex.test(value)) {
      setAmount(value);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const numericAmount = parseFloat(amount);
    if (!amount || numericAmount <= 0) {
      setError('Введите положительную сумму');
      return;
    }
    setError('');
    setLoading(true);
    try {
      const data: AccountOperationDto = { amount: numericAmount, description };
      await deposit(id!, data);
      navigate('/accounts');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Ошибка пополнения');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <h1>Пополнение счёта</h1>
      <p>Счёт: <strong>{accountName || id}</strong></p>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Сумма</label>
          <input
            type="text"
            className="form-control"
            placeholder="Введите сумму"
            value={amount}
            onChange={handleAmountChange}
            required
          />
        </div>
        <div className="mb-3">
          <label className="form-label">Описание (необязательно)</label>
          <input
            type="text"
            className="form-control"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
        {error && <div className="alert alert-danger">{error}</div>}
        <button type="submit" className="btn btn-success" disabled={loading}>
          {loading ? 'Пополнение...' : 'Пополнить'}
        </button>
        <button type="button" className="btn btn-secondary ms-2" onClick={() => navigate('/accounts')}>
          Отмена
        </button>
      </form>
    </div>
  );
};

export default AccountDeposit;