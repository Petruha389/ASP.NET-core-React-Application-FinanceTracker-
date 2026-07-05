import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createAccount } from '../../api/accounts';
import { AccountCreateDto } from '../../types/account';
import { AccountType, AccountTypeLabels, AccountTypeValues } from '../../types/enums';

const AccountCreate: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<AccountCreateDto>({
    name: '',
    type: 'Cash'
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await createAccount(formData);
      navigate('/accounts');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Ошибка создания счёта');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <h1>Создать счёт</h1>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Название</label>
          <input
            type="text"
            className="form-control"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>
        <div className="mb-3">
          <label className="form-label">Тип</label>
          <select className="form-select" name="type" value={formData.type} onChange={handleChange}>
            {AccountTypeValues.map((typeKey) => (
              <option key={typeKey} value={typeKey}>
                {AccountTypeLabels[typeKey]}
              </option>
            ))}
          </select>
        </div>
        {error && <div className="alert alert-danger">{error}</div>}
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Создание...' : 'Создать'}
        </button>
      </form>
    </div>
  );
};

export default AccountCreate;