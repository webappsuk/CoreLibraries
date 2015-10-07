using System;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.IO
{
    /// <summary>
    /// Wraps a <see cref="Stream"/> allowing an <see cref="Action"/> to be called when the <see cref="Stream"/> is <see cref="Stream.Close">closed</see>.
    /// </summary>
    public class CloseableStream : Stream
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        [NotNull]
        private readonly Stream _stream;

        /// <summary>
        /// The action to call when the stream is disposed.
        /// </summary>
        [CanBeNull]
        private Action _onClose;

        /// <summary>
        /// The action to call when the stream is disposed.
        /// </summary>
        [CanBeNull]
        private readonly Func<Exception, bool> _onError;

        /// <summary>
        /// Create a stream that will invoke the specified <see paramref="onClose"/> action when the <see paramref="stream"/> is closed/disposed.
        /// </summary>
        /// <param name="stream">The stream to wrap</param>
        /// <param name="onClose">The action to call after the specified <see paramref="stream"/> is closed.</param>
        /// <param name="onError">The optional function to call if the underlying stream errors on close (<see paramref="onClose" /> will still be called after
        /// <see paramref="onError"/>), return <see langword="true"/> to suppress the exception.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="onClose"/> is <see langword="null" />.</exception>
        [PublicAPI]
        public CloseableStream(
            [NotNull] Stream stream,
            [NotNull] Action onClose,
            Func<Exception, bool> onError = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (onClose == null) throw new ArgumentNullException(nameof(onClose));
            _stream = stream;
            _onClose = onClose;
            _onError = onError;
        }
        
        /// <inheritdoc />
        public override void Close()
        {
            // Ensure we only close once
            Action onClose = Interlocked.Exchange(ref _onClose, null);
            if (onClose == null) return;

            try
            {
                _stream.Close();
                base.Close();
            }
            catch (Exception e)
            {
                Func<Exception, bool> onError = _onError;
                if (onError != null)
                {
                    if (!onError(e))
                        throw;
                }
                else throw;
            }
            finally
            {
                onClose();
            }
        }

        /// <inheritdoc />
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            => _stream.CopyToAsync(destination, bufferSize, cancellationToken);

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        /// <inheritdoc />
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => _stream.BeginRead(buffer, offset, count, callback, state);

        /// <inheritdoc />
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => _stream.BeginWrite(buffer, offset, count, callback, state);

        /// <inheritdoc />
        public override ObjRef CreateObjRef(Type requestedType) => _stream.CreateObjRef(requestedType);

        /// <inheritdoc />
        public override int EndRead(IAsyncResult asyncResult) => _stream.EndRead(asyncResult);

        /// <inheritdoc />
        public override void EndWrite(IAsyncResult asyncResult) => _stream.EndWrite(asyncResult);

        /// <inheritdoc />
        public override bool Equals(object obj) => _stream.Equals(obj);

        /// <inheritdoc />
        public override void Flush() => _stream.Flush();

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken) => _stream.FlushAsync(cancellationToken);

        /// <inheritdoc />
        public override int GetHashCode() => _stream.GetHashCode();

        /// <inheritdoc />
        public override object InitializeLifetimeService() => _stream.InitializeLifetimeService();

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _stream.ReadAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc />
        public override int ReadByte() => _stream.ReadByte();

        /// <inheritdoc />
        public override int ReadTimeout
        {
            get { return _stream.ReadTimeout; }
            set { _stream.ReadTimeout = value; }
        }

        /// <inheritdoc />
        public override string ToString() => _stream.ToString();

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _stream.WriteAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc />
        public override void WriteByte(byte value) => _stream.WriteByte(value);

        /// <inheritdoc />
        public override int WriteTimeout
        {
            get { return _stream.WriteTimeout; }
            set { _stream.WriteTimeout = value; }
        }

        /// <inheritdoc />
        public override bool CanTimeout => _stream.CanTimeout;

        /// <inheritdoc />
        public override void SetLength(long value) => _stream.SetLength(value);

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);

        /// <inheritdoc />
        public override bool CanRead => _stream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _stream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => _stream.CanWrite;

        /// <inheritdoc />
        public override long Length => _stream.Length;

        /// <inheritdoc />
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
    }
}
