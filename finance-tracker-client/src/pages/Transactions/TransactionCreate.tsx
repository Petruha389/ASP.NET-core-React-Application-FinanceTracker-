import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { createTransaction } from '../../api/transactions';
import { TransactionCreateDto } from '../../types/transaction';

const TransactionCreate: React.FC = () => {
  const { accountId } = useParams<{ accountId: string }>();
  const navigate = useNavigate();
  const [formData, setFormData] = useState<TransactionCreateDto>({
    amount: 0,
    type: 'Income',
    description: '',
    accountId: accountId || '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: name === 'amount' ? parseFloat(value) : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.amount <= 0) {
      setError('Сумма должна быть больше нуля');
      return;
    }
    setLoading(true);
    setError('');
    try {
      await createTransaction(formData);
      navigate(`/transactions/${accountId}`);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Ошибка создания транзакции');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <h1>Создать транзакцию</h1>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Тип</label>
          <select className="form-select" name="type" value={formData.type} onChange={handleChange}>
            <option value="Income">Доход</option>
            <option value="Expense">Расход</option>
          </select>
        </div>
        <div className="mb-3">
          <label className="form-label">Сумма</label>
          <input
            type="number"
            step="0.01"
            className="form-control"
            name="amount"
            value={formData.amount}
            onChange={handleChange}
            required
          />
        </div>
        <div className="mb-3">
          <label className="form-label">Описание</label>
          <input
            type="text"
            className="form-control"
            name="description"
            value={formData.description}
            onChange={handleChange}
            required
          />
        </div>
        {error && <div className="alert alert-danger">{error}</div>}
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Создание...' : 'Создать'}
        </button>
      </form>
    </div>
  );
};

export default TransactionCreate;