using System;
using System.Formats.Asn1;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace OpenAI.Tests.MutualTls;

internal sealed class CertificateFixture : IDisposable
{
    private const string AuthorityKeyIdentifierOid = "2.5.29.35";
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";
    private const string ServerAuthenticationOid = "1.3.6.1.5.5.7.3.1";

    public X509Certificate2 RootCertificate { get; }
    public X509Certificate2 IntermediateCertificate { get; }
    public X509Certificate2 ClientCertificate { get; }
    public X509Certificate2 ServerCertificate { get; }

    private CertificateFixture(
        X509Certificate2 rootCertificate,
        X509Certificate2 intermediateCertificate,
        X509Certificate2 clientCertificate,
        X509Certificate2 serverCertificate)
    {
        RootCertificate = rootCertificate;
        IntermediateCertificate = intermediateCertificate;
        ClientCertificate = clientCertificate;
        ServerCertificate = serverCertificate;
    }

    public static CertificateFixture Create()
    {
        DateTimeOffset notBefore = DateTimeOffset.UtcNow.AddMinutes(-5);
        DateTimeOffset notAfter = DateTimeOffset.UtcNow.AddDays(7);
        string certificateSetId = Guid.NewGuid().ToString("N");

        using RSA rootKey = RSA.Create(2048);
        CertificateRequest rootRequest = CreateCertificateRequest(
            $"CN=OpenAI mTLS Test Root {certificateSetId}",
            rootKey);
        rootRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: true,
                hasPathLengthConstraint: true,
                pathLengthConstraint: 1,
                critical: true));
        rootRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                critical: true));
        X509SubjectKeyIdentifierExtension rootSubjectKeyIdentifier =
            AddSubjectKeyIdentifier(rootRequest);
        AddAuthorityKeyIdentifier(rootRequest, rootSubjectKeyIdentifier);
        X509Certificate2 rootCertificate =
            rootRequest.CreateSelfSigned(notBefore, notAfter);

        using RSA intermediateKey = RSA.Create(2048);
        CertificateRequest intermediateRequest = CreateCertificateRequest(
            $"CN=OpenAI mTLS Test Intermediate {certificateSetId}",
            intermediateKey);
        intermediateRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: true,
                hasPathLengthConstraint: true,
                pathLengthConstraint: 0,
                critical: true));
        intermediateRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                critical: true));
        X509SubjectKeyIdentifierExtension intermediateSubjectKeyIdentifier =
            AddSubjectKeyIdentifier(intermediateRequest);
        AddAuthorityKeyIdentifier(
            intermediateRequest,
            rootSubjectKeyIdentifier);
        X509Certificate2 intermediateCertificate =
            intermediateRequest.Create(
                rootCertificate,
                notBefore,
                notAfter,
                CreateSerialNumber());

        using RSA clientKey = RSA.Create(2048);
        CertificateRequest clientRequest = CreateCertificateRequest(
            $"CN=OpenAI mTLS Test Client {certificateSetId}",
            clientKey);
        SubjectAlternativeNameBuilder clientSubjectAlternativeNames = new();
        clientSubjectAlternativeNames.AddDnsName(
            $"client-{certificateSetId}.openai.test");
        AddEndEntityExtensions(
            clientRequest,
            ClientAuthenticationOid,
            intermediateSubjectKeyIdentifier,
            clientSubjectAlternativeNames.Build());
        using X509Certificate2 clientPublicCertificate =
            clientRequest.Create(
                intermediateCertificate.SubjectName,
                X509SignatureGenerator.CreateForRSA(
                    intermediateKey,
                    RSASignaturePadding.Pkcs1),
                notBefore,
                notAfter,
                CreateSerialNumber());
        using X509Certificate2 ephemeralClientCertificate =
            clientPublicCertificate.CopyWithPrivateKey(clientKey);
        X509Certificate2 clientCertificate =
            LoadPkcs12(ephemeralClientCertificate.Export(X509ContentType.Pkcs12));

        using RSA serverKey = RSA.Create(2048);
        CertificateRequest serverRequest = CreateCertificateRequest(
            "CN=127.0.0.1",
            serverKey);
        SubjectAlternativeNameBuilder subjectAlternativeNames = new();
        subjectAlternativeNames.AddIpAddress(IPAddress.Loopback);
        subjectAlternativeNames.AddDnsName("localhost");
        AddEndEntityExtensions(
            serverRequest,
            ServerAuthenticationOid,
            rootSubjectKeyIdentifier,
            subjectAlternativeNames.Build());
        using X509Certificate2 serverPublicCertificate =
            serverRequest.Create(
                rootCertificate,
                notBefore,
                notAfter,
                CreateSerialNumber());
        using X509Certificate2 ephemeralServerCertificate =
            serverPublicCertificate.CopyWithPrivateKey(serverKey);
        X509Certificate2 serverCertificate =
            LoadPkcs12(ephemeralServerCertificate.Export(X509ContentType.Pkcs12));

        return new CertificateFixture(
            rootCertificate,
            intermediateCertificate,
            clientCertificate,
            serverCertificate);
    }

    public byte[] ExportClientBundle(string password)
    {
        X509Certificate2Collection certificates =
            new(
                new X509Certificate2[]
                {
                    ClientCertificate,
                    IntermediateCertificate,
                });
        return certificates.Export(X509ContentType.Pkcs12, password);
    }

    public void Dispose()
    {
        ServerCertificate.Dispose();
        ClientCertificate.Dispose();
        IntermediateCertificate.Dispose();
        RootCertificate.Dispose();
    }

    private static CertificateRequest CreateCertificateRequest(
        string subjectName,
        RSA key)
    {
        return new CertificateRequest(
            new X500DistinguishedName(subjectName),
            key,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
    }

    private static X509SubjectKeyIdentifierExtension AddSubjectKeyIdentifier(
        CertificateRequest request)
    {
        X509SubjectKeyIdentifierExtension extension =
            new(request.PublicKey, critical: false);
        request.CertificateExtensions.Add(extension);
        return extension;
    }

    private static void AddAuthorityKeyIdentifier(
        CertificateRequest request,
        X509SubjectKeyIdentifierExtension issuerSubjectKeyIdentifier)
    {
        byte[] keyIdentifier =
            Convert.FromHexString(
                issuerSubjectKeyIdentifier.SubjectKeyIdentifier);
        AsnWriter writer = new(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.WriteOctetString(
            keyIdentifier,
            new Asn1Tag(TagClass.ContextSpecific, 0));
        writer.PopSequence();
        request.CertificateExtensions.Add(
            new X509Extension(
                AuthorityKeyIdentifierOid,
                writer.Encode(),
                critical: false));
    }

    private static void AddEndEntityExtensions(
        CertificateRequest request,
        string enhancedKeyUsageOid,
        X509SubjectKeyIdentifierExtension issuerSubjectKeyIdentifier,
        X509Extension subjectAlternativeName)
    {
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: false,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0,
                critical: true));
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature
                    | X509KeyUsageFlags.KeyEncipherment,
                critical: true));
        OidCollection enhancedKeyUsages = new();
        enhancedKeyUsages.Add(new Oid(enhancedKeyUsageOid));
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(enhancedKeyUsages, critical: true));
        AddSubjectKeyIdentifier(request);
        AddAuthorityKeyIdentifier(request, issuerSubjectKeyIdentifier);
        request.CertificateExtensions.Add(subjectAlternativeName);
    }

    private static byte[] CreateSerialNumber()
    {
        byte[] serialNumber = RandomNumberGenerator.GetBytes(16);
        serialNumber[0] &= 0x7F;
        serialNumber[^1] |= 0x01;
        return serialNumber;
    }

    private static X509Certificate2 LoadPkcs12(byte[] pfx)
    {
        // Schannel cannot use the ephemeral key from CopyWithPrivateKey. Reloading
        // creates a provider-backed key that is removed when the certificate is disposed.
#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12(
            pfx,
            password: null,
            X509KeyStorageFlags.DefaultKeySet | X509KeyStorageFlags.Exportable);
#else
#pragma warning disable SYSLIB0057
        return new X509Certificate2(
            pfx,
            password: (string)null,
            X509KeyStorageFlags.DefaultKeySet | X509KeyStorageFlags.Exportable);
#pragma warning restore SYSLIB0057
#endif
    }
}
