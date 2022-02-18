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
using FINT.Model.Felles.Kompleksedatatyper;
using Microsoft.MetadirectoryServices;

using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class HRUnit
    {
        public string UnitUri { get; set; }
        public string OrganisasjonselementOrganisasjonsid { get; set; }
        public string OrganisasjonselementNavn { get; set; }
        public string OrganisasjonselementKortnavn { get; set; }
        public string OrganisasjonselementOrganisasjonsKode { get; set; }
        public string OrganisasjonselementOrganisasjonsnummer { get; set; }
        //public Identifikator UnitCode { get; set; }
        public string UnitType { get; set; }
        public List<string> UnitMembers { get; set; }
        public string OrganisasjonselementPeriodeStart { get; set; }
        public string OrganisasjonselementPeriodeSlutt { get; set; }
        public Periode UnitPeriod { get; set; }
        public string OrganisasjonselementLeder { get; set; }
        public string OrganisasjonselementOverordnet { get; set; }
        public List<string> OrganisasjonselementUnderordnet { get; set; }
        public string ParentSchoolOrgUnit { get; set; }
        public List<string> ParentBaseOrgUnits { get; set; }

        internal CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Organisasjonselement;


            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementOrganisasjonsidUri, UnitUri));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementOrganisasjonsId, OrganisasjonselementOrganisasjonsid));

            if (!string.IsNullOrEmpty(OrganisasjonselementNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementNavn, OrganisasjonselementNavn));
            }
            if (!string.IsNullOrEmpty(OrganisasjonselementKortnavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementKortnavn, OrganisasjonselementKortnavn));
            }

            if (!string.IsNullOrEmpty(OrganisasjonselementOrganisasjonsKode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementOrganisasjonsKode, OrganisasjonselementOrganisasjonsKode));
            }

            if (!string.IsNullOrEmpty(UnitType))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementType, UnitType));
            }

            if (!string.IsNullOrEmpty(OrganisasjonselementLeder))
            {
                //csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementLederString, OrganisasjonselementLeder));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementLederRef, OrganisasjonselementLeder));
            }

            if (!string.IsNullOrEmpty(OrganisasjonselementOverordnet))
            {
                //csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementOverordnetString, OrganisasjonselementOverordnet));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementOverordnetRef, OrganisasjonselementOverordnet));
            }

            if (!string.IsNullOrEmpty(OrganisasjonselementPeriodeStart))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementGyldighetsPeriodeStart, OrganisasjonselementPeriodeStart));
            }
            if (!string.IsNullOrEmpty(OrganisasjonselementPeriodeSlutt))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(UnitAttributes.OrganisasjonselementGyldighetsPeriodeSlutt, OrganisasjonselementPeriodeSlutt));
            }

            return csentry;
        }
    }
}
