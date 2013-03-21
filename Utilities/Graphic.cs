using System;
using System.Drawing;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds image data and supports out typing of images.  Prevents unnecessary loading of images when passing data
    /// around, and makes much safer use of streams.
    /// </summary>
    public class Graphic : IEquatable<Graphic>
    {
        /// <summary>
        /// The format of the image.
        /// </summary>
        public readonly GraphicFormat Format;

        /// <summary>
        /// The data.
        /// </summary>
        [NotNull]
        public readonly HashedByteArray Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        public Graphic([NotNull]string encodedData)
        {
            byte[] data = Convert.FromBase64String(encodedData);
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="data">The data.</param>
        public Graphic([NotNull]byte[] data)
        {
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="image">The image.</param>
        public Graphic([NotNull]Image image)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                data = ms.ToArray();
            }
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public bool Equals(Graphic other)
        {
            return !ReferenceEquals(other, null) &&
                   Equals(Format, other.Format) &&
                   Equals(Data, other.Data);
        }

        /// <summary>
        /// Gets a <see cref="System.Drawing.Image"/>.
        /// </summary>
        /// <returns>System.Drawing.Image.</returns>
        [NotNull]
        public Image GetImage()
        {
            // Note the stream should not be wrapped in a using statement as Image
            // requires the stream to remain open for it's lifetime.
            // Although this is unpleasant MemoryStreams don't really require disposal
            // so it is still safe.
            //
            // The resulting Image should always be disposed if possible.
            //
            // This 'feature' of the built in Image class is one of the main reasons we
            // have this custom class.  Also the act of creating an Image is expensive
            // (goes into the COM GDI+ layer), and unecessary unless we're rendering to
            // screen.  Often we're just pushing the bytes around.
            MemoryStream ms = new MemoryStream(Data);
            return Image.FromStream(ms);
        }

        /// <summary>
        /// Gets the image as a Base 64 encoded string.
        /// </summary>
        /// <value>The encoded.</value>
        public string Encoded { get { return Data.Encoded; } }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Format.ToString();
        }
    }
}
