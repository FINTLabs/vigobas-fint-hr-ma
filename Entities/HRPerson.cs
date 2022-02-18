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
using Microsoft.MetadirectoryServices;
using FINT.Model.Administrasjon.Personal;

using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class HRPerson
    {
        public string PersonalAnsattnummerUri { get; set; }

        public string PersonalAnsattnummer { get; set; }
        public string PersonalPersonalressurskategori { get; set; }

        public string PersonalSystemId { get; set; }
        public string PersonalBrukernavn { get; set; }
        public string PersonalKontaktinformasjonEpostadresse { get; set; }
        public string PersonalKontaktinformasjonMobiltelefonnummer { get; set; }
        public string PersonalKontaktinformasjonTelefonnummer { get; set; }
        public string PersonalKontaktinformasjonSip { get; set; }
        public List<string> PersonalLederOrganisasjonselementer { get; set; }
        public string PersonalHROrganisasjonRef { get; set; }
        public string PersonNavnFornavn { get; set; }
        public string PersonNavnMellomnavn { get; set; }
        public string PersonNavnEtternavn { get; set; }
        public string PersonEpostadresse { get; set; }
        public string PersonMobilnummer { get; set; }
        public string PersonNettsted { get; set; }
        public string PersonAdresselinje { get; set; }
        public string PersonFodselsdato { get; set; }
        public string PersonFodselsnummer { get; set; }

        public List<string> PersonPostadresseAdresselinje { get; set; }
        public string PersonPostadressePostnummer { get; set; }
        public string PersonPostadressePoststed { get; set; }

        public List<string> PersonBostedsadresseAdresselinje { get; set; }
        public string PersonBostedsadressePostnummer { get; set; }
        public string PersonBostedsadressePoststed { get; set; }

        public string PersonMorsmalNavn { get; set; }
        public string PersonMorsmalKode { get; set; }

        public string PersonMalformNavn { get; set; }
        public string PersonMalformKode { get; set; }

        public string PersonKjonnNavn { get; set; }
        public string PersonKjonnKode { get; set; }

        public string PersonLandNavn { get; set; }
        public string PersonLandKode { get; set; }

        //public  HovedstillingInfo PersonHovedstilling { get; set; }
        public string PersonHovedstillingRef { get; set; }
        public string PersonHovedstillingTittel  { get; set; }
        public string PersonHovedstillingArbeidsstedUri { get; set; }
        public string PersonHovedstillingArbeidsstedNavn{ get; set; }
        public string PersonHovedstillingArbeidsstedOrganisasjonsId { get; set; }
        public string PersonHovedstillingArbeidsstedOrganisasjonsKode { get; set; }
        public string PersonHovedstillingArbeidsforholdtype  { get; set; }
        public string PersonHovedstillingStillingskode  { get; set; }
        public string PersonHovedstillingStillingskodeNavn { get; set; }
        public string PersonHovedstillingLeder { get; set; }
        public List<string> PersonEmployments { get; set; }
        public int PersonalAntallAktiveArbeidsforhold { get; set; }

        public string PersonalAktivAnsettelsesperiodeStart { get; set; }        
        public string PersonalAktivAnsettelsesperiodeSlutt { get; set; }
        public List<string> PersonalArbeidsstedRefs { get; set; }
        public List<string> PersonalBusinessUnitRefs { get; set; }
        public string PersonalPrimaryBusinessUnitRef { get; set; }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Person;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalAnsattnummerUri, PersonalAnsattnummerUri));

            if (!string.IsNullOrEmpty(PersonalSystemId))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalSystemId, PersonalSystemId));
            }

            if (!string.IsNullOrEmpty(PersonFodselsdato))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonFodselsdato, PersonFodselsdato));
            }

            if (!string.IsNullOrEmpty(PersonFodselsnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonFodselsnummer, PersonFodselsnummer));
            }

            if (!string.IsNullOrEmpty(PersonNavnFornavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonNavnFornavn, PersonNavnFornavn));
            }

            if (!string.IsNullOrEmpty(PersonNavnMellomnavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonNavnMellomnavn, PersonNavnMellomnavn));
            }

            if (!string.IsNullOrEmpty(PersonNavnEtternavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonNavnEtternavn, PersonNavnEtternavn));
            }
            if (PersonEmployments != null && PersonEmployments.Count > 0)
            {
                IList<object> IDs = new List<object>();
                foreach (var ID in PersonEmployments)
                {
                    IDs.Add(ID.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalArbeidsforholdRefs, IDs));
            }
            if (PersonalAntallAktiveArbeidsforhold > 0)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalAntallAktiveArbeidsforhold, PersonalAntallAktiveArbeidsforhold));
            }
            if (PersonalLederOrganisasjonselementer != null && PersonalLederOrganisasjonselementer.Count > 0)
            {
                IList<object> IDs = new List<object>();
                foreach (var ID in PersonalLederOrganisasjonselementer)
                {
                    IDs.Add(ID.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalLederOrganisasjonselementRef, IDs));
            }

            if (!string.IsNullOrEmpty(PersonHovedstillingRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingRef, PersonHovedstillingRef));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingTittel))
            { 
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingstittel, PersonHovedstillingTittel));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingStillingskode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingStillingskode, PersonHovedstillingStillingskode));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingStillingskodeNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingStillingskodeNavn, PersonHovedstillingStillingskodeNavn));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingArbeidsstedUri))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedOrgenhetRef, PersonHovedstillingArbeidsstedUri));
                //csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonHovedOrgenhet, PersonHovedstillingArbeidsstedUri));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingArbeidsstedOrganisasjonsId))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingArbeidsstedOrganisasjonsId, PersonHovedstillingArbeidsstedOrganisasjonsId));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingArbeidsstedOrganisasjonsKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingArbeidsstedOrganisasjonsKode, PersonHovedstillingArbeidsstedOrganisasjonsKode));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingArbeidsforholdtype))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingArbeidsforholdtype, PersonHovedstillingArbeidsforholdtype));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingArbeidsstedNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingArbeidsstedNavn, PersonHovedstillingArbeidsstedNavn));
            }
            if (!string.IsNullOrEmpty(PersonHovedstillingLeder))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalHovedstillingLederRef, PersonHovedstillingLeder));
            }

            //if (PersonBostedsadresseAdresselinje != null && PersonBostedsadresseAdresselinje.Count > 0)
            //{
            //    IList<object> lines = new List<object>();
            //    foreach (var line in PersonBostedsadresseAdresselinje)
            //    {
            //        lines.Add(line.ToString());
            //    }
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonBostedsadresseAdresselinje, lines));
            //}

            //if (!string.IsNullOrEmpty(PersonBostedsadressePostnummer))
            //{
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonBostedsadressePostnummer, PersonBostedsadressePostnummer));
            //}

            //if (!string.IsNullOrEmpty(PersonBostedsadressePoststed))
            //{
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonBostedsadressePoststed, PersonBostedsadressePoststed));
            //}

            if (PersonPostadresseAdresselinje != null && PersonPostadresseAdresselinje.Count > 0)
            {
                IList<object> lines = new List<object>();
                foreach (var line in PersonPostadresseAdresselinje)
                {
                    lines.Add(line.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKontaktinformasjonPostadresseAdresselinje, lines));
            }

            if (!string.IsNullOrEmpty(PersonPostadressePostnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKontaktinformasjonPostadressePostnummer, PersonPostadressePostnummer));
            }

            if (!string.IsNullOrEmpty(PersonPostadressePoststed))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKontaktinformasjonPostadressePoststed, PersonPostadressePoststed));
            }

            if (!string.IsNullOrEmpty(PersonalAnsattnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalAnsattnummer, PersonalAnsattnummer));
            }
            if (!string.IsNullOrEmpty(PersonalAktivAnsettelsesperiodeStart))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalAktivAnsettelsesperiodeStart, PersonalAktivAnsettelsesperiodeStart));
            }
            if (!string.IsNullOrEmpty(PersonalAktivAnsettelsesperiodeSlutt))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalAktivAnsettelsesperiodeSlutt, PersonalAktivAnsettelsesperiodeSlutt));
            }
            if (!string.IsNullOrEmpty(PersonalBrukernavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalBrukernavn, PersonalBrukernavn));
            }

            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalKontaktinformasjonEpostadresse, PersonalKontaktinformasjonEpostadresse));
            }

            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonMobiltelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer, PersonalKontaktinformasjonMobiltelefonnummer));
            }

            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonTelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalKontaktinformasjonTelefonnummer, PersonalKontaktinformasjonTelefonnummer));
            }

            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonSip))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalKontaktinformasjonSip, PersonalKontaktinformasjonSip));
            }

            if (!string.IsNullOrEmpty(PersonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKontaktinformasjonEpostadresse, PersonEpostadresse));
            }

            if (!string.IsNullOrEmpty(PersonMobilnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKontaktinformasjonMobiltelefonnummer, PersonMobilnummer));
            }

            //if (!string.IsNullOrEmpty(PersonNettsted))
            //{
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonNettsted, PersonNettsted));
            //}

            if (!string.IsNullOrEmpty(PersonMalformKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonMalformKode, PersonMalformKode));
            }

            if (!string.IsNullOrEmpty(PersonMalformNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonMalformNavn, PersonMalformNavn));
            }

            if (!string.IsNullOrEmpty(PersonMorsmalNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonMorsmalNavn, PersonMorsmalNavn));
            }

            if (!string.IsNullOrEmpty(PersonMorsmalKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonMorsmalKode, PersonMorsmalKode));
            }

            if (!string.IsNullOrEmpty(PersonKjonnNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKjonnNavn, PersonKjonnNavn));
            }

            if (!string.IsNullOrEmpty(PersonKjonnKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonKjonnKode, PersonKjonnKode));
            }

            if (!string.IsNullOrEmpty(PersonLandNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonLandsNavn, PersonLandNavn));
            }

            if (!string.IsNullOrEmpty(PersonalPersonalressurskategori))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalPersonalressurskategori, PersonalPersonalressurskategori));
            } 

            if (!string.IsNullOrEmpty(PersonalHROrganisasjonRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalOrganisasjonRef, PersonalHROrganisasjonRef));
            }

            if (PersonalArbeidsstedRefs != null && PersonalArbeidsstedRefs.Count > 0)
            {
                IList<object> IDs = new List<object>();
                foreach (var ID in PersonalArbeidsstedRefs)
                {
                    IDs.Add(ID.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalOrgenhetRefs, IDs));
            }

            if (PersonalBusinessUnitRefs != null && PersonalBusinessUnitRefs.Count > 0)
            {
                IList<object> IDs = new List<object>();
                foreach (var ID in PersonalBusinessUnitRefs)
                {
                    IDs.Add(ID.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalBusinessUnitRefs, IDs));
            }

            if (!string.IsNullOrEmpty(PersonalPrimaryBusinessUnitRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(PersonAttributes.PersonalPrimaryBusinessUnitRef, PersonalPrimaryBusinessUnitRef));
            }

            return csentry;
        }
    }
}
