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
using FINT.Model.Administrasjon.Kodeverk;
using FINT.Model.Felles;
using FINT.Model.Resource;

using FINT.Model.Felles.Kodeverk.ISO;
using System.Collections.Generic;

using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;
using System;

namespace VigoBAS.FINT.HR
{
    class HRPersonFactory
    {
        public static HRPerson Create(
            string ansattUri, 
            Person person, 
            PersonalressursResource personalressurs, 
            bool usernameToLowerCase, 
            int noOfActiveEmployments,
            Dictionary<string, 
            List<string>> leaderOfUnitDict, 
            Sprak sprakmorsmal, 
            Sprak sprakmalform, 
            Kjonn kjonn, 
            Landkode land, 
            HovedstillingInfo hovedStillingInfo, 
            List<string> arbeidsstedUris,
            List<string> businessUnitUris,
            string managerUri,
            string organizationUri)
        {
            var bilde = person?.Bilde;
            var fodselsdato = person?.Fodselsdato;
            var fodselsnummer = person.Fodselsnummer.Identifikatorverdi;
            var fornavn = person.Navn.Fornavn;
            var mellomnavn = person.Navn?.Mellomnavn;
            var etternavn = person.Navn.Etternavn;
            var bostedsadresseAdresselinje = person?.Bostedsadresse?.Adresselinje;
            var bostedsadressePostnummer = person?.Bostedsadresse?.Postnummer;
            var bostedsadressePoststed = person?.Bostedsadresse?.Poststed;
            var privatEpostadresse = person?.Kontaktinformasjon?.Epostadresse;
            var privatMobilnummer = person?.Kontaktinformasjon?.Mobiltelefonnummer;
            var privatnettsted = personalressurs?.Kontaktinformasjon?.Nettsted;
            var postadresseAdresselinje = person?.Postadresse?.Adresselinje;
            var postadressePostnummer = person?.Postadresse?.Postnummer;
            var postadressePoststed = person?.Postadresse?.Poststed;

            var ansattnummer = personalressurs?.Ansattnummer?.Identifikatorverdi;
            
            var systemId = personalressurs?.SystemId?.Identifikatorverdi;
            var brukernavn = personalressurs?.Brukernavn?.Identifikatorverdi;

            if (!string.IsNullOrEmpty(brukernavn) && usernameToLowerCase)
            {
                brukernavn = brukernavn.ToLower();
            }
            var epostadresse = personalressurs?.Kontaktinformasjon?.Epostadresse;
            var mobiltelefonnummer = personalressurs?.Kontaktinformasjon?.Mobiltelefonnummer;
           
            var telefonnummer = personalressurs?.Kontaktinformasjon?.Telefonnummer;
            var sip = personalressurs?.Kontaktinformasjon?.Sip;

            var morsmalNavn = sprakmorsmal?.Navn;
            var morsmalKode = sprakmorsmal?.Kode;

            var malformNavn = sprakmalform?.Navn;
            var malformKode = sprakmalform?.Kode;

            var kjonnNavn = kjonn?.Navn;
            var kjonnKode = kjonn?.Kode;

            var landNavn = land?.Navn;
            var landKode = land?.Kode;

            var hovedstillingId = hovedStillingInfo?.HovedstillingsID;
            var hovedstillingTittel = hovedStillingInfo?.HovedstillingsTittel;
            var hovedstillingOrgEnhetUri = hovedStillingInfo?.HovedstillingOrgUri;
            var hovedstillingBusinessUnitUri = hovedStillingInfo?.HovedstillingBusinessUnitUri;
            var hovedstillingOrgEnhetId = hovedStillingInfo.HovedstillingOrgID;
            var hovedstillingOrgEnhetKode = hovedStillingInfo?.HovedstillingOrgKode;
            var hovedstillingOrgEnhetNavn = hovedStillingInfo?.HovedstillingOrgNavn;
            var hovedstillingArbeidsforholdtype = hovedStillingInfo?.HovedstillingArbeidsforholdtype;
            var hovedstillingStillingskode = hovedStillingInfo?.HovedstillingStillingskode;
            var hovedstillingStillingskodeNavn = hovedStillingInfo?.HovedstillingStillingskodeNavn;
            var hovedstillingStillingfunksjonNavn = hovedStillingInfo?.HovedstillingFunksjonNavn;

            var leadedOrgUnits = new List<string>();

            if (leaderOfUnitDict.TryGetValue(ansattUri, out List<string> orgelementer))
            {
                leadedOrgUnits = orgelementer;
            }

            var employments = new List<string>();
            var personalressurskategori = string.Empty;

            var personalressursLinks = personalressurs.Links;

            if (personalressursLinks.TryGetValue(ResourceLink.arbeidsforhold, out List<Link> employmentLinks))
            {
                foreach (var employmentLink in employmentLinks)
                {
                    var employmentUri = employmentLink.href;
                    employments.Add(employmentUri);
                }
            }
            if (personalressursLinks.TryGetValue(ResourceLink.personalressurskategori, out List<Link> personalressurskategoriLink))
            {
                personalressurskategori = GetIdValueFromLink(personalressurskategoriLink);
            }


            return new HRPerson
            {
                PersonalAnsattnummerUri = ansattUri,
                PersonFodselsdato = fodselsdato?.ToString(dateFormat),
                PersonFodselsnummer = fodselsnummer,
                PersonNavnFornavn = fornavn,
                PersonNavnMellomnavn = mellomnavn,
                PersonNavnEtternavn = etternavn,
                PersonBostedsadresseAdresselinje = bostedsadresseAdresselinje,
                PersonMobilnummer = privatMobilnummer,
                PersonEpostadresse = privatEpostadresse,
                PersonNettsted = privatnettsted,
                PersonBostedsadressePostnummer = bostedsadressePostnummer,
                PersonBostedsadressePoststed = bostedsadressePoststed,
                PersonPostadresseAdresselinje = postadresseAdresselinje,
                PersonPostadressePostnummer = postadressePostnummer,
                PersonPostadressePoststed = postadressePoststed,

                PersonalAnsattnummer = ansattnummer,
                PersonalPersonalressurskategori = personalressurskategori,
                PersonalSystemId = systemId,
                PersonalBrukernavn = brukernavn,
                PersonalKontaktinformasjonEpostadresse = epostadresse,
                PersonalKontaktinformasjonMobiltelefonnummer = mobiltelefonnummer,
                PersonalKontaktinformasjonTelefonnummer = telefonnummer,
                PersonalKontaktinformasjonSip = sip,

                PersonMorsmalNavn = morsmalNavn,
                PersonMorsmalKode = morsmalKode,

                PersonMalformNavn = malformNavn,
                PersonMalformKode = malformKode,

                PersonKjonnKode = kjonnKode,
                PersonKjonnNavn = kjonnNavn,

                PersonLandKode = landKode,
                PersonLandNavn = landNavn,

                PersonHovedstillingRef = hovedstillingId,
                PersonHovedstillingArbeidsforholdtype = hovedstillingArbeidsforholdtype,
                PersonHovedstillingStillingskode = hovedstillingStillingskode,
                PersonHovedstillingStillingskodeNavn = hovedstillingStillingskodeNavn,
                PersonHovedstillingStillingfunksjonNavn = hovedstillingStillingfunksjonNavn,
                PersonHovedstillingTittel = hovedstillingTittel,
                PersonHovedstillingArbeidsstedUri = hovedstillingOrgEnhetUri,
                PersonalPrimaryBusinessUnitRef = hovedstillingBusinessUnitUri,
                PersonHovedstillingArbeidsstedOrganisasjonsId = hovedstillingOrgEnhetId,
                PersonHovedstillingArbeidsstedOrganisasjonsKode = hovedstillingOrgEnhetKode,
                PersonHovedstillingArbeidsstedNavn = hovedstillingOrgEnhetNavn,
                PersonHovedstillingLeder = managerUri,
                PersonalArbeidsstedRefs = arbeidsstedUris,
                PersonalBusinessUnitRefs = businessUnitUris,
                PersonalHROrganisasjonRef = organizationUri,

                PersonEmployments = employments,
                PersonalAntallAktiveArbeidsforhold = noOfActiveEmployments,
                PersonalLederOrganisasjonselementer = leadedOrgUnits
            };
        }
    }
}
