
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
    
public partial class Cards
{

    public Cards()
    {

        this.CardOperations = new HashSet<CardOperations>();

    }


    public System.Guid CardId { get; set; }

    public System.Guid AccountId { get; set; }

    public string Name { get; set; }

    public string Number { get; set; }

    public string UserSignature { get; set; }

    public System.DateTime StartDate { get; set; }

    public System.DateTime EndDate { get; set; }

    public int State { get; set; }



    public virtual Accounts Accounts { get; set; }

    public virtual ICollection<CardOperations> CardOperations { get; set; }

}

}