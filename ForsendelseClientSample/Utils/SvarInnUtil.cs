using System;
using System.IO;
using System.Net;
using System.Text;
using Org.BouncyCastle.Crypto;
using PemReader = Org.BouncyCastle.OpenSsl.PemReader;

namespace ForsendelseClientSample.Utils
{
    class SvarInnUtil
    {
        private const string SvarUtUrl = "https://test.svarut.ks.no";

        public static byte[] LastNedForsendelse(string mottakerId, string password, string forsendelsesId)
        {
            using (WebClient client = GetClient(mottakerId, password))
            {
                client.Credentials = new NetworkCredential(mottakerId, password);
                return client.DownloadData(new Uri(SvarUtUrl + "/tjenester/svarinn/forsendelse/" + forsendelsesId));
            }
        }

        public static void KvitterMottak(string mottakerId, string password, string forsendelsesId)
        {
            using (WebClient client = GetClient(mottakerId, password))
            {
                client.Credentials = new NetworkCredential(mottakerId, password);
                client.UploadString(new Uri(SvarUtUrl + "/tjenester/svarinn/kvitterMottak/forsendelse/" + forsendelsesId), "");
            }
        }

        public static AsymmetricKeyParameter GetPrivateKey()
        {
            // Erstatt sp_key i resource mappen med din egen sp_key. 
            Stream stream = new MemoryStream(ForsendelseClientSample.Properties.Resources.sp_key);
            TextReader textReader = new StreamReader(stream);
            PemReader pemReader = new PemReader(textReader);
            AsymmetricCipherKeyPair keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
            return keyPair.Private;
        }

        public static string HentForsendelser(string mottakerId, string password)
        {
            using (WebClient client = GetClient(mottakerId, password))
            {
                client.Credentials = new NetworkCredential(mottakerId, password);
                return client.DownloadString(new Uri(SvarUtUrl + "/tjenester/svarinn/mottaker/hentNyeForsendelser"));
            }
        }

        protected static WebClient GetClient(string mottakerId, string password)
        {
            var c = new WebClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(mottakerId + ":" + password));
            c.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            return c;

        }
    }
}
