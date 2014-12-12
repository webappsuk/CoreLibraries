#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Drawing.Imaging;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for Graphics.
    /// </summary>
    public static class GraphicExtensions
    {
        /// <summary>
        /// Maps underlying image format GUIDs to their enumeration equivalent.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<Guid, GraphicFormat> _graphicFormatsByGuid = new Dictionary
            <Guid, GraphicFormat>
        {
            // Memory Bitmap
            {new Guid("b96b3caa-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Bmp},
            // Normal Bitmap
            {new Guid("b96b3cab-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Bmp},
            {new Guid("b96b3cac-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Emf},
            {new Guid("b96b3cad-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Wmf},
            {new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Gif},
            {new Guid("b96b3cae-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Jpeg},
            {new Guid("b96b3caf-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Png},
            {new Guid("b96b3cb1-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Tiff},
            //{new Guid("b96b3cb2-0728-11d3-9d7b-0000f81ef32e"), ImageFormat.Exif},
            {new Guid("b96b3cb5-0728-11d3-9d7b-0000f81ef32e"), GraphicFormat.Icon}
        };

        /// <summary>
        /// Maps the image format enumeration to the system equivalent.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<GraphicFormat, ImageFormat> _graphicFormats =
            new Dictionary<GraphicFormat, ImageFormat>
            {
                {GraphicFormat.Bmp, ImageFormat.Bmp},
                {GraphicFormat.Emf, ImageFormat.Emf},
                {GraphicFormat.Wmf, ImageFormat.Wmf},
                {GraphicFormat.Gif, ImageFormat.Gif},
                {GraphicFormat.Jpeg, ImageFormat.Jpeg},
                {GraphicFormat.Png, ImageFormat.Png},
                {GraphicFormat.Tiff, ImageFormat.Tiff},
                //{ImageFormat.Exif, ImageFormat.Exif},
                {GraphicFormat.Icon, ImageFormat.Icon}
            };

        /// <summary>
        /// Converts a system image format to the Traveller enumeration equivalent.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>ImageFormat.</returns>
        public static GraphicFormat ToGraphicFormat([NotNull] this ImageFormat format)
        {
            GraphicFormat graphicFormat;
            Guid guid = format.Guid;
            if (!_graphicFormatsByGuid.TryGetValue(guid, out graphicFormat))
                throw new ArgumentOutOfRangeException("format");
            return graphicFormat;
        }

        /// <summary>
        /// Get the system image format from the Traveller enumeration.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>System.Drawing.Imaging.ImageFormat.</returns>
        [NotNull]
        public static ImageFormat ToImageFormat(this GraphicFormat format)
        {
            return _graphicFormats[format];
        }

        /// <summary>
        /// Gets the image format from the header of the image data.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>ImageFormat.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <remarks>
        /// See (amongst others) http://www.garykessler.net/library/file_sigs.html
        /// </remarks>
        public static GraphicFormat GetGraphicFormat([NotNull] this byte[] bytes)
        {
            int length = bytes.Length;
            if (length > 2)
                switch (bytes[0])
                {
                    case 0x00:
                        if ((length > 4) &&
                            (bytes[1] == 0x00) &&
                            (bytes[2] == 0x01) &&
                            (bytes[3] == 0x00))
                            return GraphicFormat.Icon;
                        break;
                    case 0x01:
                        if ((length > 4) &&
                            (bytes[1] == 0x00) &&
                            (bytes[3] == 0x00))
                        {
                            if ((length > 6) &&
                                (bytes[2] == 0x09) &&
                                (bytes[5] == 0x03))
                                return GraphicFormat.Wmf;
                            if (bytes[2] == 0x00)
                                return GraphicFormat.Emf;
                        }
                        break;
                    case 0x42:
                        if ((bytes[1] == 0x4D))
                            return GraphicFormat.Bmp;
                        break;
                    case 0x47:
                        if ((length > 3) &&
                            (bytes[1] == 0x49) &&
                            (bytes[2] == 0x46))
                            return GraphicFormat.Gif;
                        break;
                    case 0x49:
                        if ((length > 4) &&
                            (bytes[1] == 0x49) &&
                            (bytes[2] == 0x2A) &&
                            (bytes[3] == 0x00))
                            return GraphicFormat.Tiff;
                        if ((length > 3) &&
                            (bytes[1] == 0x20) &&
                            (bytes[2] == 0x49))
                            return GraphicFormat.Tiff;
                        break;
                    case 0x4D:
                        if ((length > 4) &&
                            (bytes[1] == 0x4D) &&
                            (bytes[2] == 0x00) &&
                            ((bytes[3] == 0x2A) || (bytes[3] == 0x2B)))
                            return GraphicFormat.Tiff;
                        break;
                    case 0x89:
                        if ((length > 4) &&
                            (bytes[1] == 0x50) &&
                            (bytes[2] == 0x4E) &&
                            (bytes[3] == 0x47))
                            return GraphicFormat.Png;
                        break;
                    case 0xFF:
                        if ((length > 4) &&
                            (bytes[1] == 0xD8) &&
                            (bytes[2] == 0xFF) &&
                            ((bytes[3] >= 0xE0) && (bytes[3] <= 0xEF)))
                            return GraphicFormat.Jpeg;
                        break;
                }
            throw new ArgumentOutOfRangeException("bytes");
        }
    }
}