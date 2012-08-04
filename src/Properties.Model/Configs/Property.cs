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
using System.Collections.Concurrent;
using CodeSharp.Core.DomainBase;

namespace Properties.Model
{
    /// <summary>config item
    /// </summary>
    public class Property : EntityBase<Guid>
    {
        private static readonly string VALUE = "__value";
        private static readonly CodeSharp.Core.Utils.Serializer _serializer = new CodeSharp.Core.Utils.Serializer();
        private object _lock = new object();
        private bool _useCommitted;
        private ConcurrentDictionary<string, string> _uncommitted;//No need to use ConcurrentDictionary
        private ConcurrentDictionary<string, string> _committed;
        private string _committedValue { get; set; }
        private string _uncommittedValue { get; set; }
        private bool _committedTrashed { get; set; }
        private bool _uncommittedTrashed { get; set; }

        internal Configuration Container { get; set; }

        /// <summary>unique in configuration
        /// </summary>
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreateTime { get; private set; }
        public DateTime? LastCommitTime { get; private set; }

        public bool IsTrashed { get { return this._useCommitted ? this._committedTrashed : this._uncommittedTrashed; } }
        public string Value
        {
            get { return this[VALUE]; }
            set { this[VALUE] = value; }
        }
        /// <summary>Only allow assignment to uncommitted
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public string this[string flag]
        {
            get
            {
                this.Prepare();
                return (this._useCommitted ? this._committed : this._uncommitted).GetOrAdd(flag, string.Empty);
            }
            set
            {
                this.Prepare(ref this._uncommitted, this._uncommittedValue);
                this._uncommitted.AddOrUpdate(flag, value, (k, v) => value);
                this._uncommittedValue = _serializer.JsonSerialize(this._uncommitted);
                //parent configuration make change
                this.Container.MakeChange();
            }
        }

        protected Property() { this.CreateTime = DateTime.UtcNow; }
        internal Property(string name)
            : this()
        {
            Assert.IsValidKey(name);
            this.Name = name;
        }

        public Property Committed() { this._useCommitted = true; return this; }
        public Property Uncommitted() { this._useCommitted = false; return this; }

        public void SetDescription(string description)
        {
            Assert.IsValidDescription(description);
            this.Description = description;
        }
        /// <summary>Only allow make uncommitted trashed
        /// </summary>
        public void Trash()
        {
            this.Trash(true);
        }
        /// <summary>Only allow make uncommitted trashed
        /// </summary>
        /// <param name="trashed"></param>
        public void Trash(bool trashed)
        {
            this._uncommittedTrashed = trashed;
            this.Container.MakeChange();
        }
        /// <summary>commit changes and trashed
        /// </summary>
        public void DoCommit()
        {
            this._committedValue = this._uncommittedValue;
            this._committedTrashed = this._uncommittedTrashed;
            lock (this._lock)
                this._committed = null;
            this.LastCommitTime = DateTime.UtcNow;
        }

        private void Prepare()
        {
            if (this._useCommitted)
                this.Prepare(ref this._committed, this._committedValue);
            else
                this.Prepare(ref this._uncommitted, this._uncommittedValue);
        }
        private void Prepare(ref ConcurrentDictionary<string, string> dict, string value)
        {
            if (dict != null) return;
            lock (this._lock)
                if (dict == null)
                    dict = _serializer.JsonDeserialize<ConcurrentDictionary<string, string>>(value ?? "{}");
        }
    }
}