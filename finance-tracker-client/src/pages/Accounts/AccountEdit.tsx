import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getAccountById, updateAccount } from '../../api/accounts';
import { AccountUpdateDto } from '../../types/account';
import { AccountType, AccountTypeLabels, AccountTypeValues } from '../../types/enums';

const AccountEdit: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [formData, setFormData] = useState<AccountUpdateDto>({
    name: '',
    type: 'Cash' as AccountType,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const fetchAccount = async () => {
      try {
        const res = await getAccountById(id!);
        setFormData({
          name: res.data.name,
          type: res.data.type as AccountType,
        });
      } catch (err) {
        setError('Не удалось загрузить счёт');
      } finally {
        setLoading(false);
      }
    };
    fetchAccount();
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'type' ? (value as AccountType) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      await updateAccount(id!, formData);
      navigate('/accounts');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Ошибка обновления');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div>Загрузка...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div className="container mt-4">
      <h1>Редактировать счёт</h1>
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
          <select
            className="form-select"
            name="type"
            value={formData.type}
            onChange={handleChange}
            required
          >
            {AccountTypeValues.map((typeKey) => (
              <option key={typeKey} value={typeKey}>
                {AccountTypeLabels[typeKey]}
              </option>
            ))}
          </select>
        </div>
        {error && <div className="alert alert-danger">{error}</div>}
        <button type="submit" className="btn btn-primary" disabled={saving}>
          {saving ? 'Сохранение...' : 'Сохранить'}
        </button>
      </form>
    </div>
  );
};

export default AccountEdit;