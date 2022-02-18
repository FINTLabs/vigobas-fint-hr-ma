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
using FINT.Model.Felles.Basisklasser;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Administrasjon.Organisasjon;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR
{
    class ArbeidsstedFactory
    {
        public static Organisasjonselement Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var OrganisasjonselementPostadresse = new Adresse();
            var OrganisasjonselementForretningsadresse = new Adresse();
            var OrganisasjonselementKontaktinformasjon = new Kontaktinformasjon();
            var OrganisasjonselementOrganisasjonsnavn = "";
            var OrganisasjonselementOrganisasjonsnummer = new Identifikator();
            var OrganisasjonselementGyldighetsperiode = new Periode();
            var OrganisasjonselementKortnavn = "";
            var OrganisasjonselementNavn = "";
            var OrganisasjonselementOrganisasjonsId = new Identifikator();

            if (values.TryGetValue("postadresse", out IStateValue postadrdictVal))
            {
                OrganisasjonselementPostadresse = JsonConvert.DeserializeObject<Adresse>(postadrdictVal.Value);
            }
            if (values.TryGetValue("forretningsadresse", out IStateValue forradrdictVal))
            {
                OrganisasjonselementForretningsadresse = JsonConvert.DeserializeObject<Adresse>(forradrdictVal.Value);
            }
            if (values.TryGetValue("kontaktinformasjon", out IStateValue kontindictVal))
            {
                OrganisasjonselementKontaktinformasjon = JsonConvert.DeserializeObject<Kontaktinformasjon>(kontindictVal.Value);
            }
            if (values.TryGetValue("organisasjonsnavn", out IStateValue orgnavndictVal))
            {
                OrganisasjonselementOrganisasjonsnavn = orgnavndictVal.Value;
            }
            if (values.TryGetValue("organisasjonsnummer", out IStateValue orgnrdictVal))
            {
                OrganisasjonselementOrganisasjonsnummer = JsonConvert.DeserializeObject<Identifikator>(orgnrdictVal.Value);
            }
            if (values.TryGetValue("organisasjonsId", out IStateValue orgIddictVal))
            {
                OrganisasjonselementOrganisasjonsId = JsonConvert.DeserializeObject<Identifikator>(orgIddictVal.Value);
            }
            if (values.TryGetValue("gyldighetsperiode", out IStateValue perdictVal))
            {
                OrganisasjonselementGyldighetsperiode = JsonConvert.DeserializeObject<Periode>(perdictVal.Value);
            }
            if (values.TryGetValue("kortnavn", out IStateValue kortndictVal))
            {
                OrganisasjonselementKortnavn = kortndictVal.Value;
            }
            if (values.TryGetValue("navn", out IStateValue navndictVal))
            {
                OrganisasjonselementNavn = navndictVal.Value;
            }

            return new Organisasjonselement
            {
                Postadresse = OrganisasjonselementPostadresse,
                Forretningsadresse = OrganisasjonselementForretningsadresse,
                Kontaktinformasjon = OrganisasjonselementKontaktinformasjon,
                Organisasjonsnavn = OrganisasjonselementOrganisasjonsnavn,
                Organisasjonsnummer = OrganisasjonselementOrganisasjonsnummer,
                OrganisasjonsId = OrganisasjonselementOrganisasjonsId,
                Gyldighetsperiode = OrganisasjonselementGyldighetsperiode,
                Kortnavn = OrganisasjonselementKortnavn,
                Navn = OrganisasjonselementNavn
            };
        }
    }
}
