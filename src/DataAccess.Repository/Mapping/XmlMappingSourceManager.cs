// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlMappingSourceManager.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Mapping
{
    using System.Data.Linq.Mapping;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl;

    /// <summary>
    /// Mapping source manager for XmlMappingSource
    /// </summary>
    public class XmlMappingSourceManager : IMappingSourceManager
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMappingSourceManager"/> class.
        /// </summary>
        /// <param name="mappingStream">
        /// The mapping stream.
        /// </param>
        public XmlMappingSourceManager(Stream mappingStream)
        {
            using (MemoryStream memoryStream = ReadAllToMemoryStream(mappingStream))
            {
                this.NoAssociationsMappingSource = CreateNoAssociationsMappingSource(memoryStream);
                memoryStream.Position = 0;

                this.MappingSource = XmlMappingSource.FromStream(memoryStream);
            }
        }

        #endregion

        #region Implemented Interfaces (Properties)

        #region IMappingSourceManager properties

        /// <summary>
        /// Gets the mapping source.
        /// </summary>
        /// <value>The mapping source.</value>
        public MappingSource MappingSource { get; private set; }

        /// <summary>
        /// Gets the no associations mapping source.
        /// </summary>
        /// <value>The no associations mapping source.</value>
        public MappingSource NoAssociationsMappingSource { get; private set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates the mapping source with no associations in mapping.
        /// </summary>
        /// <param name="mappingStream">
        /// The mapping stream.
        /// </param>
        /// <returns>
        /// The mapping source with no associations
        /// </returns>
        private static MappingSource CreateNoAssociationsMappingSource(Stream mappingStream)
        {
            var stripAssociationsTransform = new XslCompiledTransform();

            Stream sheetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LogicSoftware.DataAccess.Repository.Mapping.StripAssociations.xslt");
            using (XmlReader sheetReader = XmlReader.Create(sheetStream))
            {
                stripAssociationsTransform.Load(sheetReader);
            }

            using (var buffer = new MemoryStream())
            {
                using (XmlReader mappingReader = XmlReader.Create(mappingStream))
                {
                    stripAssociationsTransform.Transform(mappingReader, new XsltArgumentList(), buffer);
                }

                buffer.Position = 0;

                return XmlMappingSource.FromStream(buffer);
            }
        }

        /// <summary>
        /// Reads all stream content to memory stream.
        /// </summary>
        /// <param name="mappingStream">
        /// The mapping stream.
        /// </param>
        /// <returns>
        /// Memory stream.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller.")]
        private static MemoryStream ReadAllToMemoryStream(Stream mappingStream)
        {
            var memoryStream = new MemoryStream();
            var buffer = new byte[65536];
            int bytesRead;
            while ((bytesRead = mappingStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        #endregion
    }
}