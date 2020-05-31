// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.Json;

namespace SE.Npm
{
    /// <summary>
    /// NPM Package Meta Info
    /// </summary>
    public class PackageInfo : JsonDocument
    {
        const string PackageName = "name";
        const string PackageDescription = "description";
        const string PackagePlatformValidity = "os";
        const string PackageArchitectureValidity = "cpu";
        const string PackageConfiguration = "config";

        public static class ParameterNames
        {
            public const string StandalonePackage = "master";
            public const string SubmodulePackage = "submodule";
        }

        const string PackageVersionNumber = "version";
        const string PackageTags = "keywords";
        const string PackagePrimaryAuthor = "author";
        const string PackageAdditionalAuthors = "contributors";

        const string PersonName = "name";
        const string PersonEmail = "email";
        const string PersonHome = "url";

        const string PackageReferences = "dependencies";
        const string PackageBugTracker = "bugs";

        const string BugTrackerHome = "url";

        const string PackageLicense = "license";
        const string PackageRepositoryInfo = "repository";

        const string RepositoryType = "type";
        const string RepositoryAddress = "url";
        const string RepositoryDirectory = "directory";

        const string PackageHome = "homepage";
        const string IncludedFiles = "files";

        const string PackageDistribution = "dist";

        const string DistributionChecksum = "shasum";
        const string DistributionHome = "tarball";

        const string PackageCommandArguments = "scripts";

        PackageId id;
        /// <summary>
        /// This package's three-component ID
        /// </summary>
        public PackageId Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The package owner
        /// </summary>
        public string Owner
        {
            get { return id.Owner.ToUpperInvariant(); }
        }
        /// <summary>
        /// The package namespace
        /// </summary>
        public string Namespace
        {
            get { return Decode(id.Namespace); }
        }
        /// <summary>
        /// The package names
        /// </summary>
        public string Name
        {
            get { return Decode(id.Name); }
        }

        string description;
        /// <summary>
        /// A descriptive text displayed in some package browser
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        Uri home;
        /// <summary>
        /// The address of further information for this package
        /// </summary>
        public Uri Home
        {
            get { return home; }
            set { home = value; }
        }

        string license;
        /// <summary>
        /// The license name or location this package is distributed with
        /// </summary>
        public string License
        {
            get { return license; }
            set { license = value; }
        }

        HashSet<string> platforms;
        /// <summary>
        /// A collection of platforms this package is valid to
        /// </summary>
        public HashSet<string> Platforms
        {
            get { return platforms; }
        }

        HashSet<string> architectures;
        /// <summary>
        /// A collection of architectures this package is valid to
        /// </summary>
        public HashSet<string> Architectures
        {
            get { return architectures; }
        }

        Dictionary<string, object> parameter;
        /// <summary>
        /// Additional parameters provided while the package is processed
        /// </summary>
        public Dictionary<string, object> Parameter
        {
            get { return parameter; }
        }

        /// <summary>
        /// Identifies this package as standalone code module
        /// </summary>
        public bool IsStandaloneModule
        {
            get
            {
                return (parameter.ContainsKey(ParameterNames.StandalonePackage));
            }
        }
        /// <summary>
        /// Identifies this package as submodule of another standalone module package
        /// </summary>
        public bool IsSubmodule
        {
            get
            {
                return parameter.ContainsKey(ParameterNames.SubmodulePackage);
            }
        }

        List<IdentityInfo> authors;
        /// <summary>
        /// A collection of this packages authors
        /// </summary>
        public List<IdentityInfo> Authors
        {
            get { return authors; }
        }

        PackageVersion version;
        /// <summary>
        /// This package's version number
        /// </summary>
        public PackageVersion Version
        {
            get { return version; }
            set { version = value; }
        }

        HashSet<PackageReference> references;
        /// <summary>
        /// A collection of packages this package refers to
        /// </summary>
        public HashSet<PackageReference> References
        {
            get { return references; }
        }

        HashSet<string> tags;
        /// <summary>
        /// A collection of tags this package is associated with
        /// </summary>
        public HashSet<string> Tags
        {
            get { return tags; }
        }

        Uri bugTracker;
        /// <summary>
        /// The address of this packages bug tracker if available
        /// </summary>
        public Uri BugTracker
        {
            get { return bugTracker; }
            set { bugTracker = value; }
        }

        RepositoryInfo repository;
        /// <summary>
        /// This package's content repository
        /// </summary>
        public RepositoryInfo Repository
        {
            get { return repository; }
            set { repository = value; }
        }

        HashSet<string> files;
        /// <summary>
        /// A set of files that are exclusively included in the package
        /// </summary>
        public HashSet<string> Files
        {
            get { return files; }
        }

        DateTime releaseDate;
        /// <summary>
        /// The release date of this package set by the repository
        /// </summary>
        public DateTime ReleaseDate
        {
            get { return releaseDate; }
            set { releaseDate = value; }
        }

        DistributionInfo distributionInfo;
        /// <summary>
        /// Provides distribution info for the package content
        /// </summary>
        public DistributionInfo Distribution
        {
            get { return distributionInfo; }
        }

        Dictionary<string, string> arguments;
        /// <summary>
        /// A list of arguments that can be referenced to
        /// </summary>
        public Dictionary<string, string> Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Creates a new package meta instance
        /// </summary>
        public PackageInfo()
        {
            this.platforms = new HashSet<string>();
            this.architectures = new HashSet<string>();
            this.tags = new HashSet<string>();

            this.parameter = new Dictionary<string, object>();
            this.authors = new List<IdentityInfo>();

            this.references = new HashSet<PackageReference>();

            this.files = new HashSet<string>();

            this.arguments = new Dictionary<string, string>();
        }

        public override bool Load(Stream stream, Encoding encoding)
        {
            if (base.Load(stream, encoding))
            {
                if (Root == null)
                    return false;

                return Process(Root.Child);
            }
            else return false;
        }

        /// <summary>
        /// Tries to parse a registry entry from provided json
        /// </summary>
        /// <returns>True if successfully parsed, false otherwise</returns>
        internal bool Process(JsonNode rootNode)
        {
            if (parser == null) parser = new JsonParser(this);
            while (rootNode != null)
            {
                switch (rootNode.Name.ToLowerInvariant())
                {
                    #region PackageName = 'name';
                    case PackageName: if(rootNode.Type == JsonNodeType.String)
                        {
                            object pid; if (PackageId.TryParse(rootNode.ToString(), out pid))
                            {
                                id = pid as PackageId;
                                break;
                            }
                            else parser.Errors.Add(string.Format(ResponseCodes.PackageInfo.InvalidPackageName, rootNode.ToString()));
                        }
                        return false;
                    #endregion

                    #region PackageDescription = 'description';
                    case PackageDescription: if (rootNode.Type == JsonNodeType.String)
                        {
                            description = rootNode.ToString();
                        }
                        break;
                    #endregion

                    #region PackagePlatformValidity = 'os';
                    case PackagePlatformValidity: if (rootNode.Type == JsonNodeType.Array)
                        {
                            ReadPropertyTags(rootNode, platforms);
                        }
                        break;
                    #endregion

                    #region PackageArchitectureValidity = 'cpu';
                    case PackageArchitectureValidity: if (rootNode.Type == JsonNodeType.Array)
                        {
                            ReadPropertyTags(rootNode, architectures);
                        }
                        break;
                    #endregion

                    #region PackageConfiguration = 'config';
                    case PackageConfiguration: if (rootNode.Type == JsonNodeType.Object)
                        {
                            ReadConfigurationTags(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageVersionNumber = 'version';
                    case PackageVersionNumber: if(rootNode.Type == JsonNodeType.String)
                        {
                            version = PackageVersion.Create(rootNode.ToString());
                            if (version.IsValid && !version.IsCompatibilityVersion)
                                break;

                            parser.Errors.Add(string.Format(ResponseCodes.PackageInfo.InvalidPackageVersion, rootNode.ToString()));
                        }
                        return false;
                    #endregion

                    #region PackageTags = 'keywords';
                    case PackageTags: if (rootNode.Type == JsonNodeType.Array)
                        {
                            ReadPropertyTags(rootNode, tags);
                        }
                        break;
                    #endregion

                    #region PackagePrimaryAuthor = 'author';
                    case PackagePrimaryAuthor: if (rootNode.Type == JsonNodeType.Object)
                        {
                            authors.Insert(0, ReadPersonEntry(rootNode));
                        }
                        break;
                    #endregion

                    #region PackageAdditionalAuthors = 'contributors';
                    case PackageAdditionalAuthors: if (rootNode.Type == JsonNodeType.Array)
                        {
                            ReadAdditionalAuthors(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageReferences = 'dependencies';
                    case PackageReferences: switch (rootNode.Type)
                        {
                            case JsonNodeType.Empty: break;
                            case JsonNodeType.Object:
                                {
                                    if (ReadPackageReferences(rootNode))
                                        break;
                                }
                                goto default;
                            default:
                                {
                                    parser.Errors.Add(ResponseCodes.PackageInfo.InvalidPackageReference);
                                    return false;
                                }
                        }
                        break;
                    #endregion

                    #region PackageBugTracker = 'bugs';
                    case PackageBugTracker: if (rootNode.Type == JsonNodeType.Object)
                        {
                            ReadBugTrackerAddress(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageRepositoryInfo = 'repository';
                    case PackageRepositoryInfo: if (rootNode.Type == JsonNodeType.Object)
                        {
                            ReadRepositoryInfo(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageLicense = 'license';
                    case PackageLicense: if (rootNode.Type == JsonNodeType.String)
                        {
                            license = rootNode.ToString();
                        }
                        break;
                    #endregion

                    #region PackageInfoAddress = 'homepage';
                    case PackageHome: if (rootNode.Type == JsonNodeType.String)
                        {
                            Uri.TryCreate(rootNode.ToString(), UriKind.Absolute, out home);
                        }
                        break;
                    #endregion

                    #region IncludedFiles = 'files';
                    case IncludedFiles: if (rootNode.Type == JsonNodeType.Array)
                        {
                            ReadFiles(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageDistribution = 'dist';
                    case PackageDistribution: if (rootNode.Type == JsonNodeType.Object)
                        {
                            ReadDistributionInfo(rootNode);
                        }
                        break;
                    #endregion

                    #region PackageCommands = 'scripts';
                    case PackageCommandArguments: if (rootNode.Type == JsonNodeType.Object)
                        {
                            ReadCommandArguments(rootNode);
                        }
                        break;
                    #endregion
                }
                rootNode = rootNode.Next;
            }
            return true;
        }

        bool ReadPackageReferences(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.String)
                {
                    object pid; if (!PackageId.TryParse(property.Name, out pid))
                        return false;

                    PackageVersion ver = PackageVersion.Create(property.ToString());
                    if (!ver.IsValid)
                        return false;

                    references.Add(new PackageReference(pid as PackageId, ver));
                }

                property = property.Next;
            }
            return true;
        }
        void ReadPropertyTags(JsonNode property, HashSet<string> tags)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.String)
                    tags.Add(property.ToString());

                property = property.Next;
            }
        }
        void ReadConfigurationTags(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                switch (property.Name.ToLowerInvariant())
                {
                    #region StandaloneProgramPackage = 'master';
                    case ParameterNames.StandalonePackage: if (property.Type == JsonNodeType.String)
                        {
                            if (parameter.ContainsKey(ParameterNames.StandalonePackage)) parameter[ParameterNames.StandalonePackage] = property.ToString();
                            else parameter.Add(ParameterNames.StandalonePackage, property.ToString());
                        }
                        break;
                    #endregion

                    #region ProgramSubmodulePackage = 'submodule';
                    case ParameterNames.SubmodulePackage: if (property.Type == JsonNodeType.String)
                        {
                            if (parameter.ContainsKey(ParameterNames.SubmodulePackage)) parameter[ParameterNames.SubmodulePackage] = property.ToString();
                            else parameter.Add(ParameterNames.SubmodulePackage, property.ToString());
                        }
                        break;
                    #endregion
                }
                property = property.Next;
            }
        }
        void ReadBugTrackerAddress(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                switch (property.Name.ToLowerInvariant())
                {
                    #region BugTrackerAddress = 'url';
                    case BugTrackerHome: if (property.Type == JsonNodeType.String)
                        {
                            Uri.TryCreate(property.ToString(), UriKind.Absolute, out bugTracker);
                        }
                        break;
                    #endregion
                }
                property = property.Next;
            }
        }
        void ReadRepositoryInfo(JsonNode property)
        {
            string type = null;
            Uri url = null;
            string offset = null;

            property = property.Child;
            while (property != null)
            {
                switch (property.Name.ToLowerInvariant())
                {
                    #region RepositoryType = 'type';
                    case RepositoryType: if (property.Type == JsonNodeType.String)
                        {
                            type = property.ToString();
                        }
                        break;
                    #endregion

                    #region RepositoryAddress = 'url';
                    case RepositoryAddress: if (property.Type == JsonNodeType.String)
                        {
                            Uri.TryCreate(property.ToString(), UriKind.Absolute, out url);
                        }
                        break;
                    #endregion

                    #region RepositoryOffset = 'directory';
                    case RepositoryDirectory: if (property.Type == JsonNodeType.String)
                        {
                            offset = property.ToString();
                        }
                        break;
                    #endregion
                }
                property = property.Next;
            }

            repository = new RepositoryInfo(type, url, offset);
        }
        void ReadAdditionalAuthors(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.Object)
                {
                    IdentityInfo person = ReadPersonEntry(property);
                    if (person != null)
                        authors.Add(person);
                }

                property = property.Next;
            }
        }
        IdentityInfo ReadPersonEntry(JsonNode property)
        {
            string name = null;
            string email = null;
            Uri url = null;

            property = property.Child;
            while (property != null)
            {
                switch (property.Name.ToLowerInvariant())
                {
                    #region PersonName = 'name';
                    case PersonName: if (property.Type == JsonNodeType.String)
                        {
                            name = property.ToString();
                        }
                        break;
                    #endregion

                    #region PersonEmail = 'email';
                    case PersonEmail: if (property.Type == JsonNodeType.String)
                        {
                            email = property.ToString();
                        }
                        break;
                    #endregion

                    #region PersonHomePage = 'url';
                    case PersonHome: if (property.Type == JsonNodeType.String)
                        {
                            Uri.TryCreate(property.ToString(), UriKind.Absolute, out url);
                        }
                        break;
                    #endregion
                }
                property = property.Next;
            }

            if (!string.IsNullOrWhiteSpace(name)) return new IdentityInfo
             (
                 name,
                 email,
                 url
             );
            else return null;
        }
        bool ReadFiles(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.String)
                {
                    files.Add(property.ToString());
                }
                property = property.Next;
            }
            return true;
        }
        void ReadDistributionInfo(JsonNode property)
        {
            string checksum = null;
            Uri url = null;

            property = property.Child;
            while (property != null)
            {
                switch (property.Name.ToLowerInvariant())
                {
                    #region DistributionChecksum = 'shasum';
                    case DistributionChecksum: if (property.Type == JsonNodeType.String)
                        {
                            checksum = property.ToString();
                        }
                        break;
                    #endregion

                    #region DistributionHome = 'tarball';
                    case DistributionHome: if (property.Type == JsonNodeType.String)
                        {
                            Uri.TryCreate(property.ToString(), UriKind.Absolute, out url);
                        }
                        break;
                    #endregion
                }
                property = property.Next;
            }

            distributionInfo = new DistributionInfo(checksum, url);
        }
        void ReadCommandArguments(JsonNode property)
        {
            property = property.Child;
            while (property != null)
            {
                if (property.Type == JsonNodeType.String)
                {
                    string id = property.Name.ToLowerInvariant();
                    if (!arguments.ContainsKey(id))
                    {
                        arguments.Add(id, property.ToString());
                    }
                }
                property = property.Next;
            }
        }

        public override bool Save(Stream stream, Encoding encoding, bool formatted = true)
        {
            Clear();

            JsonNode root = AddNode(JsonNodeType.Object);
            JsonNode property;

            #region PackageName = 'name';
            if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                return false;

            property = AddNode(root, JsonNodeType.String);
            property.Name = PackageName;
            property.RawValue = id.ToString();
            #endregion

            #region PackageDescription = 'description';
            if (!string.IsNullOrWhiteSpace(description))
            {
                property = AddNode(root, JsonNodeType.String);
                property.Name = PackageDescription;
                property.RawValue = description;
            }
            #endregion

            #region PackageInfoAddress = 'homepage';
            if (home != null && !string.IsNullOrWhiteSpace(home.AbsoluteUri))
            {
                property = AddNode(root, JsonNodeType.String);
                property.Name = PackageHome;
                property.RawValue = home.AbsoluteUri;
            }
            #endregion

            #region PackageLicense = 'license';
            if (!string.IsNullOrWhiteSpace(license))
            {
                property = AddNode(root, JsonNodeType.String);
                property.Name = PackageLicense;
                property.RawValue = license;
            }
            #endregion

            #region PackagePlatformValidity = 'os';
            platforms.Remove(string.Empty);
            if (platforms.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Array);
                property.Name = PackagePlatformValidity;

                foreach (string platform in platforms)
                    AddNode(property, JsonNodeType.String).RawValue = platform;
            }
            #endregion

            #region PackageArchitectureValidity = 'cpu';
            architectures.Remove(string.Empty);
            if (architectures.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Array);
                property.Name = PackageArchitectureValidity;

                foreach (string architecture in architectures)
                    AddNode(property, JsonNodeType.String).RawValue = architecture;
            }
            #endregion

            #region PackageConfiguration = 'config';
            if (parameter.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageConfiguration;

                foreach (KeyValuePair<string, object> param in parameter)
                    SerializeParameter(property, param);
            }
            #endregion

            #region PackagePrimaryAuthor = 'author';
            if (authors.Count > 0 && authors[0] != null && !string.IsNullOrWhiteSpace(authors[0].Name))
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackagePrimaryAuthor;

                JsonNode childProperty = AddNode(property, JsonNodeType.String);
                childProperty.Name = PersonName;
                childProperty.RawValue = authors[0].Name;

                if (!string.IsNullOrWhiteSpace(authors[0].Email))
                {
                    childProperty = AddNode(property, JsonNodeType.String);
                    childProperty.Name = PersonEmail;
                    childProperty.RawValue = authors[0].Email;
                }

                if (authors[0].Home != null && !string.IsNullOrWhiteSpace(authors[0].Home.AbsoluteUri))
                {
                    childProperty = AddNode(property, JsonNodeType.String);
                    childProperty.Name = PersonHome;
                    childProperty.RawValue = authors[0].Home.AbsoluteUri;
                }
            }
            #endregion

            #region PackageAdditionalAuthors = 'contributors';
            if (authors.Count > 1)
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageAdditionalAuthors;

                for (int i = 1; i < authors.Count; i++)
                    if (authors[i] != null && !string.IsNullOrWhiteSpace(authors[i].Name))
                    {
                        JsonNode childProperty = AddNode(property, JsonNodeType.String);
                        childProperty.Name = PersonName;
                        childProperty.RawValue = authors[i].Name;

                        if (!string.IsNullOrWhiteSpace(authors[i].Email))
                        {
                            childProperty = AddNode(property, JsonNodeType.String);
                            childProperty.Name = PersonEmail;
                            childProperty.RawValue = authors[i].Email;
                        }

                        if (authors[i].Home != null && !string.IsNullOrWhiteSpace(authors[i].Home.AbsoluteUri))
                        {
                            childProperty = AddNode(property, JsonNodeType.String);
                            childProperty.Name = PersonHome;
                            childProperty.RawValue = authors[i].Home.AbsoluteUri;
                        }
                    }
            }
            #endregion

            #region PackageVersionNumber = 'version';
            if (!version.IsValid || version.IsCompatibilityVersion)
                return false;

            property = AddNode(root, JsonNodeType.String);
            property.Name = PackageVersionNumber;
            property.RawValue = version.ToString();
            #endregion

            #region PackageReferences = 'dependencies';
            if (references.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageReferences;

                foreach (PackageReference reference in references)
                {
                    JsonNode childProperty = AddNode(property, JsonNodeType.String);
                    childProperty.Name = reference.Id.ToString();
                    childProperty.RawValue = reference.Version.ToString();
                }
            }
            #endregion

            #region PackageTags = 'keywords';
            tags.Remove(string.Empty);
            if (tags.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Array);
                property.Name = PackageTags;

                foreach (string tag in tags)
                    AddNode(property, JsonNodeType.String).RawValue = tag;
            }
            #endregion

            #region PackageBugTracker = 'bugs';
            if (bugTracker != null && !string.IsNullOrWhiteSpace(bugTracker.AbsoluteUri))
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageBugTracker;

                property = AddNode(property, JsonNodeType.String);
                property.Name = BugTrackerHome;
                property.RawValue = bugTracker.AbsoluteUri;
            }
            #endregion

            #region PackageRepositoryInfo = 'repository';
            if (repository.Url != null && !string.IsNullOrWhiteSpace(repository.Url.AbsoluteUri))
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageRepositoryInfo;

                JsonNode childProperty = AddNode(property, JsonNodeType.String);
                childProperty.Name = RepositoryAddress;
                childProperty.RawValue = repository.Url.AbsoluteUri;

                if (!string.IsNullOrWhiteSpace(repository.Type))
                {
                    childProperty = AddNode(property, JsonNodeType.String);
                    childProperty.Name = RepositoryType;
                    childProperty.RawValue = repository.Type;
                }

                if (!string.IsNullOrWhiteSpace(repository.Directory))
                {
                    childProperty = AddNode(property, JsonNodeType.String);
                    childProperty.Name = RepositoryDirectory;
                    childProperty.RawValue = repository.Directory;
                }
            }
            #endregion

            #region IncludedFiles = 'files';
            files.Remove(string.Empty);
            if (files.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Array);
                property.Name = IncludedFiles;

                foreach (string file in files)
                    AddNode(property, JsonNodeType.String).RawValue = file;
            }
            #endregion

            #region PackageDistribution = 'dist'
            if (distributionInfo != null)
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageDistribution;

                JsonNode childProperty = AddNode(property, JsonNodeType.String);
                childProperty.Name = DistributionChecksum;
                childProperty.RawValue = distributionInfo.Checksum;

                childProperty = AddNode(property, JsonNodeType.String);
                childProperty.Name = DistributionHome;
                childProperty.RawValue = distributionInfo.Url.AbsoluteUri;
            }
            #endregion

            #region PackageCommandArguments = 'scripts';
            if (arguments.Count > 0)
            {
                property = AddNode(root, JsonNodeType.Object);
                property.Name = PackageCommandArguments;

                foreach (KeyValuePair<string, string> argument in arguments)
                {
                    JsonNode node = AddNode(property, JsonNodeType.String);
                    node.RawValue = argument.Value;
                    node.Name = argument.Key;
                }
            }
            #endregion

            return base.Save(stream, encoding, formatted);
        }
        void SerializeParameter(JsonNode property, KeyValuePair<string, object> param)
        {
            switch (param.Key)
            {
                #region StandaloneProgramPackage = 'master';
                case ParameterNames.StandalonePackage:
                    {
                        property = AddNode(property, JsonNodeType.String);
                        property.Name = ParameterNames.StandalonePackage;
                        property.RawValue = param.Value;
                    }
                    break;
                #endregion

                #region ProgramSubmodulePackage = 'submodule';
                case ParameterNames.SubmodulePackage:
                    {
                        property = AddNode(property, JsonNodeType.String);
                        property.Name = ParameterNames.SubmodulePackage;
                        property.RawValue = param.Value;
                    }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// Decodes a string from NPM naming convention
        /// </summary>
        /// <param name="name">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string Decode(string name)
        {
            StringBuilder result = new StringBuilder(name);
            bool turnToUpper = true;

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == '-')
                {
                    result.Remove(i, 1);
                    i--;

                    turnToUpper = true;
                }
                else if (turnToUpper)
                {
                    result[i] = Char.ToUpperInvariant(result[i]);
                    turnToUpper = false;
                }
            }
            return result.ToString();
        }
    }
}