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
using System.Linq;
using Microsoft.MetadirectoryServices;
using FINT.Model.Felles.Kompleksedatatyper;
using System;
using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class HREmployment
    {
        public string ArbeidforholdUri { get; set; }
        public string ArbeidforholdSystemId { get; set; }
        //public string AnsattSystemIdRef { get; set; }
        public string StillingsId { get; set; }
        public string Stillingstittel { get; set; }
        public string StillingNummer { get; set; }
        public DateTime GyldighetsperiodeStart { get; set; }
        public DateTime? GyldighetsperiodeSlutt { get; set; }
        public DateTime? ArbeidsforholdsperiodeStart { get; set; }
        public DateTime? ArbeidsforholdsperiodeSlutt { get; set; }
        public string Stillingskode { get; set; }
        public string Ansettelsesprosent { get; set; }
        public bool? Hovedstilling { get; set; }
        public string ArbeidsforholdPersonalressursRef { get; set; }
        public string ArbeidsforholdPersonalressursAnsattnummer { get; set; }
        public string ArbeidsforholdstypeNavn { get; set; }
        public string ArbeidsforholdstypeSystemIdUri { get; set; }
        public string ArbeidsforholdstypeKode { get; set; }
        public string ArbeidsforholdstypeSystemId { get; set; }
        public string StillingkodeSystemIdUri { get; set; }
        public string StillingkodeNavn { get; set; }
        public string StillingkodeKode { get; set; }
        public bool? StillingkodePassiv { get; set; }
        public string StillingkodeSystemId { get; set; }

        public string ArbeidsstedOrganisasjonsIdUri { get; set; }
        public string ArbeidsstedNavn { get; set; }
        public string ArbeidsstedKortnavn { get; set; }
        public string ArbeidsstedOrgKode { get; set; }
        public string ArbeidsstedOrgID { get; set; }
        public string ArbeidsstedBusinessUnitUri{ get; set; }

        public string ArbeidsforholdFunksjonKode { get; set; }
        public string ArbeidsforholdFunksjonNavn { get; set; }
        public bool? ArbeidsforholdFunksjonPassiv { get; set; }
        public string ArbeidsforholdFunksjonSystemId { get; set; }


        //public string PersonalLeder { get; set; }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Arbeidsforhold;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdUri, ArbeidforholdUri));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdSystemId, ArbeidforholdSystemId));

            if (!string.IsNullOrEmpty(StillingkodeKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdStillingskodeKode, StillingkodeKode));
            }
            if (Hovedstilling.HasValue)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdHovedstilling, Hovedstilling));
            }
            if (!string.IsNullOrEmpty(Stillingstittel))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdStillingstittel, Stillingstittel));
            }
            if (!string.IsNullOrEmpty(GyldighetsperiodeStart.ToString()))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdGyldighetsperiodeStart, GyldighetsperiodeStart.ToString()));
            }
            if (!string.IsNullOrEmpty(GyldighetsperiodeSlutt.ToString()))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdGyldighetsperiodeSlutt, GyldighetsperiodeSlutt.ToString()));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdsperiodeStart.ToString()))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdsperiodeStart, ArbeidsforholdsperiodeStart.ToString()));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdsperiodeSlutt.ToString()))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdsperiodeSlutt, ArbeidsforholdsperiodeSlutt.ToString()));
            }
            if (!string.IsNullOrEmpty(Ansettelsesprosent))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdAnsettelsesprosent, Ansettelsesprosent));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdPersonalressursRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdPersonalressursRef, ArbeidsforholdPersonalressursRef));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdPersonalressursAnsattnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdPersonalressursAnsattnummer, ArbeidsforholdPersonalressursAnsattnummer));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdstypeKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdstypeKode, ArbeidsforholdstypeKode));
            }
            if (!string.IsNullOrEmpty(ArbeidsstedOrganisasjonsIdUri))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedRef, ArbeidsstedOrganisasjonsIdUri));
            }
            if (!string.IsNullOrEmpty(ArbeidsstedOrgID))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedOrgId, ArbeidsstedOrgID));
            }
            if (!string.IsNullOrEmpty(ArbeidsstedOrgKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedOrgKode, ArbeidsstedOrgKode));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdFunksjonKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdFunksjonKode, ArbeidsforholdFunksjonKode));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdFunksjonNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdFunksjonNavn, ArbeidsforholdFunksjonNavn));
            }
            if (ArbeidsforholdFunksjonPassiv.HasValue)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdFunksjonPassiv, ArbeidsforholdFunksjonPassiv));
            }
            if (!string.IsNullOrEmpty(ArbeidsforholdFunksjonSystemId.ToString()))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(ArbeidsforholdAttributes.ArbeidsforholdFunksjonSystemId, ArbeidsforholdFunksjonSystemId));
            }

            return csentry;
        }
    }
}
