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
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace Properties.Model.Mappings
{
    public class ConfigurationMap : ClassMap<Configuration>
    {
        public ConfigurationMap()
        {
            Table("Properties_Configuration");
            Id(m => m.ID);
            Map(m => m.Name).Length(255);
            Map(m => m.Description).Length(500);
            Map(m => m.CreateTime);
            Map(m => m.AppId);
            Map(m => m.LastTime).Nullable();
            Map(Reveal.Member<Configuration>("_flagString")).Column("Flags").Length(500);
            HasMany<Property>(Reveal.Member<Configuration>("_properties"))
                .KeyColumn("ConfigurationId")
                .Cascade.SaveUpdate();
        }
    }
    public class PropertyMap : ClassMap<Property>
    {
        public PropertyMap()
        {
            Table("Properties_Property");
            Id(m => m.ID);
            Map(m => m.Name).Length(255);
            Map(m => m.Description).Length(500);
            Map(m => m.CreateTime);
            Map(m => m.LastCommitTime).Nullable();
            Map(Reveal.Member<Property>("_committedValue")).Column("CommittedValue").Length(5000);
            Map(Reveal.Member<Property>("_uncommittedValue")).Column("UncommittedValue").Length(5000);
            Map(Reveal.Member<Property>("_committedTrashed")).Column("CommittedTrashed");
            Map(Reveal.Member<Property>("_uncommittedTrashed")).Column("UncommittedTrashed");
        }
    }
}