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
using FINT.Model.Resource;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class ArbeidsforholdResourceFactory
    {
        public static ArbeidsforholdResource Create(IEmbeddedResourceObject arbeidsforholdData)
        {
            long ansettelsesprosent = 0;
            var gyldighetsperiode = new Periode();
            var arbeidsforholdperiode = new Periode();
            bool hovedstilling = false;
            string stillingsnummer = string.Empty;
            string stillingstittel = string.Empty;
            var systemId = new Identifikator();

            var values = arbeidsforholdData.State;

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
            else
            {
                arbeidsforholdperiode = null;
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
            if (values.TryGetValue(FintAttribute.systemId, out IStateValue sysiddictVal))
            {
                systemId = JsonConvert.DeserializeObject<Identifikator>(sysiddictVal.Value);
            }

            var arbeidsforholdResource = new ArbeidsforholdResource
            {
                Ansettelsesprosent = ansettelsesprosent,
                Gyldighetsperiode = gyldighetsperiode,
                Arbeidsforholdsperiode = arbeidsforholdperiode,
                Hovedstilling = hovedstilling,
                Stillingsnummer = stillingsnummer,
                Stillingstittel = stillingstittel,
                SystemId = systemId
            };

            var links = arbeidsforholdData.Links;

            if (links.TryGetValue(ResourceLink.personalressurs, out IEnumerable<ILinkObject> personalressursLink))
            {
                var arbeidsstedUri = LinkToString(personalressursLink);
                var fintLink = Link.with(arbeidsstedUri);
                arbeidsforholdResource.AddPersonalressurs(fintLink);
            }
            if (links.TryGetValue(ResourceLink.arbeidssted, out IEnumerable<ILinkObject> arbeidsstedLink))
            {
                var arbeidsstedUri = LinkToString(arbeidsstedLink);
                var fintLink = Link.with(arbeidsstedUri);
                arbeidsforholdResource.AddArbeidssted(fintLink);
            }
            if (links.TryGetValue(ResourceLink.arbeidsforholdstype, out IEnumerable<ILinkObject> arbeidsforholdtypeLink))
            {
                var arbeidsforholdtypeUri = LinkToString(arbeidsforholdtypeLink);
                var fintLink = Link.with(arbeidsforholdtypeUri);
                arbeidsforholdResource.AddArbeidsforholdstype(fintLink);
            }
            if (links.TryGetValue(ResourceLink.stillingskode, out IEnumerable<ILinkObject> stillingskodeLink))
            {
                var stillingskodeUri = LinkToString(stillingskodeLink);
                var fintLink = Link.with(stillingskodeUri);
                arbeidsforholdResource.AddStillingskode(fintLink);
            }

            return arbeidsforholdResource;
        }
    }
}
