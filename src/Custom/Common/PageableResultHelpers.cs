using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

//internal class PageableResultHelpers
//{
//    public static PageCollection<T> Create<T>(Func<int?, ClientPage<T>> firstPageFunc, Func<string?, int?, ClientPage<T>>? nextPageFunc, int? pageSize = default) where T : notnull
//    {
//        ClientPage<T> first(string? _, int? pageSizeHint) => firstPageFunc(pageSizeHint);
//        return new FuncPageable<T>(first, nextPageFunc, pageSize);
//    }

//    public static AsyncPageCollection<T> Create<T>(Func<int?, Task<ClientPage<T>>> firstPageFunc, Func<string?, int?, Task<ClientPage<T>>>? nextPageFunc, int? pageSize = default) where T : notnull
//    {
//        Task<ClientPage<T>> first(string? _, int? pageSizeHint) => firstPageFunc(pageSizeHint);
//        return new FuncAsyncPageable<T>(first, nextPageFunc, pageSize);
//    }

//    private class FuncAsyncPageable<T> : AsyncPageCollection<T> where T : notnull
//    {
//        private readonly Func<string?, int?, Task<ClientPage<T>>> _firstPageFunc;
//        private readonly Func<string?, int?, Task<ClientPage<T>>>? _nextPageFunc;
//        private readonly int? _defaultPageSize;

//        public FuncAsyncPageable(Func<string?, int?, Task<ClientPage<T>>> firstPageFunc, Func<string?, int?, Task<ClientPage<T>>>? nextPageFunc, int? defaultPageSize = default)
//        {
//            _firstPageFunc = firstPageFunc;
//            _nextPageFunc = nextPageFunc;
//            _defaultPageSize = defaultPageSize;
//        }

//        public override async IAsyncEnumerable<ClientPage<T>> AsPages(string? continuationToken = default, int? pageSizeHint = default)
//        {
//            Func<string?, int?, Task<ClientPage<T>>>? pageFunc = string.IsNullOrEmpty(continuationToken) ? _firstPageFunc : _nextPageFunc;

//            if (pageFunc == null)
//            {
//                yield break;
//            }

//            int? pageSize = pageSizeHint ?? _defaultPageSize;
//            do
//            {
//                ClientPage<T> page = await pageFunc(continuationToken, pageSize).ConfigureAwait(false);
//                SetRawResponse(page.GetRawResponse());
//                yield return page;
//                continuationToken = page.ContinuationToken;
//                pageFunc = _nextPageFunc;
//            }
//            while (!string.IsNullOrEmpty(continuationToken) && pageFunc != null);
//        }
//    }

//    private class FuncPageable<T> : PageCollection<T> where T : notnull
//    {
//        private readonly Func<string?, int?, ClientPage<T>> _firstPageFunc;
//        private readonly Func<string?, int?, ClientPage<T>>? _nextPageFunc;
//        private readonly int? _defaultPageSize;

//        public FuncPageable(Func<string?, int?, ClientPage<T>> firstPageFunc, Func<string?, int?, ClientPage<T>>? nextPageFunc, int? defaultPageSize = default)
//        {
//            _firstPageFunc = firstPageFunc;
//            _nextPageFunc = nextPageFunc;
//            _defaultPageSize = defaultPageSize;
//        }

//        public override IEnumerable<ClientPage<T>> AsPages(string? continuationToken = default, int? pageSizeHint = default)
//        {
//            Func<string?, int?, ClientPage<T>>? pageFunc = string.IsNullOrEmpty(continuationToken) ? _firstPageFunc : _nextPageFunc;

//            if (pageFunc == null)
//            {
//                yield break;
//            }

//            int? pageSize = pageSizeHint ?? _defaultPageSize;
//            do
//            {
//                ClientPage<T> page = pageFunc(continuationToken, pageSize);
//                SetRawResponse(page.GetRawResponse());
//                yield return page;
//                continuationToken = page.ContinuationToken;
//                pageFunc = _nextPageFunc;
//            }
//            while (!string.IsNullOrEmpty(continuationToken) && pageFunc != null);
//        }
//    }
//}
