using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Text reader for reading all the first column of all rows and record sets from a batch data reader as a single string.
    /// </summary>
    /// <seealso cref="System.IO.TextReader" />
    internal sealed class SqlBatchDataTextReader : TextReader
    {
        private SqlBatchDataReader _reader;
        private TextReader _currReader;
        private bool _done;

        public SqlBatchDataTextReader(SqlBatchDataReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Moves on to the next row.
        /// </summary>
        /// <returns><c>true</c> if there is another row; otherwise <c>false</c></returns>
        private bool MoveNext()
        {
            do
            {
                // ReSharper disable once PossibleNullReferenceException
                if (_reader.Read())
                {
                    _currReader = _reader.GetTextReader(0);
                    return true;
                }
            } while (_reader.NextResult());

            _done = true;
            return false;
        }

        /// <summary>
        /// Moves on to the next row asynchronously.
        /// </summary>
        /// <returns>An awaitable task that returns <c>true</c> if there is another row.</returns>
        private async Task<bool> MoveNextAsync()
        {
            do
            {
                // ReSharper disable once PossibleNullReferenceException
                if (await _reader.ReadAsync().ConfigureAwait(false))
                {
                    _currReader = _reader.GetTextReader(0);
                    return true;
                }
                // ReSharper disable once PossibleNullReferenceException
            } while (await _reader.NextResultAsync().ConfigureAwait(false));

            _done = true;
            return false;
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.TextReader" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Interlocked.Exchange(ref _reader, null)?.Dispose();
        }

        /// <summary>Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the reader.</summary>
        /// <returns>An integer representing the next character to be read, or -1 if no more characters are available or the reader does not support seeking.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override int Peek()
        {
            if (_reader == null)
                throw new ObjectDisposedException(null, Resources.SqlBatchDataTextReader_Closed);

            if (_done) return -1;

            do
            {
                while (_currReader == null)
                {
                    if (!MoveNext())
                        return -1;
                }

                int ch = _currReader.Peek();
                if (ch >= 0) return ch;

                _currReader = null;
            } while (true);
        }

        /// <summary>Reads the next character from the text reader and advances the character position by one character.</summary>
        /// <returns>The next character from the text reader, or -1 if no more characters are available. The default implementation returns -1.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override int Read()
        {
            if (_reader == null)
                throw new ObjectDisposedException(null, Resources.SqlBatchDataTextReader_Closed);

            if (_done) return -1;

            do
            {
                while (_currReader == null)
                {
                    if (!MoveNext())
                        return -1;
                }

                int ch = _currReader.Read();
                if (ch >= 0) return ch;

                _currReader = null;
            } while (true);
        }

        /// <summary>Reads a specified maximum number of characters from the current reader and writes the data to a buffer, beginning at the specified index.</summary>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count" />, depending on whether the data is available within the reader. This method returns 0 (zero) if it is called when no more characters are left to read.</returns>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source. </param>
        /// <param name="index">The position in <paramref name="buffer" /> at which to begin writing. </param>
        /// <param name="count">The maximum number of characters to read. If the end of the reader is reached before the specified number of characters is read into the buffer, the method returns. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is negative. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), Resources.SqlBatchDataTextReader_Read_BufferNull);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), Resources.SqlBatchDataTextReader_Read_NonNegativeRequired);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), Resources.SqlBatchDataTextReader_Read_NonNegativeRequired);
            if (buffer.Length - index < count)
                throw new ArgumentException(Resources.SqlBatchDataTextReader_Read_OffsetLengthInvalid);
            if (_reader == null)
                throw new ObjectDisposedException(null, Resources.SqlBatchDataTextReader_Closed);

            if (_done) return 0;

            do
            {
                while (_currReader == null)
                {
                    if (!MoveNext())
                        return 0;
                }

                int read = _currReader.Read(buffer, index, count);
                if (read > 0) return read;

                _currReader = null;
            } while (true);
        }

        /// <summary>Reads a specified maximum number of characters from the current text reader asynchronously and writes the data to a buffer, beginning at the specified index. </summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the text has been reached.</returns>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in <paramref name="buffer" /> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the text is reached before the specified number of characters is read into the buffer, the current method returns.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
        /// <exception cref="T:System.ArgumentException">The sum of <paramref name="index" /> and <paramref name="count" /> is larger than the buffer length.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The text reader has been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">The reader is currently in use by a previous read operation. </exception>
        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), Resources.SqlBatchDataTextReader_Read_BufferNull);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), Resources.SqlBatchDataTextReader_Read_NonNegativeRequired);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), Resources.SqlBatchDataTextReader_Read_NonNegativeRequired);
            if (buffer.Length - index < count)
                throw new ArgumentException(Resources.SqlBatchDataTextReader_Read_OffsetLengthInvalid);
            if (_reader == null)
                throw new ObjectDisposedException(null, Resources.SqlBatchDataTextReader_Closed);

            if (_done) return TaskResult.Zero;

            return ReadAsyncImpl();

            async Task<int> ReadAsyncImpl()
            {
                do
                {
                    while (_currReader == null)
                    {
                        if (!await MoveNextAsync().ConfigureAwait(false))
                            return 0;
                    }

                    // ReSharper disable once PossibleNullReferenceException
                    int read = await _currReader.ReadAsync(buffer, index, count).ConfigureAwait(false);
                    if (read > 0) return read;

                    _currReader = null;
                } while (true);
            }
        }
    }
}