using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ForsendelseClientSample.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Org.BouncyCastle.X509;

namespace ForsendelseClientSample
{
    [TestClass]
    public class SvarinnEksempel
    {
        private const string ForsendelsesServiceUsername = "din_bruker";
        private const string ForsendelsesServicePassword = "ditt_passord";
        private const string MottakerOrg = "ditt_orgnr";
        private const string MottakerId = "din_mottaker_id";
        private const string Password = "ditt_mottaker_passord";

        public SvarinnEksempel() { }

        /// <summary>
        /// Dette eksemplet forutsetter at følgende er gjort:
        /// 1) Har opprettet bruker og service-passord for å kunne sende inn forsendelse til SvarUt 
        /// 2) Har opprettet mottaker og service passord for å kunne laste ned forsendelser fra SvarInn komponenten i SvarUt 
        /// 3) Har lastet opp public key for mottaker i SvarUt
        /// 4) Filen sp_key.pem i Resources mappen er erstattet med din egen private key
        /// 5) Orgnr er registrert på mottaker i SvarUt
        /// 6) Private key er lagret i lokal keystore på maskinen man kjører dette eksempelet 
        /// </summary>
        [TestMethod]
        public void TestDekrypteringAvNedlastetFil()
        {

            // NB: Dette er kun gjort for testing.
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;

            // NB: Slå av Expect100Continue. Denne er pr default satt til true og kan føre til problemer.
            ServicePointManager.Expect100Continue = false;

            var forsendelsesService = new ForsendelsesService.ForsendelsesServiceV4Client();

            forsendelsesService.ClientCredentials.UserName.UserName = ForsendelsesServiceUsername;
            forsendelsesService.ClientCredentials.UserName.Password = ForsendelsesServicePassword;

            string tittel = "Dette er en ukryptert eksempelforsendelse fra .Net  - " + Guid.NewGuid().ToString();
            string forsendelsesId = forsendelsesService.sendForsendelse(ForsendelseUtil.CreateUkryptertForsendelseForOrgnr(tittel, MottakerOrg, Properties.Resources.small_pdf));
            Debug.WriteLine(string.Format("Sendte forsendelse med id {0}", forsendelsesId));
            dynamic forsendelser = JsonConvert.DeserializeObject(SvarInnUtil.HentForsendelser(MottakerId, Password));

            Debug.WriteLine("Mottar uleste forsendelser:");
            Debug.Indent();
            foreach (dynamic forsendelse in forsendelser)
            {
                string id = forsendelse.id;
                Debug.WriteLine(string.Format("Id: {0} ", id));

                byte[] kryptertData = SvarInnUtil.LastNedForsendelse(MottakerId, Password, id);
                Assert.IsNotNull(kryptertData);
                Assert.IsFalse(kryptertData.Length == 0);

                // Dekrypter og test
                byte[] dekryptertData = CMSDataKryptering.DekrypterData(kryptertData);
                Assert.IsNotNull(dekryptertData);
                Assert.IsFalse(dekryptertData.Length == 0);

                // Husk å kvittere som mottat i SvarUt etter at vi har bekreftet at nedlasting var vellykket
                SvarInnUtil.KvitterMottak(MottakerId, Password, id);
                ForsendelsesService.forsendelseStatus status = forsendelsesService.retrieveForsendelseStatus(id);
                Assert.AreEqual(status, ForsendelsesService.forsendelseStatus.LEST);
            }
            Debug.Unindent();

        }
    }
}
