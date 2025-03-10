using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.DAL.Repository
{
    public class GenericRepository<T> : _BaseRepository<T> where T : class
    {

    }
    //public class GenericRepository<T> : _BaseRepository<T> where T : class
    //{

    //    public GenericRepository() : base()
    //    {
    //    }

    //    //public GenericRepository(string connection) : base(connection) { }

    //}
}
