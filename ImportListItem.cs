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

using FINT.Model.Administrasjon.Personal;
using FINT.Model.Administrasjon.Kodeverk;
using FINT.Model.Administrasjon.Organisasjon;

namespace VigoBAS.FINT.HR

{
    class ImportListItem
    {
        public HRPerson HrPerson;
        public HREmployment HrEmployment;
        public Organisasjonselement HrOrgelement;
        public Organisasjonselement HrArbeidssted;
        public Arbeidsforhold HrArbeidsforhold;
        public Ansvar HrAnsvar;
        public Funksjon HrFunksjon;
        public Stillingskode HrStillingskode;
        public Arbeidsforholdstype HrArbeidsforholdstype;
        public Uketimetall HrTimerprUke;
        public HRGroup HrGroup;
        public HRUnit HrUnit;
        public HRBusinessUnit HrBusinessUnit;
        public HROrganization HrOrganization;


        //public HRPersonalressursFactory hrpersonalressursFactory;
        //public PersonalressursFactory personalressursFactory;
    }
}
