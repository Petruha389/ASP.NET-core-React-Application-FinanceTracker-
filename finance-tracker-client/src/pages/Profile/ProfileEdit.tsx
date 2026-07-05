import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { updateUser, changePassword } from '../../api/users';
import { UserUpdateDto, ChangePasswordDto } from '../../types/auth';

const ProfileEdit: React.FC = () => {
  const { user, setUser } = useAuth(); // нужно добавить setUser в контекст
  const [profileData, setProfileData] = useState<UserUpdateDto>({
    name: '',
    email: '',
  });
  const [passwordData, setPasswordData] = useState<ChangePasswordDto>({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: '',
  });
  const [profileError, setProfileError] = useState('');
  const [profileSuccess, setProfileSuccess] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [passwordSuccess, setPasswordSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (user) {
      setProfileData({ name: user.name, email: user.email });
    }
  }, [user]);

  const handleProfileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setProfileData({ ...profileData, [e.target.name]: e.target.value });
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPasswordData({ ...passwordData, [e.target.name]: e.target.value });
  };

  const handleProfileSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setProfileError('');
    setProfileSuccess('');
    setLoading(true);
    try {
      await updateUser(user!.id, profileData);
      // Обновляем пользователя в контексте
      setUser({ ...user!, name: profileData.name, email: profileData.email });
      setProfileSuccess('Профиль обновлён');
    } catch (err: any) {
      setProfileError(err.response?.data?.message || 'Ошибка обновления');
    } finally {
      setLoading(false);
    }
  };

  const handlePasswordSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setPasswordError('');
    setPasswordSuccess('');
    setLoading(true);
    try {
      await changePassword(user!.id, passwordData);
      setPasswordSuccess('Пароль изменён');
      setPasswordData({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
    } catch (err: any) {
      setPasswordError(err.response?.data?.message || 'Ошибка смены пароля');
    } finally {
      setLoading(false);
    }
  };

  if (!user) return <div>Загрузка...</div>;

  return (
    <div className="container mt-4">
      <h1>Редактирование профиля</h1>
      <div className="row">
        <div className="col-md-6">
          <h3>Основные данные</h3>
          <form onSubmit={handleProfileSubmit}>
            <div className="mb-3">
              <label className="form-label">Имя</label>
              <input
                type="text"
                className="form-control"
                name="name"
                value={profileData.name}
                onChange={handleProfileChange}
                required
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Email</label>
              <input
                type="email"
                className="form-control"
                name="email"
                value={profileData.email}
                onChange={handleProfileChange}
                required
              />
            </div>
            {profileError && <div className="alert alert-danger">{profileError}</div>}
            {profileSuccess && <div className="alert alert-success">{profileSuccess}</div>}
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Сохранение...' : 'Сохранить'}
            </button>
          </form>
        </div>
        <div className="col-md-6">
          <h3>Смена пароля</h3>
          <form onSubmit={handlePasswordSubmit}>
            <div className="mb-3">
              <label className="form-label">Текущий пароль</label>
              <input
                type="password"
                className="form-control"
                name="currentPassword"
                value={passwordData.currentPassword}
                onChange={handlePasswordChange}
                required
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Новый пароль</label>
              <input
                type="password"
                className="form-control"
                name="newPassword"
                value={passwordData.newPassword}
                onChange={handlePasswordChange}
                required
                minLength={6}
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Подтверждение нового пароля</label>
              <input
                type="password"
                className="form-control"
                name="confirmNewPassword"
                value={passwordData.confirmNewPassword}
                onChange={handlePasswordChange}
                required
              />
            </div>
            {passwordError && <div className="alert alert-danger">{passwordError}</div>}
            {passwordSuccess && <div className="alert alert-success">{passwordSuccess}</div>}
            <button type="submit" className="btn btn-warning" disabled={loading}>
              {loading ? 'Смена...' : 'Сменить пароль'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ProfileEdit;