// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A utility class to replace special characters by entities and vice-versa.
    /// Follows HTML 4.0 specification found at http://www.w3.org/TR/html4/sgml/entities.html
    /// </summary>
    public class HtmlEntity
    {
        private enum ParseState
        {
            Text,
            EntityStart
        }

        private static readonly int _maxEntitySize;
        private static Dictionary<int, string> _entityName;
        private static Dictionary<string, int> _entityValue;

        /// <summary>
        /// A collection of entities indexed by name.
        /// </summary>
        public static Dictionary<int, string> EntityName
        {
            get { return _entityName; }
        }

        /// <summary>
        /// A collection of entities indexed by value.
        /// </summary>
        public static Dictionary<string, int> EntityValue
        {
            get { return _entityValue; }
        }

        #region Constructors

        static HtmlEntity()
        {
            _entityName = new Dictionary<int, string>(250);
            _entityValue = new Dictionary<string, int>(250);

            _entityValue.Add("nbsp", 160);
            _entityValue.Add("iexcl", 161);
            _entityValue.Add("cent", 162);
            _entityValue.Add("pound", 163);
            _entityValue.Add("curren", 164);
            _entityValue.Add("yen", 165);
            _entityValue.Add("brvbar", 166);
            _entityValue.Add("sect", 167);
            _entityValue.Add("uml", 168);
            _entityValue.Add("copy", 169);
            _entityValue.Add("ordf", 170);
            _entityValue.Add("laquo", 171);
            _entityValue.Add("not", 172);
            _entityValue.Add("shy", 173);
            _entityValue.Add("reg", 174);
            _entityValue.Add("macr", 175);
            _entityValue.Add("deg", 176);
            _entityValue.Add("plusmn", 177);
            _entityValue.Add("sup2", 178);
            _entityValue.Add("sup3", 179);
            _entityValue.Add("acute", 180);
            _entityValue.Add("micro", 181);
            _entityValue.Add("para", 182);
            _entityValue.Add("middot", 183);
            _entityValue.Add("cedil", 184);
            _entityValue.Add("sup1", 185);
            _entityValue.Add("ordm", 186);
            _entityValue.Add("raquo", 187);
            _entityValue.Add("frac14", 188);
            _entityValue.Add("frac12", 189);
            _entityValue.Add("frac34", 190);
            _entityValue.Add("iquest", 191);
            _entityValue.Add("Agrave", 192);
            _entityValue.Add("Aacute", 193);
            _entityValue.Add("Acirc", 194);
            _entityValue.Add("Atilde", 195);
            _entityValue.Add("Auml", 196);
            _entityValue.Add("Aring", 197);
            _entityValue.Add("AElig", 198);
            _entityValue.Add("Ccedil", 199);
            _entityValue.Add("Egrave", 200);
            _entityValue.Add("Eacute", 201);
            _entityValue.Add("Ecirc", 202);
            _entityValue.Add("Euml", 203);
            _entityValue.Add("Igrave", 204);
            _entityValue.Add("Iacute", 205);
            _entityValue.Add("Icirc", 206);
            _entityValue.Add("Iuml", 207);
            _entityValue.Add("ETH", 208);
            _entityValue.Add("Ntilde", 209);
            _entityValue.Add("Ograve", 210);
            _entityValue.Add("Oacute", 211);
            _entityValue.Add("Ocirc", 212);
            _entityValue.Add("Otilde", 213);
            _entityValue.Add("Ouml", 214);
            _entityValue.Add("times", 215);
            _entityValue.Add("Oslash", 216);
            _entityValue.Add("Ugrave", 217);
            _entityValue.Add("Uacute", 218);
            _entityValue.Add("Ucirc", 219);
            _entityValue.Add("Uuml", 220);
            _entityValue.Add("Yacute", 221);
            _entityValue.Add("THORN", 222);
            _entityValue.Add("szlig", 223);
            _entityValue.Add("agrave", 224);
            _entityValue.Add("aacute", 225);
            _entityValue.Add("acirc", 226);
            _entityValue.Add("atilde", 227);
            _entityValue.Add("auml", 228);
            _entityValue.Add("aring", 229);
            _entityValue.Add("aelig", 230);
            _entityValue.Add("ccedil", 231);
            _entityValue.Add("egrave", 232);
            _entityValue.Add("eacute", 233);
            _entityValue.Add("ecirc", 234);
            _entityValue.Add("euml", 235);
            _entityValue.Add("igrave", 236);
            _entityValue.Add("iacute", 237);
            _entityValue.Add("icirc", 238);
            _entityValue.Add("iuml", 239);
            _entityValue.Add("eth", 240);
            _entityValue.Add("ntilde", 241);
            _entityValue.Add("ograve", 242);
            _entityValue.Add("oacute", 243);
            _entityValue.Add("ocirc", 244);
            _entityValue.Add("otilde", 245);
            _entityValue.Add("ouml", 246);
            _entityValue.Add("divide", 247);
            _entityValue.Add("oslash", 248);
            _entityValue.Add("ugrave", 249);
            _entityValue.Add("uacute", 250);
            _entityValue.Add("ucirc", 251);
            _entityValue.Add("uuml", 252);
            _entityValue.Add("yacute", 253);
            _entityValue.Add("thorn", 254);
            _entityValue.Add("yuml", 255);
            _entityValue.Add("fnof", 402);
            _entityValue.Add("Alpha", 913);
            _entityValue.Add("Beta", 914);
            _entityValue.Add("Gamma", 915);
            _entityValue.Add("Delta", 916);
            _entityValue.Add("Epsilon", 917);
            _entityValue.Add("Zeta", 918);
            _entityValue.Add("Eta", 919);
            _entityValue.Add("Theta", 920);
            _entityValue.Add("Iota", 921);
            _entityValue.Add("Kappa", 922);
            _entityValue.Add("Lambda", 923);
            _entityValue.Add("Mu", 924);
            _entityValue.Add("Nu", 925);
            _entityValue.Add("Xi", 926);
            _entityValue.Add("Omicron", 927);
            _entityValue.Add("Pi", 928);
            _entityValue.Add("Rho", 929);
            _entityValue.Add("Sigma", 931);
            _entityValue.Add("Tau", 932);
            _entityValue.Add("Upsilon", 933);
            _entityValue.Add("Phi", 934);
            _entityValue.Add("Chi", 935);
            _entityValue.Add("Psi", 936);
            _entityValue.Add("Omega", 937);
            _entityValue.Add("alpha", 945);
            _entityValue.Add("beta", 946);
            _entityValue.Add("gamma", 947);
            _entityValue.Add("delta", 948);
            _entityValue.Add("epsilon", 949);
            _entityValue.Add("zeta", 950);
            _entityValue.Add("eta", 951);
            _entityValue.Add("theta", 952);
            _entityValue.Add("iota", 953);
            _entityValue.Add("kappa", 954);
            _entityValue.Add("lambda", 955);
            _entityValue.Add("mu", 956);
            _entityValue.Add("nu", 957);
            _entityValue.Add("xi", 958);
            _entityValue.Add("omicron", 959);
            _entityValue.Add("pi", 960);
            _entityValue.Add("rho", 961);
            _entityValue.Add("sigmaf", 962);
            _entityValue.Add("sigma", 963);
            _entityValue.Add("tau", 964);
            _entityValue.Add("upsilon", 965);
            _entityValue.Add("phi", 966);
            _entityValue.Add("chi", 967);
            _entityValue.Add("psi", 968);
            _entityValue.Add("omega", 969);
            _entityValue.Add("thetasym", 977);
            _entityValue.Add("upsih", 978);
            _entityValue.Add("piv", 982);
            _entityValue.Add("bull", 8226);
            _entityValue.Add("hellip", 8230);
            _entityValue.Add("prime", 8242);
            _entityValue.Add("Prime", 8243);
            _entityValue.Add("oline", 8254);
            _entityValue.Add("frasl", 8260);
            _entityValue.Add("weierp", 8472);
            _entityValue.Add("image", 8465);
            _entityValue.Add("real", 8476);
            _entityValue.Add("trade", 8482);
            _entityValue.Add("alefsym", 8501);
            _entityValue.Add("larr", 8592);
            _entityValue.Add("uarr", 8593);
            _entityValue.Add("rarr", 8594);
            _entityValue.Add("darr", 8595);
            _entityValue.Add("harr", 8596);
            _entityValue.Add("crarr", 8629);
            _entityValue.Add("lArr", 8656);
            _entityValue.Add("uArr", 8657);
            _entityValue.Add("rArr", 8658);
            _entityValue.Add("dArr", 8659);
            _entityValue.Add("hArr", 8660);
            _entityValue.Add("forall", 8704);
            _entityValue.Add("part", 8706);
            _entityValue.Add("exist", 8707);
            _entityValue.Add("empty", 8709);
            _entityValue.Add("nabla", 8711);
            _entityValue.Add("isin", 8712);
            _entityValue.Add("notin", 8713);
            _entityValue.Add("ni", 8715);
            _entityValue.Add("prod", 8719);
            _entityValue.Add("sum", 8721);
            _entityValue.Add("minus", 8722);
            _entityValue.Add("lowast", 8727);
            _entityValue.Add("radic", 8730);
            _entityValue.Add("prop", 8733);
            _entityValue.Add("infin", 8734);
            _entityValue.Add("ang", 8736);
            _entityValue.Add("and", 8743);
            _entityValue.Add("or", 8744);
            _entityValue.Add("cap", 8745);
            _entityValue.Add("cup", 8746);
            _entityValue.Add("int", 8747);
            _entityValue.Add("there4", 8756);
            _entityValue.Add("sim", 8764);
            _entityValue.Add("cong", 8773);
            _entityValue.Add("asymp", 8776);
            _entityValue.Add("ne", 8800);
            _entityValue.Add("equiv", 8801);
            _entityValue.Add("le", 8804);
            _entityValue.Add("ge", 8805);
            _entityValue.Add("sub", 8834);
            _entityValue.Add("sup", 8835);
            _entityValue.Add("nsub", 8836);
            _entityValue.Add("sube", 8838);
            _entityValue.Add("supe", 8839);
            _entityValue.Add("oplus", 8853);
            _entityValue.Add("otimes", 8855);
            _entityValue.Add("perp", 8869);
            _entityValue.Add("sdot", 8901);
            _entityValue.Add("lceil", 8968);
            _entityValue.Add("rceil", 8969);
            _entityValue.Add("lfloor", 8970);
            _entityValue.Add("rfloor", 8971);
            _entityValue.Add("lang", 9001);
            _entityValue.Add("rang", 9002);
            _entityValue.Add("loz", 9674);
            _entityValue.Add("spades", 9824);
            _entityValue.Add("clubs", 9827);
            _entityValue.Add("hearts", 9829);
            _entityValue.Add("diams", 9830);
            _entityValue.Add("quot", 34);
            _entityValue.Add("amp", 38);
            _entityValue.Add("lt", 60);
            _entityValue.Add("gt", 62);
            _entityValue.Add("OElig", 338);
            _entityValue.Add("oelig", 339);
            _entityValue.Add("Scaron", 352);
            _entityValue.Add("scaron", 353);
            _entityValue.Add("Yuml", 376);
            _entityValue.Add("circ", 710);
            _entityValue.Add("tilde", 732);
            _entityValue.Add("ensp", 8194);
            _entityValue.Add("emsp", 8195);
            _entityValue.Add("thinsp", 8201);
            _entityValue.Add("zwnj", 8204);
            _entityValue.Add("zwj", 8205);
            _entityValue.Add("lrm", 8206);
            _entityValue.Add("rlm", 8207);
            _entityValue.Add("ndash", 8211);
            _entityValue.Add("mdash", 8212);
            _entityValue.Add("lsquo", 8216);
            _entityValue.Add("rsquo", 8217);
            _entityValue.Add("sbquo", 8218);
            _entityValue.Add("ldquo", 8220);
            _entityValue.Add("rdquo", 8221);
            _entityValue.Add("bdquo", 8222);
            _entityValue.Add("dagger", 8224);
            _entityValue.Add("Dagger", 8225);
            _entityValue.Add("permil", 8240);
            _entityValue.Add("lsaquo", 8249);
            _entityValue.Add("rsaquo", 8250);
            _entityValue.Add("euro", 8364);

            foreach (var item in _entityValue)
            {
                _entityName.Add(item.Value, item.Key);
            }

            _maxEntitySize = 8 + 1; // we add the # char
        }

        private HtmlEntity()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Replace known entities by characters.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <returns>The result text.</returns>
        public static string DeEntitize(string text)
        {
            if (text == null)
            {
                return null;
            }

            if (text.Length == 0)
            {
                return text;
            }

            StringBuilder sb = new StringBuilder(text.Length);
            StringBuilder entity = new StringBuilder(10);

            ParseState state = ParseState.Text;

            for (int i = 0; i < text.Length; i++)
            {
                switch (state)
                {
                    case ParseState.Text:
                        switch (text[i])
                        {
                            case '&':
                                state = ParseState.EntityStart;
                                break;

                            default:
                                sb.Append(text[i]);
                                break;
                        }
                        break;

                    case ParseState.EntityStart:
                        switch (text[i])
                        {
                            case ';':
                                if (entity.Length == 0)
                                {
                                    sb.Append("&;");
                                }
                                else
                                {
                                    if (entity[0] == '#')
                                    {
                                        string e = entity.ToString();
                                        try
                                        {
                                            string codeStr = e.Substring(1).Trim().ToLower();
                                            int fromBase;
                                            if (codeStr.StartsWith("x"))
                                            {
                                                fromBase = 16;
                                                codeStr = codeStr.Substring(1);
                                            }
                                            else
                                            {
                                                fromBase = 10;
                                            }
                                            int code = Convert.ToInt32(codeStr, fromBase);
                                            sb.Append(Convert.ToChar(code));
                                        }
                                        catch
                                        {
                                            sb.Append("&#" + e + ";");
                                        }
                                    }
                                    else
                                    {
                                        int code;
                                        if (_entityValue.TryGetValue(entity.ToString(), out code))
                                        {
                                            sb.Append(Convert.ToChar(code));
                                        }
                                        else
                                        {
                                            // nope
                                            sb.Append("&" + entity + ";");
                                        }
                                    }
                                    entity.Remove(0, entity.Length);
                                }
                                state = ParseState.Text;
                                break;

                            case '&':
                                // new entity start without end, it was not an entity...
                                sb.Append("&" + entity);
                                entity.Remove(0, entity.Length);
                                break;

                            default:
                                entity.Append(text[i]);
                                if (entity.Length > _maxEntitySize)
                                {
                                    // unknown stuff, just don't touch it
                                    state = ParseState.Text;
                                    sb.Append("&" + entity);
                                    entity.Remove(0, entity.Length);
                                }
                                break;
                        }
                        break;
                }
            }

            // finish the work
            if (state == ParseState.EntityStart)
            {
                sb.Append("&" + entity);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Clone and entitize an HtmlNode. This will affect attribute values and nodes' text. It will also entitize all child nodes.
        /// </summary>
        /// <param name="node">The node to entitize.</param>
        /// <returns>An entitized cloned node.</returns>
        public static HtmlNode Entitize(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            HtmlNode result = node.CloneNode(true);
            if (result.HasAttributes)
                Entitize(result.Attributes);

            if (result.HasChildNodes)
            {
                Entitize(result.ChildNodes);
            }
            else
            {
                if (result.NodeType == HtmlNodeType.Text)
                {
                    ((HtmlTextNode)result).Text = Entitize(((HtmlTextNode)result).Text, true, true);
                }
            }
            return result;
        }


        /// <summary>
        /// Replace characters above 127 by entities.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <returns>The result text.</returns>
        public static string Entitize(string text)
        {
            return Entitize(text, true);
        }

        /// <summary>
        /// Replace characters above 127 by entities.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <param name="useNames">If set to false, the function will not use known entities name. Default is true.</param>
        /// <returns>The result text.</returns>
        public static string Entitize(string text, bool useNames)
        {
            return Entitize(text, useNames, false);
        }

        /// <summary>
        /// Replace characters above 127 by entities.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <param name="useNames">If set to false, the function will not use known entities name. Default is true.</param>
        /// <param name="entitizeSpecial">If set to true, the [quote], [ampersand], [lower than] and [greather than] characters will be entitized.</param>
        /// <returns>The result text</returns>
        public static string Entitize(string text, bool useNames, bool entitizeSpecial)
        {
            if (text == null)
            {
                return null;
            }

            if (text.Length == 0)
            {
                return text;
            }

            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                int code = text[i];

                if (code > 127)
                {
                    string entity;

                    if (useNames && _entityName.TryGetValue(code, out entity))
                    {
                        sb.Append("&" + entity + ";");
                    }
                    else
                    {
                        sb.Append("&#" + code + ";");
                    }
                }
                else if (entitizeSpecial)
                {
                    switch (code)
                    {
                        case '&':
                            sb.Append("&amp;");
                            break;
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '"':
                            sb.Append("&quot;");
                            break;
                        case '\'':
                            sb.Append("&apos;");
                            break;
                        default:
                            sb.Append(text[i]);
                            break;
                    }
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Private Methods

        private static void Entitize(HtmlAttributeCollection collection)
        {
            foreach (HtmlAttribute at in collection)
            {
                at.Value = Entitize(at.Value);
            }
        }

        private static void Entitize(HtmlNodeCollection collection)
        {
            foreach (HtmlNode node in collection)
            {
                if (node.HasAttributes)
                    Entitize(node.Attributes);

                if (node.HasChildNodes)
                {
                    Entitize(node.ChildNodes);
                }
                else
                {
                    if (node.NodeType == HtmlNodeType.Text)
                    {
                        ((HtmlTextNode)node).Text = Entitize(((HtmlTextNode)node).Text, true, true);
                    }
                }
            }
        }

        #endregion
    }
}