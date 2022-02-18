// VIGOBAS Identity Management System 
//  Copyright (C) 2022  Vigo IKS 
//  
//  Documentation - visit https://vigobas.vigoiks.no/ 
//  
//  This program is free software: you can redistribute it and/or modify 
//  it under the terms of the GNU Affero General Public License as 
//  published by the Free Software Foundation, either version 3 of the 
//  License, or (at your option) any later version. 
//  
//  This program is distributed in the hope that it will be useful, 
//  but WITHOUT ANY WARRANTY, without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//  GNU Affero General Public License for more details. 
//  
//  You should have received a copy of the GNU Affero General Public License 
//  along with this program.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Administrasjon.Kodeverk;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR
{
    class TimerperukeFactory
    {
        public static Uketimetall Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var gyldighetsperiode = new Periode();
            string Timekode = "";
            string Timenavn = "";
            bool passiv = true;
            var TimesystemId = new Identifikator();

            if (values.TryGetValue("gyldighetsperiode", out IStateValue gyldictVal))
            {
                gyldighetsperiode = JsonConvert.DeserializeObject<Periode>(gyldictVal.Value);
            }
            
            if (values.TryGetValue("kode", out IStateValue kodedictVal))
            {
                Timekode = kodedictVal.Value;
            }
            if (values.TryGetValue("navn", out IStateValue navndictVal))
            {
                Timenavn = navndictVal.Value;
            }
            if (values.TryGetValue("passiv", out IStateValue passivdictVal))
            {
                passiv = Convert.ToBoolean(passivdictVal.Value);
            }
            if (values.TryGetValue("systemId", out IStateValue sysiddictVal))
            {
                TimesystemId = JsonConvert.DeserializeObject<Identifikator>(sysiddictVal.Value);
            }

            return new Uketimetall
            {
                Gyldighetsperiode = gyldighetsperiode,
                Kode = Timekode,
                Navn = Timenavn,
                SystemId = TimesystemId,
                Passiv = passiv
            };
        }
    }
}
