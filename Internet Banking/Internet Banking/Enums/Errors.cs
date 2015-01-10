using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Internet_Banking.Enums
{
    public class Errors
    {
        string defaultError = "Произошла неизвестная ошибка";
        string wrongData = "Введены неправильные данные";
        string moneyDeficit = "На карте недостаточно средств для проведения данной операции";
        string notAppropriateCurrencyAccount = "Услуга доступна только при наличии рублевого текущего счёта или карт-счета";
    }
}