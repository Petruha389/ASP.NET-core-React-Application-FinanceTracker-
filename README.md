# 💰 FinanceTracker

**FinanceTracker** — это веб-приложение для управления личными финансами.  
Позволяет вести учёт счетов, записывать доходы и расходы, отслеживать баланс и анализировать транзакции.  
Проект реализован на **ASP.NET Core** (бэкенд) и **React** (фронтенд) с использованием **PostgreSQL**.

---

## 🚀 Основные возможности

- 🔐 **Аутентификация и авторизация** (JWT + Cookie, роли User/Admin)
- 📊 **Дашборд** с общим балансом, количеством счетов и последними транзакциями
- 💳 **Управление счетами** (CRUD, пополнение, снятие)
- 📝 **История транзакций** по каждому счёту с фильтрацией
- 👥 **Администрирование** (управление пользователями, просмотр всех счетов и транзакций)
- 📱 **API** для фронтенда и мобильных клиентов
- 🧪 **Интеграционные тесты** (в процессе)

---

## 🛠️ Технологии

### Бэкенд
- ASP.NET Core 8
- Entity Framework Core (PostgreSQL)
- JWT + Cookie Authentication
- BCrypt для хеширования паролей
- Serilog для логирования
- Swagger/OpenAPI

### Фронтенд
- React 18 + TypeScript
- React Router v6
- Axios
- Bootstrap 5

### База данных
- PostgreSQL 15

### Инструменты
- Visual Studio 2022 / VS Code
- Git & GitHub
- Docker (планируется)

---

## 📦 Установка и запуск

### Требования
- .NET 8 SDK
- Node.js (v18+)
- PostgreSQL (локально или через Docker)

### Клонирование репозитория
```bash
git clone https://github.com/ваш-username/FinanceTracker.git
cd FinanceTracker
```
### Запуск бэкенда
```bash
cd FinanceTracker.API
dotnet run
```

### Миграции
``` powershell
dotnet ef database update --project FinanceTracker.Infrastructure --startup-project FinanceTracker.API
```

### Запуск фронтенда (React)
```bash
cd finance-tracker-client
npm install
npm run dev
```


### 📬 Контакты
Автор: Артём Русланович Петрушка

Email: petruskaartem389@gmail.com

Telegram: @ABO_S14

GitHub: https://github.com/Petruha389/
