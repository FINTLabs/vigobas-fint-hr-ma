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

using System.Collections.Generic;
using System;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Kodeverk;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class ArbeidsforholdtypeFactory
    {
        public static Arbeidsforholdstype Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var gyldighetsperiode = new Periode();
            string kode = "";
            string navn = "";
            var systemId = new Identifikator();

            if (values.TryGetValue(FintAttribute.kode, out IStateValue kodedictVal))
            {
                kode = kodedictVal.Value;
            }
            if (values.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue gyldictVal))
            {
                gyldighetsperiode = JsonConvert.DeserializeObject<Periode>(gyldictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.navn, out IStateValue navndictVal))
            {
                navn = navndictVal.Value;
            }
            if (values.TryGetValue(FintAttribute.systemId, out IStateValue sysiddictVal))
            {
                systemId = JsonConvert.DeserializeObject<Identifikator>(sysiddictVal.Value);
            }

            return new Arbeidsforholdstype
            {
                Gyldighetsperiode = gyldighetsperiode,
                Navn = navn,
                Kode = kode,               
                SystemId = systemId,
            };
        }
    }
}


