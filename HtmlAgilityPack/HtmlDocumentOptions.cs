
namespace HtmlAgilityPack
{
    using System.Text;

    /// <summary>
    /// HtmlDocument options controlling parsing and output behavior.
    /// </summary>
    public class HtmlDocumentOptions
    {
        /// <summary>
        /// Defines the default stream encoding to use. Default is UTF8.
        /// </summary>
        public static Encoding DefaultStreamEncoding = Encoding.UTF8;

        /// <summary>
        /// Adds Debugging attributes to node. Default is false.
        /// </summary>
        public bool AddDebuggingAttributes;

        /// <summary>
        /// Defines if closing for non closed nodes must be done at the end or directly in the document.
        /// Setting this to true can actually change how browsers render the page. Default is false.
        /// </summary>
        public bool AutoCloseOnEnd; // close errors at the end

        /// <summary>
        /// Defines if non closed nodes will be checked at the end of parsing. Default is true.
        /// </summary>
        public bool CheckSyntax = true;

        /// <summary>
        /// Defines if source text must be extracted while parsing errors.
        /// If the document has a lot of errors, or cascading errors, parsing performance can be dramatically affected if set to true.
        /// Default is false.
        /// </summary>
        public bool ExtractErrorSourceText;

        // turning this on can dramatically slow performance if a lot of errors are detected

        /// <summary>
        /// Defines the maximum length of source text or parse errors. Default is 100.
        /// </summary>
        public int ExtractErrorSourceTextMaxLength = 100;

        /// <summary>
        /// Defines if LI, TR, TH, TD tags must be partially fixed when nesting errors are detected. Default is false.
        /// </summary>
        public bool FixNestedTags; // fix li, tr, th, td tags

        /// <summary>
        /// Defines if output must conform to XML, instead of HTML.
        /// </summary>
        public bool OutputAsXml;

        /// <summary>
        /// Defines if attribute value output must be optimized (not bound with double quotes if it is possible). Default is false.
        /// </summary>
        public bool OutputOptimizeAttributeValues;

        /// <summary>
        /// Defines if name must be output with it's original case. Useful for asp.net tags and attributes
        /// </summary>
        public bool OutputOriginalCase;

        /// <summary>
        /// Defines if name must be output in uppercase. Default is false.
        /// </summary>
        public bool OutputUpperCase;

        /// <summary>
        /// Defines if declared encoding must be read from the document.
        /// Declared encoding is determined using the meta http-equiv="content-type" content="text/html;charset=XXXXX" html node.
        /// Default is true.
        /// </summary>
        public bool ReadEncoding = true;

        /// <summary>
        /// Defines the name of a node that will throw the StopperNodeException when found as an end node. Default is null.
        /// </summary>
        public string StopperNodeName;

        /// <summary>
        /// Defines if the 'id' attribute must be specifically used. Default is true.
        /// </summary>
        public bool UseIdAttribute = true;

        /// <summary>
        /// Defines if empty nodes must be written as closed during output. Default is false.
        /// </summary>
        public bool WriteEmptyNodes;
    }
}
