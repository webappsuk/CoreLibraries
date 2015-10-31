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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="XNode"/>.
    /// </summary>
    [PublicAPI]
    public static class XObjectExtensions
    {
        /// <summary>
        /// Compares two <see cref="XObject">XObjects</see>for equality.
        /// </summary>
        /// <param name="obj1">The first <see cref="XObject" />.</param>
        /// <param name="obj2">The second <see cref="XObject" />.</param>
        /// <param name="options">The comparison options.</param>
        /// <param name="stringComparer">The string comparer.</param>
        /// <returns>The <see cref="XObject"/> at which the first mismatch
        /// occurred; otherwise <see langword="null"/>.</returns>
        /// <remarks><para>The order of the parameters maters for ceratin options.</para></remarks>
        public static Tuple<XObject, XObject> DeepEquals(
            [CanBeNull] this XObject obj1,
            [CanBeNull] XObject obj2,
            XObjectComparisonOptions options = XObjectComparisonOptions.Default,
            [CanBeNull] StringComparer stringComparer = null)
        {
            if (stringComparer == null)
                stringComparer = StringComparer.CurrentCulture;

            // Normalize flags
            bool ignoreAttributes = (options & XObjectComparisonOptions.IgnoreAttributes) ==
                                    XObjectComparisonOptions.IgnoreAttributes;
            bool ignoreAdditionalAttributes = ignoreAttributes ||
                                        ((options & XObjectComparisonOptions.IgnoreAdditionalAttributes) ==
                                         XObjectComparisonOptions.IgnoreAdditionalAttributes);
            bool ignoreAttributeOrder = ignoreAdditionalAttributes ||
                                        ((options & XObjectComparisonOptions.IgnoreAttributeOrder) ==
                                         XObjectComparisonOptions.IgnoreAttributeOrder);

            bool ignoreElements = (options & XObjectComparisonOptions.IgnoreElements) ==
                                    XObjectComparisonOptions.IgnoreElements;
            bool ignoreAdditionalElements = ignoreElements ||
                                        ((options & XObjectComparisonOptions.IgnoreAdditionalElements) ==
                                         XObjectComparisonOptions.IgnoreAdditionalElements);
            bool ignoreElementOrder = ignoreAdditionalElements ||
                                        ((options & XObjectComparisonOptions.IgnoreElementOrder) ==
                                         XObjectComparisonOptions.IgnoreElementOrder);

            bool ignoreComments = (options & XObjectComparisonOptions.IgnoreComments) ==
                                    XObjectComparisonOptions.IgnoreComments;
            bool ignoreAdditionalCommentss = ignoreComments ||
                                        ((options & XObjectComparisonOptions.IgnoreAdditionalComments) ==
                                         XObjectComparisonOptions.IgnoreAdditionalComments);
            bool ignoreCommentOrder = ignoreAdditionalCommentss ||
                                        ((options & XObjectComparisonOptions.IgnoreCommentOrder) ==
                                         XObjectComparisonOptions.IgnoreCommentOrder);

            bool ignoreText = (options & XObjectComparisonOptions.IgnoreText) ==
                                    XObjectComparisonOptions.IgnoreText;
            bool ignoreAdditionalText = ignoreText || ((options & XObjectComparisonOptions.IgnoreAdditionalText) ==
                                         XObjectComparisonOptions.IgnoreAdditionalText);
            bool ignoreTextOrder = ignoreAdditionalText ||
                ((options & XObjectComparisonOptions.IgnoreTextOrder) ==
                                         XObjectComparisonOptions.IgnoreTextOrder);
            bool ignoreTextOutsideOfChildren = ignoreText || ((options & XObjectComparisonOptions.IgnoreTextOutsideOfChildren) ==
                                         XObjectComparisonOptions.IgnoreTextOutsideOfChildren);


            bool ignoreProcessingInstructions = (options & XObjectComparisonOptions.IgnoreProcessingInstructions) ==
                                    XObjectComparisonOptions.IgnoreProcessingInstructions;
            bool ignoreAdditionalProcessingInstructionss = ignoreProcessingInstructions ||
                                        ((options & XObjectComparisonOptions.IgnoreAdditionalProcessingInstructions) ==
                                         XObjectComparisonOptions.IgnoreAdditionalProcessingInstructions);
            bool ignoreProcessingInstructionOrder = ignoreAdditionalProcessingInstructionss ||
                                        ((options & XObjectComparisonOptions.IgnoreProcessingInstructionOrder) ==
                                         XObjectComparisonOptions.IgnoreProcessingInstructionOrder);

            bool ignoreDocumentTypes = (options & XObjectComparisonOptions.IgnoreDocumentTypes) ==
                                    XObjectComparisonOptions.IgnoreDocumentTypes;
            bool ignoreAdditionalDocumentTypess = ignoreDocumentTypes ||
                                        ((options & XObjectComparisonOptions.IgnoreAdditionalDocumentTypes) ==
                                         XObjectComparisonOptions.IgnoreAdditionalDocumentTypes);

            bool ignoreAllChildren = ignoreElements &&
                                     ignoreProcessingInstructions &&
                                     ignoreDocumentTypes &&
                                     ignoreComments &&
                                     ignoreText;
            bool allOrderSignificant = !ignoreElementOrder &&
                     !ignoreProcessingInstructionOrder &&
                     !ignoreCommentOrder &&
                     !ignoreTextOrder &&
                     !ignoreTextOutsideOfChildren;

            Stack<XObject, XObject> stack = new Stack<XObject, XObject>();
            stack.Push(obj1, obj2);

            while (stack.TryPop(out obj1, out obj2))
            {
                // Generic equality checks
                if (ReferenceEquals(obj1, obj2)) continue;
                if (ReferenceEquals(obj1, null) ||
                    ReferenceEquals(obj2, null) ||
                    obj1.NodeType != obj2.NodeType)
                    return new Tuple<XObject, XObject>(obj1, obj2);

                XContainer container1 = null;
                XContainer container2 = null;
                // Perform type specific checks.
                switch (obj1.NodeType)
                {
                    case XmlNodeType.Document:
                        // Process as container
                        container1 = (XContainer)obj1;
                        container2 = (XContainer)obj2;
                        break;

                    case XmlNodeType.Element:
                        XElement el1 = (XElement)obj1;
                        XElement el2 = (XElement)obj2;

                        if (el1.Name != el2.Name)
                            return new Tuple<XObject, XObject>(obj1, obj2);

                        if (!ignoreAttributes)
                        {
                            // Add attributes to stack for processing
                            if (ignoreAttributeOrder)
                            {
                                // Build dictionary of attributes
                                Dictionary<XName, XAttribute> el2Attributes =
                                    el2.Attributes()
                                        // ReSharper disable once PossibleNullReferenceException
                                        .ToDictionary(a => a.Name);

                                // Ignore attribute order
                                foreach (XAttribute attribute1 in el1.Attributes())
                                {
                                    XAttribute attribute2;
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (!el2Attributes.TryGetValue(attribute1.Name, out attribute2) ||
                                        !stringComparer.Equals(attribute1.Value, attribute2.Value))
                                        return new Tuple<XObject, XObject>(attribute1, attribute2);

                                    // ReSharper disable once PossibleNullReferenceException
                                    el2Attributes.Remove(attribute2.Name);
                                }

                                // If additional attributes on the second element are not being ignored, check if we
                                // additional attributes.
                                if (!ignoreAdditionalAttributes &&
                                    el2Attributes.Count > 0)
                                    return new Tuple<XObject, XObject>(null, el2Attributes.Values.First());
                            }
                            else
                            {
                                // Attribute order matters
                                XAttribute attribute1 = el1.FirstAttribute;
                                XAttribute attribute2 = el2.FirstAttribute;
                                while (attribute1 != null)
                                {
                                    // We have less attributes on the second element than the first
                                    if (attribute2 == null ||
                                        !stringComparer.Equals(attribute1.Value, attribute2.Value))
                                        return new Tuple<XObject, XObject>(attribute1, attribute2);
                                    attribute1 = attribute1.NextAttribute;
                                    attribute2 = attribute2.NextAttribute;
                                }
                                if (attribute2 != null)
                                    return new Tuple<XObject, XObject>(null, attribute2);
                            }
                        }

                        // Process as container
                        container1 = el1;
                        container2 = el2;
                        break;

                    case XmlNodeType.Attribute:
                        XAttribute attr1 = (XAttribute)obj1;
                        XAttribute attr2 = (XAttribute)obj2;
                        if (!ignoreAttributes &&
                            (attr1.Name != attr2.Name ||
                             !stringComparer.Equals(attr1.Value, attr2.Value)))
                            return new Tuple<XObject, XObject>(attr1, attr2);
                        break;

                    case XmlNodeType.ProcessingInstruction:
                        XProcessingInstruction pi1 = (XProcessingInstruction)obj1;
                        XProcessingInstruction pi2 = (XProcessingInstruction)obj2;
                        if (!ignoreProcessingInstructions &&
                            (!stringComparer.Equals(pi1.Target, pi2.Target) ||
                             !stringComparer.Equals(pi1.Data, pi2.Data)))
                            return new Tuple<XObject, XObject>(pi1, pi2);
                        break;

                    case XmlNodeType.DocumentType:
                        XDocumentType dt1 = (XDocumentType)obj1;
                        XDocumentType dt2 = (XDocumentType)obj2;
                        if (!ignoreDocumentTypes &&
                            (!stringComparer.Equals(dt1.Name, dt2.Name) ||
                             !stringComparer.Equals(dt1.PublicId, dt2.PublicId) ||
                             !stringComparer.Equals(dt1.SystemId, dt2.SystemId) ||
                             !stringComparer.Equals(dt1.InternalSubset, dt2.InternalSubset)))
                            return new Tuple<XObject, XObject>(dt1, dt2);
                        break;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        XText txt1 = (XText)obj1;
                        XText txt2 = (XText)obj2;
                        if (!ignoreText &&
                            !stringComparer.Equals(txt1.Value, txt2.Value))
                            return new Tuple<XObject, XObject>(txt1, txt2);
                        break;

                    case XmlNodeType.Comment:
                        XComment comment1 = (XComment)obj1;
                        XComment comment2 = (XComment)obj2;
                        if (!ignoreComments &&
                            !stringComparer.Equals(comment1.Value, comment2.Value))
                            return new Tuple<XObject, XObject>(comment1, comment2);
                        break;
                }

                // If we're not a container, or we're ignoring container contents, we can move on.
                if (container1 == null || ignoreAllChildren) continue;
                if (allOrderSignificant)
                {
                    // We care about the order of everything
                    XNode n1 = container1.FirstNode;
                    XNode n2 = container2.FirstNode;
                    while (n1 != null)
                    {
                        if (n2 == null || n1.NodeType != n2.NodeType)
                            return new Tuple<XObject, XObject>(n1, n2);
                        stack.Push(n1, n2);
                        n1 = n1.NextNode;
                        n2 = n2.NextNode;
                    }
                    if (n2 != null) return new Tuple<XObject, XObject>(null, n2);
                    continue;
                }

                // We care about the order of somethings, but not others.
                List<XElement> elements1;
                List<XElement> elements2;
                if (!ignoreElements && ignoreElementOrder)
                {
                    elements1 = new List<XElement>();
                    elements2 = new List<XElement>();
                }
                else elements1 = elements2 = null;
                List<XProcessingInstruction> processingInstructions1;
                List<XProcessingInstruction> processingInstructions2;
                if (!ignoreProcessingInstructions && ignoreProcessingInstructionOrder)
                {
                    processingInstructions1 = new List<XProcessingInstruction>();
                    processingInstructions2 = new List<XProcessingInstruction>();
                }
                else processingInstructions1 = processingInstructions2 = null;
                List<XComment> comments1;
                List<XComment> comments2;
                if (!ignoreComments && ignoreCommentOrder)
                {
                    comments1 = new List<XComment>();
                    comments2 = new List<XComment>();
                }
                else comments1 = comments2 = null;
                List<XText> texts1;
                List<XText> texts2;
                if (!ignoreText && ignoreTextOrder)
                {
                    texts1 = new List<XText>();
                    texts2 = new List<XText>();
                }
                else texts1 = texts2 = null;

                XDocumentType documentType1 = null;
                XDocumentType documentType2 = null;
                bool hasChildren = false;
                bool hasText = false;

                // Build child lists from first node
                List<XObject> l1 = new List<XObject>();
                XNode n = container1.FirstNode;
                while (n != null)
                {
                    switch (n.NodeType)
                    {
                        case XmlNodeType.Element:
                            hasChildren = true;
                            if (elements1 != null) elements1.Add((XElement)n);
                            else if (!ignoreElements) l1.Add(n);
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            hasText = true;
                            if (texts1 != null) texts1.Add((XText)n);
                            else if (!ignoreText) l1.Add(n);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            if (processingInstructions1 != null) processingInstructions1.Add((XProcessingInstruction)n);
                            else if (!ignoreProcessingInstructions) l1.Add(n);
                            break;
                        case XmlNodeType.Comment:
                            if (comments1 != null) comments1.Add((XComment)n);
                            else if (!ignoreComments) l1.Add(n);
                            break;
                        case XmlNodeType.DocumentType:
                            if (!ignoreDocumentTypes)
                                documentType1 = (XDocumentType)n;
                            break;
                    }
                    n = n.NextNode;
                }

                // Build child lists from second node
                List<XObject> l2 = new List<XObject>();
                n = container2.FirstNode;
                while (n != null)
                {
                    switch (n.NodeType)
                    {
                        case XmlNodeType.Element:
                            hasChildren = true;
                            if (elements2 != null) elements2.Add((XElement)n);
                            else if (!ignoreElements) l2.Add(n);
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            hasText = true;
                            if (texts2 != null) texts2.Add((XText)n);
                            else if (!ignoreText) l2.Add(n);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            if (processingInstructions2 != null) processingInstructions2.Add((XProcessingInstruction)n);
                            else if (!ignoreProcessingInstructions) l2.Add(n);
                            break;
                        case XmlNodeType.Comment:
                            if (comments2 != null) comments2.Add((XComment)n);
                            else if (!ignoreComments) l2.Add(n);
                            break;
                        case XmlNodeType.DocumentType:
                            if (!ignoreDocumentTypes)
                                documentType2 = (XDocumentType)n;
                            break;
                    }
                    n = n.NextNode;
                }

                // Add document type comparison if necessary (note we never consider order of the document type as it
                // always appears before the root node.
                if (!ignoreDocumentTypes)
                {
                    if (documentType1 != null)
                    {
                        if (documentType2 == null ||
                            !stringComparer.Equals(documentType1.Name, documentType2.Name) ||
                            !stringComparer.Equals(documentType1.PublicId, documentType2.PublicId) ||
                            !stringComparer.Equals(documentType1.SystemId, documentType2.SystemId) ||
                            !stringComparer.Equals(documentType1.InternalSubset, documentType2.InternalSubset))
                            return new Tuple<XObject, XObject>(documentType1, documentType2);

                        // Don't push onto stack, as we've already done the comparison at this point.
                    }
                    else if (!ignoreAdditionalDocumentTypess && documentType2 != null)
                        return new Tuple<XObject, XObject>(null, documentType2);
                }

                // Add order specific nodes to stack for processing.
                bool stripText = hasChildren && hasText && ignoreTextOutsideOfChildren;
                List<XObject>.Enumerator l1Enumerator = l1.GetEnumerator();
                List<XObject>.Enumerator l2Enumerator = l2.GetEnumerator();
                while (l1Enumerator.MoveNext())
                {
                    if (stripText && l1Enumerator.Current is XText) continue;

                    do
                    {
                        if (!l2Enumerator.MoveNext()) return new Tuple<XObject, XObject>(l1Enumerator.Current, null);
                    } while (stripText && l2Enumerator.Current is XText);

                    if (l1Enumerator.Current.NodeType != l2Enumerator.Current.NodeType)
                        return new Tuple<XObject, XObject>(l1Enumerator.Current, l2Enumerator.Current);
                    
                    stack.Push(l1Enumerator.Current, l2Enumerator.Current);
                }
                while (l2Enumerator.MoveNext())
                {
                    if (!stripText || !(l2Enumerator.Current is XText))
                        return new Tuple<XObject, XObject>(null, l2Enumerator.Current);
                }

                // Add order independent nodes for processing
                // ReSharper disable PossibleNullReferenceException
                if (elements1 != null)
                {
                    foreach (XElement e1 in elements1)
                    {
                        XElement e2;
                        if (!elements2.TryRemove(e => e.Name == e1.Name, out e2))
                            return new Tuple<XObject, XObject>(e1, null);
                        stack.Push(e1, e2);
                    }
                    if (!ignoreAdditionalElements && texts2.Count > 0)
                        return new Tuple<XObject, XObject>(null, elements2.First());
                }
                if (texts1 != null)
                {
                    foreach (XText t1 in texts1)
                    {
                        XText t2;
                        if (!texts2.TryRemove(t => stringComparer.Equals(t1.Value, t.Value), out t2))
                            return new Tuple<XObject, XObject>(t1, null);
                        // Don't push onto stack, as we've already done the comparison at this point.
                    }
                    if (!ignoreAdditionalText && texts2.Count > 0)
                        return new Tuple<XObject, XObject>(null, texts2.First());
                }
                if (comments1 != null)
                {
                    foreach (XComment c1 in comments1)
                    {
                        XComment c2;
                        if (!comments2.TryRemove(c => stringComparer.Equals(c1.Value, c.Value), out c2))
                            return new Tuple<XObject, XObject>(c1, null);
                        // Don't push onto stack, as we've already done the comparison at this point.
                    }
                    if (!ignoreAdditionalCommentss && comments2.Count > 0)
                        return new Tuple<XObject, XObject>(null, comments2.First());
                }
                if (processingInstructions1 != null)
                {
                    foreach (XProcessingInstruction pi1 in processingInstructions1)
                    {
                        XProcessingInstruction pi2;
                        if (!processingInstructions2.TryRemove(pi => stringComparer.Equals(pi.Target, pi1.Target) && stringComparer.Equals(pi.Data, pi1.Data), out pi2))
                            return new Tuple<XObject, XObject>(pi1, null);
                        // Don't push onto stack, as we've already done the comparison at this point.
                    }
                    if (!ignoreAdditionalProcessingInstructionss && processingInstructions2.Count > 0)
                        return new Tuple<XObject, XObject>(null, processingInstructions2.First());
                }
                // ReSharper restore PossibleNullReferenceException
            }

            // Successful comparisons result in null.
            return null;
        }
    }
}