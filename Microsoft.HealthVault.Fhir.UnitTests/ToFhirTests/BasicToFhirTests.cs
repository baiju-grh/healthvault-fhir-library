﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class BasicToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultBasicTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var basic = new ItemTypes.Basic
            {
                Gender = Gender.Female,
                BirthYear = 1975,
                City = "Redmond",
                StateOrProvince = "WA",
                PostalCode = "98052",
                Country = "USA",
                FirstDayOfWeek = DayOfWeek.Sunday,
                Languages =
                {
                    new Language(new CodableValue("English", "en", "iso639-1", "iso", "1"), true),
                    new Language(new CodableValue("French", "fr", "iso639-1", "iso", "1"), false),
                }
            };
            
            var patient = basic.ToFhir<Patient>();

            Assert.IsNotNull(patient);
            Assert.AreEqual(AdministrativeGender.Female, patient.Gender.Value);
            Assert.AreEqual(1975, ((FhirDecimal)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/birth-year").Value).Value);
            Assert.AreEqual("0", ((Coding)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/first-day-of-week").Value).Code);
            Assert.AreEqual("Sunday", ((Coding)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/first-day-of-week").Value).Display);

            Assert.AreEqual(1, patient.Address.Count);
            Assert.AreEqual("Redmond", patient.Address[0].City);
            Assert.AreEqual("WA", patient.Address[0].State);
            Assert.AreEqual("98052", patient.Address[0].PostalCode);
            Assert.AreEqual("USA", patient.Address[0].Country);

            Assert.AreEqual(2, patient.Communication.Count);
            Assert.AreEqual("English", patient.Communication[0].Language.Coding[0].Display);
            Assert.AreEqual(true, patient.Communication[0].Preferred);
        }
    }
}