
//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------


namespace InternetBankingDal
{

using System;
    using System.Collections.Generic;
    
public partial class CardOperations
{

    public System.Guid CardOperationId { get; set; }

    public System.Guid CardId { get; set; }

    public int Type { get; set; }

    public System.DateTime OperationDate { get; set; }

    public string Description { get; set; }

    public decimal Amount { get; set; }

    public int Currency { get; set; }

    public decimal AmountAccountCurrency { get; set; }

    public decimal StartBalance { get; set; }



    public virtual Cards Cards { get; set; }

}

}
