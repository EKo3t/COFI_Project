
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
    
public partial class Payments
{

    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Name { get; set; }

    public Nullable<int> VendorsId { get; set; }

    public int DefaultCommissionId { get; set; }

}

}
