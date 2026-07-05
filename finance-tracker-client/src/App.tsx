import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import PrivateRoute from './components/routing/PrivateRoute';
import Layout from './components/Layout';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import Dashboard from './pages/Dashboard';
import AccountList from './pages/Accounts/AccountList';
import UsersList from './pages/Admin/UsersList';
import AccountCreate from './pages/Accounts/AccountCreate';
import AccountDeposit from './pages/Accounts/AccountDeposit';
import AccountWithdraw from './pages/Accounts/AccountWithdraw';
import AccountEdit from './pages/Accounts/AccountEdit';
import TransactionList from './pages/Transactions/TransactionList';
import ProfileEdit from './pages/Profile/ProfileEdit';
import AdminAccountList from './pages/Admin/AdminAccountList';
import AdminTransactionList from './pages/Admin/AdminTransactionList';


const App: React.FC = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/"
            element={
              <PrivateRoute>
                <Layout>
                  <Dashboard/>
                </Layout>
              </PrivateRoute>
            }
          />
          <Route
            path="/accounts"
            element={
              <PrivateRoute>
                <Layout>
                  <AccountList />
                </Layout>
              </PrivateRoute>
            }
          />
          <Route
            path="/admin/users"
            element={
              <PrivateRoute roles={['Admin']}>
                <Layout>
                  <UsersList />
                </Layout>
              </PrivateRoute>
            }
          />
          <Route
            path="/accounts/create"
            element={
              <PrivateRoute>
                <Layout>
                <AccountCreate />
              </Layout>
            </PrivateRoute>
            }
          />
          <Route
            path="/accounts/:id/deposit"
            element={
            <PrivateRoute>
              <Layout>
                <AccountDeposit />
              </Layout>
          </PrivateRoute>
        }
        />
        <Route
          path="/accounts/:id/withdraw"
          element={
          <PrivateRoute>
            <Layout>
              <AccountWithdraw/>
            </Layout>
        </PrivateRoute>
        }
        />
        <Route
          path="/accounts/:id/edit/"
          element={
          <PrivateRoute>
            <Layout>
              <AccountEdit/>
            </Layout>
        </PrivateRoute>
        }
        />
        <Route
          path="/transactions/:accountId"
          element={
          <PrivateRoute>
            <Layout>
              <TransactionList/>
            </Layout>
        </PrivateRoute>
        }
        />
        <Route
          path="/profile/edit"
          element={
          <PrivateRoute>
            <Layout>
              <ProfileEdit/>
            </Layout>
        </PrivateRoute>
        }
        />
        <Route
          path="/profile"
          element={
          <PrivateRoute>
            <Layout>
              <ProfileEdit/>
            </Layout>
        </PrivateRoute>
        }
        />
        <Route
          path="/admin/accounts"
          element={
          <PrivateRoute roles={['Admin']}>
            <Layout>
              <AdminAccountList/>
            </Layout>
          </PrivateRoute>
          }
        />
        <Route
          path="/admin/transactions"
          element={
          <PrivateRoute roles={['Admin']}>
            <Layout>
              <AdminTransactionList />
            </Layout>
          </PrivateRoute>
          }
        />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
};

export default App;