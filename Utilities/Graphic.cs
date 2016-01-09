#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Drawing;
using System.IO;
using System.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds image data and supports out typing of images.  Prevents unnecessary loading of images when passing data
    /// around, and makes much safer use of streams.
    /// </summary>
    [PublicAPI]
    public class Graphic : IEquatable<Graphic>
    {
        /// <summary>
        /// The data.
        /// </summary>
        [NotNull]
        public readonly HashedByteArray Data;

        /// <summary>
        /// The format of the image.
        /// </summary>
        public readonly GraphicFormat Format;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        public Graphic([NotNull] string encodedData)
        {
            byte[] data = Convert.FromBase64String(encodedData);
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="data">The data.</param>
        public Graphic([NotNull] byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic" /> class from a system image.
        /// </summary>
        /// <param name="image">The image.</param>
        public Graphic([NotNull] Image image)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                data = ms.GetBuffer();
            }
            Format = data.GetGraphicFormat();
            Data = data;
        }

        /// <summary>
        /// Gets the image as a Base 64 encoded string.
        /// </summary>
        /// <value>The encoded.</value>
        [NotNull]
        public string Encoded
        {
            get { return Data.Encoded; }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public bool Equals(Graphic other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Format, other.Format) &&
                   Equals(Data, other.Data);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Graphic);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Data.GetHashCode() * 397) ^ (int)Format;
            }
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
        /// Gets the data URL for the graphic.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public string ToDataUrl()
        {
            StringBuilder builder = new StringBuilder("data:", Encoded.Length + 30);
            switch (Format)
            {
                case GraphicFormat.Bmp:
                    builder.Append("image/bmp");
                    break;
                case GraphicFormat.Emf:
                    builder.Append("image/x-emf");
                    break;
                case GraphicFormat.Wmf:
                    builder.Append("image/x-wmf");
                    break;
                case GraphicFormat.Gif:
                    builder.Append("image/gif");
                    break;
                case GraphicFormat.Jpeg:
                    builder.Append("image/jpeg");
                    break;
                case GraphicFormat.Png:
                    builder.Append("image/png");
                    break;
                case GraphicFormat.Tiff:
                    builder.Append("image/tiff");
                    break;
                case GraphicFormat.Icon:
                    builder.Append("image/x-icon");
                    break;
                default:
                    return null;
            }
            builder.Append(";base64,").Append(Encoded);

            return builder.ToString();
        }

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