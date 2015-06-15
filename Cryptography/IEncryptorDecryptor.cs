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

using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Interface to an object that can encrypt and decrypt a string.
    /// </summary>
    public interface IEncryptorDecryptor
    {
        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ContractAnnotation("null=>null;notnull=>notnull")]
        string Encrypt([CanBeNull] string input);

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="isLatestKey">if set to <see langword="true" /> the input was encrypted using the latest key.</param>
        /// <returns></returns>
        [ContractAnnotation("input:null=>null;input:notnull=>notnull")]
        string Decrypt([CanBeNull] string input, out bool isLatestKey);

        /// <summary>
        /// Attempts to decrypt the input given.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="result">The result.</param>
        /// <param name="isLatestKey">If set to <see langword="true" /> the input was encrypted using the latest key. Will be <see langword="null" /> if the operation failed.</param>
        /// <returns>
        ///   <see langword="true" /> if successful; otherwise <see langword="false" />.
        /// </returns>
        bool TryDecrypt([CanBeNull] string input, [CanBeNull] out string result, [CanBeNull] out bool? isLatestKey);
    }
}