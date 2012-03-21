using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// Used to edit nuget specification files.
    /// </summary>
    /// <remarks>
    /// <para>TODO: Currently this only supports dependency manipulation, reference &amp; file manipulation to be added.</para>
    /// </remarks>
    [UsedImplicitly]
    public class NuSpecEditor
    {
        /// <summary>
        /// The nuget specification file path.
        /// </summary>
        [NotNull]
        public readonly string Path;
        
        /// <summary>
        /// The nuspec namespace.
        /// </summary>
        [NotNull]
        public readonly XNamespace Namespace;

        /// <summary>
        /// The raw XML Document.
        /// </summary>
        [NotNull]
        public readonly XDocument Document;

        /// <summary>
        /// The package element.
        /// </summary>
        [NotNull]
        public readonly XElement PacakgeElement;

        [NotNull]
        public readonly XName MetadataXName;

        private XElement _metadataElement;

        /// <summary>
        /// The metadata XML element.
        /// </summary>
        [NotNull]
        public XElement MetadataElement
        {
            get
            {
                if (_metadataElement == null)
                {
                    _metadataElement = new XElement(MetadataXName);
                    PacakgeElement.AddFirst(_metadataElement);
                    HasChanges = true;
                }
                return _metadataElement;
            }
        }
        
        [NotNull]
        public readonly XName FilesXName;
        [NotNull]
        public readonly XName FileXName;
        private XElement _filesElement;

        /// <summary>
        /// The files XML element.
        /// </summary>
        [NotNull]
        public XElement FilesElement
        {
            get
            {
                if (_filesElement == null)
                {
                    _filesElement = new XElement(FilesXName);
                    PacakgeElement.Add(_metadataElement);
                    HasChanges = true;
                } 
                return _filesElement;
            }
        }
        
        [NotNull]
        public readonly XName ReferencesXName;
        [NotNull]
        public readonly XName ReferenceXName;
        private XElement _referencesElement;

        /// <summary>
        /// Gets the references element.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public XElement ReferencesElement
        {
            get
            {
                if (_referencesElement == null)
                {
                    _referencesElement = new XElement(ReferencesXName);
                    MetadataElement.Add(_referencesElement);
                    HasChanges = true;
                } 
                return _referencesElement;
            }
        }
        
        [NotNull]
        public readonly XName DependenciesXName;
        [NotNull]
        public readonly XName DependencyXName;
        private XElement _dependenciesElement;

        /// <summary>
        /// The dependencies element.
        /// </summary>
        [NotNull]
        public XElement DependenciesElement
        {
            get
            {
                if (_dependenciesElement == null)
                {
                    _dependenciesElement = new XElement(DependenciesXName);
                    MetadataElement.Add(_dependenciesElement);
                    HasChanges = true;
                } 
                return _dependenciesElement;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changes.
        /// </summary>
        /// <value><see langword="true"/> if this instance has changes; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool HasChanges { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuSpecEditor"/> class.
        /// </summary>
        /// <param name="path">The nuget package specification file.</param>
        /// <remarks></remarks>
        public NuSpecEditor([NotNull]string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(String.Format(Resources.NuSpecEditor_SpecificationFileNotFound, path));
            Path = path;

            Document = XDocument.Load(path, LoadOptions.PreserveWhitespace);

            if ((Document == null) || (Document.Root == null))
                throw new FormatException(
                    String.Format(Resources.NuSpecEditor_InvalidXmlDocumentRootNode, path));

            PacakgeElement = Document.Root;

            // Check root element has a package element from the correct namespace.
            if (String.IsNullOrWhiteSpace(PacakgeElement.Name.LocalName) ||
                !PacakgeElement.Name.LocalName.Equals("package") ||
                !PacakgeElement.Name.NamespaceName.Contains("nuspec.xsd"))
                throw new FormatException(
                    String.Format(Resources.NuSpecEditor_DoesNotContainPackageElement, path));

            // Set the namespace
            Namespace = PacakgeElement.Name.Namespace;

            // Create names and find key nodes if present.
            MetadataXName = Namespace + "metadata";
            _metadataElement = PacakgeElement.Elements(MetadataXName).SingleOrDefault();
            
            FilesXName = Namespace + "files";
            _filesElement = PacakgeElement.Elements(FilesXName).SingleOrDefault();
            FileXName = Namespace + "file";
            
            ReferencesXName = Namespace + "references";
            if (_metadataElement != null)
                _referencesElement = _metadataElement.Elements(ReferencesXName).SingleOrDefault();
            ReferenceXName = Namespace + "reference";

            DependenciesXName = Namespace + "dependencies";
            if (_metadataElement != null)
                _dependenciesElement = _metadataElement.Elements(DependenciesXName).SingleOrDefault();
            DependencyXName = Namespace + "dependency";
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        /// <remarks></remarks>
        [NotNull]
        public string ID
        {
            get { return GetMetadata("id"); }
            set { SetMetadata("id", value); }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Version
        {
            get { return GetMetadata("version"); }
            set { SetMetadata("version", value); }
        }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>The authors.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Authors
        {
            get { return GetMetadata("authors"); }
            set { SetMetadata("authors", value); }
        }

        /// <summary>
        /// Gets or sets the owners.
        /// </summary>
        /// <value>The owners.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Owners
        {
            get { return GetMetadata("owners"); }
            set { SetMetadata("owners", value); }
        }

        /// <summary>
        /// Gets or sets the licenseUrl.
        /// </summary>
        /// <value>The licenseUrl.</value>
        /// <remarks></remarks>
        [NotNull]
        public string LicenseUrl
        {
            get { return GetMetadata("licenseUrl"); }
            set { SetMetadata("licenseUrl", value); }
        }

        /// <summary>
        /// Gets or sets the iconUrl.
        /// </summary>
        /// <value>The iconUrl.</value>
        /// <remarks></remarks>
        [NotNull]
        public string IconUrl
        {
            get { return GetMetadata("iconUrl"); }
            set { SetMetadata("iconUrl", value); }
        }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Summary
        {
            get { return GetMetadata("summary"); }
            set { SetMetadata("summary", value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Description
        {
            get { return GetMetadata("description"); }
            set { SetMetadata("description", value); }
        }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks></remarks>
        [NotNull]
        public string Tags
        {
            get { return GetMetadata("tags"); }
            set { SetMetadata("tags", value); }
        }

        /// <summary>
        /// Gets or sets the requireLicenseAcceptance flag.
        /// </summary>
        /// <value>The requireLicenseAcceptance flag.</value>
        /// <remarks></remarks>
        public bool RequireLicenseAcceptance
        {
            get { return Boolean.Parse(GetMetadata("requireLicenseAcceptance")); }
            set { SetMetadata("requireLicenseAcceptance", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Sets the metadata property with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
        private void SetMetadata([NotNull]string name, [NotNull]string value)
        {
            XName dataName = Namespace + name;
            XElement dataNode = MetadataElement.Elements(dataName).SingleOrDefault();
            if (dataNode == null)
            {
                dataNode = new XElement(dataName, value);
                MetadataElement.Add(dataNode);
                HasChanges = true;
                return;
            }

            if (dataNode.Value.Equals(value))
                return;

            dataNode.SetValue(value);
            HasChanges = true;
        }

        /// <summary>
        /// Gets the metadata with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The value if present; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        private string GetMetadata([NotNull]string name)
        {
            XName dataName = Namespace + name;
            XElement dataNode = MetadataElement.Elements(dataName).SingleOrDefault();
            return dataNode == null ? null : dataNode.Value;
        }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<NuSpecDependency> Dependencies
        {
            get
            {
                return DependenciesElement.Elements()
                    .Select(e =>
                                {
                                    if ((e == null) ||
                                        (e.Name != DependencyXName))
                                        return default(NuSpecDependency);
                                    XAttribute idattr = e.Attributes("id").FirstOrDefault();
                                    if (idattr == null)
                                        return default(NuSpecDependency);
                                    string id = idattr.Value;
                                    if (String.IsNullOrWhiteSpace(id) || id.Contains(","))
                                        return default(NuSpecDependency);
                                    XAttribute versionAttr = e.Attributes("version").FirstOrDefault();
                                    string version = null;
                                    if (versionAttr != null)
                                    {
                                        version = versionAttr.Value;
                                        if (String.IsNullOrWhiteSpace(version))
                                            version = null;
                                    }
                                    return new NuSpecDependency(id, version);
                                }).Where(d => d != default(NuSpecDependency)).ToList();
            }
        } 

        /// <summary>
        /// Ensures the dependency is added if not present.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <remarks></remarks>
        public void EnsureDependency(string dependency)
        {
            EnsureDependency((NuSpecDependency) dependency);
        }

        /// <summary>
        /// Ensures the dependency is added if not present.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <remarks></remarks>
        public void EnsureDependency(NuSpecDependency dependency)
        {
            XElement dependencyElement = DependenciesElement.Elements()
                .FirstOrDefault(e => 
                    e != null &&
                    e.Name == DependencyXName &&
                    e.Attributes("id").FirstOrDefault(
                                    a => a.Value.Equals(dependency.Id, StringComparison.OrdinalIgnoreCase)) != null);


            if (dependencyElement == null)
            {
                // Add the dependency element
                dependencyElement = new XElement(DependencyXName,
                                                 new XAttribute("id", dependency.Id),
                                                 String.IsNullOrWhiteSpace(dependency.Version)
                                                     ? null
                                                     : new XAttribute("version", dependency.Version));
                DependenciesElement.Add(dependencyElement);
                HasChanges = true;
                return;
            }
            
            // We already have a matching dependency, check the version if necessary.
            if (dependency.Version == null) return;

            XAttribute versionAttribute = dependencyElement.Attributes("version").SingleOrDefault();
            if (versionAttribute == null)
            {
                versionAttribute = new XAttribute("version", dependency.Version);
                dependencyElement.Add(versionAttribute);
            }
            else
            {
                if (versionAttribute.Value.Equals(dependency.Version, StringComparison.OrdinalIgnoreCase))
                    return;
                versionAttribute.SetValue(dependency.Version);
            }
            HasChanges = true;
        }

        /// <summary>
        /// Removes all dependencies matching the supplied ids.
        /// </summary>
        /// <param name="ids">The ids (multiple ids can be separated with any of ',;|').</param>
        /// <remarks></remarks>
        public void RemoveDependencies(string ids)
        {
            if ((_dependenciesElement == null) || (String.IsNullOrWhiteSpace(ids)))
                return;

            List<string> allIds =
                ids.Split(new[] {',', ';', '|'}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())
                .Where(i => !String.IsNullOrWhiteSpace(i))
                .ToList();
            if (allIds.Count < 1)
                return;

            // Remove matching dependency nodes.
            foreach (XElement match in _dependenciesElement.Elements()
                .Where(e =>
                       e != null &&
                       e.Name == DependencyXName &&
                       e.Attributes("id").FirstOrDefault(a => allIds.Contains(a.Value, StringComparer.OrdinalIgnoreCase)) != null)
                       .ToList())
            {
                match.Remove();
                HasChanges = true;
            }

            if (_dependenciesElement.HasElements) return;

            // Remove empty dependencies node.
            _dependenciesElement.Remove();
            _dependenciesElement = null;
            HasChanges = true;
        }

        /// <summary>
        /// Removes all dependencies matching the regex.
        /// </summary>
        /// <param name="idregex">The regex to match the id against.</param>
        /// <remarks></remarks>
        public void RemoveDependencies(Regex idregex)
        {
            if ((_dependenciesElement == null) || (idregex == null))
                return;

            // Remove matching dependency nodes.
            foreach (XElement match in _dependenciesElement.Elements()
                .Where(e =>
                       e != null &&
                       e.Name == DependencyXName &&
                       e.Attributes("id").FirstOrDefault(a => idregex.IsMatch(a.Value)) != null).ToList())
            {
                match.Remove();
                HasChanges = true;
            }

            if (_dependenciesElement.HasElements) return;

            // Remove empty dependencies node.
            _dependenciesElement.Remove();
            _dependenciesElement = null;
            HasChanges = true;
        }

        /// <summary>
        /// Saves this instance if their are changes.
        /// </summary>
        /// <remarks></remarks>
        public void Save()
        {
            // If nothing has changed nothing to do.
            if (!HasChanges)
                return;

            // Save the nuspec
            Document.Save(Path, SaveOptions.DisableFormatting);

            // We haven't got any more changes.
            HasChanges = false;
        }
    }
}