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
using FINT.Model.Resource;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class PersonalressursRecourceFactory
    {
        public static PersonalressursResource Create(IEmbeddedResourceObject personalressursData, List<string> activeEmployments)
        {
            var ansattnummer = new Identifikator();
            var ansettelsesperiode = new Periode();
            var brukernavn = new Identifikator();
            var kontaktinformasjon = new Kontaktinformasjon();
            var systemId = new Identifikator();

            var values = personalressursData.State;

            ansattnummer = GetFintIdentifikatorFromHalAttribute(values, FintAttribute.ansattnummer);
            systemId = GetFintIdentifikatorFromHalAttribute(values, FintAttribute.systemId);
            brukernavn = GetFintIdentifikatorFromHalAttribute(values, FintAttribute.brukernavn);

            if (values.TryGetValue(FintAttribute.ansettelsesperiode, out IStateValue ansperdictVal))
            {
                ansettelsesperiode =
                    JsonConvert.DeserializeObject<Periode>(ansperdictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontindictVal))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontindictVal.Value);
            }

            var personalressursRecource = new PersonalressursResource
            {
                Ansattnummer = ansattnummer,
                Ansettelsesperiode = ansettelsesperiode,
                Brukernavn = brukernavn,
                Kontaktinformasjon = kontaktinformasjon,
                SystemId = systemId
            };

            var links = personalressursData.Links;

            var personUriList = GetStringListFromHalLink(links, ResourceLink.person);

            foreach (var personUri in personUriList)
            {
                var link = Link.with(personUri);
                personalressursRecource.AddPerson(link);
            }

            var personalressurskategoriList = GetStringListFromHalLink(links, ResourceLink.personalressurskategori);

            foreach (var personalressurskategoriUri in personalressurskategoriList)
            {
                var link = Link.with(personalressurskategoriUri);
                personalressursRecource.AddPersonalressurskategori(link);
            }

            var employmentsToAdd = new List<string>();

            if (activeEmployments != null)
            {
                employmentsToAdd = activeEmployments;
            }
            else
            {
                employmentsToAdd = GetStringListFromHalLink(links, ResourceLink.arbeidsforhold);
            }
            foreach (var employment in employmentsToAdd)
            {
                var link = Link.with(employment);
                personalressursRecource.AddArbeidsforhold(link);
            }

            return personalressursRecource;
        }
    }
}
