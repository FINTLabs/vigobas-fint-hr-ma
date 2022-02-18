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
using System.Collections.ObjectModel;
using FINT.Model.Felles.Kompleksedatatyper;
using Microsoft.MetadirectoryServices;
using HalClient.Net.Parser;
using Newtonsoft.Json;

using VigoBAS.FINT.HR.Utilities;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class HROrgFactory
    {
        public static HRGroup Create(string hrGroupUri, string groupType, HRUnit hrUnit, KeyedCollection<string, ConfigParameter> configParameters)
        {
            //string GrpPrefix = configParameters[Param.GruppePrefix].Value;
            //string GrpSuffix = configParameters[Param.GruppeSuffix].Value;

            var organisasjonsnavn = string.Empty;
            var organisasjonsUri = hrUnit.UnitUri;
            var navn = hrUnit?.OrganisasjonselementNavn;
            var kortnavn = hrUnit?.OrganisasjonselementKortnavn;
            var periodeStart = hrUnit?.OrganisasjonselementPeriodeStart;
            var periodeSlutt = hrUnit?.OrganisasjonselementPeriodeSlutt;
            var organisasjonsId = hrUnit.OrganisasjonselementOrganisasjonsid;
            var organisasjonsKode = hrUnit.OrganisasjonselementOrganisasjonsKode;
            var medlemmer = new List<string>();

            return new HRGroup
            {
                GroupIDUri = hrGroupUri,
                GroupID = organisasjonsId + Delimiter.suffix + groupType,
                GroupType = groupType,
                GroupName = (!string.IsNullOrEmpty(navn)) ? navn : organisasjonsnavn,
                GroupShortname = kortnavn,
                GroupPeriodStart = periodeStart,
                GroupPeriodSlutt = periodeSlutt,
                GroupOrgUnitRef = organisasjonsUri,
                GroupOrgUnitCode = organisasjonsKode,
                GroupMembers = medlemmer
            };
        }

        public static HRUnit Create(string orgElementUri, IReadOnlyDictionary<string, IStateValue> OrgElement)
        {
            var organisasjonsNavn = string.Empty;
            var organisasjonsKortnavn = string.Empty;
            var gyldighetsperiode = new Periode();
            var organisasjonsId = new Identifikator();
            var organisasjonsKode = new Identifikator();
            var organisasjonsnummer = new Identifikator();
            string UnitPrefix = DefaultValue.UnitPrefix;
            string UnitSuffix = DefaultValue.UnitSuffix;

            if (OrgElement.TryGetValue("navn", out IStateValue orgNamedictVal))
            {
                organisasjonsNavn = orgNamedictVal.Value;
            }
            if (OrgElement.TryGetValue("gyldighetsperiode", out IStateValue gyldictVal))
            {
                gyldighetsperiode = JsonConvert.DeserializeObject<Periode>(gyldictVal.Value);
            }
            if (OrgElement.TryGetValue("organisasjonsId", out IStateValue OrgIDdictVal))
            {
                organisasjonsId = JsonConvert.DeserializeObject<Identifikator>(OrgIDdictVal.Value);
            }
            if (OrgElement.TryGetValue("organisasjonsKode", out IStateValue OrgKodedictVal))
            {
                organisasjonsKode = JsonConvert.DeserializeObject<Identifikator>(OrgKodedictVal.Value);
            }
            if (OrgElement.TryGetValue("organisasjonsnummer", out IStateValue OrgNummerdictVal))
            {
                organisasjonsnummer = JsonConvert.DeserializeObject<Identifikator>(OrgNummerdictVal.Value);
            }
            if (OrgElement.TryGetValue("kortnavn", out IStateValue kortnavnVal))
            {
                organisasjonsKortnavn = kortnavnVal.Value;
            }

            return new HRUnit
            {
                UnitUri = orgElementUri,
                //OrganisasjonselementOrganisasjonsid = UnitPrefix + organisasjonsId.Identifikatorverdi + UnitSuffix,
                OrganisasjonselementOrganisasjonsid = organisasjonsId.Identifikatorverdi,
                OrganisasjonselementOrganisasjonsnummer = organisasjonsnummer.Identifikatorverdi,
                OrganisasjonselementNavn = organisasjonsNavn,
                OrganisasjonselementKortnavn = organisasjonsKortnavn,
                //UnitPeriod = gyldighetsperiode,
                OrganisasjonselementPeriodeStart = gyldighetsperiode?.Start.ToString(),
                OrganisasjonselementPeriodeSlutt = gyldighetsperiode?.Slutt.ToString(),
                //UnitCode = organisasjonsKode
                OrganisasjonselementOrganisasjonsKode = organisasjonsKode.Identifikatorverdi
            };
        }
        public static HROrganization Create(HRUnit topOrgUnit)
        {
            string hrOrganizationtShortName = topOrgUnit.OrganisasjonselementKortnavn;
            string organizationSuffix = (!string.IsNullOrEmpty(hrOrganizationtShortName)) ? hrOrganizationtShortName.Trim().Replace(' ', Delimiter.suffix).ToLower() : DefaultValue.organizationSuffix;

            string hrOrganizationUri = topOrgUnit.UnitUri + Delimiter.suffix + organizationSuffix;
            string hrOrganizationCode = topOrgUnit.OrganisasjonselementOrganisasjonsKode;
            string hrOrganizationtName = topOrgUnit.OrganisasjonselementNavn;

            string hrOrganizationOrganizationNumber = topOrgUnit.OrganisasjonselementOrganisasjonsnummer;

            return new HROrganization
            {
                HROrganizationUri = hrOrganizationUri,
                HROrganizationCode = hrOrganizationCode,
                HROrganizationtName = hrOrganizationtName,
                HROrganizationtShortName = hrOrganizationtShortName,
                HROrganizationOrganizationNumber = hrOrganizationOrganizationNumber
            };
        }
        public static HRBusinessUnit Create(HRUnit hrUnit, string businessUnitSuffix)
        {
            string hrOrganizationtShortName = hrUnit.OrganisasjonselementKortnavn;

            string hrOrganizationUri = hrUnit.UnitUri + Delimiter.suffix + businessUnitSuffix;
            string hrOrganizationCode = hrUnit.OrganisasjonselementOrganisasjonsKode;
            string hrOrganizationtName = hrUnit.OrganisasjonselementNavn;

            string hrOrganizationOrganizationNumber = hrUnit.OrganisasjonselementOrganisasjonsnummer;

            return new HRBusinessUnit
            {
                BusinessUnitUri = hrOrganizationUri,
                BusinessUnitOrganizationCode = hrOrganizationCode,
                BusinessUnitName = hrOrganizationtName,
                BusinessUnitShortName = hrOrganizationtShortName,
                BusinessUnitOrganizationNumber = hrOrganizationOrganizationNumber
            };
        }
    }
}
