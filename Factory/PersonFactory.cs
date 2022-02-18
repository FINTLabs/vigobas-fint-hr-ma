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
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR
{
    public class PersonFactory
    {
        public static Person Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var kontaktinformasjon = new Kontaktinformasjon();
            var postadresse = new Adresse();
            var bostedsadresse = new Adresse();
            var fodselsnummer = new Identifikator();
            DateTime fodselsdato = new DateTime();
            var navn = new Personnavn();

            if (values.TryGetValue("kontaktinformasjon", out IStateValue kondictVal))
            {
                kontaktinformasjon = JsonConvert.DeserializeObject<Kontaktinformasjon>(kondictVal.Value);
            }
            if (values.TryGetValue("postadresse", out IStateValue psadictVal))
            {
                postadresse = JsonConvert.DeserializeObject<Adresse>(psadictVal.Value);
            }
            if (values.TryGetValue("bostedsadesse", out IStateValue bsadictVal))
            {
                bostedsadresse = JsonConvert.DeserializeObject<Adresse>(bsadictVal.Value);
            }
            if (values.TryGetValue("fodselsnummer", out IStateValue fndictVal))
            {
                fodselsnummer = JsonConvert.DeserializeObject<Identifikator>(fndictVal.Value);
            }
            if (values.TryGetValue("fodselsdato", out IStateValue fddictVal))
            {
                fodselsdato = DateTime.Parse(fddictVal.Value);
            }
            if (values.TryGetValue("navn", out IStateValue ndictVal))
            {
                navn = JsonConvert.DeserializeObject<Personnavn>(ndictVal.Value);
            }
            return new Person
            {
                Kontaktinformasjon = kontaktinformasjon,
                Postadresse = postadresse,
                Bostedsadresse = bostedsadresse,
                Fodselsnummer = fodselsnummer,
                Fodselsdato = fodselsdato,
                Navn = navn
            };
        }


    }
}