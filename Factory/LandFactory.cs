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
using FINT.Model.Felles.Kodeverk.ISO;
using FINT.Model.Felles.Kompleksedatatyper;
using HalClient.Net.Parser;
using Newtonsoft.Json;

namespace VigoBAS.FINT.HR
{
    class LandFactory
    {
        public static Landkode Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var LandKode = "";
            var LandNavn = "";
            var LandGyldighetsperiode = new Periode();

            if (values.TryGetValue("kode", out IStateValue kodeVal))
            {
                LandKode = kodeVal.Value;
            }
            if (values.TryGetValue("navn", out IStateValue navnVal))
            {
                LandNavn = navnVal.Value;
            }
            if (values.TryGetValue("gyldighetsperiode", out IStateValue PeriodeVal))
            {
                LandGyldighetsperiode = JsonConvert.DeserializeObject<Periode>(PeriodeVal.Value);
            }

            return new Landkode
            {
                Kode = LandKode,
                Navn = LandNavn,
                Gyldighetsperiode = LandGyldighetsperiode,
            };
        }
    }
}
