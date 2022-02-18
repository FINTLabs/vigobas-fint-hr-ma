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
using HalClient.Net.Parser;
using Newtonsoft.Json;

using VigoBAS.FINT.HR.Utilities;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class HRBusinessUnit
    {
        public string BusinessUnitUri { get; set; }
        public string BusinessUnitOrganizationCode { get; set; }
        public string BusinessUnitName { get; set; }
        public string BusinessUnitShortName { get; set; }
        public string BusinessUnitOrganizationNumber { get; set; }
        public string BusinessUnitEmailAddress { get; set; }

        internal CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Virksomhet;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.VirksomhetUri, BusinessUnitUri));

            //if (!string.IsNullOrEmpty(BusinessUnitOrganizationCode))
            //{
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.BusinessUnitOrganizationCode, BusinessUnitOrganizationCode));
            //}
            if (!string.IsNullOrEmpty(BusinessUnitName))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.Virksomhetsnavn, BusinessUnitName));
            }
            if (!string.IsNullOrEmpty(BusinessUnitShortName))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.Virksomhetskortnavn, BusinessUnitShortName));
            }
            if (!string.IsNullOrEmpty(BusinessUnitOrganizationNumber))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.VirksomhetOrganisasjonsnummer, BusinessUnitOrganizationNumber));
            }
            if (!string.IsNullOrEmpty(BusinessUnitEmailAddress))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(VirksomhetAttributes.VirksomhetEpostAdresse, BusinessUnitEmailAddress));
            } 
            return csentry;
        }
    }
}
