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

namespace VigoBAS.FINT.HR
{
    class Constants
    {
        public const string dateFormat = "yyyy-MM-dd";
        public const string zeroDate = "1900-01-01";
        public const string infinityDate = "2099-01-01";
        public const int initialDelayMilliseconds = 500;
        public const double factor = 1.5;
        public const int retrylimit = 10;

        public struct Delimiter
        {
            public const char listDelimiter = ';';
            public const char path = '/';
            public const char roleSchool = '@';
            public const char suffix = '_';
        }

        public struct FintAttribute
        {
            public const string systemId = "systemId";
            public const string navn = "navn";
            public const string beskrivelse = "beskrivelse";
            public const string periode = "periode";
            public const string elevnummer = "elevnummer";
            public const string brukernavn = "brukernavn";
            public const string feidenavn = "feidenavn";
            public const string kontaktinformasjon = "kontaktinformasjon";
            public const string postadresse = "postadresse";
            public const string bostedsadresse = "bostedsadresse";
            public const string fodselsnummer = "fodselsnummer";
            public const string fodselsdato = "fodselsdato";
            public const string ansattnummer = "ansattnummer";
            public const string skolenummer = "skolenummer";
            public const string organisasjonsId = "organisasjonsId";
            public const string organisasjonsKode = "organisasjonsKode";
            public const string organisasjonsnummer = "organisasjonsnummer";
            public const string kortnavn = "kortnavn";
            public const string organisasjonsnavn = "organisasjonsnavn";
            public const string domenenavn = "domenenavn";
            public const string juridiskNavn = "juridiskNavn";
            public const string forretningsadresse = "forretningsadresse";
            public const string gyldighetsperiode = "gyldighetsperiode";
            public const string ansettelsesperiode = "ansettelsesperiode";
            public const string arbeidsforholdsperiode = "arbeidsforholdsperiode";
            public const string hovedstilling = "hovedstilling";
            public const string ansettelsesprosent = "ansettelsesprosent";
            public const string lonnsprosent =  "lonnsprosent";
            public const string stillingsnummer = "stillingsnummer";
            public const string stillingstittel = "stillingstittel";
            public const string tilstedeprosent = "tilstedeprosent";
            public const string kode = "kode";
        };


        public struct ResourceLink
        {
            public const string person = "person";
            public const string morsmal = "morsmal";
            public const string malform = "malform";
            public const string kjonn = "kjonn";
            public const string statsborgerskap = "statsborgerskap";
            public const string personalressurs = "personalressurs";
            public const string arbeidssted = "arbeidssted";
            public const string leder = "leder";
            public const string overordnet = "overordnet";
            public const string underordnet = "underordnet";
            public const string ansvar = "ansvar";
            public const string funksjon = "funksjon";
            public const string arbeidsforhold = "arbeidsforhold";
            public const string arbeidsforholdstype = "arbeidsforholdstype";
            public const string stillingskode = "stillingskode";
            public const string timerperuke = "timerperuke";
            public const string self = "self";
            public const string employment = "arbeidsforhold";  
            public const string parent = "overordnet";
            public const string children = "underordnet";
            public const string workplace = "arbeidssted";
            public const string manager = "leder";
            public const string resourceCategory = "personalressurskategori";
            public const string personalressurskategori = "personalressurskategori";
            public const string employmentType = "arbeidsforholdstype";
            public const string positionCode = "stillingskode";            
        }

        public struct Param
        {
            public const string clientId = "ClientId";
            public const string openIdSecret = "OpenIdSecret";
            public const string username = "Username";
            public const string password = "Password";
            public const string assetId = "AssetId";
            //public const string xClient = "xClient";
            public const string scope = "Scope";
            public const string idpUri = "IdpUri";
            public const string felleskomponentUri = "Uri felleskomponent";
            public const string httpClientTimeout = "Http Client Timeout (minutter)";

            public const string useLocalCache = "Importer fra lokal cache";
            public const string abortIfResourceTypeEmpty = "Stopp import hvis en ressurstype er tom";

            public const string useThresholdValues = "Bruk grenseverdier for tillatt endring";
            public const string overrideThresholds = "Overstyr grenseverdiene";

            public const string thresholdValuePerson = "Tillatt endring person";
            public const string thresholdValueArbeidsforhold = "Tillatt endring arbeidsforhold";
            public const string thresholdValueOrganisasjonselementGruppe = "Tillatt endring orgelement/gruppe";
            
            public const string checkUpdateResponse = "Sjekk Update Respons";

            public const string administrasjonPersonalPersonUri = "Uri persondata ansatt";
            public const string administrasjonPersonalPersonalRessursUri = "Uri personalressursdata";
            public const string administrasjonPersonalArbeidsforholdUri = "Uri arbeidsforhold";
            public const string administrasjonOrganisasjonOrganisasjonselementUri = "Uri organisasjonselement";
            //public const string administrasjonArbeidsstedUri = "Uri Arbeidssted";
            public const string administrasjonAnsvarUri = "Uri Ansvar";
            public const string administrasjonFunksjonUri = "Uri Funksjon";
            public const string administrasjonStillingsKodeUri = "Uri Stillingskode";
            public const string administrasjonTimerPrUkeUri = "Uri TimerPrUke";
            public const string administrasjonArbeidsforholdsTypeUri = "Uri Arbeidsforholdtype";
            public const string felleskodeverkSprakUri = "Uri Sprak";
            public const string felleskodeverkKjonnUri = "Uri Kjonn";
            public const string felleskodeverkLandUri = "Uri Land";

            public const string genererAllePersonalKategoriGrupper = "Generer grupper med alle personalkategorier";
            public const string genererOrgEnhetGrupper = "Generer grupper for alle orgenheter";
            public const string excludeEmptyGroups = "Ikke importer tomme grupper";
            public const string leggTilLederSomMedlemIGruppe = "Legg til leder som medlem i gruppe";
            public const string leggTilLedereIOverordnetGruppe = "Legg til leder i overordnet gruppe";
            public const string finnAlltidLederForAnsatt = "Sett leder fra overordet enhet når ansatt mangler direkte leder";
            
            public const string GruppePrefix = "Prefix:";
            public const string GruppeSuffix = "Suffix:";

            public const string gruppeAlleSuffix = "Alle";
            public const string gruppeAlleAnsatteSuffix = "Alle ansatte";
            public const string gruppeAllePolitikereSuffix = "Alle politikere";
            public const string gruppeAlleAndreSuffix = "Alle andre personalressurser";
            public const string gruppeAlleLedereSuffix = "Alle ledere";
            public const string gruppeAlleLarereSuffix = "Alle lærere";
            public const string gruppeAlleAdmSkoleSuffix = "Alle adm-ansatte alle skoler";
            public const string gruppeAlleAdmEnSkoleSuffix = "Alle adm-ansatte en skole";
            public const string gruppeAlleAnsatteIkkeSkoleSuffix = "Alle ansatte utenom skole";
            public const string gruppeAlleAndreSkoleSuffix = "Alle andre personalressurser skole";
            public const string gruppeAlleAndreIkkeSkoleSuffix = "Alle andre personalressurser utenom skole";

            public const string genererAggrGruppeAlleAnsatte = "Generer gruppe Alle ansatte (hele organisasjonen)";
            public const string organisasjonsnavn = "Organisasjonsnavn";
            public const string organisasjonskortnavn = "Organisasjonskortnavn"; 
            public const string organisasjonsnummer = "Organisasjonsnummer";
            public const string organisasjonEpostadresse = "Epostadresse organisasjon";


            public const string standardBusinessUnitName = "Fylkesadministrasjon enhetsnavn";
            public const string standardBusinessUnitOrgNo = "Fylkesadministrasjon organisasjonsnummer";
            public const string standardBusinessUnitEmailAddress = "Fylkesadministrasjon epostadresse";

            public const string gruppeAlleAnsatteRessurskategorier = "Personalressurskategorier ansatte";
            public const string gruppeAlleAnsatteArbeidsforholdtyper = "Arbeidsforholdtyper ansatte";

            public const string genererAggrGruppeAllePolitikere = "Generer gruppe Alle politikere";
            public const string gruppeAllePolitikerRessurskategorier = "Personalressurskategorier politikere";
            public const string gruppeAllePolitikereArbeidsforholdtyper = "Arbeidsforholdtyper politikere";

            public const string genererAggrGruppeAlleAndre = "Generer gruppe Alle andre personalressurser";
            public const string gruppeAlleAndreRessurskategorier = "Personalressurskategorier alle andre";
            public const string gruppeAlleAndreArbeidsforholdtyper = "Arbeidsforholdtyper alle andre";

            public const string genererAggrGruppeAlleLedere = "Generer gruppe Alle Ledere";
            public const string genererAggrGrupperAlleLarerAdmin = "Generer grupper Alle lærere/admin skole/ikke skole";

            public const string brukOrganisasjonsKode = "Organisasjonskode er enhets-id for konfigurasjonsparametrene";

            public const string alleSkoleEnheter = "Alle skoleenheter";

            public const string genererLarerAdmAnsGrupper = "Generer grupper Alle lærere/administrativt ansatte per skole";
            public const string skoleOrgenheter = "Organisasjonselementer skole";
            public const string larerArbeidsforholdtyper = "Arbeidsforholdtyper lærere";

            public const string genererAggrGrupperForenheter = "Generer aggregerte grupper for utvalgte enheter";
            public const string enheterAggrGrupper = "Enheter for gruppeaggregering";

            public const string UnitPrefix = "Prefix:";
            public const string UnitSuffix = "Suffix:";

            public const string usernameToLowerCase = "Brukernavn små bokstaver";
            public const string employmentCompareDate= "Sammenlikningsdato arbeidsforhold";
            public const string daysBeforeEmploymentStarts = "Antall dager før aktivt arbeidsforhold";
            public const string daysAfterEmploymentEnds = "Antall dager etter aktivt arbeidsforhold";
            public const string filterResourceTypes = "Filter personalressurskategori";
            public const string filterEmploymentTypes = "Filter Arbeidsforholdtype";
            public const string filterEmploymentTypesInActiveUsers = "Filter Arbeidsforholdtype midlertidig deaktivert bruker";
            public const string filterPositionCodes = "Filter stillingskode";

            public const string waitTime = "Ventetid oppdateringskall";
            public const string lowerLimit = "Nedre grense statuskø";
            public const string upperLimit = "Øvre grense statuskø";
        }

        public struct DefaultValue
        {
            public const string xClient = "test";
            public const string scope = "fint-client";
            public const string httpClientTimeout = "2";
            public const string felleskomponentUri = "https://beta.felleskomponent.no";
            public const string accessTokenUri = "https://idp.felleskomponent.no/nidp/oauth/nam/token";
            public const string administrasjonPersonalPersonUri = "/administrasjon/personal/person";
            public const string administrasjonPersonalPersonalRessursUri = "/administrasjon/personal/personalressurs";
            public const string administrasjonPersonalArbeidsforholdUri = "/administrasjon/personal/arbeidsforhold";
            public const string administrasjonOrganisasjonOrganisasjonselementUri = "/administrasjon/organisasjon/organisasjonselement";
            //public const string administrasjonrArbeidsstedUri = "/administrasjon/organisasjon/organisasjonselement";
            public const string administrasjonAnsvarUri = "/administrasjon/kodeverk/ansvar";
            public const string administrasjonFunksjonUri = "/administrasjon/kodeverk/funksjon";
            public const string administrasjonStillingsKodeUri = "/administrasjon/kodeverk/stillingskode";
            public const string administrasjonArbeidsforholdstypeUri = "/administrasjon/kodeverk/arbeidsforholdstype";
            public const string administrasjonUkeTimeTallUri = "/administrasjon/kodeverk/uketimetall";
            public const string felleskodeverkSprakUri = "/felles/kodeverk/sprak";
            public const string felleskodeverkKjonnUri = "/felles/kodeverk/kjonn";
            public const string felleskodeverkLandkodeUri = "/felles/kodeverk/landkode";
            public const string GruppePrefix = "FINT_";
            public const string GruppeSuffix = "_GRP";
            public const string UnitPrefix = "FINT_";
            public const string UnitSuffix = "_UNIT";
            public const string allPersonalresourcesSuffix = "alle personalressurser";
            public const string allEmployeesSuffix = "alle ansatte";
            public const string allPolitiansSuffix = "alle politikere";
            public const string allOthersSuffix = "alle andre personalressurser";
            public const string allLeadersSuffix = "alle ledere";
            public const string allTeachersSuffix = "alle lærere";
            public const string allAdminsOneSchoolSuffix = "alle administrativt ansatte";
            public const string allAdminsSchoolsSuffix = "alle administrativt ansatte skole";
            public const string allAdminsNonSchoolSuffix = "alle administrativt ansatte utenom skole";
            public const string allOthersSchoolSuffix = "alle andre personalressurser skole";
            public const string allOthersNonSchoolSuffix = "alle andre personalressurser utenom skole";
            public const string schoolTag = "_school";
            public const string nonSchoolTag = "_nonschool";
            public const string organizationSuffix = "org";
            public const string businessUnitSuffix = "bisunit";
        }
        
        public struct GroupType
        {
            public const string ougroup = "ougrp";
            public const string aggrEmp = "aggr.emp";
            public const string aggrPol = "aggr.pol";
            public const string aggrOth = "aggr.oth";
            public const string aggrMan = "aggr.man";
            public const string aggrFac = "aggr.fac";
            public const string aggrSta = "aggr.sta";
            public const string aggrAdm = "aggr.adm";
            public const string aggrAll = "aggr.all";
        }

        public struct PersonAttributes
        {
            public const string PersonalAnsattnummerUri = "PersonalAnsattnummerUri";
            public const string PersonalSystemId = "PersonalSystemId";
            public const string PersonalAnsattnummer = "PersonalAnsattnummer";
            public const string PersonalPersonalressurskategori = "PersonalPersonalressurskategori";
            public const string PersonalBrukernavn = "PersonalBrukernavn";
            public const string PersonalKontaktinformasjonEpostadresse = "PersonalKontaktinformasjonEpostadresse";
            public const string PersonalKontaktinformasjonMobiltelefonnummer = "PersonalKontaktinformasjonMobiltelefonnummer";
            public const string PersonalKontaktinformasjonTelefonnummer = "PersonalKontaktinformasjonTelefonnummer";
            public const string PersonalKontaktinformasjonSip = "PersonalKontaktinformasjonSip";
            public const string PersonalLederOrganisasjonselementRef = "PersonalLederOrganisasjonselementRef";
            public const string PersonalOrganisasjonRef = "PersonalOrganisasjonRef";

            public const string PersonalAntallAktiveArbeidsforhold = "PersonalAntallAktiveArbeidsforhold";
            public const string PersonalAktivAnsettelsesperiodeStart = "PersonalAktivAnsettelsesperiodeStart";
            public const string PersonalAktivAnsettelsesperiodeSlutt = "PersonalAktivAnsettelsesperiodeSlutt";

            public const string PersonNavnFornavn = "PersonNavnFornavn";
            public const string PersonNavnMellomnavn = "PersonNavnMellomnavn";
            public const string PersonNavnEtternavn = "PersonNavnEtternavn";
            public const string PersonFodselsdato = "PersonFodselsdato";
            public const string PersonFodselsnummer = "PersonFodselsnummer";
            public const string PersonKontaktinformasjonEpostadresse = "PersonKontaktinformasjonEpostadresse";
            public const string PersonKontaktinformasjonMobiltelefonnummer = "PersonKontaktinformasjonMobiltelefonnummer";
            //public const string PersonNettsted = "PersonNettsted";
            public const string PersonKontaktinformasjonPostadresseAdresselinje = "PersonKontaktinformasjonPostadresseAdresselinje";
            public const string PersonKontaktinformasjonPostadressePostnummer = "PersonKontaktinformasjonPostadressePostnummer";
            public const string PersonKontaktinformasjonPostadressePoststed = "PersonKontaktinformasjonPostadressePoststed";
            public const string PersonBostedsadresseAdresselinje = "PersonBostedsadresseAdresselinje";
            public const string PersonBostedsadressePostnummer = "PersonBostedsadressePostnummer";
            public const string PersonBostedsadressePoststed = "PersonBostedsadressePoststed";

            public const string PersonMalformNavn = "PersonMalformNavn";
            public const string PersonMalformKode = "PersonMalformKode";

            public const string PersonMorsmalNavn = "PersonMorsmalNavn";
            public const string PersonMorsmalKode = "PersonMorsmalKode";
            
            public const string PersonLandsNavn = "PersonLandsNavn";
            public const string PersonLandsKode = "PersonLandsKode";

            public const string PersonKjonnNavn = "PersonKjonnNavn";
            public const string PersonKjonnKode = "PersonKjonnKode";

            public const string PersonalArbeidsforholdRefs = "PersonalArbeidsforholdRefs";

            //public const string PersonHovedstilling = "PersonHovedstilling";
            public const string PersonalHovedstillingRef = "PersonalHovedstillingRef";
            public const string PersonalHovedstillingArbeidsforholdtype = "PersonalHovedstillingArbeidsforholdtype";
            public const string PersonalHovedstillingStillingskode = "PersonalHovedstillingStillingskode";
            public const string PersonalHovedstillingStillingskodeNavn = "PersonalHovedstillingStillingskodeNavn";
            public const string PersonalHovedstillingstittel = "PersonalHovedstillingstittel";
            // remove the ref here because there is no stillingstittel CS Object type
            //public const string PersonHovedstillingstittelRef = "PersonHovedstillingstittelRef";
            //public const string PersonHovedOrgenhet = "PersonHovedOrgenhet";
            public const string PersonalHovedOrgenhetRef = "PersonalHovedOrgenhetRef";
            public const string PersonalHovedstillingLederRef = "PersonalHovedstillingLederRef";
            public const string PersonalHovedstillingArbeidsstedNavn = "PersonalHovedstillingArbeidsstedNavn";
            public const string PersonalHovedstillingArbeidsstedOrganisasjonsId = "PersonalHovedstillingArbeidsstedOrganisasjonsId";
            public const string PersonalHovedstillingArbeidsstedOrganisasjonsKode = "PersonalHovedstillingArbeidsstedOrganisasjonsKode";

            public const string PersonalOrgenhetRefs = "PersonalArbeidsstedRefs";
            public const string PersonalBusinessUnitRefs = "PersonalVirksomhetRefs";
            public const string PersonalPrimaryBusinessUnitRef = "PersonalHovedVirksomhetRef";
        }

        public struct GroupAttributes
        {
            //public const string ObjectType = "Group";
            public const string GruppeUri = "GruppeUri";
            public const string GruppeID = "GruppeID";
            public const string GruppeNavn = "GruppeNavn";
            public const string GruppeKortnavn = "GruppeKortnavn";
            public const string GruppeGyldighetsPeriodeStart = "GruppeGyldighetsPeriodeStart";
            public const string GruppeGyldighetsPeriodeSlutt = "GruppeGyldighetsPeriodeSlutt";
            //public const string GroupOwnerString = "GroupOwnerString";
            public const string GruppeEierRef = "GruppeEierRef";
            //public const string GroupParentString = "GroupParentString";
            //public const string GroupParentRef = "GroupParentRef";
            public const string GruppeOrganisasjonselementRef = "GruppeOrganisasjonselementRef";
            public const string GruppeOrganisasjonselementKode = "GruppeOrganisasjonselementKode";
            public const string GruppeType = "GruppeType";
            public const string GruppeMedlemRefs = "GruppeMedlemRefs";
        }

        public struct UnitAttributes
        {
            public const string OrganisasjonselementOrganisasjonsidUri = "OrganisasjonselementOrganisasjonsidUri";
            //public const string ObjectType = "Unit";
            public const string OrganisasjonselementOrganisasjonsId = "OrganisasjonselementOrganisasjonsId";
            public const string OrganisasjonselementOrganisasjonsKode = "OrganisasjonselementOrganisasjonsKode";
            public const string OrganisasjonselementOrganisasjonsnummer = "OrganisasjonselementOrganisasjonsnummer";
            public const string OrganisasjonselementNavn = "OrganisasjonselementNavn";
            public const string OrganisasjonselementKortnavn = "OrganisasjonselementKortnavn";
            public const string OrganisasjonselementGyldighetsPeriodeStart = "OrganisasjonselementGyldighetsPeriodeStart";
            public const string OrganisasjonselementGyldighetsPeriodeSlutt = "OrganisasjonselementGyldighetsPeriodeSlutt";
            //public const string OrganisasjonselementLederString = "OrganisasjonselementLederString";
            public const string OrganisasjonselementLederRef = "OrganisasjonselementLederRef";
            //public const string OrganisasjonselementOverordnetString = "OrganisasjonselementOverordnetString";
            public const string OrganisasjonselementOverordnetRef = "OrganisasjonselementOverordnetRef";

            public const string OrganisasjonselementBeskrivelse = "OrganisasjonselementBeskrivelse";
            public const string OrganisasjonselementType = "OrganisasjonselementType";
        }
        public struct OrganizationAttributes
        {
            public const string OrganisasjonsUri = "OrganisasjonUri";
            //public const string ObjectType = "Organization";
            public const string OrganisasjonsKode = "OrganisasjonsKode";

            public const string Organisasjonsnavn = "Organisasjonsnavn";
            public const string Organisasjonskortnavn = "Organisasjonskortnavn";
            public const string Organisasjonsnummer = "Organisasjonsnummer";
            public const string OrganisasjonEpostAdresse = "OrganisasjonEpostAdresse";
        }
        public struct VirksomhetAttributes
        {
            //public const string ObjectType = "BusinessUnit";

            public const string VirksomhetUri = "VirksomhetUri";
            //public const string BusinessUnitOrganizationCode = "BusinessUnitOrganizationCode";
            public const string Virksomhetsnavn = "Virksomhetsnavn";
            public const string Virksomhetskortnavn = "Virksomhetskortnavn";
            public const string VirksomhetOrganisasjonsnummer = "VirksomhetOrganisasjonsnummer";
            public const string VirksomhetEpostAdresse = "VirksomhetEpostAdresse";
        }
        public struct ArbeidsforholdAttributes
        {
            public const string ArbeidsforholdUri = "ArbeidsforholdUri";
            public const string ArbeidsforholdPersonalressursRef = "ArbeidsforholdPersonalressursRef";
            public const string ArbeidsforholdPersonalressursAnsattnummer = "ArbeidsforholdPersonalressursAnsattnummer";
            public const string ArbeidsforholdSystemId = "ArbeidsforholdSystemId";
            public const string ArbeidsforholdStillingskodeKode = "ArbeidsforholdStillingskodeKode";
            public const string ArbeidsforholdStillingstittel = "ArbeidsforholdStillingstittel";
            public const string ArbeidsforholdAnsettelsesprosent = "ArbeidsforholdAnsettelsesprosent";
            public const string ArbeidsforholdGyldighetsperiodeStart = "ArbeidsforholdGyldighetsperiodeStart";
            public const string ArbeidsforholdGyldighetsperiodeSlutt = "ArbeidsforholdGyldighetsperiodeSlutt";
            public const string ArbeidsforholdArbeidsforholdsperiodeStart = "ArbeidsforholdArbeidsforholdsperiodeStart";
            public const string ArbeidsforholdArbeidsforholdsperiodeSlutt = "ArbeidsforholdArbeidsforholdsperiodeSlutt";
            public const string ArbeidsforholdHovedstilling = "ArbeidsforholdHovedstilling";
            public const string ArbeidsforholdArbeidsforholdstypeKode = "ArbeidsforholdArbeidsforholdstypeKode";
            public const string ArbeidsforholdArbeidsstedOrgKode = "ArbeidsforholdArbeidsstedOrgKode";
            public const string ArbeidsforholdArbeidsstedOrgId = "ArbeidsforholdArbeidsstedOrgId";
            public const string ArbeidsforholdArbeidsstedRef = "ArbeidsforholdArbeidsstedRef";
        }

        public struct HalObject
        {
            public const string _links = "_links";
            public const string _embedded = "_embedded";
            public const string _entries = "_entries";
        }

        public struct StateLink
        {
            public const string total_items = "total_items";
        }

        public struct HttpHeader
        {
            public const string X_Org_Id = "X-Org-Id";
            public const string X_Client = "X-Client";
            public const string Location = "Location";
        }

        public struct HttpVerb
        {
            public const string GET = "GET";
            public const string PUT = "PUT";
            public const string POST = "POST";
        }


        public struct CSobjecttypes
        {
            public const string Person = "Person";
            public const string Gruppe = "Gruppe";
            public const string Arbeidsforhold = "Arbeidsforhold";
            public const string Organisasjonselement = "Organisasjonselement";
            public const string Virksomhet = "Virksomhet";
            public const string Organisasjon = "Organisasjon";
        }

        public struct Filtertype
        {
            public const string Include = "Include";
            public const string Exclude = "Exclude";
        }

        public enum EmploymentPeriodType: int {
            InvalidPeriod = 0,
            ValidPresentPeriod = 1,
            ValidFuturePeriod = 2,
            ValidPastPeriod = 3
        }
    }
}
