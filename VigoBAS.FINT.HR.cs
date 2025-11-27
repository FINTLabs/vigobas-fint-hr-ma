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

using FINT.Model.Administrasjon.Kodeverk;
using FINT.Model.Administrasjon.Organisasjon;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Felles;
using FINT.Model.Felles.Kodeverk.ISO;
using FINT.Model.Felles.Kompleksedatatyper;
using HalClient.Net;
using HalClient.Net.Parser;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vigo.Bas.ManagementAgent.Log;
using VigoBAS.FINT.HR.Utilities;
using static VigoBAS.FINT.HR.Constants;
using static VigoBAS.FINT.HR.Utilities.Tools;


namespace VigoBAS.FINT.HR
{
    public class EzmaExtension :
    IMAExtensible2CallExport,
    IMAExtensible2CallImport,
    IMAExtensible2GetSchema,
    IMAExtensible2GetCapabilities,
    IMAExtensible2GetParameters
    {

        #region Page Size

        public int ImportMaxPageSize { get; } = 50;
        public int ImportDefaultPageSize { get; } = 12;
        public int ExportDefaultPageSize { get; set; } = 10;
        public int ExportMaxPageSize { get; set; } = 50;

        #endregion

        List<string> allreadyimported = new List<string>();
        KeyedCollection<string, ConfigParameter> _globalConfigParameters;
        // public const string dateFormat = "yyyy-MM-dd";

        private Dictionary<string, IEmbeddedResourceObject> _resourceDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, string> _personalressursIdMappingDict = new Dictionary<string, string>();
        #region Capabilities, Config parameters and Schema

        public MACapabilities Capabilities
        {
            // Returns the capabilities that this management agent has
            get
            {
                MACapabilities myCapabilities = new MACapabilities();

                myCapabilities.ConcurrentOperation = true;
                myCapabilities.ObjectRename = false;
                myCapabilities.DeleteAddAsReplace = true;
                myCapabilities.DeltaImport = false;
                myCapabilities.DistinguishedNameStyle = MADistinguishedNameStyle.None;
                myCapabilities.ExportType = MAExportType.ObjectReplace;
                myCapabilities.FullExport = true;
                myCapabilities.NoReferenceValuesInFirstExport = true;
                myCapabilities.Normalizations = MANormalizations.None;
                myCapabilities.ObjectConfirmation = MAObjectConfirmation.NoDeleteConfirmation;

                return myCapabilities;
            }
        }

        public IList<ConfigParameterDefinition> GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> configParametersDefinitions = new List<ConfigParameterDefinition>();

            switch (page)
            {
                case ConfigParameterPage.Connectivity:
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.username, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(Param.password, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.clientId, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(Param.openIdSecret, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.scope, String.Empty, DefaultValue.scope));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.idpUri, String.Empty, DefaultValue.accessTokenUri));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.assetId, String.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.felleskomponentUri, String.Empty, DefaultValue.felleskomponentUri));

                    break;
                case ConfigParameterPage.Global:
                    {
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("HTTP Client settings"));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.httpClientTimeout, String.Empty, DefaultValue.httpClientTimeout));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.useLocalCache, false));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.abortIfResourceTypeEmpty, true));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.useThresholdValues, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.overrideThresholds, false));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.thresholdValuePerson, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.thresholdValueArbeidsforhold, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.thresholdValueOrganisasjonselementGruppe, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Importparametre"));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonsnavn, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonskortnavn, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonsnummer, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonEpostadresse, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.standardBusinessUnitName, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.standardBusinessUnitOrgNo, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.standardBusinessUnitEmailAddress, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.usernameToLowerCase,false));
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.employmentCompareDate, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeEmploymentStarts, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysAfterEmploymentEnds, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.filterResourceTypes, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.filterEmploymentTypes, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.filterEmploymentTypesInActiveUsers, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.filterPositionCodes, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAllePersonalKategoriGrupper, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererOrgEnhetGrupper, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.excludeEmptyGroups, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.leggTilLederSomMedlemIGruppe, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.leggTilLedereIOverordnetGruppe, true));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.finnAlltidLederForAnsatt, false));

                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Prefix og suffix for gruppenavn"));genererAllePersonalKatergoriGrupper
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.GruppePrefix, "", DefaultValue.GruppePrefix));
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.GruppeSuffix, "", DefaultValue.GruppeSuffix));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Gruppesuffikser"));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleSuffix, String.Empty, DefaultValue.allPersonalresourcesSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAnsatteSuffix, String.Empty, DefaultValue.allEmployeesSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAllePolitikereSuffix, String.Empty, DefaultValue.allPolitiansSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAndreSuffix, String.Empty, DefaultValue.allOthersSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleLedereSuffix, String.Empty, DefaultValue.allLeadersSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleLarereSuffix, String.Empty, DefaultValue.allTeachersSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAdmSkoleSuffix, String.Empty, DefaultValue.allAdminsSchoolsSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAdmEnSkoleSuffix, String.Empty, DefaultValue.allAdminsOneSchoolSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAnsatteIkkeSkoleSuffix, String.Empty, DefaultValue.allAdminsNonSchoolSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAndreSkoleSuffix, String.Empty, DefaultValue.allOthersSchoolSuffix));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAndreIkkeSkoleSuffix, String.Empty, DefaultValue.allOthersNonSchoolSuffix));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGruppeAlleAnsatte, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAnsatteRessurskategorier, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAnsatteArbeidsforholdtyper, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGruppeAllePolitikere, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAllePolitikerRessurskategorier, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAllePolitikereArbeidsforholdtyper, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGruppeAlleAndre, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAndreRessurskategorier, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.gruppeAlleAndreArbeidsforholdtyper, String.Empty, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGruppeAlleLedere, true));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.brukOrganisasjonsKode, true));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                        
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGrupperForenheter, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateTextParameter(Param.enheterAggrGrupper, String.Empty));

                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererAggrGrupperAlleLarerAdmin, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.genererLarerAdmAnsGrupper, true));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.larerArbeidsforholdtyper, String.Empty, String.Empty));
                        configParametersDefinitions.Add(ConfigParameterDefinition.CreateTextParameter(Param.alleSkoleEnheter, String.Empty));

                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Eksportparametre"));
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.waitTime, String.Empty, String.Empty));
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.lowerLimit, String.Empty, String.Empty));
                        //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.upperLimit, String.Empty, String.Empty));
                    }
                    break;
                case ConfigParameterPage.Partition:
                case ConfigParameterPage.RunStep:
                    break;
            }

            return configParametersDefinitions;
        }

        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            
            // Configuration validation
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ParameterValidationResult myResults = new ParameterValidationResult();
            /*
            var TokenResult = TokenResponseHelper(configParameters);

            if(TokenResult.HttpStatusCode == HttpStatusCode.OK)
            {
                myResults.Code = ParameterValidationResultCode.Success;
            }
            else
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = TokenResult.ErrorDescription;
            }
            */
            myResults.Code = ParameterValidationResultCode.Success;
            return myResults;
        }

        public Schema GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            // Create CS Schema type person
            SchemaType Person = SchemaType.Create(CSobjecttypes.Person, true);

            // Anchor
            Person.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(PersonAttributes.PersonalAnsattnummerUri, AttributeType.String));

            // Attributes
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalAnsattnummer, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalPersonalressurskategori, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalBrukernavn, AttributeType.String, AttributeOperation.ImportExport));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportExport));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportExport));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalKontaktinformasjonTelefonnummer, AttributeType.String, AttributeOperation.ImportExport));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalKontaktinformasjonSip, AttributeType.String, AttributeOperation.ImportExport));
            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonalLederOrganisasjonselementRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonalOrganisasjonRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonNavnFornavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonNavnMellomnavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonNavnEtternavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonNettsted, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonFodselsdato, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonFodselsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonKontaktinformasjonPostadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKontaktinformasjonPostadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKontaktinformasjonPostadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonBostedsadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonBostedsadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonBostedsadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));

            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonMalformNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonMalformKode, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonMorsmalNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonMorsmalKode, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonLandsNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonLandsKode, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKjonnNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonKjonnKode, AttributeType.String, AttributeOperation.ImportOnly));

            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonalArbeidsforholdRefs, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalAntallAktiveArbeidsforhold, AttributeType.Integer, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalAktivAnsettelsesperiodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalAktivAnsettelsesperiodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));

            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonHovedstillingstittelRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingArbeidsforholdtype, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingStillingskode, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingStillingskodeNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingstittel, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonHovedstilling, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedOrgenhetRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingArbeidsstedOrganisasjonsId, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingArbeidsstedOrganisasjonsKode, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingArbeidsstedNavn, AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonHovedOrgenhet, AttributeType.String, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalHovedstillingLederRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonalOrgenhetRefs, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(PersonAttributes.PersonalBusinessUnitRefs, AttributeType.Reference, AttributeOperation.ImportOnly));
            Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(PersonAttributes.PersonalPrimaryBusinessUnitRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("Land", AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("Kjønn", AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("Målform", AttributeType.String, AttributeOperation.ImportOnly));
            //Person.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("Morsmål", AttributeType.String, AttributeOperation.ImportOnly));


           // Group
           SchemaType Group = SchemaType.Create(CSobjecttypes.Gruppe, true);

            // Anchor
            Group.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(GroupAttributes.GruppeUri, AttributeType.String, AttributeOperation.ImportOnly));

            // Attributes
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeID, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeOrganisasjonselementKode, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeOrganisasjonselementRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeKortnavn, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeType, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeGyldighetsPeriodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeGyldighetsPeriodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            //Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GroupOwnerString, AttributeType.String, AttributeOperation.ImportOnly));
            //Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GroupParentString, AttributeType.String, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GruppeEierRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            //Group.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(GroupAttributes.GroupParentRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Group.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(GroupAttributes.GruppeMedlemRefs, AttributeType.Reference, AttributeOperation.ImportOnly));

            // Unit
            // Group
            SchemaType Unit = SchemaType.Create(CSobjecttypes.Organisasjonselement, true);

            // Anchor 
            Unit.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(UnitAttributes.OrganisasjonselementOrganisasjonsidUri, AttributeType.String, AttributeOperation.ImportOnly));

            // Attributes
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementOrganisasjonsId, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementOrganisasjonsKode, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementBeskrivelse, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementNavn, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementKortnavn, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementType, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementGyldighetsPeriodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementGyldighetsPeriodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            //Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementLederString, AttributeType.String, AttributeOperation.ImportOnly));
            //Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementOverordnetString, AttributeType.String, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementLederRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Unit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(UnitAttributes.OrganisasjonselementOverordnetRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            //BusinessUnit
            SchemaType BusinessUnit = SchemaType.Create(CSobjecttypes.Virksomhet, true);

            BusinessUnit.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(VirksomhetAttributes.VirksomhetUri, AttributeType.String, AttributeOperation.ImportOnly));
            //BusinessUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(VirksomhetAttributes.BusinessUnitOrganizationCode, AttributeType.String, AttributeOperation.ImportOnly));
            BusinessUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(VirksomhetAttributes.Virksomhetsnavn, AttributeType.String, AttributeOperation.ImportOnly));
            BusinessUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(VirksomhetAttributes.Virksomhetskortnavn, AttributeType.String, AttributeOperation.ImportOnly));
            BusinessUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(VirksomhetAttributes.VirksomhetOrganisasjonsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            BusinessUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(VirksomhetAttributes.VirksomhetEpostAdresse, AttributeType.String, AttributeOperation.ImportOnly));

            //Organization
            SchemaType Organization = SchemaType.Create(CSobjecttypes.Organisasjon, true);

            Organization.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(OrganizationAttributes.OrganisasjonsUri, AttributeType.String, AttributeOperation.ImportOnly));
            Organization.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(OrganizationAttributes.OrganisasjonsKode, AttributeType.String, AttributeOperation.ImportOnly));
            Organization.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(OrganizationAttributes.Organisasjonsnavn, AttributeType.String, AttributeOperation.ImportOnly));
            Organization.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(OrganizationAttributes.Organisasjonskortnavn, AttributeType.String, AttributeOperation.ImportOnly));
            Organization.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(OrganizationAttributes.Organisasjonsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            Organization.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(OrganizationAttributes.OrganisasjonEpostAdresse, AttributeType.String, AttributeOperation.ImportOnly));

            // Employments
            SchemaType Employment = SchemaType.Create(CSobjecttypes.Arbeidsforhold, true);

            // Anchor
            Employment.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(ArbeidsforholdAttributes.ArbeidsforholdUri, AttributeType.String, AttributeOperation.ImportOnly));

            // Attributes            
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdPersonalressursRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdPersonalressursAnsattnummer, AttributeType.String, AttributeOperation.ImportOnly));           
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdStillingskodeKode, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdStillingstittel, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdHovedstilling, AttributeType.Boolean, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdAnsettelsesprosent, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdGyldighetsperiodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdGyldighetsperiodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdsperiodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdsperiodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedOrgId, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsstedOrgKode, AttributeType.String, AttributeOperation.ImportOnly));
            Employment.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(ArbeidsforholdAttributes.ArbeidsforholdArbeidsforholdstypeKode, AttributeType.String, AttributeOperation.ImportOnly));
           
            // Return schema
            Schema schema = Schema.Create();
            schema.Types.Add(Person);
            schema.Types.Add(Group);
            schema.Types.Add(Employment);
            schema.Types.Add(Unit);
            schema.Types.Add(BusinessUnit);
            schema.Types.Add(Organization);

            return schema;
        }
        #endregion

        #region Private REST and WS methods
        private NetworkCredential NetworkCredential(KeyedCollection<string, ConfigParameter> configParameters)
        {
            return new NetworkCredential(configParameters["Username"].Value, configParameters["Password"].SecureValue);
        }

        #endregion


        #region Import methods
        /*
         * Attributes used during import 
         */
        List<ImportListItem> ImportedObjectsList;
        int GetImportEntriesIndex, GetImportEntriesPageSize;

        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters,
                                                Schema types,
                                                OpenImportConnectionRunStep importRunStep)
        {
            _globalConfigParameters = configParameters;
            // Instantiate ImportedObjectsList
            ImportedObjectsList = new List<ImportListItem>();


            // Get data from WS
            Stopwatch Stopwatch = new Stopwatch();
            Stopwatch.Start();
            Logger.Log.Info("Starting import");

            List<string> uriPaths =
                    new List<string> {
                    DefaultValue.administrasjonPersonalArbeidsforholdUri,
                    DefaultValue.administrasjonPersonalPersonUri,
                    DefaultValue.administrasjonPersonalPersonalRessursUri,
                    DefaultValue.administrasjonOrganisasjonOrganisasjonselementUri,
                    DefaultValue.administrasjonFunksjonUri,
                    DefaultValue.administrasjonStillingsKodeUri,
                    DefaultValue.administrasjonUkeTimeTallUri,
                    DefaultValue.administrasjonArbeidsforholdstypeUri,
                    DefaultValue.administrasjonAnsvarUri,
                    DefaultValue.felleskodeverkKjonnUri,
                    DefaultValue.felleskodeverkLandkodeUri,
                    DefaultValue.felleskodeverkSprakUri
                    };


            var personalressursDict = new Dictionary<string, IEmbeddedResourceObject>();
            var personalpersonDict = new Dictionary<string, IEmbeddedResourceObject>();
            var arbeidsforholdDict = new Dictionary<string, IEmbeddedResourceObject>();
            var organisasjonselementDict = new Dictionary<string, IEmbeddedResourceObject>();
            var ansvarDict = new Dictionary<string, IEmbeddedResourceObject>();
            var stillingsKodeDict = new Dictionary<string, IEmbeddedResourceObject>();
            var timerprukeDict = new Dictionary<string, IEmbeddedResourceObject>();
            var arbeidsforholdTypeDict = new Dictionary<string, IEmbeddedResourceObject>();
            var funskjonDict = new Dictionary<string, IEmbeddedResourceObject>();
            var kjonnDict = new Dictionary<string, IEmbeddedResourceObject>();
            var sprakDict = new Dictionary<string, IEmbeddedResourceObject>();
            var landDict = new Dictionary<string, IEmbeddedResourceObject>();


            var personalRessursEntriesPath = DefaultValue.administrasjonPersonalPersonalRessursUri;
            var personalpersonEntriesPath = DefaultValue.administrasjonPersonalPersonUri;
            var arbeidsforholdEntriesPath = DefaultValue.administrasjonPersonalArbeidsforholdUri;
            var ansvarEntriesPath = DefaultValue.administrasjonAnsvarUri;
            var stillingskodeEntriesPath = DefaultValue.administrasjonStillingsKodeUri;
            var ukeTimeTallEntriesPath = DefaultValue.administrasjonUkeTimeTallUri;
            var arbeidsforholdTypeEntriesPath = DefaultValue.administrasjonArbeidsforholdstypeUri;
            var funksjonEntriesPath = DefaultValue.administrasjonFunksjonUri;
            var sprakEntriesPath = DefaultValue.felleskodeverkSprakUri;
            var landEntriesPath = DefaultValue.felleskodeverkLandkodeUri;
            var kjonnEntriesPath = DefaultValue.felleskodeverkKjonnUri;
            var organisasjonselementEntriesPath = DefaultValue.administrasjonOrganisasjonOrganisasjonselementUri;

            var felleskomponentUri = configParameters[Param.felleskomponentUri].Value;

            var componentList = new List<string>() { personalRessursEntriesPath, personalpersonEntriesPath, arbeidsforholdEntriesPath,
                                                    ansvarEntriesPath, stillingskodeEntriesPath, ukeTimeTallEntriesPath, arbeidsforholdTypeEntriesPath,
                                                    funksjonEntriesPath, sprakEntriesPath, landEntriesPath, kjonnEntriesPath, organisasjonselementEntriesPath };

            var itemsCountPerComponent = new Dictionary<string, int>();

            foreach (var component in componentList)
            {
                itemsCountPerComponent.Add(component, 0);
            }

            Dictionary<string, IEmbeddedResourceObject> resourceDict = GetDataFromFINTApi(configParameters, uriPaths);

            foreach (var uriKey in resourceDict.Keys)
            {
                Logger.Log.DebugFormat("Adding resource {0} to dictionary", uriKey);
                var resourceType = GetUriPathForClass(uriKey);

                itemsCountPerComponent[resourceType]++;

                if (resourceType == personalRessursEntriesPath)
                {
                    var personalResource = resourceDict[uriKey];
                    var ansattnummerUri = GetIdentifikatorUri(personalResource, felleskomponentUri, FintAttribute.ansattnummer);

                    personalressursDict.Add(ansattnummerUri, personalResource);
                    UpdateIdentifierMappingDict(ansattnummerUri, personalResource, ref _personalressursIdMappingDict);
                }
                else if (resourceType == arbeidsforholdEntriesPath)
                {
                    arbeidsforholdDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == organisasjonselementEntriesPath)
                {
                    organisasjonselementDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == personalpersonEntriesPath)
                {
                    personalpersonDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == funksjonEntriesPath)
                {
                    funskjonDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == stillingskodeEntriesPath)
                {
                    stillingsKodeDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == arbeidsforholdTypeEntriesPath)
                {
                    arbeidsforholdTypeDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == ansvarEntriesPath)
                {
                    ansvarDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == kjonnEntriesPath)
                {
                    kjonnDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == sprakEntriesPath)
                {
                    sprakDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == landEntriesPath)
                {
                    landDict.Add(uriKey, resourceDict[uriKey]);
                }
                else if (resourceType == ukeTimeTallEntriesPath)
                {
                    timerprukeDict.Add(uriKey, resourceDict[uriKey]);
                }
            }


            // Declare Dictionary to use later
            Dictionary<string, ImportListItem> importedObjectsDict = new Dictionary<string, ImportListItem>();
            Dictionary<string, List<(string, EmploymentPeriodType)>> employeesActiveEmployments = new Dictionary<string, List<(string, EmploymentPeriodType)>>();
            Dictionary<string, HovedstillingInfo> ansattHovedstillingsinfo = new Dictionary<string, HovedstillingInfo>();

            foreach (var ansvar in ansvarDict)
            {
                var _ansvar = new Ansvar();
                if (!importedObjectsDict.TryGetValue(ansvar.Key, out ImportListItem ansvarValue))
                {
                    _ansvar = AnsvarFactory.Create(ansvar.Value.State);
                    importedObjectsDict.Add(ansvar.Key, new ImportListItem { HrAnsvar = _ansvar });
                }
            }

            foreach (var funksjon in funskjonDict)
            {
                var _funksjon = new Funksjon();
                if (!importedObjectsDict.TryGetValue(funksjon.Key, out ImportListItem funkValue))
                {
                    _funksjon = FunksjonFactory.Create(funksjon.Value.State);
                    importedObjectsDict.Add(funksjon.Key, new ImportListItem { HrFunksjon = _funksjon });
                }
            }

            foreach (var arbeidsforholdtype in arbeidsforholdTypeDict)
            {
                var _arbeidsforholdType = new Arbeidsforholdstype();
                if (!importedObjectsDict.TryGetValue(arbeidsforholdtype.Key, out ImportListItem arbfhValue))
                {
                    _arbeidsforholdType = ArbeidsforholdtypeFactory.Create(arbeidsforholdtype.Value.State);
                    importedObjectsDict.Add(arbeidsforholdtype.Key, new ImportListItem { HrArbeidsforholdstype = _arbeidsforholdType });
                }

            }

            foreach (var stillskode in stillingsKodeDict)
            {
                var _stillingskode = new Stillingskode();
                if (!importedObjectsDict.TryGetValue(stillskode.Key, out ImportListItem stillkodeValue))
                {
                    _stillingskode = StillingskodeFactory.Create(stillskode.Value.State);
                    importedObjectsDict.Add(stillskode.Key, new ImportListItem { HrStillingskode = _stillingskode });
                }
            }

            foreach (var timer in timerprukeDict)
            {
                var _timerpruke = new Uketimetall();
                if (!importedObjectsDict.TryGetValue(timer.Key, out ImportListItem timeValue))
                {
                    _timerpruke = TimerperukeFactory.Create(timer.Value.State);
                    importedObjectsDict.Add(timer.Key, new ImportListItem { HrTimerprUke = _timerpruke });
                }
            }

            bool abortIfResourceTypeEmpty = configParameters[Param.abortIfResourceTypeEmpty].Value == "1";

            foreach (var resource in itemsCountPerComponent.Keys)
            {
                var noOfItems = itemsCountPerComponent[resource];
                if (noOfItems == 0)
                {
                    if (abortIfResourceTypeEmpty)
                    {
                        var message = string.Format("No items returned for resource {0}. Aborting the import", resource);
                        Logger.Log.Error(message);

                        throw new FINTResourceEmptyException(message);
                    }
                    else
                    {
                        var message = string.Format("No items returned for resource {0}. Continuing because abortIfResourceTypeEmpty is set to false in MA config", resource);
                        Logger.Log.Info(message);
                    }
                }
                else
                {
                    var message = string.Format("{0} items returned for resource {1}", noOfItems.ToString(), resource);
                    Logger.Log.Info(message);
                }
            }

            bool useOrgUnitCodeAsParameter = configParameters[Param.brukOrganisasjonsKode].Value == "1";

            bool generateAllPersonalGroups = configParameters[Param.genererAllePersonalKategoriGrupper].Value == "1";
            bool generateOrgUnitGroups = configParameters[Param.genererOrgEnhetGrupper].Value == "1";
            bool putLeaderAsMemberInGroup = configParameters[Param.leggTilLederSomMedlemIGruppe].Value == "1";
            bool putLeaderInParentGroup = configParameters[Param.leggTilLedereIOverordnetGruppe].Value == "1";
            bool findAlwaysManagerForEmployee = configParameters[Param.finnAlltidLederForAnsatt].Value == "1";

            bool generateGroupAllEmployees= configParameters[Param.genererAggrGruppeAlleAnsatte].Value == "1";
            bool generateGroupAllPoliticians = configParameters[Param.genererAggrGruppeAllePolitikere].Value == "1";
            bool generateGroupAllOthers = configParameters[Param.genererAggrGruppeAlleAndre].Value == "1";
            bool generateGroupAllLeaders = configParameters[Param.genererAggrGruppeAlleLedere].Value == "1";

            bool generateGroupsAllTeachersNonTeacherEmployees = configParameters[Param.genererAggrGrupperAlleLarerAdmin].Value == "1";
            bool generateAggrGroupsForUnits = configParameters[Param.genererAggrGrupperForenheter].Value == "1";

            bool generateSchoolGroups = configParameters[Param.genererLarerAdmAnsGrupper].Value == "1";
            
            HashSet<string> employeeResourceCategories = GetConfigParameterList(configParameters, Param.gruppeAlleAnsatteRessurskategorier);
            HashSet<string> employeeEmploymentTypes = GetConfigParameterList(configParameters, Param.gruppeAlleAnsatteArbeidsforholdtyper);

            HashSet<string> politicianResourceCategories = GetConfigParameterList(configParameters, Param.gruppeAllePolitikerRessurskategorier);
            HashSet<string> politicianEmploymentTypes = GetConfigParameterList(configParameters, Param.gruppeAllePolitikereArbeidsforholdtyper);

            HashSet<string> otherResourceCategories = GetConfigParameterList(configParameters, Param.gruppeAlleAndreRessurskategorier);
            HashSet<string> otherEmploymentTypes = GetConfigParameterList(configParameters, Param.gruppeAlleAndreArbeidsforholdtyper);

            HashSet<string> teacherEmploymentTypes = GetConfigParameterList(configParameters, Param.larerArbeidsforholdtyper);

            HashSet<string> allSchoolOrgUnitIds = GetConfigParameterList(configParameters, Param.alleSkoleEnheter);

            Dictionary<string, int> aggrGroupOrgUnitsParams = GetAggrGroupForUnitsParameters(configParameters, Param.enheterAggrGrupper);
            var aggrGroupOrgUnitIds = aggrGroupOrgUnitsParams.Keys;
            Dictionary<string, int> aggrGroupOrgUnitUrisAndSearchDepth = new Dictionary<string, int>();


            //string GrpSuffix = configParameters[Param.GruppeSuffix].Value;

            Dictionary<string, List<string>> leaderOfUnitDict = new Dictionary<string, List<string>>();

            Dictionary<string, string> orgUnitIdToOrgUnitUri = new Dictionary<string, string>();

            Dictionary<string, string> orgUnitUriToSchoolOrgUnit = new Dictionary<string, string>();

            List<string> allSchoolOrgUnitUris = new List<string>();
            List<string> allAggrGroupOrgUnitUris = new List<string>();

            List<string> groupsList = new List<string>();

            int noOfOrgElements = 0;

            string topOrgElementItemUri = string.Empty;
            string hrOrganizationUri = string.Empty;
            string hrStandardBusinessUnitUri = string.Empty;

            string groupAllPersonalUri = string.Empty;
            string groupAllEmployeesUri = string.Empty;
            string groupAllPoliticiansUri = string.Empty;
            string groupAllOthersUri = string.Empty;
            string groupAllLeadersUri = string.Empty;
            string groupAllTeachersUri = string.Empty;
            string groupAllSchoolAdmUri = string.Empty;
            string groupAllNonSchoolAdmUri = string.Empty;
            string groupAllSchoolOthUri = string.Empty;
            string groupAllNonSchoolOthUri = string.Empty;

            foreach (var orgElementItem in organisasjonselementDict)
            {
                var orgElementItemUri = orgElementItem.Key;

                Logger.Log.Debug($"Start reading orgelement {orgElementItemUri} into importedObjectsDict");

                var orgElement = orgElementItem.Value;

                if (!importedObjectsDict.TryGetValue(orgElementItemUri, out ImportListItem unitValue))
                {
                    var _hrUnit = HROrgFactory.Create(orgElementItemUri, orgElement.State);

                    if (IsOrgElementActive(_hrUnit))
                    {
                        var lederUri = string.Empty;
                        var overordnetOrgElementUri = string.Empty;
                        var underordnetUris = new List<string>();

                        var orgElementLinks = orgElement.Links;

                        if (orgElementLinks.TryGetValue(ResourceLink.leder, out IEnumerable<ILinkObject> lederLink))
                        {
                            var lederLinkUri = LinkToString(lederLink);

                            if (_personalressursIdMappingDict.TryGetValue(lederLinkUri, out string mappedLederUri))
                            {
                                lederUri = mappedLederUri;

                                if (!leaderOfUnitDict.TryGetValue(lederUri, out List<string> orgelementer))
                                {
                                    var orgelementList = new List<string>();
                                    orgelementList.Add(orgElementItemUri);

                                    leaderOfUnitDict.Add(lederUri, orgelementList);
                                }
                                else
                                {
                                    orgelementer.Add(orgElementItemUri);
                                    leaderOfUnitDict[lederUri] = orgelementer;
                                }
                            }
                            else
                            {
                                string message = $"{lederLinkUri} is referenced as leder by {orgElementItemUri}";
                                message += $"but the resource is missing on the {DefaultValue.administrasjonPersonalPersonalRessursUri} endpoint";
                                Logger.Log.Error(message);

                            }
                        }
                        if (orgElementLinks.TryGetValue(ResourceLink.overordnet, out IEnumerable<ILinkObject> overordnetLink))
                        {
                            overordnetOrgElementUri = LinkToString(overordnetLink);

                            if (overordnetOrgElementUri == orgElementItemUri)
                            {
                                topOrgElementItemUri = orgElementItemUri;

                                if (string.IsNullOrEmpty(_hrUnit.OrganisasjonselementNavn))
                                {
                                    var organizationName = configParameters[Param.organisasjonsnavn].Value;
                                    _hrUnit.OrganisasjonselementNavn = organizationName;
                                }
                                if (string.IsNullOrEmpty(_hrUnit.OrganisasjonselementKortnavn))
                                {
                                    var organizationShortname = configParameters[Param.organisasjonskortnavn].Value;
                                    _hrUnit.OrganisasjonselementKortnavn = organizationShortname;
                                }
                                if (string.IsNullOrEmpty(_hrUnit.OrganisasjonselementOrganisasjonsnummer))
                                {
                                    var organizationNumber = configParameters[Param.organisasjonsnummer].Value;
                                    _hrUnit.OrganisasjonselementOrganisasjonsnummer = organizationNumber;
                                }
                                var hrOrganization = HROrgFactory.Create(_hrUnit);
                                hrOrganizationUri = hrOrganization.HROrganizationUri;
                                hrOrganization.HROrganizationEmailAddress = configParameters[Param.organisasjonEpostadresse].Value;

                                importedObjectsDict.Add(hrOrganizationUri, new ImportListItem { HrOrganization = hrOrganization });

                                // create default business unit (fallback)  

                                string standardBusinessUnitName = configParameters[Param.standardBusinessUnitName].Value;
                                string standardBusinessUnitOrgNo = configParameters[Param.standardBusinessUnitOrgNo].Value;
                                string standardBusinessUnitEmailAddress = configParameters[Param.standardBusinessUnitEmailAddress].Value;

                                if (!string.IsNullOrEmpty(standardBusinessUnitName) && !string.IsNullOrEmpty(standardBusinessUnitOrgNo) && !string.IsNullOrEmpty(standardBusinessUnitEmailAddress))
                                {
                                    hrStandardBusinessUnitUri = topOrgElementItemUri + Delimiter.suffix + DefaultValue.businessUnitSuffix;

                                    var standardBusinessUnit = new HRBusinessUnit
                                    {
                                        BusinessUnitUri = hrStandardBusinessUnitUri,
                                        BusinessUnitName = standardBusinessUnitName,
                                        BusinessUnitOrganizationNumber = standardBusinessUnitOrgNo,
                                        BusinessUnitEmailAddress = standardBusinessUnitEmailAddress
                                    };
                                    importedObjectsDict.Add(hrStandardBusinessUnitUri, new ImportListItem { HrBusinessUnit = standardBusinessUnit });
                                }
                            }
                            var orgCode = _hrUnit.OrganisasjonselementOrganisasjonsKode;

                            if (allSchoolOrgUnitIds.Contains(orgCode))
                            {
                                var hrBusinessUnit = HROrgFactory.Create(_hrUnit, DefaultValue.businessUnitSuffix);
                                var hrBusinessUnitUri = hrBusinessUnit.BusinessUnitUri;
                                importedObjectsDict.Add(hrBusinessUnitUri, new ImportListItem { HrBusinessUnit = hrBusinessUnit });
                            }
                        }
                        if (orgElementLinks.TryGetValue(ResourceLink.underordnet, out IEnumerable<ILinkObject> underordnetLink))
                        {
                            underordnetUris = LinksToStrings(underordnetLink);
                        }
                        _hrUnit.OrganisasjonselementLeder = lederUri;
                        _hrUnit.OrganisasjonselementOverordnet = overordnetOrgElementUri;
                        _hrUnit.OrganisasjonselementUnderordnet = underordnetUris;

                        importedObjectsDict.Add(orgElementItemUri, new ImportListItem { HrUnit = _hrUnit });
                        Logger.Log.Debug($"Adding hrUnit object {orgElementItemUri} to importedObjectsDict");

                        var orgUnitId = (useOrgUnitCodeAsParameter) ? _hrUnit.OrganisasjonselementOrganisasjonsKode : _hrUnit.OrganisasjonselementOrganisasjonsid;
                        orgUnitIdToOrgUnitUri.Add(orgUnitId, orgElementItemUri);

                        noOfOrgElements++;

                        if (generateAllPersonalGroups || generateGroupAllEmployees || generateGroupAllPoliticians || generateGroupAllOthers || generateGroupAllLeaders || generateGroupsAllTeachersNonTeacherEmployees)
                        {
                            if (overordnetOrgElementUri == orgElementItemUri)
                            {
                                var _hrGroup = new HRGroup();
                                var orgElementState = orgElement.State;

                                if (generateAllPersonalGroups)
                                {
                                    (groupAllPersonalUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri, GroupType.aggrAll, _hrUnit, configParameters);

                                    if (!importedObjectsDict.TryGetValue(groupAllPersonalUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllPersonalUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }

                                if (generateGroupAllEmployees)
                                {
                                    (groupAllEmployeesUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri, GroupType.aggrEmp, _hrUnit, configParameters);

                                    if (!importedObjectsDict.TryGetValue(groupAllEmployeesUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllEmployeesUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }
                                if (generateGroupAllPoliticians)
                                {
                                    (groupAllPoliticiansUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri, GroupType.aggrPol, _hrUnit, configParameters);

                                    if (!importedObjectsDict.TryGetValue(groupAllPoliticiansUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllPoliticiansUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }
                                if (generateGroupAllOthers)
                                {
                                    (groupAllOthersUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri, GroupType.aggrOth, _hrUnit, configParameters);

                                    if (!importedObjectsDict.TryGetValue(groupAllOthersUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllOthersUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }
                                if (generateGroupAllLeaders)
                                {
                                    (groupAllLeadersUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri, GroupType.aggrMan, _hrUnit, configParameters);

                                    if (!importedObjectsDict.TryGetValue(groupAllLeadersUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllLeadersUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }
                                if (generateGroupsAllTeachersNonTeacherEmployees)
                                {
                                    var isSchoolNonSchoolGroup = true;

                                    var isSchoolGroup = true;

                                    (groupAllTeachersUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri + DefaultValue.schoolTag, GroupType.aggrFac, _hrUnit, configParameters, isSchoolGroup, isSchoolNonSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupAllTeachersUri, out ImportListItem dummyValue))
                                    {
                                        importedObjectsDict.Add(groupAllTeachersUri, new ImportListItem { HrGroup = _hrGroup });
                                    }

                                    (groupAllSchoolAdmUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri + DefaultValue.schoolTag, GroupType.aggrAdm, _hrUnit, configParameters, isSchoolGroup, isSchoolNonSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupAllSchoolAdmUri, out ImportListItem dummyValue1))
                                    {
                                        importedObjectsDict.Add(groupAllSchoolAdmUri, new ImportListItem { HrGroup = _hrGroup });
                                    }

                                    (groupAllSchoolOthUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri + DefaultValue.schoolTag, GroupType.aggrOth, _hrUnit, configParameters, isSchoolGroup, isSchoolNonSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupAllSchoolOthUri, out ImportListItem dummyValue3))
                                    {
                                        importedObjectsDict.Add(groupAllSchoolOthUri, new ImportListItem { HrGroup = _hrGroup });
                                    }

                                    isSchoolGroup = false;

                                    (groupAllNonSchoolAdmUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri + DefaultValue.nonSchoolTag, GroupType.aggrAdm, _hrUnit, configParameters, isSchoolGroup, isSchoolNonSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupAllNonSchoolAdmUri, out ImportListItem dummyValue2))
                                    {
                                        importedObjectsDict.Add(groupAllNonSchoolAdmUri, new ImportListItem { HrGroup = _hrGroup });
                                    }

                                    (groupAllNonSchoolOthUri, _hrGroup) = GenerateAggrGroup(topOrgElementItemUri + DefaultValue.nonSchoolTag, GroupType.aggrOth, _hrUnit, configParameters, isSchoolGroup, isSchoolNonSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupAllNonSchoolOthUri, out ImportListItem dummyValue4))
                                    {
                                        importedObjectsDict.Add(groupAllNonSchoolOthUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }
                            }
                        }
                        if (allSchoolOrgUnitIds.Contains(orgUnitId))
                        {
                            allSchoolOrgUnitUris.Add(orgElementItemUri);

                            if (generateSchoolGroups)
                            {
                                var isSchoolGroup = true;
                                var _hrGroup = new HRGroup();
                                var groupSchoolEmployeesUri = string.Empty;
                                var groupTeachersUri = string.Empty;
                                var groupSchoolAdminsUri = string.Empty;
                                var groupSchoolOthersUri = string.Empty;

                                if (generateAllPersonalGroups)
                                {
                                    var groupSchoolAllPersonalUri = string.Empty;

                                    (groupSchoolAllPersonalUri, _hrGroup) = GenerateAggrGroup(orgElementItemUri, GroupType.aggrAll, _hrUnit, configParameters, isSchoolGroup);

                                    if (!importedObjectsDict.TryGetValue(groupSchoolAllPersonalUri, out ImportListItem dummyValue0))
                                    {
                                        importedObjectsDict.Add(groupSchoolAllPersonalUri, new ImportListItem { HrGroup = _hrGroup });
                                    }
                                }

                                (groupSchoolEmployeesUri, _hrGroup) = GenerateAggrGroup(orgElementItemUri, GroupType.aggrEmp, _hrUnit, configParameters, isSchoolGroup);

                                if (!importedObjectsDict.TryGetValue(groupSchoolEmployeesUri, out ImportListItem dummyValue))
                                {
                                    importedObjectsDict.Add(groupSchoolEmployeesUri, new ImportListItem { HrGroup = _hrGroup });
                                }

                                (groupTeachersUri, _hrGroup) = GenerateAggrGroup(orgElementItemUri, GroupType.aggrFac, _hrUnit, configParameters, isSchoolGroup);

                                if (!importedObjectsDict.TryGetValue(groupTeachersUri, out ImportListItem dummyValue1))
                                {
                                    importedObjectsDict.Add(groupTeachersUri, new ImportListItem { HrGroup = _hrGroup });
                                }

                                (groupSchoolAdminsUri, _hrGroup) = GenerateAggrGroup(orgElementItemUri, GroupType.aggrAdm, _hrUnit, configParameters, isSchoolGroup);

                                if (!importedObjectsDict.TryGetValue(groupSchoolAdminsUri, out ImportListItem dummyValue2))
                                {
                                    importedObjectsDict.Add(groupSchoolAdminsUri, new ImportListItem { HrGroup = _hrGroup });
                                }

                                (groupSchoolOthersUri, _hrGroup) = GenerateAggrGroup(orgElementItemUri, GroupType.aggrOth, _hrUnit, configParameters, isSchoolGroup);

                                if (!importedObjectsDict.TryGetValue(groupSchoolOthersUri, out ImportListItem dummyValue3))
                                {
                                    importedObjectsDict.Add(groupSchoolOthersUri, new ImportListItem { HrGroup = _hrGroup });
                                }
                            }
                        }

                        if (generateAggrGroupsForUnits && aggrGroupOrgUnitIds.Contains(orgUnitId))
                        {
                            var depth = aggrGroupOrgUnitsParams[orgUnitId];

                            aggrGroupOrgUnitUrisAndSearchDepth.Add(orgElementItemUri, depth);
                        }

                        if (generateOrgUnitGroups)
                        {
                            var groupUri = orgElementItemUri + Delimiter.suffix + GroupType.ougroup;
                            var _hrGroup = HROrgFactory.Create(groupUri, GroupType.ougroup, _hrUnit, configParameters);
                            //var groupIDUri = _hrGroup.GroupIDUri;

                            if (!importedObjectsDict.TryGetValue(groupUri, out ImportListItem orgValue))
                            {
                                _hrGroup.GroupOwner = lederUri;
                                _hrGroup.GroupParent = overordnetOrgElementUri + Delimiter.suffix + GroupType.ougroup;

                                // only active employees will be added to the group as members. This is done later in the code
                                importedObjectsDict.Add(_hrGroup.GroupIDUri, new ImportListItem { HrGroup = _hrGroup });
                                groupsList.Add(groupUri);
                            }
                        }
                    }

                }

            }
            foreach (var schoolOrgUnitUri in allSchoolOrgUnitUris)
            {
                var baseParentIsSchoolUnit = true;
                SetParentBaseUrisOnSubUnits(schoolOrgUnitUri, schoolOrgUnitUri, importedObjectsDict, baseParentIsSchoolUnit);
            }
            
            if (generateAggrGroupsForUnits)
            {
                foreach (string orgElementUri in aggrGroupOrgUnitUrisAndSearchDepth.Keys)
                {
                    int depth = aggrGroupOrgUnitUrisAndSearchDepth[orgElementUri];

                    List<string> aggrGroupOrgUnitUris = GetSubOrgUnitUris(orgElementUri, depth, importedObjectsDict);

                    foreach (var uri in aggrGroupOrgUnitUris)
                    {
                        if (!allAggrGroupOrgUnitUris.Contains(uri))
                        {
                            allAggrGroupOrgUnitUris.Add(uri);
                        }
                    }                    
                }
                foreach (var aggrGroupOrgUnitUri in allAggrGroupOrgUnitUris)
                {
                    Logger.Log.Debug($"Trying to add aggrgroups for {aggrGroupOrgUnitUri} to CS");
                    if (importedObjectsDict.TryGetValue(aggrGroupOrgUnitUri, out ImportListItem hrUnitImportListItem))
                    {
                        var thisHrUnit = hrUnitImportListItem.HrUnit;

                        var _hrGroup = new HRGroup();
                        var employeeGroupUri = string.Empty;
                        var otherGroupUri = string.Empty;

                        if (generateAllPersonalGroups)
                        {
                            var allPersonalAggrGroupUri = string.Empty;

                            (allPersonalAggrGroupUri, _hrGroup) = GenerateAggrGroup(aggrGroupOrgUnitUri, GroupType.aggrAll, thisHrUnit, configParameters);

                            if (!importedObjectsDict.TryGetValue(allPersonalAggrGroupUri, out ImportListItem dummyValue))
                            {
                                importedObjectsDict.Add(allPersonalAggrGroupUri, new ImportListItem { HrGroup = _hrGroup });
                            }
                        }

                        (employeeGroupUri, _hrGroup) = GenerateAggrGroup(aggrGroupOrgUnitUri, GroupType.aggrEmp, thisHrUnit, configParameters);

                        if (!importedObjectsDict.TryGetValue(employeeGroupUri, out ImportListItem dummyValue0))
                        {
                            importedObjectsDict.Add(employeeGroupUri, new ImportListItem { HrGroup = _hrGroup });
                            Logger.Log.Debug($"Adding aggrgroup for {employeeGroupUri} to CS");
                        }

                        (otherGroupUri, _hrGroup) = GenerateAggrGroup(aggrGroupOrgUnitUri, GroupType.aggrOth, thisHrUnit, configParameters);

                        if (!importedObjectsDict.TryGetValue(otherGroupUri, out ImportListItem dummyValue1))
                        {
                            importedObjectsDict.Add(otherGroupUri, new ImportListItem { HrGroup = _hrGroup });
                            Logger.Log.Debug($"Adding aggrgroup for {otherGroupUri} to CS");
                        }
                    }
                }
                foreach (var orgUnitUri in allAggrGroupOrgUnitUris)
                {
                    SetParentBaseUrisOnSubUnits(orgUnitUri, orgUnitUri, importedObjectsDict);
                }
            }

            if (generateOrgUnitGroups && putLeaderInParentGroup)
            {
                foreach (var groupIDUri in groupsList)
                {
                    if (importedObjectsDict.TryGetValue(groupIDUri, out ImportListItem importListItem))
                    {
                        var hrGroup = importListItem.HrGroup;

                        var hrGroupOwner = hrGroup.GroupOwner;

                        if (!string.IsNullOrEmpty(hrGroupOwner))
                        {
                            var parentUri = hrGroup.GroupParent;

                            if (importedObjectsDict.TryGetValue(parentUri, out ImportListItem parentImportListItem))
                            {
                                var parentHrGroup = parentImportListItem.HrGroup;

                                var parentHrGroupMembers = parentHrGroup.GroupMembers;

                                if (!parentHrGroupMembers.Contains(hrGroupOwner))
                                {
                                    parentHrGroupMembers.Add(hrGroupOwner);
                                }
                            }
                        }                            
                    }
                }
            }

            Logger.Log.Info("Finished parsing and generating org units and org unit groups");

            Logger.Log.Info("Start parsing arbeidsforhold");
            string startValue = configParameters[Param.daysBeforeEmploymentStarts].Value;
            int dayBeforeEmploymentStarts = (string.IsNullOrEmpty(startValue)) ? 0 : Int32.Parse(startValue);

            string endValue = configParameters[Param.daysAfterEmploymentEnds].Value;
            int daysAfterEmploymentEnds = (string.IsNullOrEmpty(endValue)) ? 0 : Int32.Parse(endValue);

            //string employmentCompareDateString = configParameters[Param.employmentCompareDate].Value;

            //DateTime employmentCompareDate = !(string.IsNullOrEmpty(employmentCompareDateString)) ? DateTime.Parse(employmentCompareDateString) : DateTime.Today;
            DateTime employmentCompareDate = DateTime.Today;

            HashSet<string> excludedResourceTypes = new HashSet<string>();
            var paramExcludedResourceTypes = configParameters[Param.filterResourceTypes].Value;

            excludedResourceTypes = GetParameterList(paramExcludedResourceTypes);

            //HashSet<string> excludedEmploymentTypes = new HashSet<string>();

            var paramFilterEmploymentTypes = configParameters[Param.filterEmploymentTypes].Value;
            var paramfilterEmploymentTypesInActiveUsers = configParameters[Param.filterEmploymentTypesInActiveUsers].Value;

            var filterEmploymentTypesInActiveUsersList = !String.IsNullOrEmpty(paramfilterEmploymentTypesInActiveUsers) ? paramfilterEmploymentTypesInActiveUsers.Split(',') : null;

            Dictionary<string, (string Filtertype, List<string> EmploymenttypeList)> filterEmpTypesPerResourceTypeDict = new Dictionary<string, (string, List<string>)>();
            List<string> globalExcludedEmploymentTypes = new List<string>();

            if (paramFilterEmploymentTypes.Contains(';'))
            {
                var filterEmpTypesPerResourceTypeList = paramFilterEmploymentTypes.Split(';');

                foreach (var filterEmpTypesPerResourceType in filterEmpTypesPerResourceTypeList)
                {
                    var employmentTypeList = new List<string>();

                    var filterValue = filterEmpTypesPerResourceType.Split('|');

                    var filterTypeAndResType = filterValue[0];

                    var filterType = string.Empty;

                    bool filterCodePresent;
                    var filterCode = filterTypeAndResType.Substring(0, 1);

                    if (filterCode == "+")
                    {
                        filterType = Filtertype.Include;
                        filterCodePresent = true;
                    }
                    else if (filterCode == "-")
                    {
                        filterType = Filtertype.Exclude;
                        filterCodePresent = true;
                    }
                    else
                    {
                        filterType = Filtertype.Exclude;
                        filterCodePresent = false;
                    }

                    string resType = string.Empty;

                    if (filterCodePresent)
                    {
                        resType = filterTypeAndResType.Substring(1);
                    }
                    else
                    {
                        resType = filterTypeAndResType;
                    }

                    var employmentTypes = filterValue[1].Split(',');
                    foreach (var employmentType in employmentTypes)
                    {
                        employmentTypeList.Add(employmentType);
                    }
                    Logger.Log.Info($"Adding employment type filter: Resourcetype {resType}, filter type {filterType}, employment types {filterValue[1]}");

                    filterEmpTypesPerResourceTypeDict.Add(resType, (filterType, employmentTypeList));
                }
                globalExcludedEmploymentTypes = filterEmpTypesPerResourceTypeDict["*"].EmploymenttypeList;
            }
            else
            {
                var excludedEmploymentTypes = paramFilterEmploymentTypes.Split(',');

                foreach (var excludedEmploymentType in excludedEmploymentTypes)
                {
                    globalExcludedEmploymentTypes.Add(excludedEmploymentType);
                }
            }

            //excludedEmploymentTypes = GetExcludedItemCodes(paramExcludedEmploymentTypes);

            Dictionary<string, string> personalAndResourceCategory = new Dictionary<string, string>();
            Dictionary<string, HashSet<string>> personalAndEmploymentTypes = new Dictionary<string, HashSet<string>>();
            //Dictionary<string, string> politicianAndResourceCategory = new Dictionary<string, string>();
            //Dictionary<string, HashSet<string>> politicianAndEmploymentTypes = new Dictionary<string, HashSet<string>>();
            //Dictionary<string, string> otherAndResourceCategory = new Dictionary<string, string>();
            //Dictionary<string, HashSet<string>> otherAndEmploymentTypes = new Dictionary<string, HashSet<string>>();

            HashSet<string> excludedPositionCodes = new HashSet<string>();
            var paramExcludedPositionCodes = configParameters[Param.filterPositionCodes].Value;

            excludedPositionCodes = GetParameterList(paramExcludedPositionCodes);

            int noOfEmployments = 0;

            foreach (var arbeidsforholdItem in arbeidsforholdDict)
            {
                var arbeidforholdSystemUri = arbeidsforholdItem.Key;
                var arbeidsforhold = arbeidsforholdItem.Value;

                var arbeidsforholdLinks = arbeidsforhold.Links;

                if (arbeidsforholdLinks.TryGetValue(ResourceLink.personalressurs, out IEnumerable<ILinkObject> personalResourceLink))
                {
                    var personalressursLinkUri = LinkToString(personalResourceLink);

                    if (_personalressursIdMappingDict.TryGetValue(personalressursLinkUri, out string personalressursUri))
                    {
                        var state = arbeidsforhold.State;
                        if (state.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue employmentPeriod))
                        {
                            EmploymentPeriodType employmentPeriodType = CheckValidPeriod(personalressursUri, arbeidforholdSystemUri, employmentPeriod, dayBeforeEmploymentStarts, daysAfterEmploymentEnds, employmentCompareDate);

                            if (employmentPeriodType != EmploymentPeriodType.InvalidPeriod)
                            {
                                if (arbeidsforhold.Links.TryGetValue(ResourceLink.arbeidssted, out IEnumerable<ILinkObject> arbeidsStedLink))
                                {
                                    var arbeidsstedUri = LinkToString(arbeidsStedLink);

                                    if (importedObjectsDict.TryGetValue(arbeidsstedUri, out ImportListItem dummyImportListItem))
                                    {

                                        if (personalressursDict.TryGetValue(personalressursUri, out IEmbeddedResourceObject personalResourceData))
                                {
                                    var personalResourceLinks = personalResourceData.Links;
                                    if (personalResourceLinks.TryGetValue(ResourceLink.resourceCategory, out IEnumerable<ILinkObject> resourceCategoryLink))
                                    {
                                        string resourceCategoryId = GetIdValueFromLink(resourceCategoryLink);

                                        if (!excludedResourceTypes.Contains(resourceCategoryId))
                                        {
                                            if (!personalAndResourceCategory.TryGetValue(personalressursUri, out string dummyValue))
                                            {
                                                personalAndResourceCategory.Add(personalressursUri, resourceCategoryId);
                                            }
                                            //if (!politicianAndResourceCategory.TryGetValue(personalressursUri, out string dummyValue1))
                                            //{
                                            //    politicianAndResourceCategory.Add(personalressursUri, resourceCategoryId);
                                            //}
                                            //if (!otherAndResourceCategory.TryGetValue(personalressursUri, out string dummyValue2))
                                            //{
                                            //    otherAndResourceCategory.Add(personalressursUri, resourceCategoryId);
                                            //}

                                            if (arbeidsforholdLinks.TryGetValue(ResourceLink.arbeidsforholdstype, out IEnumerable<ILinkObject> employmentTypeLink))
                                            {
                                                var employmentTypeId = GetIdValueFromLink(employmentTypeLink);

                                                var filterType = string.Empty;
                                                var filterEmploymentTypes = new List<string>();

                                                if (filterEmpTypesPerResourceTypeDict.TryGetValue(resourceCategoryId, out (string Filtertype, List<string> EmploymentTypes) filterEmploymentTypesForResourceType))
                                                {
                                                    filterType = filterEmploymentTypesForResourceType.Filtertype;
                                                    filterEmploymentTypes = filterEmploymentTypesForResourceType.EmploymentTypes;

                                                    Logger.Log.Info($"{personalressursUri}: Employment type filter hit for resource type {resourceCategoryId}. Filtertype is {filterType}");
                                                }
                                                else
                                                {
                                                    Logger.Log.Info($"{personalressursUri}: Global Employment type filter used for resource type {resourceCategoryId}");

                                                    filterType = Filtertype.Exclude;
                                                    filterEmploymentTypes = globalExcludedEmploymentTypes;
                                                }

                                                if ((filterType == Filtertype.Exclude && !filterEmploymentTypes.Contains(employmentTypeId)) || (filterType == Filtertype.Include && filterEmploymentTypes.Contains(employmentTypeId)))
                                                {
                                                    Logger.Log.Info($"{personalressursUri}: {arbeidforholdSystemUri} has valid employment type {employmentTypeId}");

                                                    if (arbeidsforholdLinks.TryGetValue(ResourceLink.stillingskode, out IEnumerable<ILinkObject> positionCodeLink))
                                                    {
                                                        var positionCodeId = GetIdValueFromLink(positionCodeLink);
                                                        if (!excludedPositionCodes.Contains(positionCodeId))
                                                        {
                                                            Logger.Log.Info($"{personalressursUri}: {arbeidforholdSystemUri} has valid position code {positionCodeId}");
                                                            noOfEmployments++;

                                                            (string, EmploymentPeriodType) employmentAndPeriodType = (arbeidforholdSystemUri, employmentPeriodType);

                                                            if (!employeesActiveEmployments.TryGetValue(personalressursUri, out List<(string, EmploymentPeriodType)> val))
                                                            {
                                                                employeesActiveEmployments.Add(personalressursUri, new List<(string, EmploymentPeriodType)> { employmentAndPeriodType });
                                                            }
                                                            else
                                                            {
                                                                employeesActiveEmployments[personalressursUri].Add(employmentAndPeriodType);
                                                            }
                                                            if (!personalAndEmploymentTypes.TryGetValue(personalressursUri, out HashSet<string> employmentTypes))
                                                            {
                                                                personalAndEmploymentTypes.Add(personalressursUri, new HashSet<string> { employmentTypeId });
                                                            }
                                                            else
                                                            {
                                                                personalAndEmploymentTypes[personalressursUri].Add(employmentTypeId);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Logger.Log.Info($"{personalressursUri}: {arbeidforholdSystemUri} has excluded position code {positionCodeId} and is not added to CS");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.Log.Info($"{personalressursUri}: {arbeidforholdSystemUri} has excluded employment type {employmentTypeId} and is not added to CS");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.Log.Info($"{personalressursUri}: Excluded personal resource category {resourceCategoryId}");
                                        }
                                    }
                                }
                                    }
                                    else
                                    {
                                        Logger.Log.Error($"Active arbeidsforhold {arbeidforholdSystemUri} is related to inactive/missing organisasjonselement {arbeidsstedUri}");
                                    }
                                }
                                else
                                {
                                    Logger.Log.Error($"Active arbeidsforhold {arbeidforholdSystemUri} is missing mandatory relation to {ResourceLink.arbeidssted}");
                                }
                            }
                        }
                    }
                    else
                    {
                        string message = $"{personalressursLinkUri} is referenced by {arbeidforholdSystemUri}";
                        message += $"but the resource is missing on the {DefaultValue.administrasjonPersonalPersonalRessursUri} endpoint";
                        Logger.Log.Error(message);
                    }
                }

            }

            Logger.Log.Info("Finished parsing arbeidsforhold and generating for CS");

            Logger.Log.Info("Start parsing personalressurs resources and generating Person objects for CS");
            int noOfPersonalResources = 0;

            bool usernameToLowerCase = configParameters[Param.usernameToLowerCase].Value == "1";

            foreach (var employeeAndEmployment in employeesActiveEmployments)
            {
                var personalResourceUri = employeeAndEmployment.Key;
                var resourcecategory = personalAndResourceCategory[personalResourceUri];

                List<(string employmentUri, EmploymentPeriodType employmentPeriodType)> employmentsAndPeriodType = employeeAndEmployment.Value;

                int noOfEmploymentsAndPeriodType = employmentsAndPeriodType.Count;

                Logger.Log.DebugFormat("{0}: Number of active employments (including future and past employments): {1}", personalResourceUri, noOfEmploymentsAndPeriodType);

                List<int> futureEmployments = new List<int>();
                List<int> pastEmployments = new List<int>();

                for (int i= 0; i < noOfEmploymentsAndPeriodType; i++)
                {
                    var employmentWithPeriodType = employmentsAndPeriodType[i];
                    EmploymentPeriodType employmentPeriodType = employmentWithPeriodType.employmentPeriodType;

                    if (employmentPeriodType == EmploymentPeriodType.ValidFuturePeriod)
                    {
                        futureEmployments.Add(i);
                    }
                    if (employmentPeriodType == EmploymentPeriodType.ValidPastPeriod)
                    {
                        pastEmployments.Add(i);
                    }
                }
                List<string> employments = new List<string>();

                var noOfFutureEmployments = futureEmployments.Count;
                var noOfPastEmployments = pastEmployments.Count;

                Logger.Log.DebugFormat("{0}: Number of future employments: {1}", personalResourceUri, noOfFutureEmployments);
                Logger.Log.DebugFormat("{0}: Number of past employments: {1}", personalResourceUri, noOfPastEmployments);

                if (noOfEmploymentsAndPeriodType > noOfFutureEmployments + noOfPastEmployments)
                {
                    for (int i = 0; i < noOfEmploymentsAndPeriodType; i++)
                    {
                        var employmentWithPeriodType = employmentsAndPeriodType[i];

                        if (!futureEmployments.Contains(i) && ! pastEmployments.Contains(i) )
                        {
                            string employmentUri = employmentWithPeriodType.employmentUri;
                            employments.Add(employmentUri);
                        }
                    }
                }
                else
                {
                    if (noOfFutureEmployments > 0)
                    {
                        foreach (int i in futureEmployments)
                        {
                            var employmentWithPeriodType = employmentsAndPeriodType[i];
                            string employmentUri = employmentWithPeriodType.employmentUri;
                            employments.Add(employmentUri);
                        }
                    }
                    if (noOfPastEmployments > 0)
                    {
                        foreach (int i in pastEmployments)
                        {
                            var employmentWithPeriodType = employmentsAndPeriodType[i];
                            string employmentUri = employmentWithPeriodType.employmentUri;
                            employments.Add(employmentUri);
                        }
                    }
                }

                var _hovedstillingsInfo = new HovedstillingInfo();
                var hovedstillingsUri = string.Empty;

                int noOfActiveEmployments = 0;

                List<Periode> employmentPeriods = new List<Periode>();

                List<string> arbeidsstedUris = new List<string>();
                List<string> businessUnitsUris = new List<string>();

                var _personalRessurs = new PersonalressursResource();

                if (personalressursDict.TryGetValue(personalResourceUri, out IEmbeddedResourceObject personalResourceData))
                {
                    _personalRessurs = PersonalressursRecourceFactory.Create(personalResourceData, employments);

                    foreach (var employmentUri in employments)
                    {
                        if (arbeidsforholdDict.TryGetValue(employmentUri, out IEmbeddedResourceObject arbeidsforhold))
                        {
                            noOfActiveEmployments++;
                            var _arbeidsforholdstype = new Arbeidsforholdstype();
                            var _stillingskode = new Stillingskode();
                            var _arbeidssted = new HRUnit();
                            var _arbeidsforhold = new ArbeidsforholdResource();

                            var arbeidsforholdLinks = arbeidsforhold.Links;

                            if (arbeidsforholdLinks.TryGetValue(ResourceLink.stillingskode, out IEnumerable<ILinkObject> stillingskodeLink))
                            {
                                var stillingskodeLinkUri = LinkToString(stillingskodeLink);
                                if (stillingsKodeDict.TryGetValue(stillingskodeLinkUri, out IEmbeddedResourceObject stillingsData))
                                {
                                    _stillingskode = StillingskodeFactory.Create(stillingsData.State);
                                }
                            }
                            if (arbeidsforholdLinks.TryGetValue(ResourceLink.arbeidsforholdstype, out IEnumerable<ILinkObject> arbeidsforholdstypeLink))
                            {
                                var arbeidsforholdtypeUrilink = LinkToString(arbeidsforholdstypeLink);
                                if (arbeidsforholdTypeDict.TryGetValue(arbeidsforholdtypeUrilink, out IEmbeddedResourceObject arbeidsforholdstypeData))
                                {
                                    _arbeidsforholdstype = ArbeidsforholdtypeFactory.Create(arbeidsforholdstypeData.State);
                                }
                            }
                            if (arbeidsforholdLinks.TryGetValue(ResourceLink.arbeidssted, out IEnumerable<ILinkObject> arbeidsstedLink))
                            {
                                var ArbeidsstedUri = LinkToString(arbeidsstedLink);
                                if (importedObjectsDict.TryGetValue(ArbeidsstedUri, out ImportListItem importListItemValue))
                                {
                                    _arbeidssted = importListItemValue.HrUnit;
                                
                                    if (!arbeidsstedUris.Contains(ArbeidsstedUri))
                                    {
                                        arbeidsstedUris.Add(ArbeidsstedUri);
                                    }
                                    string businessUnitUri = (!string.IsNullOrEmpty(_arbeidssted.ParentSchoolOrgUnit)) ? _arbeidssted.ParentSchoolOrgUnit + Delimiter.suffix + DefaultValue.businessUnitSuffix : hrStandardBusinessUnitUri;

                                    if (!string.IsNullOrEmpty(businessUnitUri) && !businessUnitsUris.Contains(businessUnitUri))
                                    {
                                    
                                        businessUnitsUris.Add(businessUnitUri);
                                    }
                                };
                                // add members to groups
                                if (generateOrgUnitGroups)
                                {
                                    var groupUri = ArbeidsstedUri + Delimiter.suffix + GroupType.ougroup;

                                    if (importedObjectsDict.TryGetValue(groupUri, out ImportListItem importListGroupItem))
                                    {
                                        var hrGroupObject = importListGroupItem.HrGroup;

                                        if (!hrGroupObject.GroupMembers.Contains(personalResourceUri))
                                        {
                                            var hrGroupOwner = hrGroupObject.GroupOwner;
                                            var isGroupOwner = personalResourceUri == hrGroupOwner;

                                            if (!isGroupOwner || putLeaderAsMemberInGroup)
                                            {
                                                hrGroupObject.GroupMembers.Add(personalResourceUri);
                                            }
                                        }
                                    }
                                }
                                if (generateAllPersonalGroups)
                                {
                                    if (importedObjectsDict.TryGetValue(groupAllPersonalUri, out ImportListItem importListGroupItem))
                                    {
                                        var hrGroupObject = importListGroupItem.HrGroup;

                                        if (!hrGroupObject.GroupMembers.Contains(personalResourceUri))
                                        {
                                            hrGroupObject.GroupMembers.Add(personalResourceUri);
                                        }
                                    }
                                }
                                var employmentTypes = new HashSet<string>();

                                if (personalAndEmploymentTypes.TryGetValue(personalResourceUri, out HashSet<string> employmentTypesFound))
                                {
                                    employmentTypes = employmentTypesFound;
                                }
                                if (generateGroupAllEmployees)
                                {
                                    if (employeeResourceCategories.Contains(resourcecategory) || employeeEmploymentTypes.Overlaps(employmentTypes))
                                    {
                                        if (importedObjectsDict.TryGetValue(groupAllEmployeesUri, out ImportListItem groupImportListItem))
                                        {
                                            var groupMembers = groupImportListItem.HrGroup.GroupMembers;

                                            if(!groupMembers.Contains(personalResourceUri))
                                            {
                                                groupMembers.Add(personalResourceUri);
                                            }
                                        }
                                    }
                                }
                                if (generateGroupAllPoliticians)
                                {
                                    if (politicianResourceCategories.Contains(resourcecategory) || politicianEmploymentTypes.Overlaps(employmentTypes))
                                    {
                                        if (importedObjectsDict.TryGetValue(groupAllPoliticiansUri, out ImportListItem groupImportListItem))
                                        {
                                            var groupMembers = groupImportListItem.HrGroup.GroupMembers;

                                            if (!groupMembers.Contains(personalResourceUri))
                                            {
                                                groupMembers.Add(personalResourceUri);
                                            }
                                        }
                                    }
                                }
                                if (generateGroupAllOthers)
                                {
                                    if (otherResourceCategories.Contains(resourcecategory) || otherEmploymentTypes.Overlaps(employmentTypes))
                                    {
                                        if (importedObjectsDict.TryGetValue(groupAllOthersUri, out ImportListItem groupImportListItem))
                                        {
                                            var groupMembers = groupImportListItem.HrGroup.GroupMembers;

                                            if (!groupMembers.Contains(personalResourceUri))
                                            {
                                                groupMembers.Add(personalResourceUri);
                                            }
                                        }
                                    }
                                }                            
                            }
                            _arbeidsforhold = ArbeidsforholdResourceFactory.Create(arbeidsforhold);

                            if (_arbeidsforhold.Hovedstilling)
                            {
                                hovedstillingsUri = employmentUri;
                            }

                            var _HREmployments = new HREmployment();
                            _HREmployments = HREmploymentFactory.Create(employmentUri, _arbeidsforhold, _personalRessurs, _stillingskode, _arbeidsforholdstype, _arbeidssted, hrStandardBusinessUnitUri);

                            // Add Employments to Dictionary for import to CS
                            if (!importedObjectsDict.Keys.Contains(employmentUri))
                            {
                                importedObjectsDict.Add(employmentUri, new ImportListItem() { HrEmployment = _HREmployments });
                            }
                            Periode _periode = _arbeidsforhold?.Gyldighetsperiode;

                            if (_periode != null)
                            {
                                employmentPeriods.Add(_periode);
                            }
                            // Generate aggrated groups 
                            if (generateGroupsAllTeachersNonTeacherEmployees||generateSchoolGroups || generateAggrGroupsForUnits)
                            {
                                var arbeidsstedUri = _HREmployments.ArbeidsstedOrganisasjonsIdUri;

                                if (importedObjectsDict.TryGetValue(arbeidsstedUri, out ImportListItem hrUnitImportListItem))
                                {
                                    var hrUnit = hrUnitImportListItem.HrUnit;
                                    var parentSchoolOrgUnit = hrUnit?.ParentSchoolOrgUnit;
                                    var parentBaseOrgUnits = hrUnit?.ParentBaseOrgUnits;

                                    var isSchoolOrgUnit = !string.IsNullOrEmpty(parentSchoolOrgUnit);
                                    var isAggrGroupOrgUnit = (parentBaseOrgUnits != null && parentBaseOrgUnits.Count > 0);

                                    var isEmployee = !otherResourceCategories.Contains(resourcecategory);

                                    var groupUriPrefix = string.Empty;
                                    var groupType = string.Empty;

                                    if (isEmployee)
                                    {
                                        groupUriPrefix = topOrgElementItemUri;

                                        if (isSchoolOrgUnit)
                                        {
                                            var thisEmploymentType = _HREmployments.ArbeidsforholdstypeSystemId;

                                            if (teacherEmploymentTypes.Contains(thisEmploymentType))
                                            {
                                                groupType = GroupType.aggrFac;
                                            }
                                            else
                                            {
                                                groupType = GroupType.aggrAdm;
                                            }
                                        }
                                        else
                                        {
                                            groupType = GroupType.aggrAdm;
                                        }
                                    }
                                    else
                                    {
                                        groupType = GroupType.aggrOth;
                                    }

                                    if (isSchoolOrgUnit)
                                    {
                                        groupUriPrefix = topOrgElementItemUri + DefaultValue.schoolTag;
                                    }
                                    else
                                    {
                                        groupUriPrefix = topOrgElementItemUri + DefaultValue.nonSchoolTag;
                                    }
                                    if (generateGroupsAllTeachersNonTeacherEmployees)
                                    {
                                        var groupUri = groupUriPrefix + Delimiter.suffix + groupType;
                                        AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict);

                                    }
                                    if (generateSchoolGroups && isSchoolOrgUnit)
                                    {                                    
                                        var groupUri = parentSchoolOrgUnit + Delimiter.suffix + groupType;

                                        if (groupType != GroupType.aggrOth)
                                        {
                                            AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict, putLeaderAsMemberInGroup);
                                        }
                                        else
                                        {
                                            AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict);
                                        }
                                        if (isEmployee)
                                        {
                                            var groupSchoolEmployeesUri = parentSchoolOrgUnit + Delimiter.suffix + GroupType.aggrEmp;
                                            AddMemberToHRGroup(groupSchoolEmployeesUri, personalResourceUri, importedObjectsDict, putLeaderAsMemberInGroup);        
                                        }
                                        if (generateAllPersonalGroups)
                                        {
                                            var allGroupUri = parentSchoolOrgUnit + Delimiter.suffix + GroupType.aggrAll;
                                            AddMemberToHRGroup(allGroupUri, personalResourceUri, importedObjectsDict, putLeaderAsMemberInGroup);
                                        }
                                    }
                                    if (generateAggrGroupsForUnits && isAggrGroupOrgUnit)
                                    {
                                        foreach (var parentUri in parentBaseOrgUnits)
                                        {

                                            if (generateAllPersonalGroups)
                                            {
                                                var groupUri = parentUri + Delimiter.suffix + GroupType.aggrAll;
                                                AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict, putLeaderAsMemberInGroup);
                                            }
                                            if (isEmployee)
                                            {
                                                var groupUri = parentUri + Delimiter.suffix + GroupType.aggrEmp;
                                                AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict, putLeaderAsMemberInGroup);
                                            }
                                            else
                                            {
                                                var groupUri = parentUri + Delimiter.suffix + GroupType.aggrOth;
                                                AddMemberToHRGroup(groupUri, personalResourceUri, importedObjectsDict);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    } //
                    
                    if (string.IsNullOrEmpty(hovedstillingsUri))
                    {                        
                        hovedstillingsUri = employments[0];
                        Logger.Log.Info($"No primary employment set in HR for employee {personalResourceUri}. Active employment {hovedstillingsUri} set as primary");
                    }

                    HREmployment hovedstillingCandicate = new HREmployment();

                    if (importedObjectsDict.TryGetValue(hovedstillingsUri, out ImportListItem importListItem))
                    {
                        hovedstillingCandicate = importListItem.HrEmployment;                        
                    }                    

                    if (hovedstillingCandicate != null && filterEmploymentTypesInActiveUsersList != null && filterEmploymentTypesInActiveUsersList.Contains(hovedstillingCandicate.ArbeidsforholdstypeKode))
                    {
                        Logger.Log.Info($"Primary employment {hovedstillingsUri} has type {hovedstillingCandicate.ArbeidsforholdstypeKode} indicating this employment gives an inactive user for employee {personalResourceUri}");
                        foreach (var employmentUri in employments)
                        {
                            if (importedObjectsDict.TryGetValue(employmentUri, out ImportListItem employmentItem))
                            {
                                hovedstillingCandicate = employmentItem.HrEmployment;
                                if (!filterEmploymentTypesInActiveUsersList.Contains(hovedstillingCandicate.ArbeidsforholdstypeKode))
                                {
                                    Logger.Log.Info($"Active employment {employmentUri} for employee {personalResourceUri} is set as primary instead of {hovedstillingsUri}");
                                    break;
                                }
                            }                            
                        }
                    }

                    _hovedstillingsInfo = HovedstillingInfoFactory.Create(hovedstillingCandicate);//, employmentUri, _arbeidsforhold, _stillingskode, _arbeidssted);

                    var managerUri = string.Empty;

                    var mainPositionOrgUnitUri = _hovedstillingsInfo.HovedstillingOrgUri;

                    if (!string.IsNullOrEmpty(mainPositionOrgUnitUri))
                    {
                        managerUri = GetManagerForEmployee(personalResourceUri, mainPositionOrgUnitUri, findAlwaysManagerForEmployee, importedObjectsDict);
                    }
                    #region Person
                    var _personData = LinkToString(personalResourceData.Links[ResourceLink.person]);

                    if (personalpersonDict.TryGetValue(_personData, out IEmbeddedResourceObject personData))
                    {
                        var _morsmal = new Sprak();
                        var _malform = new Sprak();
                        var _kjonn = new Kjonn();
                        var _statsborgerskap = new Landkode();

                        var personDataLinks = personData.Links;

                        if (personData.Links.TryGetValue(ResourceLink.morsmal, out IEnumerable<ILinkObject> morsmalLink))
                        {
                            var morsmalLinkUri = LinkToString(morsmalLink);
                            if (sprakDict.TryGetValue(morsmalLinkUri, out IEmbeddedResourceObject linkObjects))
                            {
                                _morsmal = SprakFactory.Create(linkObjects.State);
                            }
                        }
                        if (personData.Links.TryGetValue(ResourceLink.malform, out IEnumerable<ILinkObject> malformLink))
                        {
                            var malformLinkUri = LinkToString(malformLink);
                            if (sprakDict.TryGetValue(malformLinkUri, out IEmbeddedResourceObject linkObjects))
                            {
                                _malform = SprakFactory.Create(linkObjects.State);
                            }
                        }
                        if (personData.Links.TryGetValue(ResourceLink.kjonn, out IEnumerable<ILinkObject> kjonnLink))
                        {
                            var kjonnLinkUri = LinkToString(kjonnLink);
                            if (kjonnDict.TryGetValue(kjonnLinkUri, out IEmbeddedResourceObject linkObjects))
                            {
                                _kjonn = KjonnFactory.Create(linkObjects.State);
                            }
                        }
                        if (personData.Links.TryGetValue(ResourceLink.statsborgerskap, out IEnumerable<ILinkObject> statsborgerskapLink))
                        {
                            var statsborgerskapLinkUri = LinkToString(statsborgerskapLink);
                            if (landDict.TryGetValue(statsborgerskapLinkUri, out IEmbeddedResourceObject linkObject))
                            {
                                _statsborgerskap = LandFactory.Create(linkObject.State);
                            }
                        }
                        var _person = new Person();

                        _person = PersonFactory.Create(personData.State);

                        var HRperson = new HRPerson();

                        HRperson = HRPersonFactory.Create(
                            personalResourceUri, 
                            _person, _personalRessurs, 
                            usernameToLowerCase, 
                            noOfActiveEmployments, 
                            leaderOfUnitDict, 
                            _morsmal, 
                            _malform, 
                            _kjonn, 
                            _statsborgerskap, 
                            _hovedstillingsInfo, 
                            arbeidsstedUris,
                            businessUnitsUris,
                            managerUri,
                            hrOrganizationUri
                            );

                        // Add HRPerson to Dictionary for later import to CS
                        if (!importedObjectsDict.TryGetValue(personalResourceUri, out ImportListItem value))
                        {
                            if (generateGroupAllLeaders)
                            {
                                if (HRperson.PersonalLederOrganisasjonselementer.Count > 0)
                                {
                                    if (importedObjectsDict.TryGetValue(groupAllLeadersUri, out ImportListItem groupImportlistItem))
                                    {
                                        var groupMembers = groupImportlistItem.HrGroup.GroupMembers;
                                        if (!groupMembers.Contains(personalResourceUri))
                                        {
                                            groupMembers.Add(personalResourceUri);
                                        }
                                    }
                                }
                            }

                            (string starttime, string endtime) startAndEndDate = GetStartAndEndDate(employmentPeriods);

                            HRperson.PersonalAktivAnsettelsesperiodeStart = startAndEndDate.starttime;

                            if (!string.IsNullOrEmpty(startAndEndDate.endtime))
                            {
                                HRperson.PersonalAktivAnsettelsesperiodeSlutt = startAndEndDate.endtime;
                            }

                            importedObjectsDict.Add(personalResourceUri, new ImportListItem() { HrPerson = HRperson });

                            Logger.Log.Info($"{HRperson.PersonalAnsattnummerUri} Personalressurs to CS: {HRperson.PersonFodselsnummer};{HRperson.PersonalAnsattnummer};{HRperson.PersonNavnFornavn};{HRperson.PersonNavnEtternavn}" +
                                $";{HRperson.PersonalPersonalressurskategori};{HRperson.PersonHovedstillingArbeidsforholdtype};{HRperson.PersonHovedstillingStillingskode};{HRperson.PersonHovedstillingStillingskodeNavn};" +
                                $"{HRperson.PersonHovedstillingArbeidsstedOrganisasjonsKode};{HRperson.PersonHovedstillingArbeidsstedNavn};{HRperson.PersonHovedstillingLeder};{HRperson.PersonalKontaktinformasjonMobiltelefonnummer}$");

                            noOfPersonalResources++;
                        }
                    }
                }
                #endregion
            }
            Logger.Log.Info("Finished parsing personalressurs resources");

            Logger.Log.Info($"Number of active organisasjonselement resources are: {noOfOrgElements.ToString()}");
            Logger.Log.Info($"Number of active personalressurs resources are: {noOfPersonalResources.ToString()}");
            Logger.Log.Info($"Number of active arbeidsforhold resources are: { noOfEmployments.ToString()}");

            var jsonFolder = MAUtils.MAFolder;
            var filePath = jsonFolder + "\\CSImportInfo.json";

            var currentRunResultList = new List<RunResultInfo>();

            bool useThresholdValues = configParameters[Param.useThresholdValues].Value == "1";
            bool overrideThresholds = configParameters[Param.overrideThresholds].Value == "1";

            if (useThresholdValues)
            {
                Logger.Log.InfoFormat($"Parameter '{Param.useThresholdValues}' is set to true. Checking that threshold values are met");

                bool continueImportToCS = true;
                bool thresholdIsExceeded = false;
                try
                {
                    string thresholdValuePerson = configParameters[Param.thresholdValuePerson].Value;
                    string thresholdValueArbeidsforhold = configParameters[Param.thresholdValueArbeidsforhold].Value;
                    string thresholdValueOrganisasjonselementGruppe = configParameters[Param.thresholdValueOrganisasjonselementGruppe].Value;

                    if (File.Exists(filePath))
                    {
                        var runResultList = GetRunResultInfo(filePath);

                        foreach (var runResult in runResultList)
                        {
                            var objectType = runResult.ObjectType;
                            var noOfObjectsLastRun = runResult.NoOfObjects;
                            int currentNoOfObjects = 0;

                            var threshold = string.Empty;

                            switch (objectType)
                            {
                                case CSobjecttypes.Person:
                                    {
                                        currentNoOfObjects = noOfPersonalResources;
                                        threshold = thresholdValuePerson;
                                        break;
                                    }
                                case CSobjecttypes.Arbeidsforhold:
                                    {
                                        currentNoOfObjects = noOfEmployments;
                                        threshold = thresholdValueArbeidsforhold;
                                        break;
                                    }
                                case CSobjecttypes.Organisasjonselement:
                                    {
                                        currentNoOfObjects = noOfOrgElements;
                                        threshold = thresholdValueOrganisasjonselementGruppe;
                                        break;
                                    }
                            }
                            bool thisRunResultOk = CheckDifferenceObjectType(objectType, currentNoOfObjects, noOfObjectsLastRun, threshold );

                            var currentRunResult = new RunResultInfo { ObjectType = objectType, NoOfObjects = currentNoOfObjects };

                            if (thisRunResultOk)
                            {
                                currentRunResultList.Add(currentRunResult);
                            }
                            else
                            {
                                if (overrideThresholds)
                                {
                                    currentRunResultList.Add(currentRunResult);
                                    Logger.Log.Info($"Import continues because parameter '{Param.overrideThresholds}' is set to true");
                                    thresholdIsExceeded = true;
                                }
                                else
                                {
                                    continueImportToCS = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Log.Info($"Previous run result file {filePath} was not found, eg. this is the initial run so the import continues");

                        var personRunResult = new RunResultInfo { ObjectType = CSobjecttypes.Person, NoOfObjects = noOfPersonalResources };
                        currentRunResultList.Add(personRunResult);
                        var employmentRunResult = new RunResultInfo { ObjectType = CSobjecttypes.Arbeidsforhold, NoOfObjects = noOfEmployments};
                        currentRunResultList.Add(employmentRunResult);
                        var unitRunResult = new RunResultInfo { ObjectType = CSobjecttypes.Organisasjonselement, NoOfObjects = noOfOrgElements };
                        currentRunResultList.Add(unitRunResult);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (continueImportToCS)
                {
                    if (thresholdIsExceeded)
                    {
                        Logger.Log.Info($"Import to CS continues: One of more threshold values are exceeded, but the parameter '{Param.overrideThresholds}' is set to true");
                    }
                    else
                    {
                        Logger.Log.Info($"Import to CS continues: Run result is below the threshold value for all object types");
                    }
                }
                else
                {
                    var message = "The import to CS is aborted because one of more threshold values are exceeded";
                    Logger.Log.Error(message);

                    throw new Exception(message);
                }
            }

            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, currentRunResultList);
            }

            foreach (var key in importedObjectsDict.Keys)
            {
                var objectToImport = importedObjectsDict[key];
                ImportedObjectsList.Add(objectToImport);
            }

            // Set index values and page size
            GetImportEntriesIndex = 0;
            GetImportEntriesPageSize = importRunStep.PageSize;

            Logger.Log.InfoFormat("Parsing and staging of data for CS took {0} min and {1} sec", Stopwatch.Elapsed.Minutes, Stopwatch.Elapsed.Seconds);

            return new OpenImportConnectionResults();
        }

        private Dictionary<string, int> GetAggrGroupForUnitsParameters(KeyedCollection<string, ConfigParameter> configParameters, string enheterAggrGrupper)
        {
            var aggrGroupUnitsParameters = new Dictionary<string, int>();

            HashSet<string> items = GetConfigParameterList(configParameters, Param.enheterAggrGrupper);

            foreach (var item in items)
            {
                string unitId;
                int depth;

                if (item.Contains(":"))
                {
                    var unitIdAndDepth = item.Split(':');
                    unitId = unitIdAndDepth[0];
                    depth = Int32.Parse(unitIdAndDepth[1]);
                }
                else
                {
                    unitId = item;
                    depth = 0;
                }
                aggrGroupUnitsParameters.Add(unitId, depth);
            }
            return aggrGroupUnitsParameters;
        }

        private List<string> GetSubOrgUnitUris (string orgUnitUri, int searchDepth, Dictionary<string, ImportListItem> importedObjectsDict)
        {
            List<string> orgUnitUris = new List<string>();

            if (!orgUnitUris.Contains(orgUnitUri))
            {
                orgUnitUris.Add(orgUnitUri);
                Logger.Log.Debug($"GetSubOrgUnitUris: Added {orgUnitUri} to orgunit list before recursive call");
            }

            if (importedObjectsDict.TryGetValue(orgUnitUri, out ImportListItem importListItem))
            {
                int subUnitCount = 0;
                var thisOrgUnit = importListItem.HrUnit;
                var subUnits = thisOrgUnit?.OrganisasjonselementUnderordnet;
                if (subUnits != null)
                {
                    subUnitCount = subUnits.Count;
                }
            
                if (subUnitCount > 0 )
                {
                    Logger.Log.Debug($"GetSubOrgUnitUris: {orgUnitUri} has {subUnitCount.ToString()} subunits");
                    if (searchDepth > 1)
                    {
                        foreach (var uri in subUnits)
                        {
                            Logger.Log.Debug($"GetSubOrgUnitUris: Recursive call with {uri} and search depth {(searchDepth - 1).ToString()}");
                            List<string> tempSubUnitUris = GetSubOrgUnitUris(uri, searchDepth - 1, importedObjectsDict);

                            foreach (var tempUri in tempSubUnitUris)
                            {
                                if (!orgUnitUris.Contains(tempUri))
                                {
                                    orgUnitUris.Add(tempUri);
                                    Logger.Log.Debug($"GetSubOrgUnitUris: Added {tempUri} to orgunit list after recursive call");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var uri in subUnits)
                        {
                            if (!orgUnitUris.Contains(uri))
                            {
                                orgUnitUris.Add(uri);
                            }
                        }
                    }
                }
                else
                {
                    Logger.Log.Debug($"GetSubOrgUnitUris: {orgUnitUri} has no subunits");
                }
            }
            return orgUnitUris;
        }
        private void SetParentBaseUrisOnSubUnits(string baseOrgUnitUri, string subOrgUnitUri, Dictionary<string, ImportListItem> importedObjectsDict, bool baseParentIsSchoolUnit=false)
        {
            if (importedObjectsDict.TryGetValue(subOrgUnitUri, out ImportListItem importListItem))
            {
                var thisOrgUnit = importListItem.HrUnit;

                if (baseParentIsSchoolUnit)
                {
                    thisOrgUnit.ParentSchoolOrgUnit = baseOrgUnitUri;
                }
                else
                {
                    var parentBaseOrgUnits = thisOrgUnit.ParentBaseOrgUnits;
                    if (parentBaseOrgUnits == null || parentBaseOrgUnits.Count == 0)
                    {
                        thisOrgUnit.ParentBaseOrgUnits = new List<string> { baseOrgUnitUri };
                    }
                    else
                    {
                        thisOrgUnit.ParentBaseOrgUnits.Add(baseOrgUnitUri);
                    }
                }

                var subUnitUris= thisOrgUnit.OrganisasjonselementUnderordnet;

                foreach (var subUnitUri in subUnitUris)
                {
                    SetParentBaseUrisOnSubUnits(baseOrgUnitUri, subUnitUri, importedObjectsDict, baseParentIsSchoolUnit);
                }
            }
        }

        private ( string, string) GetStartAndEndDate(List<Periode> employmentPeriods)
        {
            DateTime firstDate = DateTime.Parse(infinityDate);
            DateTime? lastDate = DateTime.Parse(zeroDate);

            foreach (var employmentPeriod in employmentPeriods)
            {
                var startDate = employmentPeriod.Start;
                var endDate = employmentPeriod?.Slutt;

                if (startDate < firstDate )
                {
                    firstDate = startDate;
                }
                if (endDate == null)
                {
                    lastDate = DateTime.Parse(infinityDate);
                }
                else
                {
                    if (endDate > lastDate)
                    {
                        lastDate = endDate;
                    }
                }
            }
            var firstDateAsString = firstDate.ToString(dateFormat);
            var lastDateAsString = string.Empty;

            if (lastDate < DateTime.Parse(infinityDate))
            {
                lastDateAsString = lastDate?.ToString(dateFormat);
            }
            return (firstDateAsString, lastDateAsString);
        }

        private List<RunResultInfo> GetRunResultInfo(string filePath)
        {

            var runResultInfo = new List<RunResultInfo>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();

                JArray objectArray = JArray.Parse(resourceJson);

                IList<RunResultInfo> runResults = objectArray.Select(p => new RunResultInfo
                {
                    ObjectType = (string)p["ObjectType"],
                    NoOfObjects = (int)p["NoOfObjects"]

                }).ToList();

                runResultInfo = (List<RunResultInfo>)runResults;
            }
            return runResultInfo;
        }

        private bool CheckDifferenceObjectType(string objectType, int currentRunObjects, int lastRunObjects, string threshold)
        {
            bool runResult = false;
            double diffNum = 0.0;
            double diffPro = 0.0;
            ThresholdStatus tStatus = ThresholdStatus.Unkown;
            ThresholdType tType = ThresholdType.Unknown;
            double dThreshold = 0.0;

            string tempLog = null;
            CheckDifference(lastRunObjects, currentRunObjects, threshold, out diffNum, out diffPro, out tStatus, out dThreshold, out tType);
            if (tStatus == ThresholdStatus.Exceeded)
            {
                tempLog = "Limit exceeded - Object type: " + objectType + " - Number of object(s) in current/previous run: " + currentRunObjects.ToString() + "/" + lastRunObjects.ToString() + 
                    " - difference: " + diffNum.ToString() + "  object(s) (" + diffPro.ToString(".00") + "%). Threshold limit is set to " + threshold;
                Logger.Log.ErrorFormat(tempLog);
                runResult = false;
            }
            else if (tStatus == ThresholdStatus.Unkown)
            {
                tempLog = "Accepted (unknown limit) - Object type: " + objectType + " - Number of object(s) in current/previous run: " + currentRunObjects.ToString() + "/" + lastRunObjects.ToString() + 
                    " - difference: " + diffNum.ToString() + " object(s) (" + diffPro.ToString(".00") + "%). Limit value not set or can't be used (only number or percent values are allowed). Changes for this object typeare allowed";
                Logger.Log.InfoFormat(tempLog);
                runResult = true;
            }
            else
            {
                tempLog = "Accepted - Object type: " + objectType + " -  Number of object(s) in current/previous run: " + currentRunObjects.ToString() + "/" + lastRunObjects.ToString() + 
                    " - difference: " + diffNum.ToString() + " object(s) (" + diffPro.ToString(".00") + "%). Threshold limit is set to  " + threshold;
                Logger.Log.InfoFormat(tempLog);
                runResult = true;
            }
            return runResult;
        }

        private enum ThresholdStatus
        {
            Unkown,
            Within,
            Exceeded
        }

        private enum ThresholdType
        {
            Unknown,
            Numeric,
            Procent
        }

        private void CheckDifference(int numberInLastRun, int numberInCurrentRun, string threshold, out double differenceNumeric, out double differenceProcent, out ThresholdStatus thresholdStatus, out double thresholdValue, out ThresholdType thresholdType)
        {
            thresholdValue = 0.0;
            double highValue;
            double lowValue;

            if (numberInLastRun > numberInCurrentRun)
            {
                highValue = numberInLastRun;
                lowValue = numberInCurrentRun;
            }
            else
            {
                highValue = numberInCurrentRun;
                lowValue = numberInLastRun;
            }

            differenceNumeric = highValue - lowValue;
            differenceProcent = (differenceNumeric / lowValue) * 100;

            if (threshold == "")
                threshold = null;

            if (threshold == null || IsNumeric(threshold.Replace("%", "").Trim()) == false)
            {
                thresholdStatus = ThresholdStatus.Unkown;
                thresholdType = ThresholdType.Unknown;
            }
            else
            {
                if (threshold.Contains("%"))
                    thresholdType = ThresholdType.Procent;
                else
                    thresholdType = ThresholdType.Numeric;

                thresholdValue = double.Parse(threshold.Replace("%", "").Trim());

                if (thresholdType == ThresholdType.Numeric)
                {
                    if (differenceNumeric > thresholdValue)
                        thresholdStatus = ThresholdStatus.Exceeded;
                    else
                        thresholdStatus = ThresholdStatus.Within;
                }
                else
                {
                    if (differenceProcent > thresholdValue)
                        thresholdStatus = ThresholdStatus.Exceeded;
                    else
                        thresholdStatus = ThresholdStatus.Within;
                }
            }
        }
        private void AddMemberToHRGroup (string groupUri, string personalResourceUri, Dictionary<string, ImportListItem> importedObjectsDict, bool putLeaderAsMemberInGroup=false)
        {
            Logger.Log.Debug($"Trying to add {personalResourceUri} to group {groupUri}");

            if (importedObjectsDict.TryGetValue(groupUri, out ImportListItem groupItem))
            {
                var group = groupItem.HrGroup;
                var groupMembers = group?.GroupMembers;

                if (!groupMembers.Contains(personalResourceUri))
                {
                    var hrGroupOwner = group?.GroupOwner;
                    var isGroupOwner = personalResourceUri == hrGroupOwner;

                    if (!isGroupOwner || putLeaderAsMemberInGroup)
                    {
                        groupMembers.Add(personalResourceUri);
                    }
                }
            }
        }

    private string GetManagerForEmployee(string personalResourceUri, string mainPositionOrgUnitUri, bool findAlwaysManagerForEmployee, Dictionary<string, ImportListItem> importedObjectsDict)
        {
            var managerUri = string.Empty;

            if (importedObjectsDict.TryGetValue(mainPositionOrgUnitUri, out ImportListItem importListItem))
            {
                var thisOrgUnit = importListItem.HrUnit;
                
                var parentOrgElement = thisOrgUnit.OrganisasjonselementOverordnet;

                bool managerFound = false;
                bool isMainPositionOrgUnit = true;
                bool isOwnManager = false;

                while ((findAlwaysManagerForEmployee || isMainPositionOrgUnit || isOwnManager) && !string.IsNullOrEmpty(parentOrgElement) && !managerFound && parentOrgElement != thisOrgUnit.UnitUri)
                {
                    var thisOrgUnitManagerUri = thisOrgUnit?.OrganisasjonselementLeder;

                    if (!string.IsNullOrEmpty(thisOrgUnitManagerUri) && personalResourceUri != thisOrgUnitManagerUri)
                    {
                        managerUri = thisOrgUnitManagerUri;
                        managerFound = true;                        
                    }
                    else
                    {
                        if (importedObjectsDict.TryGetValue(parentOrgElement, out ImportListItem parentImportItem))
                        {
                            thisOrgUnit = parentImportItem.HrUnit;
                            parentOrgElement = thisOrgUnit.OrganisasjonselementOverordnet;
                            isMainPositionOrgUnit = false;

                            if (personalResourceUri == thisOrgUnitManagerUri)
                            {
                                isOwnManager = true;
                            }
                        }
                    }
                }
            }
            return managerUri;
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStepInfo)
        {
            Logger.Log.Info("Import to CS finished");
            return new CloseImportConnectionResults();
        }

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            /* This method will be invoked multiple times, once for each "page" */

            List<CSEntryChange> csentries = new List<CSEntryChange>();
            GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();

            // If no result, return empty success
            if (ImportedObjectsList == null || ImportedObjectsList.Count == 0)
            {
                importReturnInfo.CSEntries = csentries;
                return importReturnInfo;
            }

            bool excludeEmptyGroups = _globalConfigParameters[Param.excludeEmptyGroups].Value == "1";

            // Parse a full page or to the end
            for (int currentPage = 0;
                GetImportEntriesIndex < ImportedObjectsList.Count && currentPage < GetImportEntriesPageSize;
                GetImportEntriesIndex++, currentPage++)
            {
                if (ImportedObjectsList[GetImportEntriesIndex].HrPerson != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrPerson.GetCSEntryChange());
                }

                if (ImportedObjectsList[GetImportEntriesIndex].HrEmployment != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrEmployment.GetCSEntryChange());
                }

                if (ImportedObjectsList[GetImportEntriesIndex].HrGroup != null)
                {
                    if (excludeEmptyGroups)
                    {
                        HRGroup hrGroupToImport = ImportedObjectsList[GetImportEntriesIndex].HrGroup;
                        var groupMembers = hrGroupToImport.GroupMembers;

                        if (groupMembers != null && groupMembers.Count > 0)
                        {
                            csentries.Add(hrGroupToImport.GetCSEntryChange());
                        }
                        else
                        {
                            Logger.Log.Info($"{hrGroupToImport.GroupIDUri} has no members and is not imported to CS");
                        }
                    }
                    else
                    {
                        csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrGroup.GetCSEntryChange());
                    }
                }

                if (ImportedObjectsList[GetImportEntriesIndex].HrUnit != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrUnit.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].HrBusinessUnit != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrBusinessUnit.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].HrOrganization != null)
                {
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].HrOrganization.GetCSEntryChange());
                }
            }

            // Store and return
            importReturnInfo.CSEntries = csentries;
            importReturnInfo.MoreToImport = GetImportEntriesIndex < ImportedObjectsList.Count;
            return importReturnInfo;
        }

        #endregion

        #region Export

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            Logger.Log.Info("Starting export");
            _globalConfigParameters = configParameters;

            var personalressursUri = DefaultValue.administrasjonPersonalPersonalRessursUri;
            var felleskomponentUri = _globalConfigParameters[Param.felleskomponentUri].Value;

            var uriPaths = new List<string> {
                personalressursUri
            };

            var resources = GetDataFromFINTApi(configParameters, uriPaths);

            foreach (var uriKey in resources.Keys)
            {
                //Logger.Log.DebugFormat("Adding resource {0} to dictionary", uriKey);
                var resourceType = GetUriPathForClass(uriKey);


                if (resourceType == personalressursUri)
                {
                    var resource = resources[uriKey];
                    var ansattnummerUri = GetIdentifikatorUri(resource, felleskomponentUri, FintAttribute.ansattnummer);

                    if (!_resourceDict.TryGetValue(ansattnummerUri, out IEmbeddedResourceObject existingResource))
                    {
                        _resourceDict.Add(ansattnummerUri, resource);
                    }
                    else
                    {
                        Logger.Log.ErrorFormat($"Duplicate resource {ansattnummerUri} in personalressurs items. Something is wrong in source system");
                    }
                }
            }
            return;
        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            Logger.Log.Debug("Opening PutExportEntries");

            Dictionary<string, Dictionary<string, string>> personsAndAttributesToModify = new Dictionary<string, Dictionary<string, string>>();

            foreach (CSEntryChange csentry in csentries)
            {
                Logger.Log.DebugFormat("Exporting csentry {0} with modificationType {1}", csentry.DN, csentry.ObjectModificationType);

                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Add:
                        Logger.Log.Debug("UpdateAdd hit");
                        break;
                    case ObjectModificationType.Delete:
                        Logger.Log.Debug("UpdateDelete hit");
                        break;
                    case ObjectModificationType.Replace:
                        Logger.Log.Debug("UpdateReplace hit");
                        GetExportDataToModify(csentry, ref personsAndAttributesToModify);
                        break;
                    case ObjectModificationType.Update:
                        Logger.Log.Debug("UpdateUpdate hit");
                        GetExportDataToModify(csentry, ref personsAndAttributesToModify);
                        break;
                    case ObjectModificationType.Unconfigured:
                        Logger.Log.Debug("UpdateUnconfigured hit");
                        break;
                    case ObjectModificationType.None:
                        Logger.Log.Debug("UpdateNone hit");
                        break;
                }
            }
            Logger.Log.Debug("Passed Foreach loop PutExportEntries");
            if (personsAndAttributesToModify != null)
            {
                UpdateFintData(personsAndAttributesToModify);
                PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
                return exportEntriesResults;

            }
            return null;
        }
        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            Logger.Log.Info("Ending export");
        }
        #endregion

        #region private_functions

        private List<LastUpdateTimestamp> GetLastUpdatedTimestamps(HttpClient httpClient, string baseUri, List<string> resources)
        {
            var lastUpdatedTimestamps = new List<LastUpdateTimestamp>();

            foreach (var resource in resources)
            {
                var resourceUri = baseUri + resource;
                var lastUpdatedUrl = resourceUri + "/last-updated";
                try
                {
                    var response = httpClient.GetStringAsync(lastUpdatedUrl).Result;
                    var lastUpdatedTimestamp = JsonConvert.DeserializeObject<LastUpdateTimestamp>(response);

                    lastUpdatedTimestamp.Resource = resourceUri;

                    //.DateTime.ToLocalTime()
                    var lastUpdatedAsString = DateTimeOffset.FromUnixTimeMilliseconds(lastUpdatedTimestamp.LastUpdated).ToString();
                    lastUpdatedTimestamp.LastUpdatedAsString = lastUpdatedAsString;

                    lastUpdatedTimestamps.Add(lastUpdatedTimestamp);
                }
                catch (AggregateException ex)
                {
                    Logger.Log.ErrorFormat("Getting last-updated timestamp from {0} failed with response: {1}", resourceUri.ToString(), ex.Message);
                }
            }

            return lastUpdatedTimestamps;
        }


        private Dictionary<string, IEmbeddedResourceObject> GetDataFromFINTApi(KeyedCollection<string, ConfigParameter> configParameters, List<string> uriPaths)
        {
            var accessTokenUri = configParameters[Param.idpUri].Value;
            var clientId = configParameters[Param.clientId].Value;
            var clientSecret = Decrypt(configParameters[Param.openIdSecret].SecureValue);
            var username = configParameters[Param.username].Value;
            var password = Decrypt(configParameters[Param.password].SecureValue);
            var scope = configParameters[Param.scope].Value;
            var xOrgId = configParameters[Param.assetId].Value;
            //var xClient = configParameters[Param.xClient].Value;
            var felleskomponentUri = configParameters[Param.felleskomponentUri].Value;
            var httpClientTimeout = Double.Parse(configParameters[Param.httpClientTimeout].Value);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var bearerToken = GetBearenToken(accessTokenUri, clientId, clientSecret, username, password, scope);

            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(bearerToken);

                client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Org_Id, xOrgId);
                //client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Client, xClient);

                client.HttpClient.Timeout = TimeSpan.FromMinutes(httpClientTimeout);

                Logger.Log.InfoFormat("Getting lastupdated timestamps for all resources");
                var lastUpdatedTimeStamps = GetLastUpdatedTimestamps(client.HttpClient, felleskomponentUri, uriPaths);

                var jsonFolder = MAUtils.MAFolder;
                var filePath = jsonFolder + "\\lastUpdated.json";

                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, lastUpdatedTimeStamps);
                }



                Logger.Log.Info("Get all resources started");

                bool useLocalCache = configParameters[Param.useLocalCache].Value == "1";

                if (useLocalCache)
                {
                    Logger.Log.Info($"Parameter {Param.useLocalCache} is set to true. All resources are fetched from local cache");
                }
                var responseList = GetDataAsync(felleskomponentUri, uriPaths, client, useLocalCache).Result;

                Logger.Log.Info("Get all resources ended");

                var resourcesDict = new Dictionary<string, IEmbeddedResourceObject>();
                foreach (var response in responseList)
                {
                    //var embedded = response.Resource.Embedded;
                    var entries = response.EmbeddedResources;
                    //var entries = new List<IEmbeddedResourceObject>();

                    //if (embedded.ContainsKey(HalObject._entries))
                    //{
                    //  entries = embedded[HalObject._entries].ToList();
                    //var state = response.Resource.State;

                    foreach (var entry in entries)
                    {
                        var links = entry.Links;
                        if (links.TryGetValue(ResourceLink.self, out IEnumerable<ILinkObject> selfLink))
                        {
                            var resourceUri = LinkToString(selfLink);
                            if (!(resourcesDict.ContainsKey(resourceUri)))
                            {
                                resourcesDict.Add(resourceUri, entry);
                            }
                        }
                        else
                        {
                            var entryAsJson = JsonConvert.SerializeObject(entry);
                            Logger.Log.ErrorFormat("Resource with missing self link: {0}", entryAsJson);
                        }
                    }
                    //}
                }
                return resourcesDict;
            }
        }


        static private async Task<HalJsonParseResult[]> GetDataAsync(string felleskomponentUri, List<string> uriPaths, IHalHttpClient client, bool useLocalCache)
        {

            Logger.Log.Info("GetDataAsync started");
            //IEnumerable<Task<IHalHttpResponseMessage>> downloadTaskQuery =
            //        from uriPath in uriPaths select ProcessURLAsync(uriPath, felleskomponentUri, client);

            //Task<IHalHttpResponseMessage>[] downloadTasks = downloadTaskQuery.ToArray();

            //IHalHttpResponseMessage[] employeeData = await Task.WhenAll(downloadTasks);

            IEnumerable<Task<HalJsonParseResult>> downloadTaskQuery =
                    from uriPath in uriPaths select ProcessURLAsync(uriPath, felleskomponentUri, client, useLocalCache);

            Task<HalJsonParseResult>[] downloadTasks = downloadTaskQuery.ToArray();

            HalJsonParseResult[] employeeData = await Task.WhenAll(downloadTasks);

            Logger.Log.Info("GetDataAsync ended");

            return employeeData;
        }

        static private async Task<IHalHttpResponseMessage> ProcessURL(Uri url, IHalHttpClient client)
        {
            var response = await client.GetAsync(url);
            return response;
        }

        static private async Task<HalJsonParseResult> ProcessURLAsync(string uriPath, string felleskomponentUri, IHalHttpClient client, bool useLocalCache)
        {
            HalJsonParseResult result = null;

            string jsonFolder = MAUtils.MAFolder;
            string fileName = uriPath.Substring(1).Replace('/', '_');
            string filePath = jsonFolder + "\\" + fileName + ".json";

            if (useLocalCache)
            {
                if (File.Exists(filePath))
                {
                    Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                    result = GetDataFromFile(filePath);
                }
                else
                {
                    Logger.Log.InfoFormat("File {0} does not exist, no previous responses has been saved to disk", filePath);
                }
            }
            else
            {
                Uri uri = new Uri(felleskomponentUri + uriPath);

                Logger.Log.InfoFormat("Getting data from {0}", uri.ToString());
                try
                {
                    var response = await client.GetAsync(uri);

                    var httpResponse = await client.HttpClient.GetStringAsync(uri);

                    var halJsonParser = new HalJsonParser();
                    result = halJsonParser.Parse(httpResponse);

                    var state = response.Resource.State;
                    int totalItems = Int32.Parse(state[StateLink.total_items].Value);

                    var stateValues = result.StateValues.First().Value;

                    Logger.Log.InfoFormat("Data from {0} returned with {1} items", uri.ToString(), totalItems.ToString());

                    if (totalItems > 0)
                    {
                        Logger.Log.InfoFormat("Writing response to file {0}", filePath);
                        var httpResponseAsJson = JObject.Parse(httpResponse);

                        WriteDataToFile(filePath, httpResponseAsJson);
                    }
                    else
                    {
                        if (File.Exists(filePath))
                        {
                            Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                            result = GetDataFromFile(filePath);
                        }
                        else
                        {
                            Logger.Log.InfoFormat("File {0} does not exist, no previous responses has been saved to disk", filePath);
                        }
                    }
                }
                catch (AggregateException ex)
                {
                    Logger.Log.ErrorFormat("Getting data from {0} failed with response: {1}", uri.ToString(), ex.Message);
                    throw ex;
                }

                catch (HalHttpRequestException ex)
                {
                    result = HandleRequestError(uri, ex, filePath);
                    if (result == null)
                    {
                        throw ex;
                    }
                }
                catch (HttpRequestException ex)
                {
                    result = HandleRequestError(uri, ex, filePath);
                    if (result == null)
                    {
                        Logger.Log.ErrorFormat("Reading file {0} returned no objects", filePath);
                        throw ex;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    result = HandleRequestError(uri, ex, filePath);
                    if (result == null)
                    {
                        Logger.Log.ErrorFormat("Reading file {0} returned no objects", filePath);
                        throw ex;
                    }
                }
                catch (WebException ex)
                {
                    result = HandleRequestError(uri, ex, filePath);
                    if (result == null)
                    {
                        Logger.Log.ErrorFormat("Reading file {0} returned no objects", filePath);
                        throw ex;
                    }
                }
            }            
            return result;
        }

        private static HalJsonParseResult HandleRequestError(Uri uri, Exception ex, string filePath)
        {
            HalJsonParseResult result;
            var message = ex?.Message;
            var exceptionType = ex?.GetType().ToString();
            var innerexception = ex?.InnerException?.Message;
            Logger.Log.ErrorFormat("Getting resource uri: {0} failed with error type {1}. HTTP Message: {2}, Inner exception: {3}"
                            , uri.ToString(), exceptionType, message, innerexception);

            if (File.Exists(filePath))
            {
                Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                result = GetDataFromFile(filePath);
                Logger.Log.DebugFormat("Finished getting last saved response from file {0}", filePath);
            }
            else
            {
                Logger.Log.InfoFormat("File {0} does not exist, no previous responses has been saved to disk", filePath);
                result = null;
            }
            return result;

        }

        private static HalJsonParseResult GetDataFromFile(string filePath)
        {
            HalJsonParseResult result;
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                result = halJsonParser.Parse(resourceJson);
            }
            return result;
        }

        private static void WriteDataToFile(string filePath, object dataObject)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, dataObject);
            }
        }

        private void GetDataFromFile(string filePath, ref Dictionary<string, IEmbeddedResourceObject> resourcesDict)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                var result = halJsonParser.Parse(resourceJson);
                var entries = result.EmbeddedResources;

                foreach (var entry in entries)
                {
                    var resourceUri = LinkToString(entry.Links[ResourceLink.self]);

                    if (!(resourcesDict.ContainsKey(resourceUri)))
                    {
                        resourcesDict.Add(resourceUri, entry);
                    }
                }
            }
        }
        private void GetDataFromFile(string filePath, string felleskomponentUri, ref Dictionary<string, IEmbeddedResourceObject> resourcesDict)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                var result = halJsonParser.Parse(resourceJson);
                var entries = result.EmbeddedResources;

                foreach (var entry in entries)
                {
                    var resourceUri = GetSystemIdUri(entry, felleskomponentUri);

                    if (!(resourcesDict.ContainsKey(resourceUri)))
                    {
                        resourcesDict.Add(resourceUri, entry);
                    }
                }
            }
        }



        private string NormalizeUri(string uri)
        {
            string pattern = @"(?<path>.*/)(.+)";
            string replacement = "${path}";
            var path = Regex.Replace(uri, pattern, replacement).ToLower();
            string pattern2 = @"(.*/)(?<id>.+)";
            string replacement2 = "${id}";
            var idValue = Regex.Replace(uri, pattern2, replacement2);
            var normalizedUri = path + idValue;
            return normalizedUri;
        }

        private static string Decrypt(SecureString inStr)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(inStr);
            string decrString = Marshal.PtrToStringUni(ptr);
            return decrString;
        }

        private string GetFintType(string fintUri)
        {
            var segments = fintUri.Split('/');
            var noOfSegments = segments.Length;
            var fintType = segments[noOfSegments - 3];

            return fintType;
        }

        private string GetUriPathForClass(string uriString)
        {
            var uri = new Uri(uriString);
            var uriPath = uri.AbsolutePath;
            string pattern = @"(?<path>.*)(/.+/.+)";
            string replacement = "${path}";
            var result = Regex.Replace(uriPath, pattern, replacement);
            return result;
        }

        private TokenResponse TokenResponseHelper(KeyedCollection<string, ConfigParameter> configParameters)
        {
            var accessTokenUri = configParameters[Param.idpUri].Value;
            var clientId = configParameters[Param.clientId].Value;
            var clientSecret = Decrypt(configParameters[Param.openIdSecret].SecureValue);
            var username = configParameters[Param.username].Value;
            var password = Decrypt(configParameters[Param.password].SecureValue);
            var scope = configParameters[Param.scope].Value;

            var TokenClient = new HttpClient();

            var TokenResponse = TokenClient.RequestTokenAsync(new TokenRequest
            {
                Address = accessTokenUri,

                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = OidcConstants.GrantTypes.Password,

                Parameters =
                {
                    { OidcConstants.TokenRequest.UserName, username },
                    { OidcConstants.TokenRequest.Password, password },
                    { OidcConstants.TokenRequest.Scope, scope }
                }
            }).Result;

            return TokenResponse;
        }

        private HashSet<string> GetParameterList(string configParameterValue)
        {
            var paramList = new HashSet<string>();

            if (configParameterValue.Contains(","))
            {
                foreach (string param in configParameterValue.Split(','))
                {
                    paramList.Add(param);
                }
            }
            else if (!(string.IsNullOrEmpty(configParameterValue)))
            {
                paramList.Add(configParameterValue);
            }
            return paramList;
        }
        private (string, HRGroup) GenerateAggrGroup (string orgElementItemUri, string groupType, HRUnit hrUnit, KeyedCollection<string, ConfigParameter> configParameters, bool IsSchoolGroup = false, bool isSchoolNonSchoolGroup = false)
        {
            var aggrGroupUri = orgElementItemUri + Delimiter.suffix + groupType;

            var allPersonalSuffix = configParameters[Param.gruppeAlleSuffix].Value;
            var allEmployeesSuffix = configParameters[Param.gruppeAlleAnsatteSuffix].Value;
            var allPolitiansSuffix = configParameters[Param.gruppeAllePolitikereSuffix].Value;
            var allLeadersSuffix = configParameters[Param.gruppeAlleLedereSuffix].Value;
            var allTeachersSuffix = configParameters[Param.gruppeAlleLarereSuffix].Value;
            var allAdminsSchoolsSuffix = configParameters[Param.gruppeAlleAdmSkoleSuffix].Value;
            var allAdminsOneSchoolSuffix = configParameters[Param.gruppeAlleAdmEnSkoleSuffix].Value;
            var allAdminsNonSchoolSuffix = configParameters[Param.gruppeAlleAnsatteIkkeSkoleSuffix].Value;
            var allOthersSchoolSuffix = configParameters[Param.gruppeAlleAndreSkoleSuffix].Value;
            var allOthersNonSchoolSuffix = configParameters[Param.gruppeAlleAndreIkkeSkoleSuffix].Value;
            var allOthersSuffix = configParameters[Param.gruppeAlleAndreSuffix].Value;

            var hrGroup = HROrgFactory.Create(aggrGroupUri, groupType, hrUnit, configParameters);

            hrGroup.GroupOwner = hrUnit?.OrganisasjonselementLeder;

            var nameSuffix = string.Empty;
            switch (groupType)
            {
                case GroupType.aggrAll:
                    {
                        nameSuffix = allPersonalSuffix;
                        break;
                    }
                case GroupType.aggrEmp:
                    {
                        nameSuffix = allEmployeesSuffix;
                        break;
                    }
                case GroupType.aggrPol:
                    {
                        nameSuffix = allPolitiansSuffix;
                        break;
                    }
                case GroupType.aggrMan:
                    {
                        nameSuffix = allLeadersSuffix;
                        break;
                    }
                case GroupType.aggrFac:
                    {
                        nameSuffix = allTeachersSuffix;
                        break;
                    }
                case GroupType.aggrAdm:
                    {
                        nameSuffix = (isSchoolNonSchoolGroup) ? ((IsSchoolGroup) ? allAdminsSchoolsSuffix : allAdminsNonSchoolSuffix) : allAdminsOneSchoolSuffix;
                        break;
                    }
                case GroupType.aggrOth:
                    {
                        nameSuffix = (isSchoolNonSchoolGroup) ? ((IsSchoolGroup ) ? allOthersSchoolSuffix : allOthersNonSchoolSuffix): allOthersSuffix;
                        break;
                    }
            }
            var groupId = GetIdValueFromLink(aggrGroupUri);
            hrGroup.GroupID = groupId;

            nameSuffix = ' ' + nameSuffix.Trim();

            var baseGroupName = hrGroup.GroupName;
            hrGroup.GroupName = baseGroupName + nameSuffix;

            var baseGroupShortName = hrGroup.GroupShortname;
            hrGroup.GroupShortname = baseGroupShortName + nameSuffix;

            return (aggrGroupUri , hrGroup);
        }

        private static EmploymentPeriodType CheckValidPeriod(string personalressursUri, string arbeidsforholdUri, IStateValue periodeValue, int daysBefore, int daysAhead, DateTime employmentCompareDate)
        {
            var periodType = EmploymentPeriodType.InvalidPeriod;

            var period = JsonConvert.DeserializeObject<Periode>(periodeValue.Value);

            var periodStart = period.Start;
            var calculatedPeriodStart = periodStart.AddDays(-daysBefore);
            var periodSlutt = (period?.Slutt != null) ? period.Slutt : DateTime.Parse(infinityDate);
            var calulatedPeriodSlutt = (period?.Slutt != null) ? period.Slutt?.AddDays(daysAhead) : DateTime.Parse(infinityDate);

            if (calculatedPeriodStart <= employmentCompareDate && calulatedPeriodSlutt >= employmentCompareDate)
            {
                if (periodStart <= employmentCompareDate && periodSlutt >= employmentCompareDate)
                {
                    periodType = EmploymentPeriodType.ValidPresentPeriod;
                    Logger.Log.Info($"{personalressursUri}: Valid present employment {arbeidsforholdUri}. PeriodStart {periodStart} is before and PeriodSlutt {periodSlutt} is after employment compare date {employmentCompareDate}");
                }
                else if (periodStart > employmentCompareDate)
                {
                    periodType = EmploymentPeriodType.ValidFuturePeriod;
                    Logger.Log.Info($"{personalressursUri}: Valid future employment {arbeidsforholdUri}. PeriodStart {periodStart} is after employment compare date {employmentCompareDate} but calculated start date {calculatedPeriodStart} is before compare date");
                }
                else
                {
                    periodType = EmploymentPeriodType.ValidPastPeriod;
                    Logger.Log.Info($"{personalressursUri}: Valid past employment {arbeidsforholdUri}. PeriodSlutt {periodSlutt} is before employment compare date {employmentCompareDate} but after calculated end date {calulatedPeriodSlutt} is after compare date");
                }
            }
            return periodType;
        }
        private static bool IsOrgElementActive(HRUnit hrUnit)
        {
            var currentDate = DateTime.Today;
            var startDate = DateTime.Parse(hrUnit.OrganisasjonselementPeriodeStart);
            var endDate = !String.IsNullOrEmpty(hrUnit?.OrganisasjonselementPeriodeSlutt) ? DateTime.Parse(hrUnit.OrganisasjonselementPeriodeSlutt) : DateTime.Parse(infinityDate);

            if (startDate <= currentDate && currentDate <= endDate)
            {
                Logger.Log.Info($"Org Unit {hrUnit.OrganisasjonselementNavn} ({hrUnit.OrganisasjonselementOrganisasjonsKode}) is active");
                return true;
            }
            Logger.Log.Info($"Org Unit {hrUnit.OrganisasjonselementNavn} ({hrUnit.OrganisasjonselementOrganisasjonsKode}) is not active. Valid period: {hrUnit.OrganisasjonselementPeriodeStart} - {hrUnit.OrganisasjonselementPeriodeSlutt}");
            return false;
        }
        #endregion

        #region private export methods

        private void GetExportDataToModify(CSEntryChange csentry, ref Dictionary<string, Dictionary<string, string>> personsToModify)
        {
            string personId = csentry.AnchorAttributes[0].Value.ToString();
            Logger.Log.DebugFormat("FintEduPersonUpdate started for person: {0}", personId);
            Dictionary<string, string> changedAttributes = new Dictionary<string, string>();
            foreach (var attributeChange in csentry.AttributeChanges)
            {
                string changedValue = string.Empty;
                var attributeName = attributeChange.Name;

                switch (attributeName)
                {
                    case PersonAttributes.PersonalBrukernavn:
                        {
                            changedValue = csentry.AttributeChanges[PersonAttributes.PersonalBrukernavn].ValueChanges[0].Value.ToString();
                            changedAttributes.Add(PersonAttributes.PersonalBrukernavn, changedValue);

                            Logger.Log.DebugFormat("Export {0}: {1}", PersonAttributes.PersonalBrukernavn, changedValue);
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonEpostadresse:
                        {
                            changedValue = csentry.AttributeChanges[PersonAttributes.PersonalKontaktinformasjonEpostadresse].ValueChanges[0].Value.ToString();
                            changedAttributes.Add(PersonAttributes.PersonalKontaktinformasjonEpostadresse, changedValue);

                            Logger.Log.DebugFormat("Export {0}: {1}", PersonAttributes.PersonalKontaktinformasjonEpostadresse, changedValue);
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer:
                        {
                            changedValue = csentry.AttributeChanges[PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer].ValueChanges[0].Value.ToString();
                            changedAttributes.Add(PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer, changedValue);

                            Logger.Log.DebugFormat("Export {0}: {1}", PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer, changedValue);
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonTelefonnummer:
                        {
                            changedValue = csentry.AttributeChanges[PersonAttributes.PersonalKontaktinformasjonTelefonnummer].ValueChanges[0].Value.ToString();
                            changedAttributes.Add(PersonAttributes.PersonalKontaktinformasjonTelefonnummer, changedValue);

                            Logger.Log.DebugFormat("Export {0}: {1}", PersonAttributes.PersonalKontaktinformasjonTelefonnummer, changedValue);
                            break;
                        }

                }
            }
            personsToModify.Add(personId, changedAttributes);
        }

        private void UpdateFintData(Dictionary<string, Dictionary<string, string>> resourcesAndAttributesToModify)
        {
            var updateDictionary = new Dictionary<string, JObject>();
            foreach (var resourceUri in resourcesAndAttributesToModify.Keys)
            {
                if (!updateDictionary.TryGetValue(resourceUri, out JObject jObject))
                {
                    var attributes = resourcesAndAttributesToModify[resourceUri];
                    if (attributes != null && attributes.Count > 0)
                    {
                        JObject updateObject = GetUpdateObject(resourceUri, attributes);

                        if (updateObject != null)
                        {
                            updateDictionary.Add(resourceUri, updateObject); 
                        }
                    }
                    else
                    {
                        Logger.Log.InfoFormat("Resource {0} used in UpdateFintData but has no attributes to modify", resourceUri);
                    }
                }
            }

            if (updateDictionary.Count > 0)
            {
                UpdateResourcesAsync(updateDictionary, HttpVerb.PUT).Wait();
            }
        }

        private async Task UpdateResourcesAsync(Dictionary<string, JObject> updates, string httpVerb)
        {

            var accessTokenUri = _globalConfigParameters[Param.idpUri].Value;
            var clientId = _globalConfigParameters[Param.clientId].Value;
            var clientSecret = Decrypt(_globalConfigParameters[Param.openIdSecret].SecureValue);
            var username = _globalConfigParameters[Param.username].Value;
            var password = Decrypt(_globalConfigParameters[Param.password].SecureValue);
            var scope = _globalConfigParameters[Param.scope].Value;
            var xOrgId = _globalConfigParameters[Param.assetId].Value;
            //var xClient = _exportConfigParameters[Param.xClient].Value;
            var felleskomponentUri = _globalConfigParameters[Param.felleskomponentUri].Value;

            //int waitTime = Int32.Parse(_globalConfigParameters[Param.waitTime].Value);
            //int lowerLimit = Int32.Parse(_globalConfigParameters[Param.lowerLimit].Value);
            //int upperLimit = Int32.Parse(_globalConfigParameters[Param.upperLimit].Value);

            int waitTime = 100;
            int lowerLimit = 4;
            int upperLimit = 7;

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var bearerToken = GetBearenToken(accessTokenUri, clientId, clientSecret, username, password, scope);

            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(bearerToken);
                client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Org_Id, xOrgId);

                Queue<String> statusQueue = new Queue<string>();

                foreach (var update in updates)
                {
                    string uri = update.Key;
                    JObject jsonObject = update.Value;
                    string statusUrl = await UpdateResourceAsync(uri, jsonObject, httpVerb, client).ConfigureAwait(false);

                    await Task.Delay(waitTime).ConfigureAwait(false);

                    HttpStatusCode statusCode = await CheckUpdateStatusAsync(statusUrl, client).ConfigureAwait(false);
                    Logger.Log.InfoFormat("Resource {0}: http status is {1} on first response from {2}", uri, statusCode, statusUrl);

                    if (statusCode == HttpStatusCode.Accepted)
                    {
                        statusQueue.Enqueue(statusUrl);
                        Logger.Log.InfoFormat("Resource {0}: Enqueuing location url {1}", uri, statusUrl);

                        if (statusQueue.Count >= upperLimit)
                        {
                            Logger.Log.InfoFormat("Status Queue reached upper element limit ({0}). Stopping new updates until status queue is below {1} elements", upperLimit.ToString(), lowerLimit.ToString());

                            await CheckStatusQueue(statusQueue, waitTime, lowerLimit, client);

                            Logger.Log.InfoFormat("Status Queue is below lower element limit: {0}. Starting new updates again", lowerLimit.ToString());
                        }
                    }
                    else if (statusCode == HttpStatusCode.OK)
                    {
                        Logger.Log.InfoFormat("Resource {0}: http status is {1} on {2}", uri, HttpStatusCode.OK, statusUrl);
                    }
                }
                if (statusQueue.Count > 0)
                {
                    Logger.Log.Info("Status Queue still not empty. Checking Queue again");

                    await CheckStatusQueue(statusQueue, waitTime, 1, client);
                }
            }
        }
        private async Task CheckStatusQueue(Queue<string> statusQueue, int waitTime, int lowerLimit, IHalHttpClient client)
        {
            int cycleCount = 1;
            int count = 0;

            double delay = waitTime;

            int remainingQueueLength = statusQueue.Count;

            while (statusQueue.Count >= lowerLimit && cycleCount <= retrylimit)
            {
                ++count;

                var statusUrl = statusQueue.Dequeue();

                await Task.Delay(waitTime).ConfigureAwait(false);

                Logger.Log.InfoFormat("Location url {0}: Dequeued and checking status", statusUrl);

                var statusCode = await CheckUpdateStatusAsync(statusUrl, client).ConfigureAwait(false);

                if (statusCode == HttpStatusCode.Accepted)
                {
                    statusQueue.Enqueue(statusUrl);
                    Logger.Log.InfoFormat("Location url {0}: Statuscode {1}, enqueuing url again", statusUrl, statusCode.ToString());
                }
                else if (statusCode == HttpStatusCode.OK)
                {
                    Logger.Log.InfoFormat("Location url {0}: Statuscode {1} - Update confirmed", statusUrl, HttpStatusCode.OK);
                }

                if (remainingQueueLength > 0 && count == remainingQueueLength)
                {
                    ++cycleCount;
                    delay *= factor;
                    var waitmilliseconds = (int)Math.Round(delay);

                    Logger.Log.InfoFormat("The status queue was not emptied during this cycle, {0} elements remaining. Waiting {1} milliseconds before running cycle no {2}",
                        remainingQueueLength.ToString(), waitmilliseconds.ToString(), cycleCount.ToString());

                    await Task.Delay(waitmilliseconds).ConfigureAwait(false);

                    remainingQueueLength = statusQueue.Count;

                    count = 0;
                }                
            }

        }
        private async Task<HttpStatusCode> CheckUpdateStatusAsync(string statusUri, IHalHttpClient client)
        {
            HttpStatusCode statusCode = 0;
            try
            {
                IHalHttpResponseMessage statusResponse = await InvokeRequestAsync(HttpVerb.GET, statusUri, null, client).ConfigureAwait(false);
                statusCode = statusResponse.Message.StatusCode;
            }
            catch (AggregateException aggregateEx)
            {
                aggregateEx.Handle(e =>
                {
                    if (e is HalHttpRequestException hal)
                    {
                        var halStatusCode = hal.StatusCode; // response status code

                        statusCode = halStatusCode;

                        var statusCodeDescription = string.Empty;

                        switch (halStatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                {
                                    statusCodeDescription = "400 Bad request";
                                    break;
                                }
                            case HttpStatusCode.NotFound:
                                {
                                    statusCodeDescription = "404 Not found";
                                    break;
                                }
                            case HttpStatusCode.InternalServerError:
                                {
                                    statusCodeDescription = "500 Internal server error";
                                    break;
                                }
                        }
                        var resource = hal.Resource;

                        var message = string.Empty;
                        var responsestatus = string.Empty;
                        var exception = string.Empty;

                        if (resource != null)
                        {
                            if (resource.State != null)
                            {
                                var state = resource.State;
                                foreach (var key in state.Keys)
                                {
                                    switch (key)
                                    {
                                        case "message":
                                            {
                                                message = state[key].Value;
                                                break;
                                            }

                                        case "responseStatus":
                                            {
                                                responsestatus = state[key].Value;
                                                break;
                                            }
                                        case "exception":
                                            {
                                                exception = state[key].Value;
                                                break;
                                            }
                                    }
                                }
                            }
                            Logger.Log.ErrorFormat("Location {0}: Getting update status failed. HTTP response: {1}, Message: {2}, Inner response status: {3}, Exception: {4}"
                                , statusUri, statusCodeDescription, message, responsestatus, exception);
                        }
                        return true;

                    }
                    return false;
                });

            }
            catch (HalHttpRequestException e)
            {
                var halStatusCode = e.StatusCode; // response status code
                var statusCodeDescription = string.Empty;

                switch (halStatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            statusCodeDescription = "400 Bad request";
                            break;
                        }
                    case HttpStatusCode.NotFound:
                        {
                            statusCodeDescription = "404 Not found";
                            break;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            statusCodeDescription = "500 Internal server error";
                            break;
                        }
                }
                var message = e?.Message;
                var innerexception = e?.InnerException?.Message;
                Logger.Log.ErrorFormat("Location {0}: Getting update status failed. HTTP response: {1}, Message: {2}, Inner exception: {3}"
                                , statusUri, statusCodeDescription, message, innerexception);

            }
            return statusCode;

        }

        private async Task<string> UpdateResourceAsync(string resourceUri, JObject jsonObject, string httpVerb, IHalHttpClient client)
        {
            var statusUri = String.Empty;
            try
            {
                Logger.Log.InfoFormat("Resource {0}: starting async update", resourceUri);

                IHalHttpResponseMessage putResponse = await InvokeRequestAsync(httpVerb, resourceUri, jsonObject, client).ConfigureAwait(false);

                statusUri = putResponse.Message.Headers.GetValues(HttpHeader.Location).FirstOrDefault();

                Logger.Log.InfoFormat("Resource {0}: status uri for PUT Response {1}", resourceUri, statusUri);


            }
            catch (AggregateException aggregateEx)
            {
                aggregateEx.Handle(e =>
                {
                    if (e is HalHttpRequestException hal)
                    {
                        var halStatusCode = hal.StatusCode; // response status code
                        var statusCodeDescription = string.Empty;

                        switch (halStatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                {
                                    statusCodeDescription = "400 Bad request";
                                    break;
                                }
                            case HttpStatusCode.InternalServerError:
                                {
                                    statusCodeDescription = "500 Internal server error";
                                    break;
                                }
                        }
                        var resource = hal.Resource;

                        var message = string.Empty;
                        var responsestatus = string.Empty;
                        var exception = string.Empty;

                        if (resource != null)
                        {
                            if (resource.State != null)
                            {
                                var state = resource.State;
                                foreach (var key in state.Keys)
                                {
                                    switch (key)
                                    {
                                        case "message":
                                            {
                                                message = state[key].Value;
                                                break;
                                            }

                                        case "responseStatus":
                                            {
                                                responsestatus = state[key].Value;
                                                break;
                                            }
                                        case "exception":
                                            {
                                                exception = state[key].Value;
                                                break;
                                            }
                                    }
                                }
                            }
                            Logger.Log.ErrorFormat("Resource {0}: update failed. HTTP response: {1}, Message: {2}, Inner response status: {3}, Exception: {4}"
                                , resourceUri, statusCodeDescription, message, responsestatus, exception);
                        }
                        return true;

                    }
                    return false;
                });
            }
            catch (HalHttpRequestException e)
            {
                var halStatusCode = e.StatusCode;
                var statusCodeDescription = string.Empty;

                switch (halStatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            statusCodeDescription = "400 Bad request";
                            break;
                        }
                    case HttpStatusCode.NotFound:
                        {
                            statusCodeDescription = "404 Not found";
                            break;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            statusCodeDescription = "500 Internal server error";
                            break;
                        }
                }
                var message = e?.Message;
                var innerexception = e?.InnerException?.Message;
                Logger.Log.ErrorFormat("Resource {0}: update failed. HTTP response: {1}, Message: {2}, Inner exception: {3}"
                                , resourceUri, statusCodeDescription, message, innerexception);

            }
            return statusUri;
        }

        private JObject GetUpdateObject(string resourceUri, Dictionary<string, string> attributes)
        {
            var response = _resourceDict[resourceUri];
            var resourceAsJson = string.Empty;
            var attrvalues = string.Empty;
            var jsonObject = new JObject();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new LowercaseContractResolver()
            };

            var personalResource = PersonalressursRecourceFactory.Create(response, null);

            foreach (var attribute in attributes)
            {
                var csAttributeName = attribute.Key;
                var csAttributeValue = attribute.Value;
                switch (csAttributeName)
                {
                    case PersonAttributes.PersonalBrukernavn:
                        {                           
                            personalResource.Brukernavn.Identifikatorverdi = csAttributeValue;                            
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonEpostadresse:
                        {
                            personalResource.Kontaktinformasjon.Epostadresse = csAttributeValue;
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonMobiltelefonnummer:
                        {
                            personalResource.Kontaktinformasjon.Mobiltelefonnummer = csAttributeValue;
                            break;
                        }
                    case PersonAttributes.PersonalKontaktinformasjonTelefonnummer:
                        {
                            personalResource.Kontaktinformasjon.Telefonnummer = csAttributeValue;
                            break;
                        }
                }
                attrvalues += ", " + csAttributeName + "=" + csAttributeValue;
            }

            resourceAsJson = JsonConvert.SerializeObject(personalResource, settings);
            jsonObject = JsonConvert.DeserializeObject<JObject>(resourceAsJson);

            Logger.Log.InfoFormat("Updating resource {0} with: {1}", resourceUri, attrvalues);
            Logger.Log.DebugFormat("JSON body to send: {0}", resourceAsJson);

            return jsonObject;
        }

        private JObject GetJsonLinks(IReadOnlyDictionary<string, IEnumerable<ILinkObject>> links)
        {
            dynamic jValue = new JObject();
            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.person:
                        {
                            jValue.person = new JArray() as dynamic;

                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                dynamic link = new JObject();
                                var hrefValue = linkObject.Href.ToString();
                                link.href = hrefValue;
                                jValue.person.Add(link);
                            }
                            break;
                        }
                    case ResourceLink.self:
                        {
                            jValue.self = new JArray() as dynamic;

                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                dynamic link = new JObject();
                                var hrefValue = linkObject.Href.ToString();
                                link.href = hrefValue;
                                jValue.self.Add(link);
                            }
                            break;
                        }
                }
            }
            return jValue;
        }

        #endregion

        #region private helper methods

        private HashSet<string> GetConfigParameterList (KeyedCollection<string, ConfigParameter> configParameters, string configParameter)
        {
            var parameterList = configParameters[configParameter].Value.Split(',');

            var parameterHashset = new HashSet<string>();

            foreach (var parameter in parameterList)
            {
                parameterHashset.Add(parameter);
            }
            return parameterHashset;
        }
        private void UpdateIdentifierMappingDict(string idUri, IEmbeddedResourceObject resourceObject, ref Dictionary<string, string> idMappingDict)
        {
            if (resourceObject.Links.TryGetValue(ResourceLink.self, out IEnumerable<ILinkObject> selfLinks))
            {
                foreach (var link in selfLinks)
                {
                    var selfUri = LinkToString(link);
                    Logger.Log.DebugFormat("UpdateResourceIdMappingDict: Adding key {0} and value {1} to dictionary", selfUri, idUri);
                    try
                    {
                        idMappingDict.Add(selfUri, idUri);
                    }
                    catch (Exception e)
                    {
                        Logger.Log.ErrorFormat("UpdateResourceIdMappingDict: Inconsistent self links. Adding key {0} and value {1} to dictionary failed. Error message {2}", selfUri, idUri, e.Message);
                    }
                }
            }
        }
        private string GetIdentifikatorUri(IEmbeddedResourceObject resource, string felleskomponentUri, string identifikatorName)
        {
            var identifikatorValue = GetIdentifikatorValue(resource, identifikatorName);
            var selfLinkUri = LinkToString(resource.Links[ResourceLink.self]);
            var uriClassPath = GetUriPathForClass(selfLinkUri);
            var uriPath = felleskomponentUri + uriClassPath;

            var identifikatorUri = (uriPath + Delimiter.path + identifikatorName).ToLower() + Delimiter.path + identifikatorValue;

            return identifikatorUri;

        }

        private string GetSystemIdUri(IEmbeddedResourceObject resource, string felleskomponentUri)
        {
            var systemIdValue = GetIdentifikatorValue(resource, FintAttribute.systemId);

            var selfLinkUri = LinkToString(resource.Links[ResourceLink.self]);

            var uriClassPath = GetUriPathForClass(selfLinkUri);

            var uriPath = felleskomponentUri + uriClassPath;

            var systemIdUri = (uriPath + Delimiter.path + FintAttribute.systemId).ToLower() + Delimiter.path + systemIdValue;

            return systemIdUri;

        }

        private string GetIdentifikatorValue(IEmbeddedResourceObject resource, string indentifikatorName)
        {
            var identikatorAttribute = resource.State[indentifikatorName];
            var identikator = JsonConvert.DeserializeObject<Identifikator>(identikatorAttribute.Value);
            var identikatorValue = identikator.Identifikatorverdi;

            return identikatorValue;
        }



        private string GetBearenToken(string accessTokenUri, string clientId, string clientSecret, string username, string password, string scope)
        {

            var tokenClient = new HttpClient();
            var tokenResponse = tokenClient.RequestTokenAsync(new TokenRequest
            {
                Address = accessTokenUri,

                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = OidcConstants.GrantTypes.Password,

                Parameters =
                {
                    { OidcConstants.TokenRequest.UserName, username },
                    { OidcConstants.TokenRequest.Password, password },
                    { OidcConstants.TokenRequest.Scope, scope }
                }
            }).Result;

            var bearerToken = tokenResponse.AccessToken;

            return bearerToken;
        }

        private IHalHttpResponseMessage InvokeRequest(string method, string uriString, JObject json, IHalHttpClient client)
        {
            var uri = new Uri(uriString);
            var response = new HttpResponseMessage() as IHalHttpResponseMessage;

            switch (method)
            {
                case HttpVerb.GET:
                    {
                        response = client.GetAsync(uri).Result;
                        break;
                    }
                case HttpVerb.PUT:
                    {
                        response = client.PutAsync(uri, json).Result;
                        break;
                    }
            }
            return response;
        }

        private async Task<IHalHttpResponseMessage> InvokeRequestAsync(string method, string uriString, JObject json, IHalHttpClient client)
        {
            var escapedUriString = Uri.EscapeUriString(uriString);

            var uri = new Uri(escapedUriString);

            var response = new HttpResponseMessage() as IHalHttpResponseMessage;

            switch (method)
            {
                case HttpVerb.GET:
                    {
                        response = await client.GetAsync(uri);
                        break;
                    }
                case HttpVerb.PUT:
                    {
                        response = await client.PutAsync(uri, json);
                        break;
                    }
            }
            return response;
        }

        #endregion

    }
}
