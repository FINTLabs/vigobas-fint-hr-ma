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

using FINT.Model.Administrasjon.Personal;
using System;
using FINT.Model.Administrasjon.Kodeverk;
using FINT.Model.Administrasjon.Organisasjon;
using FINT.Model.Resource;
using System.Collections.Generic;

using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class HREmploymentFactory
    {
        public static HREmployment Create(
            string arbeidsforholdUri, 
            ArbeidsforholdResource Arbeidsforhold, 
            PersonalressursResource personalressursResource,
            Stillingskode Stillingskode, 
            Arbeidsforholdstype Arbeidsforholdstype, 
            HRUnit Arbeidssted,
            string standardBusinessUnitUri, Funksjon Funksjon)
        {
            var arbeidforholdSystemId = Arbeidsforhold.SystemId.Identifikatorverdi;
            var ansettelsesprosent = Arbeidsforhold?.Ansettelsesprosent;
            var stilingstittel = Arbeidsforhold?.Stillingstittel;
            var stillingnummer = Arbeidsforhold?.Stillingsnummer;
            var hovedstilling = Arbeidsforhold?.Hovedstilling;
            var stillingskodeKode = Stillingskode?.Kode;
            var gyldighetsperiodeStart = Arbeidsforhold.Gyldighetsperiode.Start;
            var gyldighetsperiodeSlutt = Arbeidsforhold?.Gyldighetsperiode?.Slutt;
            var arbeidsforholdsperiodeStart = Arbeidsforhold?.Arbeidsforholdsperiode?.Start;
            var arbeidsforholdsperiodeSlutt = Arbeidsforhold?.Arbeidsforholdsperiode?.Slutt;
            var arbforholdTypeNavn = Arbeidsforholdstype?.Navn;
            var arbforholdTypeKode = Arbeidsforholdstype?.Kode;

            var arbeidsstedOrgID = Arbeidssted?.OrganisasjonselementOrganisasjonsid;
            var arbeidsstedOrgKode = Arbeidssted?.OrganisasjonselementOrganisasjonsKode;
            var arbeidsstedsUri = Arbeidssted.UnitUri;
            
            var ansattnummer = personalressursResource.Ansattnummer.Identifikatorverdi;
            var personalressursIDUri = string.Empty;
            var arbeidsforholdtypeIDUri = string.Empty;
            var arbeidsforholdTypeSystemID = string.Empty;

            var arbeidsforholdFunksjonKode = Funksjon?.Kode;
            var arbeidsforholdFunksjonNavn = Funksjon?.Navn;
            var arbeidsforholdFunksjonPassiv = Funksjon?.Passiv;
            var arbeidsfoholdSystemId = Funksjon.SystemId.Identifikatorverdi;

            var links = Arbeidsforhold.Links;

            if (links.TryGetValue(ResourceLink.personalressurs, out List<Link> personalressursLink))
            {
                personalressursIDUri = LinkToString(personalressursLink);
            }
            if (links.TryGetValue(ResourceLink.arbeidsforholdstype, out List<Link> arbeidsforholdtypeLink))
            {
                arbeidsforholdtypeIDUri = LinkToString(arbeidsforholdtypeLink);
                arbeidsforholdTypeSystemID = GetIdValueFromLink(arbeidsforholdtypeIDUri);
            }

            return new HREmployment
            {
                ArbeidforholdUri = arbeidsforholdUri,
                ArbeidforholdSystemId = arbeidforholdSystemId,
                Ansettelsesprosent = ansettelsesprosent?.ToString(),
                GyldighetsperiodeStart = gyldighetsperiodeStart,
                GyldighetsperiodeSlutt = gyldighetsperiodeSlutt,
                ArbeidsforholdsperiodeStart = arbeidsforholdsperiodeStart,
                ArbeidsforholdsperiodeSlutt = arbeidsforholdsperiodeSlutt,
                Stillingstittel = stilingstittel,
                StillingNummer = stillingnummer,
                Hovedstilling = hovedstilling,
                ArbeidsforholdstypeNavn = arbforholdTypeNavn,
                ArbeidsforholdstypeKode = arbforholdTypeKode,
                ArbeidsforholdstypeSystemId = arbeidsforholdTypeSystemID,
                ArbeidsforholdstypeSystemIdUri = arbeidsforholdtypeIDUri,
                StillingkodeKode = stillingskodeKode,
                ArbeidsstedOrgID = arbeidsstedOrgID,
                ArbeidsstedOrgKode = arbeidsstedOrgKode,
                ArbeidsstedOrganisasjonsIdUri = arbeidsstedsUri,
                ArbeidsforholdPersonalressursRef = personalressursIDUri,
                ArbeidsforholdPersonalressursAnsattnummer = ansattnummer,
                ArbeidsforholdFunksjonKode = arbeidsforholdFunksjonKode,
                ArbeidsforholdFunksjonNavn = arbeidsforholdFunksjonNavn,
                ArbeidsforholdFunksjonPassiv = arbeidsforholdFunksjonPassiv,
                ArbeidsforholdFunksjonSystemId = arbeidsfoholdSystemId
            };
        }
    }
}
