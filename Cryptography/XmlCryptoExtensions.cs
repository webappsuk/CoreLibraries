using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Extensions for Xml cryptography.
    /// </summary>
    public static class XmlCryptoExtensions
    {
        /// <summary>
        /// Decrypts a <see cref="XNode"/>.
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <param name="decryptedXml">The decrypted XML.</param>
        /// <param name="isLatestKey">The latest key.</param>
        /// <returns>
        /// The upmost <see cref="XElement"/> that was decrypted.
        /// </returns>
        [UsedImplicitly]
        public static bool TryDecrypt([NotNull] this XNode inputNode, IEncryptorDecryptor encryptorDecryptor, out XElement decryptedXml, out bool? isLatestKey)
        {
            decryptedXml = null;
            isLatestKey = null;
            try
            {
                bool l;
                decryptedXml = Decrypt(inputNode, encryptorDecryptor, out l);
                isLatestKey = l;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Decrypts a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <param name="decryptedXml">The decrypted XML.</param>
        /// <param name="isLatestKey">The is latest key.</param>
        /// <returns>
        /// The upmost <see cref="XmlElement"/> that was decrypted.
        /// </returns>
        [UsedImplicitly]
        public static bool TryDecrypt([NotNull] XmlNode inputNode, IEncryptorDecryptor encryptorDecryptor, out XmlElement decryptedXml, out bool? isLatestKey)
        {
            decryptedXml = null;
            isLatestKey = null;
            try
            {
                bool l;
                decryptedXml = Decrypt(inputNode, encryptorDecryptor, out l);
                isLatestKey = l;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Decrypts a <see cref="XNode"/>.
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <param name="isLatestKey">if set to <see langword="true"/> the latest key was always used.</param>
        /// <returns>
        /// The upmost <see cref="XElement"/> that was decrypted.
        /// </returns>
        [UsedImplicitly]
        [CanBeNull]
        public static XElement Decrypt(this XNode inputNode, IEncryptorDecryptor encryptorDecryptor, out bool isLatestKey)
        {
            XElement element;
            XDocument ownerDocument;
            isLatestKey = true;
            switch (inputNode.NodeType)
            {
                case XmlNodeType.Element:
                    element = inputNode as XElement;
                    ownerDocument = inputNode.Document;
                    if (ownerDocument == null)
                        return null;
                    break;
                case XmlNodeType.Document:
                    ownerDocument = inputNode as XDocument;
                    if (ownerDocument == null)
                        return null;
                    element = ownerDocument.Root;
                    break;

                /* 
             * TODO WE COULD SUPPORT THE FOLLOWING TYPES
            case XmlNodeType.DocumentFragment:
            case XmlNodeType.Attribute:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Comment:
             */
                default:
                    throw new ArgumentOutOfRangeException("inputNode",
                                                          string.Format(Resources.Cryptographer_Decrypt_CannotDecryptNode,
                                                                        inputNode.NodeType));
            }

            // If we don't have an element and an owner document nothing to do!
            if (element == null)
                return null;

            Stack<XElement> encryptedElements = new Stack<XElement>();
            encryptedElements.Push(element);
            while (encryptedElements.Count > 0)
            {
                element = encryptedElements.Pop();

                // This will always be true, unless an unencrypted element is passed in initially.
                if (element.Name == "Encrypted")
                {
                    // Decrypt the element
                    bool l;
                    string decryptedXmlStr = encryptorDecryptor.Decrypt(element.Value, out l);
                    if (string.IsNullOrEmpty(decryptedXmlStr))
                        continue;

                    // Update output flags.
                    isLatestKey &= l;

                    XElement decryptedElement = XElement.Parse(decryptedXmlStr);
                    element.ReplaceWith(decryptedElement);
                    element = decryptedElement;
                }

                // Grab encrypted elements of this node.
                foreach (XElement ee in element.Descendants("Encrypted"))
                    encryptedElements.Push(ee);
            }
            return element;
        }

        /// <summary>
        /// Decrypts a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <param name="isLatestKey">if set to <see langword="true"/> the latest key was always used.</param>
        /// <returns>
        /// The upmost <see cref="XmlElement"/> that was decrypted.
        /// </returns>
        [UsedImplicitly]
        [CanBeNull]
        public static XmlElement Decrypt([NotNull] this XmlNode inputNode, IEncryptorDecryptor encryptorDecryptor, out bool isLatestKey)
        {
            XmlElement element;
            XmlDocument ownerDocument;
            isLatestKey = true;
            switch (inputNode.NodeType)
            {
                case XmlNodeType.Element:
                    element = inputNode as XmlElement;
                    ownerDocument = inputNode.OwnerDocument;
                    if (ownerDocument == null)
                        return null;
                    break;
                case XmlNodeType.Document:
                    ownerDocument = inputNode as XmlDocument;
                    if (ownerDocument == null)
                        return null;
                    element = ownerDocument.DocumentElement;
                    break;

                /* 
             * TODO WE COULD SUPPORT THE FOLLOWING TYPES
            case XmlNodeType.DocumentFragment:
            case XmlNodeType.Attribute:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Comment:
             */
                default:
                    throw new ArgumentOutOfRangeException("inputNode",
                                                          string.Format(Resources.Cryptographer_Decrypt_CannotDecryptNode,
                                                                        inputNode.NodeType));
            }

            // If we don't have an element and an owner document nothing to do!
            if (element == null)
                return null;

            // Create a loader document.
            XmlDocument loaderDocument = new XmlDocument(ownerDocument.NameTable);

            // Set up XPath Navigator
            XPathNavigator xPathNavigator = ownerDocument.CreateNavigator();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
            namespaceManager.AddNamespace("b", ownerDocument.NamespaceURI);

            Stack<XmlElement> encryptedElements = new Stack<XmlElement>();
            encryptedElements.Push(element);
            while (encryptedElements.Count > 0)
            {
                element = encryptedElements.Pop();

                // Check we have a parent node, if we don't we're not part of a document.
                XmlNode parentNode = element.ParentNode;
                if (parentNode == null)
                    continue;

                // This will always be true, unless an unencrypted element is passed in initially.
                if (element.LocalName == "Encrypted")
                {
                    // Decrypt the element
                    bool l;
                    string decryptedXmlStr = encryptorDecryptor.Decrypt(element.InnerText, out l);
                    if (string.IsNullOrEmpty(decryptedXmlStr))
                        continue;

                    // Update output flags.
                    isLatestKey &= l;

                    // Create document fragment
                    loaderDocument.LoadXml(decryptedXmlStr);

                    if (loaderDocument.DocumentElement == null)
                        continue;

                    // Import the element into the owner document
                    XmlElement decryptedElement =
                        ownerDocument.ImportNode(loaderDocument.DocumentElement, true) as XmlElement;

                    if (decryptedElement == null)
                        continue;

                    // Replace encrypted element with decrypted one.
                    parentNode.ReplaceChild(decryptedElement, element);

                    element = decryptedElement;
                }

                // Search for child elements that are encrypted.
                XmlNodeList encryptedChildNodes = element.SelectNodes("//b:Encrypted", namespaceManager);
                if ((encryptedChildNodes == null) ||
                    (encryptedChildNodes.Count < 1))
                    continue;

                // Push encrypted nodes onto stack.
                foreach (
                    XmlElement e in
                        from XmlNode eNode in encryptedChildNodes
                        select eNode as XmlElement
                            into e
                            where e.LocalName == "Encrypted"
                            select e)
                    encryptedElements.Push(e);
            }
            return element;
        }

        /// <summary>
        /// Encrypts a <see cref="XNode"/>
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <returns>
        /// The upmost <see cref="XElement"/> that was encrypted.
        /// </returns>
        [UsedImplicitly]
        [CanBeNull]
        public static XElement Encrypt([NotNull] this XNode inputNode, IEncryptorDecryptor encryptorDecryptor)
        {
            XElement element;
            XDocument ownerDocument;
            switch (inputNode.NodeType)
            {
                case XmlNodeType.Element:
                    element = inputNode as XElement;
                    ownerDocument = inputNode.Document;
                    if (ownerDocument == null)
                        return null;
                    break;
                case XmlNodeType.Document:
                    ownerDocument = inputNode as XDocument;
                    if (ownerDocument == null)
                        return null;
                    element = ownerDocument.Root;
                    break;

                /* 
             * TODO WE COULD SUPPORT THE FOLLOWING TYPES
            case XmlNodeType.DocumentFragment:
            case XmlNodeType.Attribute:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Comment:
             */
                default:
                    throw new ArgumentOutOfRangeException("inputNode",
                                                          string.Format(Resources.Cryptographer_Encrypt_CannotEncryptNode,
                                                                        inputNode.NodeType));
            }

            // If we don't have an element and an owner document nothing to do!
            if (element == null)
                return null;

            // Create an encrypted element.
            XElement encryptedElement = new XElement("Encrypted", encryptorDecryptor.Encrypt(element.ToString()));
            element.ReplaceWith(encryptedElement);
            return encryptedElement;
        }

        /// <summary>
        /// Encrypts a <see cref="XmlNode"/>
        /// </summary>
        /// <param name="inputNode">The input node.</param>
        /// <param name="encryptorDecryptor">The encryptor decryptor.</param>
        /// <returns>
        /// The upmost <see cref="XmlElement"/> that was encrypted.
        /// </returns>
        [UsedImplicitly]
        [CanBeNull]
        public static XmlElement Encrypt([NotNull] this XmlNode inputNode, IEncryptorDecryptor encryptorDecryptor)
        {
            XmlElement element;
            XmlDocument ownerDocument;
            switch (inputNode.NodeType)
            {
                case XmlNodeType.Element:
                    element = inputNode as XmlElement;
                    ownerDocument = inputNode.OwnerDocument;
                    if (ownerDocument == null)
                        return null;
                    break;
                case XmlNodeType.Document:
                    ownerDocument = inputNode as XmlDocument;
                    if (ownerDocument == null)
                        return null;
                    element = ownerDocument.DocumentElement;
                    break;

                /* 
             * TODO WE COULD SUPPORT THE FOLLOWING TYPES
            case XmlNodeType.DocumentFragment:
            case XmlNodeType.Attribute:
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Comment:
             */
                default:
                    throw new ArgumentOutOfRangeException("inputNode",
                                                          string.Format(Resources.Cryptographer_Encrypt_CannotEncryptNode,
                                                                        inputNode.NodeType));
            }

            // If we don't have an element and an owner document nothing to do!
            if (element == null)
                return null;

            // Check we have a parent node (otherwise nothing to replace).
            XmlNode parentNode = element.ParentNode;
            if (parentNode == null)
                return null;

            // Create an encrypted element.
            XmlElement encryptedElement = ownerDocument.CreateElement("Encrypted", ownerDocument.NamespaceURI);
            encryptedElement.InnerText = encryptorDecryptor.Encrypt(element.OuterXml);

            parentNode.ReplaceChild(encryptedElement, element);
            return encryptedElement;
        }
    }
}
