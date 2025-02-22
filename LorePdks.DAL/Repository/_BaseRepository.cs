using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
//using DapperExtensions;
using System.Transactions;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;
//using SQLinq;
//using OYS.DAL.extensions;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Serilog;
using LorePdks.COMMON.Configuration;

namespace LorePdks.DAL.Repository
{
    public abstract class _BaseRepository<T> where T : class 
    {

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_constr);
            }
        }



        static Type type;
        //static string _schema;
        static string _tableName;


        static _BaseRepository()
        {
            var a = new SqlServerDialect();
            DapperExtensions.DapperExtensions.SqlDialect = new SqlServerDialect();
            type = typeof(T);
            //_schema = typeof(T).GetMethod("GetSchema").Invoke(null, null).ToString();
            _tableName = type.CustomAttributes.Where(x => x.AttributeType.Name == "TableAttribute").FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString();
        }

        private string _constr = CoreConfig.ConnectionString;
        //private string _idField = CoreConfig.IDProperty;
        private string _createdDateField = CoreConfig.CreatedDateProperty;
        private string _createdUserField = CoreConfig.CreatedUserProperty;
        private string _modifiedDateField = CoreConfig.ModifiedDateProperty;
        private string _modifiedUserField = CoreConfig.ModifiedUserProperty;
        private string _isDeletedField = CoreConfig.IsDeletedProperty;
        private string _createdIpField = CoreConfig.CreatedIpAdressProperty;
        private string _modifiedIpField = CoreConfig.ModifiedIpAdressProperty;

        private string _ipAddress;
        private long _kisiId;
        private string _loginName;

        private string _makinaField = "Makina";
        private string _sgZaman = "SGZaman";
        private string _sgKullanici = "SGKullanici";
        private string _zaman = "Zaman";
        private string _kullanici = "Kullanici";

        private string _idField
        {
            get
            {
                var propInfo = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(KeyAttribute))).FirstOrDefault();
                if (propInfo == null)
                {
                    return CoreConfig.IDProperty;
                }

                return propInfo.Name.ToString();
            }
        }

        /// <summary>
        /// buradaki dataları commondan alabilimek gerek bunları commonda httprequest içerisine her daim yazıp dönen bir tekil metod olsun
        /// </summary>
        public _BaseRepository()
        {
            this._ipAddress = "";
            this._kisiId = 0;
            this._loginName="";
        }

        public _BaseRepository(string connection)
        {

            _constr = connection;
            //this.Connection = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connection].ConnectionString);
        }

        //  public _BaseRepository(UserTokenDTO contx)
        //  {
        //      this._userTokenDto = contx;
        //      this._ipAddress = _userTokenDto.ip;
        //      this._kisiId = _userTokenDto.userDto.id;
        //      this._loginName = _userTokenDto.loginName;
        //  }

        public T Get(long ID)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "";
                if (isPropertyVar(_isDeletedField))
                {
                    // query = String.Format("select * from {0} where ({1} = 0)", _tableName, _isDeletedField);
                    query = String.Format("select * from {0} (nolock) where {1} = @ID and ({2} = 0)", _tableName, _idField, _isDeletedField);
                }
                else
                {

                    query = String.Format("select * from {0} (nolock) where {1} = @ID", _tableName, _idField);
                }

                var data = con.Query<T>(query, new { ID = ID });
                if (data.Count() == 0)
                    return null;

                return data.First();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Get(string whereQuery, object param = null, OrderOption orderOption = OrderOption.asc, params Enum[] orderProp)
        {
            if (String.IsNullOrEmpty(whereQuery))
                whereQuery = "1 = 1";

            var orderList = new List<string>();
            if (orderProp.Length == 0)
                orderList.Add(_idField);
            for (int i = 0; i < orderProp.Length; i++)
                orderList.Add(Enum.GetName(orderProp[i].GetType(), orderProp[i]));


            var query = "";
            if (isPropertyVar(_isDeletedField))
            {
                query = String.Format("select * from {0} (nolock) where ({1} = 0) and ({2})", _tableName, _isDeletedField, whereQuery);
            }
            else
            {
                query = String.Format("select * from  {0} (nolock) where ({1})", _tableName, whereQuery);
            }



            //var query = String.Format("select * from {0} where ({1} = 0) and ({2})", type.Name, _isDeletedField, whereQuery);

            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var result = con.Query<T>(query, param);

                if (result.Count() == 0)
                {
                    con.Close();
                    con.Dispose();
                    return null;
                }

                var data = result.First();
                con.Close();
                con.Dispose();
                return data;
            }
        }


        public List<T> GetList()
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "";

                if (isPropertyVar(_isDeletedField))
                {
                    query = String.Format("select * from {0} (nolock) where ({1} = 0)", _tableName, _isDeletedField);
                }
                else
                {
                    query = String.Format("select * from {0} (nolock)", _tableName);
                }


                var data = con.Query<T>(query, null).ToList();

                return data;
            }
        }
        public List<T> GetListWithPagination(int pageNumber, int itemsPerPage)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "";

                if (isPropertyVar(_isDeletedField))
                {
                    // query = String.Format("select * from {0} where ({1} = 0)", _tableName, _isDeletedField);
                    //  query = String.Format("select * from {0} where {1} = @ID and ({2} = 0)", _tableName, _idField, _isDeletedField);
                    query = String.Format("select * from {0} (nolock) where ({3} = 0) ORDER BY ID OFFSET ({1}-1)*{2} ROWS FETCH NEXT {2} ROWS ONLY", _tableName, pageNumber, itemsPerPage, _isDeletedField);

                }
                else
                {
                    query = String.Format("select * from {0} (nolock) ORDER BY ID OFFSET ({1}-1)*{2} ROWS FETCH NEXT {2} ROWS ONLY", _tableName, pageNumber, itemsPerPage);
                }


                var data = con.Query<T>(query, null);

                return data.ToList();
            }
        }
        public int Count()
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                string query = "";

                if (isPropertyVar(_isDeletedField))
                {
                    query = String.Format("select count(*) from {0} (nolock) where ({1} = 0)", _tableName, _isDeletedField);
                }
                else
                {
                    query = String.Format("select count(*) from {0} (nolock)", _tableName);
                }

                var data = con.Query<int>(query, null).FirstOrDefault();

                return data;
            }
        }


        public List<T> GetList(string whereQuery, object param = null, OrderOption orderOption = OrderOption.asc, params Enum[] orderProp)
        {
            if (String.IsNullOrEmpty(whereQuery))
                whereQuery = "1 = 1";
            var orderList = new List<string>();
            if (orderProp.Length == 0)
                orderList.Add(_idField);
            for (int i = 0; i < orderProp.Length; i++)
                orderList.Add(Enum.GetName(orderProp[i].GetType(), orderProp[i]));


            var query = "";
            if (isPropertyVar(_isDeletedField))
            {
                query = String.Format("select * from {0} (nolock) where {1} and ({4} = 0) order by {2} {3}", _tableName, whereQuery, String.Join(",", orderList.ToArray()), orderOption, _isDeletedField);

            }
            else
            {
                query = String.Format("select * from {0} (nolock) where {1} order by {2} {3}", _tableName, whereQuery, String.Join(",", orderList.ToArray()), orderOption);

            }

            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                var result = con.Query<T>(query, param);
                if (result.Count() == 0)
                    return new List<T>();
                var data = result.ToList();
                return data;
            }
        }

        //    {
        //        if (con.State == ConnectionState.Closed)
        public List<dynamic> QueryDyn(string sql, object param)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                var query = con.Query(sql, param);
                var data = query.ToList();
                return data;
            }
        }
        public string CallSPWithSQLParams(SqlCommand sqlCommand, int type)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                sqlCommand.Connection = con;
                SqlDataReader rdr = sqlCommand.ExecuteReader();
                string hata = "";
                bool hasHata = false;
                if (type == 1)
                {
                    while (rdr.Read())
                    {
                        if (Convert.ToInt32(rdr["Hata"]) != 0)
                        {
                            hata = rdr["Mesaj"].ToString();
                            hasHata = true;
                        }
                    }
                    if (hasHata)
                    {
                        return hata;
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;

            }
        }
        public List<T1> Query<T1>(string sql, object param)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                var query = con.Query<T1>(sql, param);
                var data = query.ToList();
                return data;
            }
        }

        public int Insert(T entity, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                if (Transaction.Current != null)
                    con.EnlistTransaction(Transaction.Current);

                //AKSİS 
                if (isPropertyVar(_makinaField))
                {
                    typeof(T).GetProperty(_makinaField).SetValue(entity, _ipAddress, null);
                }
                if (isPropertyVar(_zaman))
                {
                    typeof(T).GetProperty(_zaman).SetValue(entity, DateTime.Now, null);
                }
                if (isPropertyVar(_kullanici))
                {
                    typeof(T).GetProperty(_kullanici).SetValue(entity, _loginName, null);
                }
                //OYS

                if (isPropertyVar(_createdDateField))
                {
                    typeof(T).GetProperty(_createdDateField).SetValue(entity, DateTime.Now, null);
                }
                if (isPropertyVar(_isDeletedField))
                {
                    typeof(T).GetProperty(_isDeletedField).SetValue(entity, false, null);
                }
                if (isPropertyVar(_createdIpField))
                {
                    typeof(T).GetProperty(_createdIpField).SetValue(entity, _ipAddress, null);
                }

                if (isPropertyVar(_createdUserField))
                {
                    typeof(T).GetProperty(_createdUserField).SetValue(entity, _kisiId, null);
                }



                //typeof(T).GetProperty(_operationField).SetValue(entity, 2, null);
                dynamic queryResult = con.Insert<T>(entity);
                return Convert.ToInt32(queryResult);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void InsertAll(List<T> list, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                foreach (var item in list)
                {
                    //AKSİS 
                    if (isPropertyVar(_makinaField))
                    {
                        typeof(T).GetProperty(_makinaField).SetValue(item, _ipAddress, null);
                    }
                    if (isPropertyVar(_zaman))
                    {
                        typeof(T).GetProperty(_zaman).SetValue(item, DateTime.Now, null);
                    }
                    if (isPropertyVar(_kullanici))
                    {
                        typeof(T).GetProperty(_kullanici).SetValue(item, _loginName, null);
                    }
                    //OYS

                    if (isPropertyVar(_createdDateField))
                    {
                        typeof(T).GetProperty(_createdDateField).SetValue(item, DateTime.Now, null);
                    }
                    if (isPropertyVar(_isDeletedField))
                    {
                        typeof(T).GetProperty(_isDeletedField).SetValue(item, false, null);
                    }
                    if (isPropertyVar(_createdIpField))
                    {
                        typeof(T).GetProperty(_createdIpField).SetValue(item, _ipAddress, null);
                    }

                    if (isPropertyVar(_createdUserField))
                    {
                        typeof(T).GetProperty(_createdUserField).SetValue(item, _kisiId, null);
                    }
                }

                var prop = type.GetProperties();
                prop = prop.Where(x => x.Name != _idField).ToArray(); // id alanını çıkarıyoruz çünkü autoincrement
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})",
                    _tableName,
                    string.Join(", ", prop.Select(c => c.Name)),
                    string.Join(", @", prop.Select(c => c.Name)));

                int queryResult = con.Execute(sql, list);

                con.Close();
                con.Dispose();
            }
        }

        public int InsertWithId(T item, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                if (isPropertyVar(_makinaField))
                {
                    typeof(T).GetProperty(_makinaField).SetValue(item, _ipAddress, null);
                }
                if (isPropertyVar(_zaman))
                {
                    typeof(T).GetProperty(_zaman).SetValue(item, DateTime.Now, null);
                }
                if (isPropertyVar(_kullanici))
                {
                    typeof(T).GetProperty(_kullanici).SetValue(item, _loginName, null);
                }
                //OYS

                if (isPropertyVar(_createdDateField))
                {
                    typeof(T).GetProperty(_createdDateField).SetValue(item, DateTime.Now, null);
                }
                if (isPropertyVar(_isDeletedField))
                {
                    typeof(T).GetProperty(_isDeletedField).SetValue(item, false, null);
                }
                if (isPropertyVar(_createdIpField))
                {
                    typeof(T).GetProperty(_createdIpField).SetValue(item, _ipAddress, null);
                }
                if (isPropertyVar(_createdUserField))
                {
                    typeof(T).GetProperty(_createdUserField).SetValue(item, _kisiId, null);
                }

                var prop = type.GetProperties();
                // prop = prop.Where(x => x.Name != _idField).ToArray(); // id alanını çıkarıyoruz çünkü autoincrement
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})",
                    _tableName,
                    string.Join(", ", prop.Select(c => c.Name)),
                    string.Join(", @", prop.Select(c => c.Name)));

                int queryResult = con.Execute(sql, item);
                con.Close();
                con.Dispose();
                return queryResult;
            }
        }

        public int InsertOperationLog(T entity)
        {
            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                if (Transaction.Current != null)
                    con.EnlistTransaction(Transaction.Current);
                dynamic queryResult = con.Insert<T>(entity);
                return queryResult;
            }
        }

        public void UpdateAll(List<T> list, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            //oracle data adapter bug fix burada uygulanmadı
            var equals = new List<string>();

            var nonUpdatePropList = new List<string>() { _idField, _createdDateField, _createdUserField };
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (nonUpdatePropList.Contains(prop.Name))
                    continue;

                equals.Add(string.Format("{0}=@{1}", prop.Name, prop.Name));
            }

            //equals.Add(string.Format("{0}=SYSDATE()", _modifiedDateField));
            //equals.Add(string.Format("{0}={1}", _modifiedUserField, this._userID));

            string command = string.Format("update {0} set {1} where {2} = @ID", _tableName, string.Join(", ", equals.ToArray()), _idField);

            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                foreach (var item in list)
                {
                    //AKSİS
                    if (isPropertyVar(_makinaField))
                    {
                        typeof(T).GetProperty(_makinaField).SetValue(item, _ipAddress, null);
                    }
                    if (isPropertyVar(_sgZaman))
                    {
                        typeof(T).GetProperty(_sgZaman).SetValue(item, DateTime.Now, null);
                    }
                    if (isPropertyVar(_sgKullanici))
                    {
                        typeof(T).GetProperty(_sgKullanici).SetValue(item, _loginName, null);
                    }
                    //OYS

                    if (isPropertyVar(_modifiedDateField))
                    {
                        typeof(T).GetProperty(_modifiedDateField).SetValue(item, DateTime.Now, null);
                    }

                    if (isPropertyVar(_modifiedIpField))
                    {
                        typeof(T).GetProperty(_modifiedIpField).SetValue(item, _ipAddress, null);
                    }
                    if (isPropertyVar(_modifiedUserField))
                    {
                        typeof(T).GetProperty(_modifiedUserField).SetValue(item, _kisiId, null);
                    }
                    //var id = Convert.ToInt64(typeof(T).GetProperty(_idField).GetValue(item, null));
                    //    log.Info(SerializerHelper.SeserializeObject(new LogCRUDInfo() { ID = id, Oper = "Update", Type = type.Name, Entity = item }));
                }

                var result = con.Execute(command, list);
            }
        }


        public bool Update(T entity, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            using (var con = this.Connection)
            {

                if (con.State == ConnectionState.Closed)
                    con.Open();

                if (Transaction.Current != null)
                    con.EnlistTransaction(Transaction.Current);
                //typeof(T).GetProperty(_modifiedDateField).SetValue(entity, DateTime.Now, null);

                //AKSİS
                if (isPropertyVar(_makinaField))
                {
                    typeof(T).GetProperty(_makinaField).SetValue(entity, _ipAddress, null);
                }
                if (isPropertyVar(_sgZaman))
                {
                    typeof(T).GetProperty(_sgZaman).SetValue(entity, DateTime.Now, null);
                }
                if (isPropertyVar(_sgKullanici))
                {
                    typeof(T).GetProperty(_sgKullanici).SetValue(entity, _loginName, null);
                }
                //OYS

                if (isPropertyVar(_modifiedDateField))
                {
                    typeof(T).GetProperty(_modifiedDateField).SetValue(entity, DateTime.Now, null);
                }

                if (isPropertyVar(_modifiedIpField))
                {
                    typeof(T).GetProperty(_modifiedIpField).SetValue(entity, _ipAddress, null);
                }
                if (isPropertyVar(_modifiedUserField))
                {
                    typeof(T).GetProperty(_modifiedUserField).SetValue(entity, _kisiId, null);
                }

                var result = con.Update<T>(entity);

                return result;
            }
        }

        public bool Delete(T entity, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);
            if (entity == null)
            {
                return true;
            }

            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                if (Transaction.Current != null)
                    con.EnlistTransaction(Transaction.Current);
                //OYS
                if (isPropertyVar(_isDeletedField))
                {
                    typeof(T).GetProperty(_isDeletedField).SetValue(entity, true, null);

                    //return this.Update(entity);
                    if (isPropertyVar(_modifiedDateField))
                    {
                        typeof(T).GetProperty(_modifiedDateField).SetValue(entity, DateTime.Now, null);
                    }
                    if (isPropertyVar(_modifiedIpField))
                    {
                        typeof(T).GetProperty(_modifiedIpField).SetValue(entity, _ipAddress, null);
                    }
                    if (isPropertyVar(_modifiedUserField))
                    {
                        typeof(T).GetProperty(_modifiedUserField).SetValue(entity, _kisiId, null);
                    }
                    var result = con.Update<T>(entity);
                    return result;
                }
                else
                {
                    // TODO: burada gerçek silme yapmalıyız yada o tabloya alan ekleyip isdeleted modeli devam edilebilir bun düşünelim!!!! ae
                }


                return true;
            }
        }

        public bool AksisDelete(T entity, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            checkYerineLogin(memberName);

            if (entity == null)
            {
                return true;
            }

            using (var con = this.Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                if (Transaction.Current != null)
                    con.EnlistTransaction(Transaction.Current);
                //OYS
                if (isPropertyVar(_isDeletedField))
                {
                    typeof(T).GetProperty(_isDeletedField).SetValue(entity, true, null);

                    //return this.Update(entity);
                    if (isPropertyVar(_modifiedDateField))
                    {
                        typeof(T).GetProperty(_modifiedDateField).SetValue(entity, DateTime.Now, null);
                    }
                    if (isPropertyVar(_modifiedIpField))
                    {
                        typeof(T).GetProperty(_modifiedIpField).SetValue(entity, _ipAddress, null);
                    }
                    if (isPropertyVar(_modifiedUserField))
                    {
                        typeof(T).GetProperty(_modifiedUserField).SetValue(entity, _kisiId, null);
                    }
                    var result = con.Update<T>(entity);
                    return result;
                }
                else
                {
                    var result = con.Delete<T>(entity);
                    return result;
                }


                return true;
            }
        }
        /// <summary>
        /// üretilen t de istenilen property varmı yokmu onu döner
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool isPropertyVar(string propertyName)
        {
            var isProperty = type.GetProperty(propertyName);

            //   var isDeletedVarMiaa = type.GetProperty("aaa");

            if (isProperty == null)
            {
                return false;
            }

            return true;
        }


        public List<string> yerineLogindeGecilebilecekSaveMetodlariList()
        {
            List<string> list = new List<string>();

            // dış metodlarda izin verilmek istenenleri ekleylim
            list.Add("getLoginByIdmDto");
            list.Add("DeleteKisiTokenByLoginName");

            //aşağıdakiler kendi iç çağrış metodları bunlar mutlaka olmalı kaldırmayın
            list.Add("Save");
            list.Add("GetList");
            list.Add("Delete");
            list.Add("Update");
            list.Add("AksisDelete");
            list.Add("UpdateAll");
            list.Add("Insert");
            list.Add("InsertAll");

            return list;
        }

        public bool checkYerineLogin(string metod)
        {
            //string m = MethodBase.GetCurrentMethod().Name;
            //MethodBase m1 = MethodBase.GetCurrentMethod();
            //var aaa = m1.IsGenericMethod;
            //var aaaa = m1.IsGenericMethodDefinition;
            //var aaaaa = m1.ReflectedType.Name ;
            //var aaaaaa = m1.ReflectedType.FullName ;
          //  if (_userTokenDto != null && _userTokenDto.isYerineLogin && !yerineLogindeGecilebilecekSaveMetodlariList().Contains(metod))
          //  {
          //      throw new AppException(MessageCode.ERROR_500, $"Yerine Login Olduğunuz için kaydetme/güncelleme/silme işlemi yapılamaz.");
          //  }

            return false;
            // string metod = "";
            // if (_userTokenDto.isYerineLogin && !yerineLogindeGecilebilecekSaveMetodlariList().Contains(metod))
            // {
            //     throw new OYSException(MessageCode.ERROR_500, $"Yerine Login Olduğunuz için kayıt işlemi yapılamaz.");
            // }
            //
            // return false;
        }


        public bool Save(T entity, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
        
            checkYerineLogin(memberName);

            bool value = false;
            var id = Convert.ToInt32(typeof(T).GetProperty(_idField).GetValue(entity, null));
            if (id > 0)
            {
                value = this.Update(entity);
            }
            else
            {
                int returnId = this.Insert(entity);
                if (returnId > 0) { value = true; }
            }
            return value;
        }



        public TransactionScope BeginTransaction()
        {
            if (Transaction.Current == null)
                return new TransactionScope();

            return new TransactionScope(Transaction.Current);
        }

    }

    public enum OrderOption
    {
        asc,
        desc
    }

    //Dynamic Query
    public class QueryResult
    {
        /// <summary>
        /// The _result
        /// </summary>
        private readonly Tuple<string, dynamic> _result;

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql
        {
            get
            {
                return _result.Item1;
            }
        }

        /// <summary>
        /// Gets the param.
        /// </summary>
        /// <value>
        /// The param.
        /// </value>
        public dynamic Param
        {
            get
            {
                return _result.Item2;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult" /> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="param">The param.</param>
        public QueryResult(string sql, dynamic param)
        {
            _result = new Tuple<string, dynamic>(sql, param);
        }
    }

    /// <summary>
    /// Dynamic query class.
    /// </summary>
    public sealed class DynamicQuery
    {
        /// <summary>
        /// Gets the insert query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The Sql query based on the item properties.
        /// </returns>
        public static string GetInsertQuery(string tableName, dynamic item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] columns = props.Select(p => p.Name).Where(s => s != "ID").ToArray();

            return string.Format("INSERT INTO {0} ({1}) OUTPUT inserted.ID VALUES (@{2})",
                                 tableName,
                                 string.Join(",", columns),
                                 string.Join(",@", columns));
        }





        /// <summary>
        /// Gets the update query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The Sql query based on the item properties.
        /// </returns>
        public static string GetUpdateQuery(string tableName, dynamic item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] columns = props.Select(p => p.Name).ToArray();

            var parameters = columns.Select(name => name + "=@" + name).ToList();

            return string.Format("UPDATE {0} SET {1} WHERE ID=@ID", tableName, string.Join(",", parameters));
        }

        /// <summary>
        /// Gets the dynamic query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A result object with the generated sql and dynamic params.</returns>
        public static QueryResult GetDynamicQuery<T>(string tableName, Expression<Func<T, bool>> expression)
        {
            var queryProperties = new List<QueryParameter>();
            var body = (BinaryExpression)expression.Body;
            IDictionary<string, Object> expando = new ExpandoObject();
            var builder = new StringBuilder();

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            WalkTree(body, ExpressionType.Default, ref queryProperties);

            // convert the query parms into a SQL string and dynamic property object
            builder.Append("SELECT * FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE ");

            for (int i = 0; i < queryProperties.Count(); i++)
            {
                QueryParameter item = queryProperties[i];

                if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                {
                    builder.Append(string.Format("{0} {1} {2} @{1}{3} ", item.LinkingOperator, item.PropertyName, item.QueryOperator, i));
                }
                else
                {
                    builder.Append(string.Format("{0} {1} @{0}{2} ", item.PropertyName, item.QueryOperator, i));
                }

                expando[item.PropertyName + i] = item.PropertyValue;
            }

            return new QueryResult(builder.ToString().TrimEnd(), expando);
        }

        /// <summary>
        /// Walks the tree.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private static void WalkTree(BinaryExpression body, ExpressionType linkingType,
                                     ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                string propertyName = GetPropertyName(body);
                dynamic propertyValue = body.Right;
                string opr = GetOperator(body.NodeType);
                string link = GetOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue.Value, opr));
            }
            else
            {
                WalkTree((BinaryExpression)body.Left, body.NodeType, ref queryProperties);
                WalkTree((BinaryExpression)body.Right, body.NodeType, ref queryProperties);
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>The property name for the property expression.</returns>
        private static string GetPropertyName(BinaryExpression body)
        {
            string propertyName = body.Left.ToString().Split(new char[] { '.' })[1];

            if (body.Left.NodeType == ExpressionType.Convert)
            {
                // hack to remove the trailing ) when convering.
                propertyName = propertyName.Replace(")", string.Empty);
            }

            return propertyName;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The expression types SQL server equivalent operator.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return "AND";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Default:
                    return string.Empty;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Class that models the data structure in coverting the expression tree into SQL and Params.
    /// </summary>
    internal class QueryParameter
    {
        public string LinkingOperator { get; set; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public string QueryOperator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter" /> class.
        /// </summary>
        /// <param name="linkingOperator">The linking operator.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="queryOperator">The query operator.</param>
        internal QueryParameter(string linkingOperator, string propertyName, object propertyValue, string queryOperator)
        {
            this.LinkingOperator = linkingOperator;
            this.PropertyName = propertyName;
            this.PropertyValue = propertyValue;
            this.QueryOperator = queryOperator;
        }

    }
    //public class Linq<T> : ISQLinq {

    //    public Linq()
    //    {

    //    }
    //    public SQLinqResult ToSQL(int existingParameterCount = 0, string parameterNamePrefix = "sqlinq_")
    //    {
    //        return new SQLinq<T>().ToSQL(existingParameterCount, parameterNamePrefix);
    //    }
    //}
}
