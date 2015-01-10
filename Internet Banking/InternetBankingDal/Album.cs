
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
    
public partial class Album
{

    public Album()
    {

        this.Carts = new HashSet<Cart>();

        this.OrderDetails = new HashSet<OrderDetail>();

    }


    public int AlbumId { get; set; }

    public int GenreId { get; set; }

    public int ArtistId { get; set; }

    public string Title { get; set; }

    public decimal Price { get; set; }

    public string AlbumArtUrl { get; set; }



    public virtual Artist Artist { get; set; }

    public virtual Genre Genre { get; set; }

    public virtual ICollection<Cart> Carts { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }

}

}