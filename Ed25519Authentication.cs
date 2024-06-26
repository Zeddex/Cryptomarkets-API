using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace Cryptomarkets.Apis
{
    public class Ed25519Authentication
    {
        public static string SignMessage(string message, string privateKeyString)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            if (string.IsNullOrEmpty(privateKeyString)) throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKeyString));

            var privateKeyBytes = Convert.FromBase64String(privateKeyString);

            var messageBytes = Encoding.UTF8.GetBytes(message);

            var signer = new Ed25519Signer();

            var privateKeyParam = new Ed25519PrivateKeyParameters(privateKeyBytes, 0);
            signer.Init(true, privateKeyParam);

            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            var signature = signer.GenerateSignature();

            var signatureString = Convert.ToBase64String(signature);

            return signatureString;
        }
    }
}
