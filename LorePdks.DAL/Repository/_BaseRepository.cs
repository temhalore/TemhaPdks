using Dapper;
using DapperExtensions;
using DapperExtensions.Sql;
using LorePdks.COMMON.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;

namespace LorePdks.DAL.Repository
{

    public enum OrderOption
    {
        asc,
        desc
    }

    public class LogCRUDInfo
    {
        public string Oper { get; set; }
        public string Type { get; set; }
        public long ID { get; set; }
        public object Entity { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKey : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }

    public abstract class _BaseRepository<T> where T : class
    {
        //private string _idField = Configuration.IDProperty;
        private string _createdDateField = CoreConfig.CreatedDateProperty;
        private string _createdUserField = CoreConfig.CreatedUserProperty;
        private string _modifiedDateField = CoreConfig.ModifiedDateProperty;
        private string _modifiedUserField = CoreConfig.ModifiedUserProperty;
        private string _isDeletedField = CoreConfig.IsDeletedProperty;
        private string _mainSequenceTable = "s_sequence_table";

        //internal ILog log;
        static Type type;
        private int _userID;

        private string _idField
        {
            get
            {
                if (typeof(T).Name == "T_ORT_BOLUM")
                    return "BIRIMKOD";

                if (typeof(T).Name == "V_PER_PERSONELINFO")
                    return "KIMLIK_NO";

                var propInfo = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute))).FirstOrDefault();
                if (propInfo == null)
                {
                    return null;
                }
                //if (propInfo == null)
                //    throw new Exception("PK Propertiy doesn't set or not a valid mask");

                return propInfo.Name.ToString();
            }
        }

        private DbConnection Connection
        {
            get
            {
                // return new OracleConnection(Configuration.ConnectionString);
                return new MySqlConnection(CoreConfig.ConnectionString);
            }
        }

        static _BaseRepository()
        {
            //DapperExtensions.DapperExtensions.SqlDialect = new OracleDialect();
            DapperExtensions.DapperExtensions.SqlDialect = new MySqlDialect();
            type = typeof(T);
        }
        public _BaseRepository()
        {
            //var principal = System.Threading.Thread.CurrentPrincipal;
            //if (principal != null && principal.Identity != null && !String.IsNullOrWhiteSpace(principal.Identity.Name))
            //    this._userID = Convert.ToInt64(principal.Identity.Name);

            //if (HttpContext.Current != null && HttpContext.Current.Items["UserID"] != null)
            //    this._userID = Convert.ToInt64(HttpContext.Current.Items["UserID"]);

            // log = LogManager.GetLogger(this.GetType());
            //log4net.ThreadContext.Properties["customUserName"] = this._userID;
            //log4net.ThreadContext.Properties["customName"] = "";
            //log4net.ThreadContext.Properties["customUserType"] = "";

            //con = new OracleConnection(Configuration.ConnectionString);
            //con.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T Get(long ID)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var query = con.Query<T>(string.Format("select * from {0} where {1} = @ID and ISDELETED = 0", type.Name, _idField), new { ID });

                if (query.Count() == 0)
                {
                    con.Close();
                    con.Dispose();
                    return Activator.CreateInstance<T>();
                }

                var data = query.First();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Get(string whereQuery, object param = null, OrderOption orderOption = OrderOption.asc, params Enum[] orderProp)
        {
            if (string.IsNullOrEmpty(whereQuery))
                whereQuery = "1 = 1";

            var orderList = new List<string>();
            if (orderProp.Length == 0)
                orderList.Add(_idField);
            for (int i = 0; i < orderProp.Length; i++)
                orderList.Add(Enum.GetName(orderProp[i].GetType(), orderProp[i]));

            var query = string.Format("select * from {0} where ({1} = 0) and ({2})", type.Name, _isDeletedField, whereQuery);

            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var result = con.Query<T>(query, param);

                if (result.Count() == 0)
                {
                    con.Close();
                    con.Dispose();
                    return Activator.CreateInstance<T>();
                }

                var data = result.Single();
                con.Close();
                con.Dispose();
                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var query = con.Query<T>(string.Format("select * from {0} where {1} = @ISDELETED", type.Name, _isDeletedField), new { IsDeleted = 0 });

                if (query.Count() == 0)
                {
                    con.Close();
                    con.Dispose();
                    return new List<T>();
                }

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereQuery"></param>
        /// <param name="param"></param>
        /// <param name="orderOption"></param>
        /// <param name="orderProp"></param>
        /// <returns></returns>
        public List<T> GetList(string whereQuery, object param = null, OrderOption orderOption = OrderOption.asc, params Enum[] orderProp)
        {
            if (string.IsNullOrEmpty(whereQuery))
                whereQuery = "1 = 1";

            var orderList = new List<string>();
            if (orderProp.Length == 0)
                orderList.Add(_idField);
            for (int i = 0; i < orderProp.Length; i++)
                orderList.Add(Enum.GetName(orderProp[i].GetType(), orderProp[i]));



            var query = string.Format("select * from {0} where ({1} = 0) and ({2}) order by {3} {4}",
                type.Name, _isDeletedField, whereQuery, string.Join(",", orderList.ToArray()), orderOption);

            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var result = con.Query<T>(query, param);

                if (result.Count() == 0)
                {
                    con.Close();
                    con.Dispose();
                    return new List<T>();
                }

                var data = result.ToList();
                con.Close();
                con.Dispose();
                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> Query(string sql, object param)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var query = con.Query<T>(sql, param);

                //if (query.Count() == 0)
                //    return new List<T>();

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// Paging Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="startIndex"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<T> Query(string sql, object param, int start, int take)
        {
            using (var con = new MySqlConnection(CoreConfig.ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                start = start <= 1 ? 0 : start;

                //var startIndex = (start - 1) * take;
                var pagingSQL = string.Format(@"select * from (
                                                  select ROWNUM RowNumber, queryRN.* from({0}) queryRN
                                                ) queryMain where queryMain.RowNumber >= {1} and queryMain.RowNumber <= {2}", sql, start + 1, start + take);

                var query = con.Query<T>(pagingSQL, param);

                //if (query.Count() == 0)
                //    return new List<T>();

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> QueryDyn(string sql, object param)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var query = con.Query(sql, param);

                //if (query.Count() == 0)
                //    return new List<T>();

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<TNew> QueryDyn<TNew>(string sql, object param) where TNew : class
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var query = con.Query<TNew>(sql, param);

                //if (query.Count() == 0)
                //    return new List<T>();

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        public IEnumerable<TNew> QueryDyn<TNew>(string sql, object param, int start, int take) where TNew : class
        {
            using (var con = new MySqlConnection(CoreConfig.ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                start = start <= 1 ? 0 : start;

                //var startIndex = (start - 1) * take;
                var pagingSQL = string.Format(@"select * from (
                                                  select ROWNUM RowNumber, queryRN.* from({0}) queryRN
                                                ) queryMain where queryMain.RowNumber >= {1} and queryMain.RowNumber <= {2}", sql, start + 1, start + take);

                var query = con.Query<TNew>(pagingSQL, param);

                //if (query.Count() == 0)
                //    return new List<T>();

                var data = query.ToList();

                con.Close();
                con.Dispose();

                return data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var list = GetList();
            return list == null ? 0 : list.Count(); ;
            //var query = String.Format("select count(*) from {0} where {1} = @IsDeleted", type.Name, _isDeletedField);

            //var com = con.CreateCommand();
            //com.CommandText = query;
            //com.Parameters.Add("IsDeleted", 0);

            //var result = Convert.ToInt32(com.ExecuteScalar());
            //com.Dispose();
            //con.Close();
            //return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereQuery"></param>
        /// <param name="param"></param>
        /// <param name="orderOption"></param>
        /// <param name="orderProp"></param>
        /// <returns></returns>
        public int Count(string whereQuery, object param = null, OrderOption orderOption = OrderOption.asc, params Enum[] orderProp)
        {
            var list = GetList(whereQuery, param, orderOption, orderProp);
            return list == null ? 0 : list.Count();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Count(string sql, object param = null)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                var cmd = string.Format("select count(*) from ({0})", sql);
                var query = con.ExecuteScalar(cmd, param);

                var count = Convert.ToInt32(query);

                con.Close();
                con.Dispose();

                return count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(T entity)
        {

            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);


                typeof(T).GetProperty(_createdDateField).SetValue(entity, DateTime.Now, null);
                if (typeof(T).GetProperty(_createdUserField) != null)
                    typeof(T).GetProperty(_createdUserField).SetValue(entity, _userID, null);



                var propertyDict = GetProperties(entity);

                #region  oracle data adapter bug fix
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.PropertyType != typeof(string))
                        continue;

                    var propValue = prop.GetValue(entity);
                    if (propValue == null)
                        continue;

                    var val = propValue.ToString();
                    if (val.Length < 2001 || val.Length > 4000)
                        continue;

                    var charDif = 4001 - val.Length;
                    var sb = new StringBuilder();
                    sb.Append(val);
                    sb.Append(' ', charDif);

                    prop.SetValue(entity, sb.ToString(), null);
                }
                #endregion


                //if (Convert.ToInt32(typeof(T).GetProperty(_idField).GetValue(entity)) <= 0)
                //    typeof(T).GetProperty(_idField).SetValue(entity, sequenceVal, null);

                propertyDict = GetProperties(entity);
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})",
                    typeof(T).Name,
                    string.Join(", ", propertyDict.Select(c => c.Key)),
                    string.Join(", @", propertyDict.Select(c => c.Key)));

                var sequenceVal = GetSequenceNextVal(type.Name);  // save den önce al
                int queryResult = con.Execute(sql, propertyDict);

                if (sequenceVal<=0)
                {
                    //burada eğer entty nin id propertysi dolu ise onu yazalım dışarıdan gelmiş olabilir.
                    var manuelID = Convert.ToInt64(typeof(T).GetProperty(_idField).GetValue(entity, null));
                    if (manuelID > 0)
                        sequenceVal = Convert.ToInt32( manuelID);
                }
                //   log.Info(SerializerHelper.SeserializeObject(new LogCRUDInfo() { ID = sequenceVal, Oper = "Insert", Type = type.Name, Entity = entity }));
                typeof(T).GetProperty(_idField).SetValue(entity, sequenceVal, null);
                con.Close();
                con.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void InsertAll(List<T> list)
        {
            //oracle data adapter bug fix burada uygulanmadı
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                if (typeof(T).GetProperty(_idField) != null)
                {
                    int idCount = 0;
                    foreach (var item in list)
                    {
                        var sequenceVal = GetSequenceNextVal(typeof(T).Name) + idCount;

                        typeof(T).GetProperty(_createdDateField).SetValue(item, DateTime.Now, null);
                        if (typeof(T).GetProperty(_createdUserField) != null)
                            typeof(T).GetProperty(_createdUserField).SetValue(item, _userID, null);
                        typeof(T).GetProperty(_idField).SetValue(item, sequenceVal, null);
                        idCount++;
                    }
                }

                var prop = type.GetProperties();
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES (:{2})",
                    typeof(T).Name,
                    string.Join(", ", prop.Select(c => c.Name)),
                    string.Join(", @", prop.Select(c => c.Name)));

                int queryResult = con.Execute(sql, list);

                con.Close();
                con.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                if (typeof(T).GetProperty(_modifiedDateField) != null)
                    typeof(T).GetProperty(_modifiedDateField).SetValue(entity, DateTime.Now, null);
                if (typeof(T).GetProperty(_modifiedUserField) != null)
                    typeof(T).GetProperty(_modifiedUserField).SetValue(entity, _userID, null);

                #region  oracle data adapter bug fix
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.PropertyType != typeof(string))
                        continue;

                    var propValue = prop.GetValue(entity);
                    if (propValue == null)
                        continue;

                    var val = propValue.ToString();
                    if (val.Length < 2001 || val.Length > 4000)
                        continue;

                    var charDif = 4001 - val.Length;
                    var sb = new StringBuilder();
                    sb.Append(val);
                    sb.Append(' ', charDif);

                    prop.SetValue(entity, sb.ToString(), null);
                }
                #endregion

                List<T> entList = new List<T>();
                entList.Add(entity);

                UpdateAll(entList);
                var result = true;

                // var result = con.Update<T>(entity);

                var id = Convert.ToInt64(typeof(T).GetProperty(_idField).GetValue(entity, null));
                //  log.Info(SerializerHelper.SeserializeObject(new LogCRUDInfo() { ID = id, Oper = "Update", Type = type.Name, Entity = entity }));

                con.Close();
                con.Dispose();

                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void UpdateAll(List<T> list)
        {
            //oracle data adapter bug fix burada uygulanmadı
            var equals = new List<string>();
            var parameters = new List<MySqlParameter>();

            var nonUpdatePropList = new List<string>() { _idField, _createdDateField, _createdUserField, _modifiedDateField, _modifiedUserField };
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (nonUpdatePropList.Contains(prop.Name))
                    continue;

                equals.Add(string.Format("{0}=@{1}", prop.Name, prop.Name));
            }

            equals.Add(string.Format("{0}=SYSDATE()", _modifiedDateField));
            equals.Add(string.Format("{0}={1}", _modifiedUserField, _userID));

            string command = string.Format("update {0} set {1} where {2} = @ID", type.Name, string.Join(", ", equals.ToArray()), _idField);

            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);

                var result = con.Execute(command, list);

                foreach (var item in list)
                {
                    var id = Convert.ToInt64(typeof(T).GetProperty(_idField).GetValue(item, null));
                    //    log.Info(SerializerHelper.SeserializeObject(new LogCRUDInfo() { ID = id, Oper = "Update", Type = type.Name, Entity = item }));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            var isDeleted = Convert.ToInt32(typeof(T).GetProperty(_isDeletedField).GetValue(entity, null));
            if (isDeleted == 0)
            {
                typeof(T).GetProperty(_isDeletedField).SetValue(entity, 1, null);

                return Update(entity);
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool Delete(long ID)
        {
            var entity = Get(ID);
            return Delete(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Save(T entity)
        {
            var id = Convert.ToInt64(typeof(T).GetProperty(_idField).GetValue(entity, null));
            if (id > 0)
            {
                Update(entity);

                // List<T> entList = new List<T>();
                // entList.Add(entity);
                //
                // this.UpdateAll(entList);
            }

            else
            {
                var sequenceVal = GetSequenceNextVal(typeof(T).Name); // insert önesi al               
                Insert(entity);
                typeof(T).GetProperty(_idField).SetValue(entity, sequenceVal); // sonrası çak
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TransactionScope BeginTransaction()
        {
            if (Transaction.Current == null)
                return new TransactionScope();

            return new TransactionScope(Transaction.Current);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private Dictionary<string, object> GetProperties(T obj)
        {
            var dict = new Dictionary<string, object>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;

                var name = property.Name;
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);

                dict.Add(name, value);
            }

            return dict;
        }
        //private List<string> GetPropertyDiff<T>(T obj1, T obj2)
        //{
        //    List<string> changes = new List<string>();
        //    var disableProperties = new List<string>() { _createdUserField, _createdDateField, _modifiedUserField, _modifiedDateField, _idField };
        //    try
        //    {
        //        PropertyInfo[] properties = typeof(T).GetProperties();
        //        foreach (PropertyInfo pi in properties)
        //        {
        //            if (disableProperties.Contains(pi.Name))
        //                continue;

        //            object value1 = typeof(T).GetProperty(pi.Name).GetValue(obj1, null);
        //            object value2 = typeof(T).GetProperty(pi.Name).GetValue(obj2, null);

        //            if (value1 != value2 && (value1 == null || !value1.Equals(value2)))
        //            {
        //                changes.Add(string.Format("{0} from {1} to {2}", pi.Name, value1, value2));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return changes;
        //}

        //TABLO BAZLI ID TUTACAK OLAN VE YENİ ID ÜRETECEK OLAN METOD
        public int GetSequenceNextVal(string tabloName)
        {
            using (var con = Connection)
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                con.EnlistTransaction(Transaction.Current);


                //Auto_increment lı versiyo o tablo için sıradaki id değerini verir
                string select = string.Format("SELECT NVL(Auto_increment,0) FROM information_schema.tables WHERE table_name = '{0}' ", tabloName);
              
                var selectResult = con.Query<int>(select).SingleOrDefault();

                //TABLOYA İLK KAYDI MANULE OLARAK AT SQL DEN SONRASINDA BU KOD İŞLEYECEKTİR YOKSA İŞLEMEZ

                //  string select = string.Format("SELECT ID from {0} where TABLO_ADI='{1}'", _mainSequenceTable, tabloName);
                //
                //  // var result = con.Execute(select, null);
                //  var selectResult = con.Query<T>(select);
                //
                //  // var query = con.Query<TNew>(pagingSQL, param);
                //  // var data = query.ToList();
                //
                //
                //  if (selectResult == null || selectResult.Count() == 0)
                //  {
                //      string insert = string.Format("INSERT INTO {0} (ID,TABLO_ADI) VALUES (0,'{1}')", _mainSequenceTable, tabloName);
                //
                //      var selectInsert = con.Query<T>(insert);
                //  }
                //
                //
                //
                //  string update = string.Format("UPDATE {0} SET ID = ID+1 WHERE TABLO_ADI = '{1}'", _mainSequenceTable, tabloName);
                //
                //  var updateResult = con.Query<T>(update);
                //
                //
                //  var sequenceVal = con.Query<long>(String.Format("SELECT ID from {0} where TABLO_ADI='{1}'", _mainSequenceTable, tabloName)).First();

                con.Close();
                con.Dispose();

                return selectResult;
            }
        }
    }
}
