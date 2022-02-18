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
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Personal;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR
{
    class PersonalressursFactory
    {
        public static Personalressurs Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var ansattnummer = new Identifikator();
            var ansettelsesperiode = new Periode();
            var brukernavn = new Identifikator();
            var kontaktinformasjon = new Kontaktinformasjon();
            var systemId = new Identifikator();

            if (values.TryGetValue("ansattnummer", out IStateValue ansnrdictVal))
            {
                ansattnummer =
                    JsonConvert.DeserializeObject<Identifikator>(ansnrdictVal.Value);
            }
            if (values.TryGetValue("ansettelsesperiode", out IStateValue ansperdictVal))
            {
                ansettelsesperiode =
                    JsonConvert.DeserializeObject<Periode>(ansperdictVal.Value);
            }
            if (values.TryGetValue("brukernavn", out IStateValue brkndictVal))
            {
                brukernavn =
                    JsonConvert.DeserializeObject<Identifikator>(brkndictVal.Value);
            }
            if (values.TryGetValue("kontaktinformasjon", out IStateValue kontindictVal))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontindictVal.Value);
            }
            if (values.TryGetValue("systemId", out IStateValue sysiddictVal))
            {
                systemId =
                    JsonConvert.DeserializeObject<Identifikator>(sysiddictVal.Value);
            }

            return new Personalressurs
            {
                Ansattnummer = ansattnummer,
                Ansettelsesperiode = ansettelsesperiode,
                Brukernavn = brukernavn,
                Kontaktinformasjon = kontaktinformasjon,
                SystemId = systemId
            };
        }
    }
}
