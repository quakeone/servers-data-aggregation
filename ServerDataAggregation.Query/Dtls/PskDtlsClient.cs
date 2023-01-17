using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace ServersDataAggregation.Query.Dtls;

public class PskDtlsClient : AbstractTlsClient
{
    private static readonly int[] DefaultCipherSuites = new int[]
    {
        CipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
        CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256,
        CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA,
        CipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
        CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256,
        CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256,
        CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA
    };

    protected TlsSession mSession;
    protected TlsPskIdentity mPskIdentity;

    public PskDtlsClient(TlsPskIdentity pskIdentity)
        : this(new BcTlsCrypto(new SecureRandom()), pskIdentity)
    {
    }

    public PskDtlsClient(TlsCrypto crypto, TlsPskIdentity pskIdentity)
        : base(crypto)
    {
        this.mPskIdentity = pskIdentity;
    }

    public override int[] GetCipherSuites()
    {
        return DefaultCipherSuites;
    }

    public override TlsPskIdentity GetPskIdentity()
    {
        return mPskIdentity;
    }

    public override TlsAuthentication GetAuthentication()
    {
        /*
         * Note: This method is not called unless a server certificate is sent, which may be the
         * case e.g. for RSA_PSK key exchange.
         */
        throw new TlsFatalAlert(AlertDescription.internal_error);
    }

    protected override ProtocolVersion[] GetSupportedVersions()
    {
        return ProtocolVersion.DTLSv12.Only();
    }

    protected override int[] GetSupportedCipherSuites()
    {
        return TlsUtilities.GetSupportedCipherSuites(Crypto, DefaultCipherSuites);
    }

    public override IDictionary GetClientExtensions()
    {
        IDictionary clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
        {
            /*
             * NOTE: If you are copying test code, do not blindly set these extensions in your own client.
             */
            TlsExtensionsUtilities.AddMaxFragmentLengthExtension(clientExtensions, MaxFragmentLength.pow2_9);
        }
        return clientExtensions;
    }


    public override void NotifyServerVersion(ProtocolVersion serverVersion)
    {
        base.NotifyServerVersion(serverVersion);
    }

    public override void NotifyHandshakeComplete()
    {
        base.NotifyHandshakeComplete();

        TlsSession newSession = m_context.ResumableSession;
        if (newSession != null)
        {
            byte[] newSessionID = newSession.SessionID;
            string hex = Hex(newSessionID);

            //if (this.mSession != null && Arrays.AreEqual(this.mSession.SessionID, newSessionID))
            //{
            //    Console.WriteLine("Resumed session: " + hex);
            //}
            //else
            //{
            //    Console.WriteLine("Established session: " + hex);
            //}

            this.mSession = newSession;
        }
    }


    protected String Hex(byte[] data)
    {
        return data == null ? "(null)" : Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(data);
    }
    //protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
    //{
    //    return new TlsPskKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mPskIdentity, null, mDHVerifier, null,
    //        mNamedCurves, mClientECPointFormats, mServerECPointFormats);
    //}
}
