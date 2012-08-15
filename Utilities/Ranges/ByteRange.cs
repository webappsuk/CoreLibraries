namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="byte"/>s.
    /// </summary>
    public class ByteRange : Range<byte>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step.</param>
        public ByteRange(byte start, byte end, byte step = (byte) 1) 
            : base(start, end, step)
        { }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} - {1}", Start, End);
        }
    }
}
