//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Lib.Dal.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArticleType
    {
        public ArticleType()
        {
            this.StaticPages = new HashSet<StaticPage>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
    
        public virtual ICollection<StaticPage> StaticPages { get; set; }
    }
}
