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
using System;
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Organisasjon;
using FINT.Model.Resource;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;

namespace VigoBAS.FINT.HR
{
    class OrganisasjonselementResourceFactory
    {
        public static OrganisasjonselementResource Create(IEmbeddedResourceObject orgElementData)
        {
            var navn = string.Empty;
            var kortnavn = string.Empty;
            var organisasjonsId = new Identifikator();
            var organisasjonsKode = new Identifikator();

            var values = orgElementData.State;

            navn = GetStringValueFromHalAttribute(values, FintAttribute.navn);
            kortnavn = GetStringValueFromHalAttribute(values, FintAttribute.kortnavn);

            organisasjonsId = GetFintIdentifikatorFromHalAttribute(values, FintAttribute.organisasjonsId);
            organisasjonsKode = GetFintIdentifikatorFromHalAttribute(values, FintAttribute.organisasjonsKode);

            var organisasjonselementResource = new OrganisasjonselementResource
            {
                Navn =navn,
                Kortnavn = kortnavn,
                OrganisasjonsId = organisasjonsId,
                OrganisasjonsKode = organisasjonsKode
            };

            var links = orgElementData.Links;

            var parentOrgUnitUriList = GetStringListFromHalLink(links, ResourceLink.parent);

            foreach (var uri in parentOrgUnitUriList)
            {
                var link = Link.with(uri);
                organisasjonselementResource.AddOverordnet(link);
            }

            var leaderOrgUnitUriList = GetStringListFromHalLink(links, ResourceLink.leder);

            foreach (var uri in leaderOrgUnitUriList)
            {
                var link = Link.with(uri);
                organisasjonselementResource.AddLeder(link);
            }

            var childrenOrgUnitUriList = GetStringListFromHalLink(links, ResourceLink.children);

            foreach (var uri in childrenOrgUnitUriList)
            {
                var link = Link.with(uri);
                organisasjonselementResource.AddUnderordnet(link);
            }

            return organisasjonselementResource;
        }

    }
}
