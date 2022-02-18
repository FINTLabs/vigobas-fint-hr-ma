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
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Felles.Kompleksedatatyper;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR

{
    class HRPersonalressursFactory
    {
        public static HRPersonalressurs Create(Personalressurs personalressurs)
        {
            var ansattnummer = personalressurs.Ansattnummer.Identifikatorverdi;
            var ansettelsesperiodeStart = personalressurs?.Ansettelsesperiode.Start;
            var ansettelsesperiodeSlutt = personalressurs?.Ansettelsesperiode.Slutt;
            var brukernavn = personalressurs?.Brukernavn.Identifikatorverdi;


            //var bilde = person?.Bilde;
            //var fodselsdato = person?.Fodselsdato;
            //var fodselsnummer = person.Fodselsnummer.Identifikatorverdi;
            //var navnFornavn = person.Navn.Fornavn;
            //var navnMellomnavn = person.Navn?.Mellomnavn;
            //var navnEtternavn = person.Navn.Etternavn;


            return new HRPersonalressurs
            {
                PersonalAnsattnummer = ansattnummer,
                PersonalAnsettelsesperiodeStart = ansettelsesperiodeStart,
                PersonalAnsettelsesperiodeSlutt = ansettelsesperiodeSlutt,
                PersonalBrukernavn = brukernavn //,
                //PersonBilde = bilde,
                //PersonFodselsdato = fodselsdato,
                //PersonFodselsnummer = fodselsnummer,
                //PersonNavnFornavn = navnFornavn,
                //PersonNavnMellomnavn = navnMellomnavn,
                //PersonNavnEtternavn = navnEtternavn
            };
        }
    }
}
