using ForsendelseClientSample.ForsendelsesService;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace ForsendelseClientSample.Utils
{
    class ForsendelseUtil
    {
        public static forsendelse CreateKryptertForsendelse(string tittel)
        {
            forsendelse forsendelse = new forsendelse {kryptert = true, tittel = tittel, krevNiva4Innlogging = false, avgivendeSystem = "Avgivende system .Net"};
            forsendelse.dokumenter = new[] {CreateKryptertDokument()};
            forsendelse.mottaker = CreatePrivatPerson();
            forsendelse.printkonfigurasjon = CreatePrintkonfigurasjon();
            return forsendelse;
        }

        public static forsendelse CreateUkryptertForsendelse(string tittel)
        {
            var testForsendelse = new forsendelse { kryptert = false, tittel = tittel, krevNiva4Innlogging = false, avgivendeSystem = "Avgivende system .Net"};
            testForsendelse.mottaker = CreatePrivatPerson();
            var dok = new dokument();
            dok.filnavn = "small.pdf";
            dok.mimetype = "application/pdf";
            dok.data = ForsendelseClientSample.Properties.Resources.small_pdf;
            testForsendelse.dokumenter = new dokument[1] { dok };
            testForsendelse.printkonfigurasjon = CreatePrintkonfigurasjon();

            return testForsendelse;
        }

        public static forsendelse CreateUkryptertForsendelseForOrgnr(string tittel, string orgnr, byte[] dataBytes)
        {
            var testForsendelse = new forsendelse { kryptert = false, tittel = tittel, krevNiva4Innlogging = false, avgivendeSystem = "Avgivende system .Net" };
            testForsendelse.mottaker = CreateOrganisasjon(orgnr);
            var dok = new dokument();
            dok.filnavn = "small.pdf";
            dok.mimetype = "application/pdf";
            dok.data = dataBytes;
            testForsendelse.dokumenter = new dokument[1] { dok };
            testForsendelse.printkonfigurasjon = CreatePrintkonfigurasjon();

            return testForsendelse;
        }

        private static mottaker CreateOrganisasjon(string orgnr)
        {
            return new organisasjon 
            {
                orgnr = orgnr,
                adresse1 = "Hakkebakkeskogen 111",
                navn = "Bamsefar",
                postnr = "5236",
                poststed = "RÅDAL"
            };
        }

        private static printkonfigurasjon CreatePrintkonfigurasjon()
        {
            return new printkonfigurasjon
            {
                brevtype = brevtype.APOST,
                brevtypeSpecified = true,
                fargePrint = false,
                tosidig = false
            };
        }

        private static mottaker CreatePrivatPerson()
        {
            return new privatPerson
            {
                fodselsnr = "12345678910",
                adresse1 = "Hakkebakkeskogen 111",
                navn = "Bamsefar",
                postnr = "5236",
                poststed = "RÅDAL"
            };
        }

        public static dokument CreateKryptertDokument()
        {
            dokument dok = new dokument();
            dok.mimetype = "application/pdf";
            dok.filnavn = "test.pdf";
            X509Certificate sertifikat = HentX509Certificate();
            KrypterDokumentMedSertifikat(dok, sertifikat);

            return dok;
        }

        private static void KrypterDokumentMedSertifikat(dokument dok, X509Certificate sertifikat)
        {
            byte[] ukryptertBytes = ForsendelseClientSample.Properties.Resources.small_pdf;
            byte[] kryptertData = CMSDataKryptering.KrypterData(ukryptertBytes, sertifikat);
            dok.data = kryptertData;
        }

        private static X509Certificate HentX509Certificate()
        {
            // svarut_public.pem må lastes ned og erstattes
            X509CertificateParser certParser = new X509CertificateParser();
            X509Certificate certificate = certParser.ReadCertificate(ForsendelseClientSample.Properties.Resources.svarut_public);

            return certificate;
        }
    }
}
