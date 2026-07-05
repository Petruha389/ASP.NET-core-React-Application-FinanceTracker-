import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { getAllAccounts } from '../api/accounts';
import { AccountResponseDto } from '../types/account';

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [accounts, setAccounts] = useState<AccountResponseDto[]>([]);
  const [totalBalance, setTotalBalance] = useState(0);

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const response = await getAllAccounts();
        setAccounts(response.data);
        const sum = response.data.reduce((acc, a) => acc + a.balance, 0);
        setTotalBalance(sum);
      } catch (error) {
        console.error('Error fetching accounts', error);
      }
    };
    fetchAccounts();
  }, []);

  return (
    <div>
      <h1>Добро пожаловать, {user?.name}!</h1>
      <div className="row mt-4">
        <div className="col-md-4">
          <div className="card text-white bg-primary mb-3">
            <div className="card-body">
              <h5 className="card-title">Общий баланс</h5>
              <p className="card-text display-6">{totalBalance.toFixed(2)} ₽</p>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card text-white bg-success mb-3">
            <div className="card-body">
              <h5 className="card-title">Количество счетов</h5>
              <p className="card-text display-6">{accounts.length}</p>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card text-white bg-info mb-3">
            <div className="card-body">
              <h5 className="card-title">Роль</h5>
              <p className="card-text display-6">{user?.role}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;