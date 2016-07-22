// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace HtmlAgilityPack
{
    /// <summary>
    /// Represents a complete HTML document.
    /// </summary>
    public class HtmlDocument
    {
        /// <summary>
        /// Defines the max level we would go deep into the html document
        /// </summary>
        private static int _maxDepthLevel = int.MaxValue;

        //private bool _onlyDetectEncoding;
        private Encoding _declaredencoding;
        private Encoding _streamencoding;

        private HtmlNode _documentnode;

        internal Dictionary<string, HtmlNode> Nodesid;

        private List<HtmlParseError> _parseerrors;

        /// <summary>
        /// HtmlDocument options.
        /// </summary>
        public readonly HtmlDocumentOptions Options = new HtmlDocumentOptions();

        #region Static Members

        internal static readonly string HtmlExceptionRefNotChild = "Reference node must be a child of this node";

        internal static readonly string HtmlExceptionUseIdAttributeFalse =
            "You need to set UseIdAttribute property to true to enable this feature";

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of an HTML document.
        /// </summary>
        public HtmlDocument()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Defines the max level we would go deep into the html document. If this depth level is exceeded, and exception is
        /// thrown.
        /// </summary>
        public static int MaxDepthLevel
        {
            get { return _maxDepthLevel; }
            set { _maxDepthLevel = value; }
        }

        /// <summary>
        /// Gets the document's declared encoding.
        /// Declared encoding is determined using the meta http-equiv="content-type" content="text/html;charset=XXXXX" html node.
        /// </summary>
        public Encoding DeclaredEncoding
        {
            get { return _declaredencoding; }
        }

        /// <summary>
        /// Gets the root node of the document.
        /// </summary>
        public HtmlNode DocumentNode
        {
            get { return _documentnode; }
        }

        /// <summary>
        /// Gets the document's output encoding.
        /// </summary>
        public Encoding Encoding
        {
            get { return GetOutEncoding(); }
        }

        /// <summary>
        /// Gets a list of parse errors found in the document.
        /// </summary>
        public IEnumerable<HtmlParseError> ParseErrors
        {
            get { return _parseerrors; }
        }

        /// <summary>
        /// Gets the document's stream encoding.
        /// </summary>
        public Encoding StreamEncoding
        {
            get { return _streamencoding; }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Loads an HTML document from a stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public static HtmlDocument Load(Stream stream)
        {
            return Load(new StreamReader(stream, HtmlDocumentOptions.DefaultStreamEncoding));
        }

        /// <summary>
        /// Loads an HTML document from a stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static HtmlDocument Load(Stream stream, Encoding encoding)
        {
            return Load(new StreamReader(stream, encoding));
        }

        /// <summary>
        /// Loads the HTML document from the specified TextReader.
        /// </summary>
        /// <param name="reader">The TextReader used to feed the HTML data into the document. May not be null.</param>
        public static HtmlDocument Load(TextReader reader)
        {
            // all Load methods pass down to this one
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var doc = new HtmlDocument();

            var parser = new HtmlParser(doc);

            var options = doc.Options;

            //doc._onlyDetectEncoding = false;

            if (options.UseIdAttribute)
            {
                doc.Nodesid = new Dictionary<string, HtmlNode>();
            }
            else
            {
                doc.Nodesid = null;
            }

            StreamReader sr = reader as StreamReader;
            if (sr != null)
            {
                try
                {
                    // trigger bom read if needed
                    sr.Peek();
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                // ReSharper restore EmptyGeneralCatchClause
                {
                    // void on purpose
                }
                doc._streamencoding = sr.CurrentEncoding;
            }
            else
            {
                doc._streamencoding = null;
            }

            doc._documentnode = parser.Parse(reader.ReadToEnd());
            doc._declaredencoding = parser._declaredencoding;
            doc._parseerrors = parser._parseerrors;

            if (doc.Options.CheckSyntax)
            {
                parser.FixOpenedNodes();
            }

            return doc;
        }

        /// <summary>
        /// Loads the HTML document from the specified string.
        /// </summary>
        /// <param name="html">String containing the HTML document to load. May not be null.</param>
        public static HtmlDocument Parse(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            using (StringReader sr = new StringReader(html))
            {
                return Load(sr);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves the HTML document to the specified stream.
        /// </summary>
        /// <param name="outStream">The stream to which you want to save.</param>
        public void Save(Stream outStream)
        {
            StreamWriter sw = new StreamWriter(outStream, GetOutEncoding());
            Save(sw);
        }

        /// <summary>
        /// Saves the HTML document to the specified stream.
        /// </summary>
        /// <param name="outStream">The stream to which you want to save. May not be null.</param>
        /// <param name="encoding">The character encoding to use. May not be null.</param>
        public void Save(Stream outStream, Encoding encoding)
        {
            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            StreamWriter sw = new StreamWriter(outStream, encoding);
            Save(sw);
        }


        /// <summary>
        /// Saves the HTML document to the specified StreamWriter.
        /// </summary>
        /// <param name="writer">The StreamWriter to which you want to save.</param>
        public void Save(StreamWriter writer)
        {
            Save((TextWriter)writer);
        }

        /// <summary>
        /// Saves the HTML document to the specified TextWriter.
        /// </summary>
        /// <param name="writer">The TextWriter to which you want to save. May not be null.</param>
        public void Save(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            (new HtmlWriter(this)).WriteTo(writer, DocumentNode);

            writer.Flush();
        }

        /// <summary>
        /// Saves the HTML document to the specified XmlWriter.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save.</param>
        public void Save(XmlWriter writer)
        {
            (new HtmlWriter(this)).WriteTo(writer, DocumentNode);

            writer.Flush();
        }

        /// <summary>
        /// Determines if the specified character is considered as a whitespace character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>true if if the specified character is considered as a whitespace character.</returns>
        public static bool IsWhiteSpace(int c)
        {
            if ((c == 10) || (c == 13) || (c == 32) || (c == 9))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an HTML attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute. May not be null.</param>
        /// <returns>The new HTML attribute.</returns>
        public HtmlAttribute CreateAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new HtmlAttribute(this) { Name = name };
        }

        /// <summary>
        /// Creates an HTML attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute. May not be null.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>The new HTML attribute.</returns>
        public HtmlAttribute CreateAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute att = CreateAttribute(name);
            att.Value = value;
            return att;
        }

        /// <summary>
        /// Creates an HTML comment node.
        /// </summary>
        /// <returns>The new HTML comment node.</returns>
        public HtmlCommentNode CreateComment()
        {
            return new HtmlCommentNode(this, -1);
        }

        /// <summary>
        /// Creates an HTML comment node with the specified comment text.
        /// </summary>
        /// <param name="comment">The comment text. May not be null.</param>
        /// <returns>The new HTML comment node.</returns>
        public HtmlCommentNode CreateComment(string comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }

            return new HtmlCommentNode(this, -1) { Comment = comment };
        }

        /// <summary>
        /// Creates an HTML element node with the specified name.
        /// </summary>
        /// <param name="name">The qualified name of the element. May not be null.</param>
        /// <returns>The new HTML node.</returns>
        public HtmlNode CreateElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new HtmlNode(HtmlNodeType.Element, this, -1) { Name = name };
        }

        /// <summary>
        /// Creates an HTML text node.
        /// </summary>
        /// <returns>The new HTML text node.</returns>
        public HtmlTextNode CreateTextNode()
        {
            return new HtmlTextNode(this, -1);
        }

        /// <summary>
        /// Creates an HTML text node with the specified text.
        /// </summary>
        /// <param name="text">The text of the node. May not be null.</param>
        /// <returns>The new HTML text node.</returns>
        public HtmlTextNode CreateTextNode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            HtmlTextNode t = CreateTextNode();
            t.Text = text;
            return t;
        }

        /// <summary>
        /// Gets the HTML node with the specified 'id' attribute value.
        /// </summary>
        /// <param name="id">The attribute id to match. May not be null.</param>
        /// <returns>The HTML node with the matching id or null if not found.</returns>
        public HtmlNode GetElementById(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (Nodesid == null)
            {
                throw new Exception(HtmlExceptionUseIdAttributeFalse);
            }

            return Nodesid.GetValueOrNull(id.ToLower());
        }

        /// <summary>
        /// Gets all HTML nodes with the specified tag name.
        /// </summary>
        /// <param name="name">The element name to match. May not be null.</param>
        /// <returns>HTML nodes with matching tag name.</returns>
        public IEnumerable<HtmlNode> GetElementsByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return _documentnode.Descendants(name);
        }

        #endregion

        #region Internal Methods

        internal Encoding GetOutEncoding()
        {
            // when unspecified, use the stream encoding first
            return _declaredencoding ?? (_streamencoding ?? HtmlDocumentOptions.DefaultStreamEncoding);
        }

        internal HtmlNode GetXmlDeclaration()
        {
            if (!_documentnode.HasChildNodes)
                return null;

            foreach (HtmlNode node in _documentnode._childnodes)
                if (node.Name == "?xml") // it's ok, names are case sensitive
                    return node;

            return null;
        }

        internal void SetIdForNode(HtmlNode node, string id)
        {
            if (!Options.UseIdAttribute)
                return;

            if ((Nodesid == null) || (id == null))
                return;

            if (node == null)
                Nodesid.Remove(id.ToLower());
            else
                Nodesid[id.ToLower()] = node;
        }

        #endregion
    }
}