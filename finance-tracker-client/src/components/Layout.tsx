import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div>
      <nav className="navbar navbar-expand-lg navbar-light bg-light">
        <div className="container">
          <Link className="navbar-brand" to="/">FinanceTracker</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <Link className="nav-link" to="/">Дашборд</Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/accounts">Счета</Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/profile">Профиль</Link>
              </li>
              {user?.role === 'Admin' && (
                <>
                  <li className="nav-item">
                    <Link className="nav-link" to="/admin/users">Пользователи</Link>
                  </li>
                  <li className="nav-item">
                    <Link className="nav-link" to="/admin/accounts">Все счета</Link>
                  </li>
                  <li className="nav-item">
                    <Link className="nav-link" to="/admin/transactions">Все транзакции</Link>
                  </li>
                </>
              )}
            </ul>
            <ul className="navbar-nav">
              <li className="nav-item">
                <span className="navbar-text me-2">Привет, {user?.name}!</span>
              </li>
              <li className="nav-item">
                <button className="btn btn-outline-danger btn-sm" onClick={handleLogout}>Выйти</button>
              </li>
            </ul>
          </div>
        </div>
      </nav>
      <main className="container mt-4">
        {children}
      </main>
    </div>
  );
};

export default Layout;