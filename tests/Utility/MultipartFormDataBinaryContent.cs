using System;
using System.ClientModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests;

internal class MultipartFormDataBinaryContent : BinaryContent
{
    private readonly MultipartFormDataContent _multipartContent;

    private const int BoundaryLength = 70;
    private const string BoundaryValues = "0123456789=ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

    public MultipartFormDataBinaryContent()
    {
        _multipartContent = new MultipartFormDataContent(CreateBoundary());
    }

    public string ContentType => _multipartContent.Headers.ContentType.ToString();

    public void Add(byte[] bytes, string name, string fileName, string contentType = null)
    {
        ByteArrayContent content = new(bytes);
        if (contentType is not null)
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        }
        _multipartContent.Add(content, name, fileName);
    }

#if NET6_0_OR_GREATER
    private static string CreateBoundary() =>
        string.Create(BoundaryLength, 0, (chars, _) =>
        {
            Span<byte> random = stackalloc byte[BoundaryLength];
            Random.Shared.NextBytes(random);

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = BoundaryValues[random[i] % BoundaryValues.Length];
            }
        });
#else
    private static readonly Random _random = new();

    private static string CreateBoundary()
    {
        Span<char> chars = stackalloc char[BoundaryLength];

        byte[] random = new byte[BoundaryLength];
        lock (_random)
        {
            _random.NextBytes(random);
        }

        // Instead of `% BoundaryValues.Length` as is used above, use a mask to achieve the same result.
        // `% BoundaryValues.Length` is optimized to the equivalent on .NET Core but not on .NET Framework.
        const int Mask = 255 >> 2;
        Debug.Assert(BoundaryValues.Length - 1 == Mask);

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = BoundaryValues[random[i] & Mask];
        }

        return chars.ToString();
    }
#endif

    public override bool TryComputeLength(out long length)
    {
        // We can't call the protected method on HttpContent

        if (_multipartContent.Headers.ContentLength is long contentLength)
        {
            length = contentLength;
            return true;
        }

        length = 0;
        return false;
    }

    public override void WriteTo(Stream stream, CancellationToken cancellationToken = default)
    {
#if NET5_0_OR_GREATER
        _multipartContent.CopyTo(stream, default, cancellationToken);
#else
        // TODO: polyfill sync-over-async for netstandard2.0 for Azure clients.
        // Tracked by https://github.com/Azure/azure-sdk-for-net/issues/42674
        _multipartContent.CopyToAsync(stream).GetAwaiter().GetResult();
#endif
    }

    public override async Task WriteToAsync(Stream stream, CancellationToken cancellationToken = default)
    {
#if NET5_0_OR_GREATER
        await _multipartContent.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
#else
        await _multipartContent.CopyToAsync(stream).ConfigureAwait(false);
#endif
    }

    public override void Dispose()
    {
        _multipartContent.Dispose();
    }
}
