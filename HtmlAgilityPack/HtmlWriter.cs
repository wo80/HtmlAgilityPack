
namespace HtmlAgilityPack
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Write HtmlNode to string.
    /// </summary>
    public class HtmlWriter
    {
        HtmlDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlWriter" /> class.
        /// </summary>
        /// <param name="document">The owner document.</param>
        public HtmlWriter(HtmlDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Saves all the children of the node to the specified TextWriter.
        /// </summary>
        /// <param name="writer">The TextWriter to which you want to save.</param>
        /// <param name="node">The HtmlNode to write.</param>
        /// <param name="level">Identifies the level we are in starting at root with 0</param>
        public void WriteContentTo(TextWriter writer, HtmlNode node, int level = 0)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException(HtmlNode.DepthLevelExceptionMessage);
            }

            if (!node.HasChildNodes)
            {
                return;
            }

            foreach (HtmlNode cild in node.ChildNodes)
            {
                WriteTo(writer, cild, level + 1);
            }
        }

        /// <summary>
        /// Saves all the children of the node to a string.
        /// </summary>
        /// <param name="node">The HtmlNode to write.</param>
        /// <returns>The saved string.</returns>
        public string WriteContentTo(HtmlNode node)
        {
            StringWriter sw = new StringWriter();
            WriteContentTo(sw, node);
            sw.Flush();
            return sw.ToString();
        }

        /// <summary>
        /// Saves the current node to the specified TextWriter.
        /// </summary>
        /// <param name="writer">The TextWriter to which you want to save.</param>
        /// <param name="node">The HtmlNode to write.</param>
        /// <param name="level">identifies the level we are in starting at root with 0</param>
        public void WriteTo(TextWriter writer, HtmlNode node, int level = 0)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    html = ((HtmlCommentNode)node).Comment;
                    if (_document.OptionOutputAsXml)
                        writer.Write("<!--" + GetXmlComment((HtmlCommentNode)node) + " -->");
                    else
                        writer.Write(html);
                    break;

                case HtmlNodeType.Document:
                    if (_document.OptionOutputAsXml)
                    {
#if SILVERLIGHT || PocketPC || METRO
						writer.Write("<?xml version=\"1.0\" encoding=\"" + _document.GetOutEncoding().WebName + "\"?>");
#else
                        writer.Write("<?xml version=\"1.0\" encoding=\"" + _document.GetOutEncoding().BodyName + "\"?>");
#endif
                        // check there is a root element
                        if (_document.DocumentNode.HasChildNodes)
                        {
                            int rootnodes = _document.DocumentNode._childnodes.Count;
                            if (rootnodes > 0)
                            {
                                HtmlNode xml = _document.GetXmlDeclaration();
                                if (xml != null)
                                    rootnodes--;

                                if (rootnodes > 1)
                                {
                                    if (_document.OptionOutputUpperCase)
                                    {
                                        writer.Write("<SPAN>");
                                        WriteContentTo(writer, node, level);
                                        writer.Write("</SPAN>");
                                    }
                                    else
                                    {
                                        writer.Write("<span>");
                                        WriteContentTo(writer, node, level);
                                        writer.Write("</span>");
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    WriteContentTo(writer, node, level);
                    break;

                case HtmlNodeType.Text:
                    html = ((HtmlTextNode)node).Text;
                    writer.Write(_document.OptionOutputAsXml ? HtmlEncode(html) : html);
                    break;

                case HtmlNodeType.Element:
                    string name = _document.OptionOutputUpperCase ? node.Name.ToUpper() : node.Name;

                    if (_document.OptionOutputOriginalCase)
                        name = node.OriginalName;

                    if (_document.OptionOutputAsXml)
                    {
                        if (name.Length > 0)
                        {
                            if (name[0] == '?')
                                // forget this one, it's been done at the document level
                                break;

                            if (name.Trim().Length == 0)
                                break;
                            name = HtmlDocument.GetXmlName(name);
                        }
                        else
                            break;
                    }

                    writer.Write("<" + name);
                    WriteAttributes(writer, node, false);

                    if (node.HasChildNodes)
                    {
                        writer.Write(">");
                        bool cdata = false;
                        if (_document.OptionOutputAsXml && HtmlNode.IsCDataElement(node.Name))
                        {
                            // this code and the following tries to output things as nicely as possible for old browsers.
                            cdata = true;
                            writer.Write("\r\n//<![CDATA[\r\n");
                        }


                        if (cdata)
                        {
                            if (node.HasChildNodes)
                                // child must be a text
                                WriteTo(writer, node.ChildNodes[0], level);

                            writer.Write("\r\n//]]>//\r\n");
                        }
                        else
                            WriteContentTo(writer, node, level);

                        writer.Write("</" + name);
                        if (!_document.OptionOutputAsXml)
                            WriteAttributes(writer, node, true);

                        writer.Write(">");
                    }
                    else
                    {
                        if (HtmlNode.IsEmptyElement(node.Name))
                        {
                            if ((_document.OptionWriteEmptyNodes) || (_document.OptionOutputAsXml))
                                writer.Write(" />");
                            else
                            {
                                if (node.Name.Length > 0 && node.Name[0] == '?')
                                    writer.Write("?");

                                writer.Write(">");
                            }
                        }
                        else
                            writer.Write("></" + name + ">");
                    }
                    break;
            }
        }

        /// <summary>
        /// Saves the current node to the specified XmlWriter.
        /// </summary>
        /// <param name="writer">The XmlWriter to which you want to save.</param>
        /// <param name="node">The HtmlNode to write.</param>
        public void WriteTo(XmlWriter writer, HtmlNode node)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    writer.WriteComment(GetXmlComment((HtmlCommentNode)node));
                    break;

                case HtmlNodeType.Document:
#if SILVERLIGHT || PocketPC || METRO
						writer.WriteProcessingInstruction("xml",
													  "version=\"1.0\" encoding=\"" +
													  _document.GetOutEncoding().WebName + "\"");
#else
                    writer.WriteProcessingInstruction("xml",
                                                 "version=\"1.0\" encoding=\"" +
                                                 _document.GetOutEncoding().BodyName + "\"");
#endif

                    if (node.HasChildNodes)
                    {
                        foreach (HtmlNode subnode in node.ChildNodes)
                        {
                            WriteTo(writer, subnode);
                        }
                    }
                    break;

                case HtmlNodeType.Text:
                    string html = ((HtmlTextNode)node).Text;
                    writer.WriteString(html);
                    break;

                case HtmlNodeType.Element:
                    string name = _document.OptionOutputUpperCase ? node.Name.ToUpper() : node.Name;

                    if (_document.OptionOutputOriginalCase)
                        name = node.OriginalName;

                    writer.WriteStartElement(name);
                    WriteAttributes(writer, node);

                    if (node.HasChildNodes)
                    {
                        foreach (HtmlNode subnode in node.ChildNodes)
                        {
                            WriteTo(writer, subnode);
                        }
                    }
                    writer.WriteEndElement();
                    break;
            }
        }

        /// <summary>
        /// Saves the current node to a string.
        /// </summary>
        /// <param name="node">The HtmlNode to write.</param>
        /// <returns>The saved string.</returns>
        public string WriteTo(HtmlNode node)
        {
            using (StringWriter sw = new StringWriter())
            {
                WriteTo(sw, node);
                sw.Flush();
                return sw.ToString();
            }
        }

        internal static string GetXmlComment(HtmlCommentNode comment)
        {
            string s = comment.Comment;
            return s.Substring(4, s.Length - 7).Replace("--", " - -");
        }

        internal static void WriteAttributes(XmlWriter writer, HtmlNode node)
        {
            if (!node.HasAttributes)
            {
                return;
            }
            // we use Hashitems to make sure attributes are written only once
            foreach (HtmlAttribute att in node.Attributes.Hashitems.Values)
            {
                writer.WriteAttributeString(att.XmlName, att.Value);
            }
        }

        internal void WriteAttribute(TextWriter writer, HtmlAttribute att)
        {
            string name;
            string quote = att.QuoteType == AttributeValueQuote.DoubleQuote ? "\"" : "'";
            if (_document.OptionOutputAsXml)
            {
                name = _document.OptionOutputUpperCase ? att.XmlName.ToUpper() : att.XmlName;
                if (_document.OptionOutputOriginalCase)
                    name = att.OriginalName;

                writer.Write(" " + name + "=" + quote + HtmlEncode(att.XmlValue) + quote);
            }
            else
            {
                name = _document.OptionOutputUpperCase ? att.Name.ToUpper() : att.Name;
                if (_document.OptionOutputOriginalCase)
                    name = att.OriginalName;
                if (att.Name.Length >= 4)
                {
                    if ((att.Name[0] == '<') && (att.Name[1] == '%') &&
                        (att.Name[att.Name.Length - 1] == '>') && (att.Name[att.Name.Length - 2] == '%'))
                    {
                        writer.Write(" " + name);
                        return;
                    }
                }
                if (_document.OptionOutputOptimizeAttributeValues)
                    if (att.Value.IndexOfAny(new char[] { (char)10, (char)13, (char)9, ' ' }) < 0)
                        writer.Write(" " + name + "=" + att.Value);
                    else
                        writer.Write(" " + name + "=" + quote + att.Value + quote);
                else
                    writer.Write(" " + name + "=" + quote + att.Value + quote);
            }
        }

        internal void WriteAttributes(TextWriter writer, HtmlNode node, bool closing)
        {
            if (!node.HasAttributes)
            {
                return;
            }

            var attributes = node.Attributes;

            if (_document.OptionOutputAsXml)
            {
                // we use Hashitems to make sure attributes are written only once
                foreach (HtmlAttribute att in attributes.Hashitems.Values)
                {
                    WriteAttribute(writer, att);
                }
                return;
            }

            /*
            if (!closing)
            {
                if (attributes != null)
                    foreach (HtmlAttribute att in attributes)
                        WriteAttribute(writer, att);

                if (!_document.OptionAddDebuggingAttributes) return;

                WriteAttribute(writer, _document.CreateAttribute("_closed", Closed.ToString()));
                WriteAttribute(writer, _document.CreateAttribute("_children", ChildNodes.Count.ToString()));

                int i = 0;
                foreach (HtmlNode n in ChildNodes)
                {
                    WriteAttribute(writer, _document.CreateAttribute("_child_" + i,
                                                                           n.Name));
                    i++;
                }
            }
            else
            {
                if (_endnode == null || _endnode._attributes == null || _endnode == this)
                    return;

                foreach (HtmlAttribute att in _endnode._attributes)
                    WriteAttribute(writer, att);

                if (!_document.OptionAddDebuggingAttributes) return;

                WriteAttribute(writer, _document.CreateAttribute("_closed", Closed.ToString()));
                WriteAttribute(writer, _document.CreateAttribute("_children", ChildNodes.Count.ToString()));
            }
            //*/
        }

        /// <summary>
        /// Applies HTML encoding to a specified string.
        /// </summary>
        /// <param name="html">The input string to encode. May not be null.</param>
        /// <returns>The encoded string.</returns>
        public static string HtmlEncode(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            // replace & by &amp; but only once!
            Regex rx = new Regex("&(?!(amp;)|(lt;)|(gt;)|(quot;))", RegexOptions.IgnoreCase);
            return rx.Replace(html, "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
