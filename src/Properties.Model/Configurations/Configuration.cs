//  Copyright 2012 wsky. wskyhx@gmail.com
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, 
//  software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSharp.Core.DomainBase;

namespace Properties.Model
{
    /// <summary>configs table
    /// </summary>
    public class Configuration : EntityBase<Guid>, IAggregateRoot
    {
        private static readonly CodeSharp.Core.Utils.Serializer _serializer = new CodeSharp.Core.Utils.Serializer();
        private string _flagString { get; set; }
        private IList<string> _flags;
        private IList<Property> _properties;

        public Guid AppId { get; private set; }
        /// <summary>unique in app
        /// </summary>
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreateTime { get; private set; }
        public DateTime? LastTime { get; private set; }
        public IEnumerable<string> Flags { get { return this.GetFlags().AsEnumerable(); } }
        public IEnumerable<string> Properties { get { return this._properties.Select(o => o.Name).Distinct(); } }

        protected Configuration()
        {
            this.CreateTime = DateTime.UtcNow;
            this._properties = new List<Property>();
        }
        public Configuration(Application app, string name)
            : this()
        {
            Assert.IsValidKey(name);
            this.Name = name;
            Assert.IsNotNull(app);
            this.AppId = app.ID;
        }

        public void SetDescription(string description)
        {
            Assert.IsValidDescription(description);
            this.Description = description;
        }

        public Property GetProperty(string name)
        {
            var p = this._properties.FirstOrDefault(o => o.Name == name);
            if (p != null)
                p.Container = this;
            return p;
        }
        public Property AddProperty(string name)
        {
            Assert.IsFalse(this._properties.Any(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
            this._properties.Add(new Property(name));
            this.MakeChange();
            return this.GetProperty(name);
        }
        public void AddFlag(string flag)
        {
            Assert.IsFalse(this.GetFlags().Contains(flag));
            this.GetFlags().Add(flag);
            this.ResetFlags();
            this.MakeChange();
        }
        public void RemoveFlag(string flag)
        {
            this.GetFlags().Remove(flag);
            this.ResetFlags();
            this.MakeChange();
        }

        internal void MakeChange()
        {
            this.LastTime = DateTime.UtcNow;
        }

        private IList<string> GetFlags()
        {
            if (this._flags == null)
                this._flags = _serializer.JsonDeserialize<IList<string>>(this._flagString ?? string.Empty) ?? new List<string>();
            return this._flags;
        }
        private void ResetFlags()
        {
            this._flagString = _serializer.JsonSerialize(this.GetFlags());
        }
    }
}