
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
    
public partial class aspnet_Paths
{

    public aspnet_Paths()
    {

        this.aspnet_PersonalizationPerUser = new HashSet<aspnet_PersonalizationPerUser>();

    }


    public System.Guid ApplicationId { get; set; }

    public System.Guid PathId { get; set; }

    public string Path { get; set; }

    public string LoweredPath { get; set; }



    public virtual aspnet_Applications aspnet_Applications { get; set; }

    public virtual aspnet_PersonalizationAllUsers aspnet_PersonalizationAllUsers { get; set; }

    public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }

}

}
