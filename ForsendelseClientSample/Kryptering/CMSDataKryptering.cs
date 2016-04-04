using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;


namespace ForsendelseClientSample
{
    class CMSDataKryptering
    {
        public static byte[] KrypterData(byte[] ukryptertBytes, X509Certificate sertifikat)
        {
/*            AlgorithmIdentifier hash = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance);
            AlgorithmIdentifier mask = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hash);
            AlgorithmIdentifier pSource = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdPSpecified, new DerOctetString(new byte[0]));
            RsaesOaepParameters parameters = new RsaesOaepParameters(hash, mask, pSource);
            AlgorithmIdentifier keyEncryptionScheme = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdRsaesOaep, parameters);*/

            CmsEnvelopedDataGenerator envelopedDataGenerator = new CmsEnvelopedDataGenerator();
          
            envelopedDataGenerator.AddKeyTransRecipient(sertifikat);
            CmsEnvelopedData cmsData = envelopedDataGenerator.Generate(new CmsProcessableByteArray(ukryptertBytes), CmsEnvelopedGenerator.Aes256Cbc);
            return cmsData.GetEncoded();
        }

        public static byte[] DekrypterData2(byte[] kryptertData, AsymmetricKeyParameter privateKey)
        {

            CmsEnvelopedDataParser cmsEnvelopedDataParser = new CmsEnvelopedDataParser(kryptertData);
            RecipientInformationStore recipientInformationStore = cmsEnvelopedDataParser.GetRecipientInfos();
   
            IEnumerator enumerator = recipientInformationStore.GetRecipients().GetEnumerator();
            enumerator.MoveNext();
            RecipientInformation recipientInformation = enumerator.Current as RecipientInformation;
            return recipientInformation.GetContent(privateKey);

        }

        public static byte[] DekrypterData(byte[] kryptertData, AsymmetricKeyParameter privateKey)
        {
            // Dette krever at man har lagret private key i lokal keystore
            var envelopedCms = new EnvelopedCms();
            envelopedCms.Decode(kryptertData);
            envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);
            return envelopedCms.ContentInfo.Content;
        }


    }

}
