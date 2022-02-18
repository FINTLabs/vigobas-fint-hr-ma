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
    class HRPersonalressurs
    {
        // The correspoding Fint attributes are listed in the comments

        // Fint Personal
        public string PersonalAnsattnummer { get; set; }
        public DateTime? PersonalAnsettelsesperiodeStart { get; set; }
        public DateTime? PersonalAnsettelsesperiodeSlutt { get; set; }
        public string PersonalBrukernavn { get; set; }

        //// Fint Person
        //public string PersonBilde { get; set; }
        //public DateTime? PersonFodselsdato { get; set; }
        //public string PersonFodselsnummer { get; set; }
        //public string PersonNavnFornavn { get; set; }
        //public string PersonNavnMellomnavn { get; set; }
        //public string PersonNavnEtternavn { get; set; }

        // Fint employment

    }
}
