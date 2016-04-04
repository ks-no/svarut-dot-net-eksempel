using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ForsendelseClientSample.ForsendelsesService;
using ForsendelseClientSample.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ForsendelseClientSample
{
 
    [TestClass]
    public class SendForsendelseEksempel
    {
        public SendForsendelseEksempel()
        {
          
        }

        private TestContext _testContextInstance;
        private const string UserName = "din_bruker";
        private const string Password = "ditt_passord";


        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        /// <summary>
        /// Dette eksemplet forutsetter at følgende er gjort:
        /// 1) Har opprettet bruker og service-passord for å kunne sende inn forsendelse til SvarUt 
        /// </summary>
        [TestMethod]
        public void TestAtSendForsendelseReturnererId()
        {
            // NB: Dette er kun gjort for testing.
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

            // NB: Slå av Expect100Continue. Denne er pr default satt til true og kan føre til problemer.
            ServicePointManager.Expect100Continue = false;

            var forsendelsesService = new ForsendelsesService.ForsendelsesServiceV4Client();

            forsendelsesService.ClientCredentials.UserName.UserName = UserName;
            forsendelsesService.ClientCredentials.UserName.Password = Password;

            string tittel = "Dette er en ukryptert eksempelforsendelse fra .Net  - " + Guid.NewGuid().ToString();
            string id = forsendelsesService.sendForsendelse(ForsendelseUtil.CreateUkryptertForsendelse(tittel));
            Assert.IsNotNull("Id skal ikke være null. Forsendelse feilet.", id);      
        }

        /// <summary>
        /// Dette eksemplet forutsetter at følgende er gjort:
        /// 1) Har opprettet bruker og service-passord for å kunne sende inn forsendelse til SvarUt 
        /// 3) Har lastet ned public key fra SvarUt og erstattet filen svarut_public.pem i resources mappen
        /// 4) Orgnr er registrert på mottaker i SvarUt
        /// </summary>
        [TestMethod]
        public void TestAtKryptertSendForsendelseReturnererId()
        {
            // NB: Dette er kun gjort for testing.
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

            // NB: Slå av Expect100Continue. Denne er pr default satt til true og kan føre til problemer.
            ServicePointManager.Expect100Continue = false;

            var forsendelsesService = new ForsendelsesService.ForsendelsesServiceV4Client();

            forsendelsesService.ClientCredentials.UserName.UserName = UserName;
            forsendelsesService.ClientCredentials.UserName.Password = Password;

            string tittel = "Dette er en kryptert eksempelforsendelse fra .Net - " + Guid.NewGuid().ToString();
            string id = forsendelsesService.sendForsendelse(ForsendelseUtil.CreateKryptertForsendelse(tittel));
            Assert.IsNotNull("Id skal ikke være null. Forsendelse feilet.", id);
        }
    }
}
