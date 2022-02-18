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
    class HRGroup
    {
        public string GroupIDUri { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupShortname { get; set; }
        public string GroupOrgUnitCode { get; set; }
        public string GroupOrgUnitRef { get; set; }
        public string GroupType { get; set; }
        public List<string> GroupMembers { get; set; }
        //public Periode GroupPeriod { get; set; }
        public string GroupPeriodStart { get; set; }
        public string GroupPeriodSlutt { get; set; }
        public string GroupOwner { get; set; }
        public string GroupParent { get; set; }

        internal CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSobjecttypes.Gruppe;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeUri, GroupIDUri));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeID, GroupID));

            if (!string.IsNullOrEmpty(GroupName))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeNavn, GroupName));
            }

            if (!string.IsNullOrEmpty(GroupShortname))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeKortnavn, GroupShortname));
            }

            if (!string.IsNullOrEmpty(GroupOrgUnitCode))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeOrganisasjonselementKode, GroupOrgUnitCode));
            }

            if (!string.IsNullOrEmpty(GroupOrgUnitRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeOrganisasjonselementRef, GroupOrgUnitRef));
            }

            if (!string.IsNullOrEmpty(GroupType))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeType, GroupType));
            }

            if (!string.IsNullOrEmpty(GroupOwner))
            {
                //csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GroupOwnerString, GroupOwner));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeEierRef, GroupOwner));
            }

            //if (!string.IsNullOrEmpty(GroupParent))
            //{
            //    //csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GroupParentString, GroupParent));
            //    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GroupParentRef, GroupParent));
            //}

            if (!string.IsNullOrEmpty(GroupPeriodStart))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeGyldighetsPeriodeStart, GroupPeriodStart));
            }
            if (!string.IsNullOrEmpty(GroupPeriodSlutt))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeGyldighetsPeriodeSlutt, GroupPeriodSlutt));
            }

            if (GroupMembers != null && GroupMembers.Count > 0)
            {
                IList<object> Members = new List<object>();
                foreach (var member in GroupMembers)
                {
                    Members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(GroupAttributes.GruppeMedlemRefs, Members));
            }

            return csentry;
        }
    }
}
