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
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Personal;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class ArbeidsforholdFactory
    {
        public static Arbeidsforhold Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            long ansettelsesprosent = 0;
            var gyldighetsperiode = new Periode();
            var arbeidsforholdperiode = new Periode();
            bool hovedstilling = false;
            string stillingsnummer = string.Empty;
            string stillingstittel = string.Empty;
            string funksjon = string.Empty();
            var systemId = new Identifikator();

            if (values.TryGetValue(FintAttribute.ansettelsesprosent, out IStateValue ansdictVal))
            {
                ansettelsesprosent = long.Parse(ansdictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue gyldictVal))
            {
                gyldighetsperiode = JsonConvert.DeserializeObject<Periode>(gyldictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.arbeidsforholdsperiode, out IStateValue arbperdictVal))
            {
                arbeidsforholdperiode = JsonConvert.DeserializeObject<Periode>(arbperdictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.hovedstilling, out IStateValue hovstdictVal))
            {
                hovedstilling = Convert.ToBoolean(hovstdictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.stillingsnummer, out IStateValue stilnrdictVal))
            {
                stillingsnummer = stilnrdictVal.Value;
            }
            if (values.TryGetValue(FintAttribute.stillingstittel, out IStateValue stiltitdictVal))
            {
                stillingstittel = stiltitdictVal.Value;
            }
            if (values.TryGetValue(FintAttribute.funksjon, out IStateValue funksjdictVal))
            {
                funksjon = funksjdictVal.Value;
            }
            if (values.TryGetValue(FintAttribute.systemId, out IStateValue sysiddictVal))
            {
                systemId = JsonConvert.DeserializeObject<Identifikator>(sysiddictVal.Value);
            }

            return new Arbeidsforhold
            {
                Ansettelsesprosent = ansettelsesprosent,
                Gyldighetsperiode = gyldighetsperiode,
                Arbeidsforholdsperiode = arbeidsforholdperiode,
                Hovedstilling = hovedstilling,
                Stillingsnummer = stillingsnummer,
                Stillingstittel = stillingstittel,
                Funksjon = funksjon,
                SystemId = systemId
            };
        }
    }
}
