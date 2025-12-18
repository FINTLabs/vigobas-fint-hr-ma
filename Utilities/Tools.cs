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
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Resource;
using static VigoBAS.FINT.HR.Constants;
using Vigo.Bas.ManagementAgent.Log;


namespace VigoBAS.FINT.HR.Utilities
{
    class Tools
    {
        public static string GetStringValueFromHalAttribute (IReadOnlyDictionary<string, IStateValue> keyValues, string fintAttribute)
        {
            var attributeValue = string.Empty;

            if (keyValues.TryGetValue(fintAttribute, out IStateValue stateValue))
            {
                attributeValue = stateValue.Value;
            }

            return attributeValue;
        }
        public static Identifikator GetFintIdentifikatorFromHalAttribute (IReadOnlyDictionary<string, IStateValue> keyValues, string fintAttribute)
        {
            var identifikatorValue = new Identifikator();

            if (keyValues.TryGetValue(fintAttribute, out IStateValue stateValue))
            {
                identifikatorValue = JsonConvert.DeserializeObject<Identifikator>(stateValue.Value);
            }
            return identifikatorValue;
        }

        public static List<string> GetStringListFromHalLink (IReadOnlyDictionary<string, IEnumerable<ILinkObject>> links, string fintLinkName)
        {
            var linkList = new List<string>();
            if (links.TryGetValue(fintLinkName, out IEnumerable<ILinkObject> fintLinks))
            {
                foreach (var fintLink in fintLinks)
                {
                    var linkUri = LinkToString(fintLink);
                    linkList.Add(linkUri);
                }
            }
            return linkList;
        }



    public static string GetIdValueFromLink(string linkAsString)
        {
            string idValue = linkAsString.Split('/').Last();

            return idValue;
        }
        public static string GetIdValueFromLink(IEnumerable<ILinkObject> links)
        {
            string href = LinkToString(links);
            string idValue = href.Split('/').Last();

            return idValue;
        }
        public static string GetIdValueFromLink(List<Link> links)
        {
            var link = links[0];            
            string href = link.href;
            string idValue = href.Split('/').Last();

            return idValue;
        }

        public static string LinkToString(List<Link> links)
        {
            var link = links[0];
            string href = link.href;
            var normalizedUri = NormalizeUri(href);
            return normalizedUri;
        }

        public static string LinkToString(ILinkObject link)
        {
            var uriAsString = link.Href.ToString();
            var normalizedUri = NormalizeUri(uriAsString);
            return normalizedUri;
        }
        public static string LinkToString(IEnumerable<ILinkObject> links)
        {
            var uriAsString = string.Empty;
            var normalizedUri = string.Empty;
            var noOfLinks = links.Count();
            if (noOfLinks > 1)
            {
                var linksAsString = string.Empty;
                foreach (var link in links)
                {
                    var linkString = link.Href.ToString();
                    linksAsString += linkString + Delimiter.listDelimiter;
                }
                //var message = "Found more than one link in self: " + linksAsString;
                //Logger.Log.InfoFormat(message);
            }
            uriAsString = links.First().Href.ToString();
            normalizedUri = NormalizeUri(uriAsString);

            return normalizedUri;
        }
        public static List<string> LinksToStrings(IEnumerable<ILinkObject> links)
        {
            var returnUris = new List<string>();
            
            foreach (var link in links)
            {
                var uriAsString = link.Href.ToString();
                var normalizedUri = NormalizeUri(uriAsString);
            returnUris.Add(normalizedUri);
            }         
            return returnUris;
        }

        public static string NormalizeUri(string uri)
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

        public static bool CheckValidPeriod(IStateValue periodeValue, int daysBefore, int daysAhead)
        {
            bool periodIsValid = false;
            var period = new Periode();

            //periodList = JsonConvert.DeserializeObject<List<Periode>>(periodeValue.Value);

            //var period = periodList.First();
            var periodStart = period.Start;
            var periodSlutt = (period?.Slutt!=null) ? period.Slutt : DateTime.Parse(infinityDate);

            var daysBeforeToday = DateTime.Today.AddDays(-daysBefore);
            var daysAheadToday = DateTime.Today.AddDays(daysAhead);

            if (periodStart <= daysBeforeToday && periodSlutt >= daysAheadToday)
            {
                periodIsValid = true;
            }
            return periodIsValid;
        }

        public static string Decrypt(SecureString inStr)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(inStr);
            string decrString = Marshal.PtrToStringUni(ptr);
            return decrString;
        }

        public static string GetFintType(string fintUri)
        {
            var segments = fintUri.Split(Delimiter.path);
            var noOfSegments = segments.Length;
            var fintType = segments[noOfSegments - 3];

            return fintType;
        }

        public static string GetUriPathForClass(string uriString)
        {
            var uri = new Uri(uriString);
            var uriPath = uri.AbsolutePath;
            string pattern = @"(?<path>.*)(/.+/.+)";
            string replacement = "${path}";
            var result = Regex.Replace(uriPath, pattern, replacement);

            return result;
        }

        public static System.Boolean IsNumeric(System.Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());

                return true;
            }
            catch { }
            return false;
        }

    }
}
