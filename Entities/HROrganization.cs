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
using Microsoft.MetadirectoryServices;

using static VigoBAS.FINT.HR.Constants;

namespace VigoBAS.FINT.HR
{
    class HROrganization
    {
        public string HROrganizationUri { get; set; }
        public string HROrganizationCode { get; set; }
        public string HROrganizationtName { get; set; }
        public string HROrganizationtShortName { get; set; }
        public string HROrganizationOrganizationNumber { get; set; }
        public string HROrganizationEmailAddress { get; set; }


        internal CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Organisasjon;


            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.OrganisasjonsUri, HROrganizationUri));

            if (!string.IsNullOrEmpty(HROrganizationCode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.OrganisasjonsKode, HROrganizationCode));
            }
            if (!string.IsNullOrEmpty(HROrganizationtName))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.Organisasjonsnavn, HROrganizationtName));
            }
            if (!string.IsNullOrEmpty(HROrganizationtShortName))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.Organisasjonskortnavn, HROrganizationtShortName));
            }
            if (!string.IsNullOrEmpty(HROrganizationOrganizationNumber))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.Organisasjonsnummer, HROrganizationOrganizationNumber));
            }
            if (!string.IsNullOrEmpty(HROrganizationEmailAddress))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(OrganizationAttributes.OrganisasjonEpostAdresse, HROrganizationEmailAddress));
            }
            return csentry;
        }
    }
}
