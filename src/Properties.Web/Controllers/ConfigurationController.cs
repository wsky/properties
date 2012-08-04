using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace Properties.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Properties()
        {
            return View(); 
        }
    }
}

namespace Properties
{
    public class ConfigurationInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreatorId { get; set; }
    }
    public class PropertyInfo
    {
        private static readonly System.Web.Script.Serialization.JavaScriptSerializer _serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        private object _lock = new object();
        private System.Collections.Concurrent.ConcurrentDictionary<string, string> _dict_management;
        private System.Collections.Concurrent.ConcurrentDictionary<string, string> _dict_release;
        public string Name { get; set; }
        public string ManagementValueString { get; set; }
        public string ReleaseValueString { get; set; }
        public string Value { get; set; }

        public string ManagementValue(string flag)
        {
            this.PrepareManagement();
            return this._dict_management.GetOrAdd(flag, string.Empty);
        }
        public void ManagementValue(string flag, string value)
        {
            this.PrepareManagement();
            this._dict_management.AddOrUpdate(flag, value, (k, v) => value);
        }
        public string ReleaseValue(string flag)
        {
            this.PrepareRelease();
            return this._dict_release.GetOrAdd(flag, string.Empty);
        }
        public void ReleaseValue(string flag, string value)
        {
            this.PrepareRelease();
            this._dict_release.AddOrUpdate(flag, value, (k, v) => value);
        }

        public void Commit()
        {
            this.ReleaseValueString = this.ManagementValueString;
            lock (this._lock)
                this._dict_release = null;
        }
        private void PrepareManagement()
        {
            if (this._dict_management != null) return;
            lock (this._lock)
                if (this._dict_management == null)
                    this._dict_management = _serializer.Deserialize<System.Collections.Concurrent.ConcurrentDictionary<string, string>>(this.ManagementValueString);
        }
        private void PrepareRelease()
        {
            if (this._dict_release != null) return;
            lock (this._lock)
                if (this._dict_release == null)
                    this._dict_release = _serializer.Deserialize<System.Collections.Concurrent.ConcurrentDictionary<string, string>>(this.ReleaseValueString);
        }
    }
    public class Account
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public static class SqlHelper
    {
        public static IEnumerable<ConfigurationInfo> GetConfigurations(Account account)
        {
            using (var conn = Connection())
                return conn.Query<ConfigurationInfo>("select CreatorId=@id", new { id = account.ID });
        }
        private static System.Data.Common.DbConnection Connection()
        {
            return new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["propertis"].ToString());
        }
    }
}