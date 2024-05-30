using System;
using System.ClientModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

internal class MultipartFormDataBinaryContent : BinaryContent
{
    private readonly MultipartFormDataContent _multipartContent;

    private static Random _random = new();
    private static readonly char[] _boundaryValues = "0123456789=ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz".ToCharArray();

    public MultipartFormDataBinaryContent()
    {
        _multipartContent = new MultipartFormDataContent(CreateBoundary());
    }

    public string ContentType
    {
        get
        {
            Debug.Assert(_multipartContent.Headers.ContentType is not null);

            return _multipartContent.Headers.ContentType!.ToString();
        }
    }

    internal HttpContent HttpContent => _multipartContent;

    public void Add(Stream content, string name, string fileName = default, string contentType = null)
    {
        Argument.AssertNotNull(content, nameof(content));
        Argument.AssertNotNullOrEmpty(name, nameof(name));

        Add(new StreamContent(content), name, fileName, contentType);
    }

    //public void Add(Stream stream, string name, string fileName = default)
    //{
    //    Add(new StreamContent(stream), name, fileName);
    //}

    public void Add(string content, string name, string fileName = default)
    {
        Add(new StringContent(content), name, fileName);
    }

    public void Add(int content, string name, string fileName = default)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#GFormatString
        string value = content.ToString("G", CultureInfo.InvariantCulture);
        Add(new StringContent(value), name, fileName);
    }

    public void Add(double content, string name, string fileName = default)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#GFormatString
        string value = content.ToString("G", CultureInfo.InvariantCulture);
        Add(new StringContent(value), name, fileName);
    }

    public void Add(byte[] content, string name, string fileName = default)
    {
        Add(new ByteArrayContent(content), name, fileName);
    }

    public void Add(BinaryData content, string name, string fileName = default)
    {
        Add(new ByteArrayContent(content.ToArray()), name, fileName);
    }

    private void Add(HttpContent content, string name, string filename, string contentType)
    {
        if (filename != null)
        {
            Argument.AssertNotNullOrEmpty(filename, nameof(filename));
            AddFileNameHeader(content, name, filename);
        }
        if (contentType != null)
        {
            Argument.AssertNotNullOrEmpty(contentType, nameof(contentType));
            AddContentTypeHeader(content, contentType);
        }
        _multipartContent.Add(content, name);
    }

    private void Add(HttpContent content, string name, string fileName)
    {
        if (fileName is not null)
        {
            AddFileNameHeader(content, name, fileName);
        }

        _multipartContent.Add(content, name);
    }

    private static void AddFileNameHeader(HttpContent content, string name, string filename)
    {
        // Add the content header manually because the default implementation
        // adds a `filename*` parameter to the header, which RFC 7578 says not
        // to do.  We are following up with the BCL team per correctness.
        ContentDispositionHeaderValue header = new("form-data")
        {
            Name = name,
            FileName = filename
        };
        content.Headers.ContentDisposition = header;
    }

    public static void AddContentTypeHeader(HttpContent content, string contentType)
    {
        MediaTypeHeaderValue header = new MediaTypeHeaderValue(contentType);
        content.Headers.ContentType = header;
    }

    private static string CreateBoundary()
    {
        Span<char> chars = new char[70];

        byte[] random = new byte[70];
        _random.NextBytes(random);

        // The following will sample evenly from the possible values.
        // This is important to ensuring that the odds of creating a boundary
        // that occurs in any content part are astronomically small.
        int mask = 255 >> 2;

        Debug.Assert(_boundaryValues.Length - 1 == mask);

        for (int i = 0; i < 70; i++)
        {
            chars[i] = _boundaryValues[random[i] & mask];
        }

        return chars.ToString();
    }

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
        // TODO: polyfill sync-over-async for netstandard2.0 for Azure clients.
        // Tracked by https://github.com/Azure/azure-sdk-for-net/issues/42674

#if NET6_0_OR_GREATER
        _multipartContent.CopyTo(stream, default, cancellationToken);
#else
    _multipartContent.CopyToAsync(stream).GetAwaiter().GetResult();
#endif
    }

    public override async Task WriteToAsync(Stream stream, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
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
