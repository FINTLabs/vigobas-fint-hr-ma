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
    public class HovedstillingInfo
    {
        public string HovedstillingsID { get; set; }
        public string HovedstillingOrgID { get; set; }
        public string HovedstillingOrgKode { get; set; }
        public string HovedstillingOrgNavn { get; set; }
        public string HovedstillingOrgUri { get; set; }
        public string HovedstillingBusinessUnitUri { get; set; }
        public string HovedstillingsTittel { get; set; }
        public string HovedstillingArbeidsforholdtype { get; set; }
        public string HovedstillingStillingskode { get; set; }
        public string HovedstillingStillingskodeNavn { get; set; }
    }
}
