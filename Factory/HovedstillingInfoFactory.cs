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
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Administrasjon.Organisasjon;
using FINT.Model.Administrasjon.Kodeverk;
using FINT.Model.Resource;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;


namespace VigoBAS.FINT.HR
{
    class HovedstillingInfoFactory
    {
        // new HovedstillingInfo { HovedstillingsID = arbeidforholdUri, HovedstillingsTittel = hovedstillingstittel, HovedstillingsOrgID = arbeidsstedUri }

        public static HovedstillingInfo Create(HREmployment arbeidsforhold) //, string arbeidforholdUri,ArbeidsforholdResource arbeidsforholdResource, Stillingskode stillingskode, OrganisasjonselementResource organisasjonselement)
        {
            var hovedstillingstittel = arbeidsforhold?.Stillingstittel;            
            var stillingskodeKode = arbeidsforhold.StillingkodeKode;
            var stillingskodeNavn = arbeidsforhold.StillingkodeNavn;

            var arbeidsstedId = arbeidsforhold.ArbeidsstedOrgID;
            var arbeidsstedKode = arbeidsforhold.ArbeidsstedOrgKode;
            var arbeidsstedNavn = arbeidsforhold.ArbeidsstedNavn;

            var arbeidsstedUri = arbeidsforhold.ArbeidsstedOrganisasjonsIdUri;
            var businessUnitUri = arbeidsforhold?.ArbeidsstedBusinessUnitUri;
            var arbeidforholdUri = arbeidsforhold.ArbeidforholdUri;
            var arbeidforholdType = arbeidsforhold.ArbeidsforholdstypeSystemId;

            var funksjonKode = arbeidsforhold.ArbeidsforholdFunksjonKode;
            var funksjonNavn = arbeidsforhold.ArbeidsforholdFunksjonNavn;
            var funksjonPassiv = arbeidsforhold.ArbeidsforholdFunksjonPassiv;
            var funksjonSystemId = arbeidsforhold.ArbeidsforholdFunksjonSystemId;

            //var links = arbeidsforholdResource.Links;
            
            //if (links.TryGetValue(ResourceLink.arbeidssted, out List<Link> arbeidsstedLink))
            //{
            //    arbeidsstedUri = LinkToString(arbeidsstedLink);
            //}
            //if (links.TryGetValue(ResourceLink.arbeidsforholdstype, out List<Link> arbeidsforholdstypeLink))
            //{
            //    arbeidforholdType = GetIdValueFromLink(arbeidsforholdstypeLink);
            //}
            //if (links.TryGetValue(ResourceLink.stillingskode, out List<Link> stillingskodeLink))
            //{
            //    stillingskodeId = GetIdValueFromLink(stillingskodeLink);
            //}

            return new HovedstillingInfo
            {
                HovedstillingsID = arbeidforholdUri,
                HovedstillingsTittel = hovedstillingstittel,
                HovedstillingOrgUri = arbeidsstedUri,
                HovedstillingBusinessUnitUri = businessUnitUri,

                HovedstillingOrgID = arbeidsstedId,
                HovedstillingOrgKode = arbeidsstedKode,
                HovedstillingOrgNavn = arbeidsstedNavn,

                HovedstillingArbeidsforholdtype = arbeidforholdType,
                HovedstillingStillingskode = stillingskodeKode,
                HovedstillingStillingskodeNavn = stillingskodeNavn,
                
                HovedstillingFunksjonKode = funksjonKode,
                HovedstillingFunksjonNavn = funksjonNavn,
                HovedstillingFunksjonPassiv = funksjonPassiv,
                HovedstillingFunksjonSystemId = funksjonSystemId
            };
        }
    }
}
