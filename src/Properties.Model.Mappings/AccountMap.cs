﻿//  Copyright 2012 wsky. wskyhx@gmail.com
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, 
//  software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and limitations under the License.

using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace Properties.Model.Mappings
{
    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Properties_Account");
            Id(m => m.ID);
            Map(m => m.Name).Length(255);
            Map(m => m.CreateTime);
            Map(Reveal.Member<Account>("_password")).Column("Password").Length(255);
        }
    }
}