using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RNF_Web.Models
{
    public class GlobalUtils
    {
        db_RNFEntities db = new db_RNFEntities();
        public string InitCap(string texto)
        {
            return db.Database.SqlQuery<string>("SELECT [dbo].[InitCap](@p0)", texto).FirstOrDefault();
        }
    }
}