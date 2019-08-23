using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace REDTransport.NET.Collections
{
    public class AsyncEnumerableAdapter<TInput, TOutput> : IAsyncEnumerable<TOutput>
    {
        public IAsyncEnumerable<TInput> Enumerable { get; }

        public Func<TInput, ValueTask<TOutput>> Adapter { get; }

        
        public AsyncEnumerableAdapter(IAsyncEnumerable<TInput> enumerable, Func<TInput, ValueTask<TOutput>> adapter)
        {
            Enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
            Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }
        

        public IAsyncEnumerator<TOutput> GetAsyncEnumerator(
            CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerableAdapterEnumerator(
                Enumerable.GetAsyncEnumerator(cancellationToken),
                Adapter,
                cancellationToken
            );
        }

        public class AsyncEnumerableAdapterEnumerator : IAsyncEnumerator<TOutput>
        {
            public IAsyncEnumerator<TInput> Enumerator { get; }
            
            public Func<TInput, ValueTask<TOutput>> Adapter { get; }

            public CancellationToken CancellationToken { get; }


            private TOutput _current;
            
            
            public AsyncEnumerableAdapterEnumerator(
                IAsyncEnumerator<TInput> enumerator,
                Func<TInput, ValueTask<TOutput>> adapter,
                CancellationToken cancellationToken
            )
            {
                Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
                Adapter = adapter ?? throw new ArgumentNullException(nameof(Adapter));
                CancellationToken = cancellationToken;
            }


            public ValueTask DisposeAsync()
            {
                return Enumerator.DisposeAsync();
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                if (await Enumerator.MoveNextAsync())
                {
                    _current = await Adapter(Enumerator.Current);

                    return true;
                }

                return false;
            }

            public TOutput Current => _current;
        }
    }
}