using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Domain.Entities.Enums
{
        public enum UserRole
        {
            User = 1,
            Admin = 2
        }

    public enum Currency
        {
            Rub = 1,
            Usd = 2,
            Eur = 3
        }
    public enum AccountType
        {
        /// <summary>Наличные средства (не привязаны к конкретному счёту).</summary>
        Cash,

        /// <summary>Банковская карта (дебетовая/кредитная).</summary>
        Card,

        /// <summary>Накопительный счёт (обычно с процентной ставкой).</summary>
        Savings,

        /// <summary>Депозит (вклад с фиксированным сроком и ставкой).</summary>
        Deposit,

        /// <summary>Кредитный счёт (для учёта задолженности).</summary>
        Credit,

        /// <summary>Расчётный (текущий) счёт для повседневных операций.</summary>
        Current,

        /// <summary>Инвестиционный счёт (для торговли ценными бумагами).</summary>
        Investment,

        /// <summary>Валютный счёт (счета в иностранной валюте).</summary>
        ForeignCurrency
    }

    public enum TransactionType
    {
        Income = 1,
        Expense = 2,
        Transfer = 3
    }
}
