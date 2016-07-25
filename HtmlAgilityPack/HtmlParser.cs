﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlAgilityPack
{
    /// <summary>
    /// The HTML parser.
    /// </summary>
    public class HtmlParser
    {
        #region Resetters (nested tags)

        private static readonly string[] ResetterLi = new string[]
        {
            "ul",
            "ol"
        };

        private static readonly string[] ResetterOption = new string[]
        {
            "select"
        };

        private static readonly string[] ResetterTr = new string[]
        {
            "table",
        };

        private static readonly string[] ResetterP = new string[]
        {
            "div",
            "header",
            "footer",
            "article",
            "section"
        };

        private static readonly string[] ResetterThTd = new string[]
        {
            "tr",
            "table",
        };

        #endregion

        private int _c;
        private int _index;

        private int _line;
        private int _lineposition, _maxlineposition;

        private string _remainder;
        private int _remainderOffset;

        private bool _fullcomment;

        private ParseState _oldstate;
        private ParseState _state;

        private HtmlAttribute _currentattribute;

        internal Encoding _declaredencoding;

        internal string Text;

        internal Dictionary<string, HtmlNode> Lastnodes = new Dictionary<string, HtmlNode>();
        internal Dictionary<int, HtmlNode> Openednodes;

        internal List<HtmlParseError> _parseerrors = new List<HtmlParseError>();

        HtmlDocument _document;

        private HtmlNode _currentnode;
        private HtmlNode _documentnode;
        private HtmlNode _lastparentnode;

        HtmlDocumentOptions Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlParser" /> class.
        /// </summary>
        /// <param name="document">The owner document.</param>
        public HtmlParser(HtmlDocument document)
        {
            _document = document;

            Options = document.Options;

            if (Options.CheckSyntax)
            {
                Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                Openednodes = null;
            }
        }

        public HtmlNode Parse(string text)
        {
            int lastquote = 0;

            Lastnodes = new Dictionary<string, HtmlNode>();
            _c = 0;
            _fullcomment = false;
            _parseerrors = new List<HtmlParseError>();
            _line = 1;
            _lineposition = 1;
            _maxlineposition = 1;

            this.Text = text;

            _state = ParseState.Text;
            _oldstate = _state;

            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            _documentnode._innerlength = Text.Length;
            _documentnode._outerlength = Text.Length;

            _remainderOffset = Text.Length;

            _lastparentnode = _documentnode;
            _currentnode = CreateNode(HtmlNodeType.Text, 0);
            _currentattribute = null;

            _index = 0;
            PushNodeStart(HtmlNodeType.Text, 0);
            while (_index < Text.Length)
            {
                _c = Text[_index];
                IncrementPosition();

                switch (_state)
                {
                    case ParseState.Text:
                        if (NewCheck())
                            continue;
                        break;

                    case ParseState.WhichTag:
                        if (NewCheck())
                            continue;
                        if (_c == '/')
                        {
                            PushNodeNameStart(false, _index);
                        }
                        else
                        {
                            PushNodeNameStart(true, _index - 1);
                            DecrementPosition();
                        }
                        _state = ParseState.Tag;
                        break;

                    case ParseState.Tag:
                        if (NewCheck())
                            continue;
                        if (IsWhiteSpace(_c))
                        {
                            PushNodeNameEnd(_index - 1);
                            if (_state != ParseState.Tag)
                                continue;
                            _state = ParseState.BetweenAttributes;
                            continue;
                        }
                        if (_c == '/')
                        {
                            PushNodeNameEnd(_index - 1);
                            if (_state != ParseState.Tag)
                                continue;
                            _state = ParseState.EmptyTag;
                            continue;
                        }
                        if (_c == '>')
                        {
                            PushNodeNameEnd(_index - 1);
                            if (_state != ParseState.Tag)
                                continue;
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            if (_state != ParseState.Tag)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                        }
                        break;

                    case ParseState.BetweenAttributes:
                        if (NewCheck())
                            continue;

                        if (IsWhiteSpace(_c))
                            continue;

                        if ((_c == '/') || (_c == '?'))
                        {
                            _state = ParseState.EmptyTag;
                            continue;
                        }

                        if (_c == '>')
                        {
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }

                            if (_state != ParseState.BetweenAttributes)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }

                        PushAttributeNameStart(_index - 1);
                        _state = ParseState.AttributeName;
                        break;

                    case ParseState.EmptyTag:
                        if (NewCheck())
                            continue;

                        if (_c == '>')
                        {
                            if (!PushNodeEnd(_index, true))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }

                            if (_state != ParseState.EmptyTag)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        _state = ParseState.BetweenAttributes;
                        break;

                    case ParseState.AttributeName:
                        if (NewCheck())
                            continue;

                        if (IsWhiteSpace(_c))
                        {
                            PushAttributeNameEnd(_index - 1);
                            _state = ParseState.AttributeBeforeEquals;
                            continue;
                        }
                        if (_c == '=')
                        {
                            PushAttributeNameEnd(_index - 1);
                            _state = ParseState.AttributeAfterEquals;
                            continue;
                        }
                        if (_c == '>')
                        {
                            PushAttributeNameEnd(_index - 1);
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            if (_state != ParseState.AttributeName)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        break;

                    case ParseState.AttributeBeforeEquals:
                        if (NewCheck())
                            continue;

                        if (IsWhiteSpace(_c))
                            continue;
                        if (_c == '>')
                        {
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            if (_state != ParseState.AttributeBeforeEquals)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        if (_c == '=')
                        {
                            _state = ParseState.AttributeAfterEquals;
                            continue;
                        }
                        // no equals, no whitespace, it's a new attrribute starting
                        _state = ParseState.BetweenAttributes;
                        DecrementPosition();
                        break;

                    case ParseState.AttributeAfterEquals:
                        if (NewCheck())
                            continue;

                        if (IsWhiteSpace(_c))
                            continue;

                        if ((_c == '\'') || (_c == '"'))
                        {
                            _state = ParseState.QuotedAttributeValue;
                            PushAttributeValueStart(_index, _c);
                            lastquote = _c;
                            continue;
                        }
                        if (_c == '>')
                        {
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            if (_state != ParseState.AttributeAfterEquals)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        PushAttributeValueStart(_index - 1);
                        _state = ParseState.AttributeValue;
                        break;

                    case ParseState.AttributeValue:
                        if (NewCheck())
                            continue;

                        if (IsWhiteSpace(_c))
                        {
                            PushAttributeValueEnd(_index - 1);
                            _state = ParseState.BetweenAttributes;
                            continue;
                        }

                        if (_c == '>')
                        {
                            PushAttributeValueEnd(_index - 1);
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            if (_state != ParseState.AttributeValue)
                                continue;
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        break;

                    case ParseState.QuotedAttributeValue:
                        if (_c == lastquote)
                        {
                            PushAttributeValueEnd(_index - 1);
                            _state = ParseState.BetweenAttributes;
                            continue;
                        }
                        if (_c == '<')
                        {
                            if (_index < Text.Length)
                            {
                                if (Text[_index] == '%')
                                {
                                    _oldstate = _state;
                                    _state = ParseState.ServerSideCode;
                                    continue;
                                }
                            }
                        }
                        break;

                    case ParseState.Comment:
                        if (_c == '>')
                        {
                            if (_fullcomment)
                            {
                                if ((Text[_index - 2] != '-') ||
                                    (Text[_index - 3] != '-'))
                                {
                                    continue;
                                }
                            }
                            if (!PushNodeEnd(_index, false))
                            {
                                // stop parsing
                                _index = Text.Length;
                                break;
                            }
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                            continue;
                        }
                        break;

                    case ParseState.ServerSideCode:
                        if (_c == '%')
                        {
                            if (_index < Text.Length)
                            {
                                if (Text[_index] == '>')
                                {
                                    switch (_oldstate)
                                    {
                                        case ParseState.AttributeAfterEquals:
                                            _state = ParseState.AttributeValue;
                                            break;

                                        case ParseState.BetweenAttributes:
                                            PushAttributeNameEnd(_index + 1);
                                            _state = ParseState.BetweenAttributes;
                                            break;

                                        default:
                                            _state = _oldstate;
                                            break;
                                    }
                                    IncrementPosition();
                                }
                            }
                        }
                        break;

                    case ParseState.PcData:
                        // look for </tag + 1 char

                        // check buffer end
                        if ((_currentnode._namelength + 3) <= (Text.Length - (_index - 1)))
                        {
                            if (string.Compare(Text.Substring(_index - 1, _currentnode._namelength + 2),
                                               "</" + _currentnode.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                int c = Text[_index - 1 + 2 + _currentnode.Name.Length];
                                if ((c == '>') || (IsWhiteSpace(c)))
                                {
                                    // add the script as a text node
                                    HtmlNode script = CreateNode(HtmlNodeType.Text,
                                                                 _currentnode._outerstartindex +
                                                                 _currentnode._outerlength);
                                    script._outerlength = _index - 1 - script._outerstartindex;
                                    _currentnode.AppendChild(script);


                                    PushNodeStart(HtmlNodeType.Element, _index - 1);
                                    PushNodeNameStart(false, _index - 1 + 2);
                                    _state = ParseState.Tag;
                                    IncrementPosition();
                                }
                            }
                        }
                        break;
                }
            }

            // finish the current work
            if (_currentnode._namestartindex > 0)
            {
                PushNodeNameEnd(_index);
            }
            PushNodeEnd(_index, false);

            // we don't need this anymore
            Lastnodes.Clear();

            return _documentnode;
        }

        public void FixOpenedNodes()
        {
            if (Openednodes == null) return;

            foreach (HtmlNode node in Openednodes.Values)
            {
                if (!node._starttag) // already reported
                {
                    continue;
                }

                string html;
                if (Options.ExtractErrorSourceText)
                {
                    html = node.OuterHtml;
                    if (html.Length > Options.ExtractErrorSourceTextMaxLength)
                    {
                        html = html.Substring(0, Options.ExtractErrorSourceTextMaxLength);
                    }
                }
                else
                {
                    html = string.Empty;
                }
                AddError(
                    HtmlParseErrorCode.TagNotClosed,
                    node._line, node._lineposition,
                    node._streamposition, //html,
                    "End tag </" + node.Name + "> was not found");
            }

            // we don't need this anymore
            Openednodes.Clear();
            Openednodes = null;
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

        #region Internal Methods

        internal HtmlAttribute CreateAttribute()
        {
            return new HtmlAttribute(_document);
        }

        internal HtmlNode CreateNode(HtmlNodeType type)
        {
            return CreateNode(type, -1);
        }

        internal HtmlNode CreateNode(HtmlNodeType type, int index)
        {
            if (type == HtmlNodeType.Comment)
            {
                return new HtmlCommentNode(_document, index);
            }

            if (type == HtmlNodeType.Text)
            {
                return new HtmlTextNode(_document, index);
            }

            var node = new HtmlNode(type, _document, index);

            if (Openednodes != null && !node.Closed)
            {
                // -1 means the node comes from public
                if (index != -1)
                {
                    // we use the index as the key
                    Openednodes.Add(index, node);
                }
            }

            return node;
        }

        internal void UpdateLastParentNode()
        {
            do
            {
                if (_lastparentnode.Closed)
                    _lastparentnode = _lastparentnode.ParentNode;

            } while ((_lastparentnode != null) && (_lastparentnode.Closed));

            if (_lastparentnode == null)
                _lastparentnode = _documentnode;
        }

        internal void CloseNode(HtmlNode node, HtmlNode endnode, int level = 0)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException(HtmlNode.DepthLevelExceptionMessage);
            }

            if (!_document.Options.AutoCloseOnEnd)
            {
                // close all children
                if (node._childnodes != null)
                {
                    foreach (HtmlNode child in node._childnodes)
                    {
                        if (child.Closed)
                            continue;

                        // create a fake closer node
                        HtmlNode close = new HtmlNode(node.NodeType, _document, -1);
                        close._endnode = close;
                        CloseNode(child, close, level + 1);
                    }
                }
            }

            if (!node.Closed)
            {
                node._endnode = endnode;

                if (Openednodes != null)
                    Openednodes.Remove(node._outerstartindex);

                HtmlNode self = Lastnodes.GetValueOrNull(node.Name);
                if (self == node)
                {
                    Lastnodes.Remove(node.Name);
                    UpdateLastParentNode();
                }

                if (endnode == node)
                    return;

                // create an inner section
                node._innerstartindex = node._outerstartindex + node._outerlength;
                node._innerlength = endnode._outerstartindex - node._innerstartindex;

                // update full length
                node._outerlength = (endnode._outerstartindex + endnode._outerlength) - node._outerstartindex;
            }
        }

        #region Private Methods

        private void AddError(HtmlParseErrorCode code, int line, int linePosition, int streamPosition, string reason)
        {
            HtmlParseError err = new HtmlParseError(code, line, linePosition, streamPosition, null, reason);
            _parseerrors.Add(err);
            return;
        }

        #endregion

        private void CloseCurrentNode()
        {
            if (_currentnode.Closed) // text or document are by def closed
                return;

            bool error = false;
            HtmlNode prev = Lastnodes.GetValueOrNull(_currentnode.Name);

            // find last node of this kind
            if (prev == null)
            {
                if (HtmlNode.IsClosedElement(_currentnode.Name))
                {
                    // </br> will be seen as <br>
                    CloseNode(_currentnode, _currentnode);

                    // add to parent node
                    if (_lastparentnode != null)
                    {
                        HtmlNode foundNode = null;
                        Stack<HtmlNode> futureChild = new Stack<HtmlNode>();
                        for (HtmlNode node = _lastparentnode.LastChild; node != null; node = node.PreviousSibling)
                        {
                            if ((node.Name == _currentnode.Name) && (!node.HasChildNodes))
                            {
                                foundNode = node;
                                break;
                            }
                            futureChild.Push(node);
                        }
                        if (foundNode != null)
                        {
                            while (futureChild.Count != 0)
                            {
                                HtmlNode node = futureChild.Pop();
                                _lastparentnode.RemoveChild(node);
                                foundNode.AppendChild(node);
                            }
                        }
                        else
                        {
                            _lastparentnode.AppendChild(_currentnode);
                        }
                    }
                }
                else
                {
                    // node has no parent
                    // node is not a closed node

                    if (HtmlNode.CanOverlapElement(_currentnode.Name))
                    {
                        // this is a hack: add it as a text node
                        HtmlNode closenode = CreateNode(HtmlNodeType.Text, _currentnode._outerstartindex);
                        closenode._outerlength = _currentnode._outerlength;
                        ((HtmlTextNode)closenode).Text = ((HtmlTextNode)closenode).Text.ToLower();
                        if (_lastparentnode != null)
                        {
                            _lastparentnode.AppendChild(closenode);
                        }
                    }
                    else
                    {
                        if (HtmlNode.IsEmptyElement(_currentnode.Name))
                        {
                            AddError(
                                HtmlParseErrorCode.EndTagNotRequired,
                                _currentnode._line, _currentnode._lineposition,
                                _currentnode._streamposition, //_currentnode.OuterHtml,
                                "End tag </" + _currentnode.Name + "> is not required");
                        }
                        else
                        {
                            // node cannot overlap, node is not empty
                            AddError(
                                HtmlParseErrorCode.TagNotOpened,
                                _currentnode._line, _currentnode._lineposition,
                                _currentnode._streamposition, //_currentnode.OuterHtml,
                                "Start tag <" + _currentnode.Name + "> was not found");
                            error = true;
                        }
                    }
                }
            }
            else
            {


                if (Options.FixNestedTags)
                {
                    if (FindResetterNodes(prev, GetResetters(_currentnode.Name)))
                    {
                        AddError(
                            HtmlParseErrorCode.EndTagInvalidHere,
                            _currentnode._line, _currentnode._lineposition,
                            _currentnode._streamposition, //_currentnode.OuterHtml,
                            "End tag </" + _currentnode.Name + "> invalid here");
                        error = true;
                    }
                }

                if (!error)
                {
                    Lastnodes[_currentnode.Name] = prev._prevwithsamename;
                    CloseNode(prev, _currentnode);
                }
            }


            // we close this node, get grandparent
            if (!error)
            {
                if ((_lastparentnode != null) &&
                    ((!HtmlNode.IsClosedElement(_currentnode.Name)) ||
                     (_currentnode._starttag)))
                {
                    UpdateLastParentNode();
                }
            }
        }

        private string CurrentNodeName()
        {
            return Text.Substring(_currentnode._namestartindex, _currentnode._namelength);
        }


        private void DecrementPosition()
        {
            _index--;
            if (_lineposition == 1)
            {
                _lineposition = _maxlineposition;
                _line--;
            }
            else
            {
                _lineposition--;
            }
        }

        private HtmlNode FindResetterNode(HtmlNode node, string name)
        {
            HtmlNode resetter = Lastnodes.GetValueOrNull(name);
            if (resetter == null)
                return null;

            if (resetter.Closed)
                return null;

            if (resetter._streamposition < node._streamposition)
            {
                return null;
            }

            return resetter;
        }

        private bool FindResetterNodes(HtmlNode node, string[] names)
        {
            if (names == null)
            {
                return false;
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (FindResetterNode(node, names[i]) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void FixNestedTag(string name, string[] resetters)
        {
            if (resetters == null)
            {
                return;
            }

            HtmlNode prev = Lastnodes.GetValueOrNull(_currentnode.Name);

            // if we find a previous unclosed same name node, without a resetter
            // node between, we must close it
            if (prev == null || (Lastnodes[name].Closed))
            {
                return;
            }

            // try to find a resetter node, if found, we do nothing
            if (FindResetterNodes(prev, resetters))
            {
                return;
            }

            // ok we need to close the prev now
            // create a fake closer node
            HtmlNode close = new HtmlNode(prev.NodeType, _document, -1);
            close._endnode = close;
            CloseNode(prev, close);
        }

        private void FixNestedTags()
        {
            // we are only interested by start tags, not closing tags
            if (!_currentnode._starttag)
            {
                return;
            }

            string name = CurrentNodeName();
            FixNestedTag(name, GetResetters(name));
        }

        private string[] GetResetters(string name)
        {
            switch (name)
            {
                case "li":
                    return ResetterLi;

                case "option":
                    return ResetterOption;

                case "tr":
                    return ResetterTr;

                case "p":
                    return ResetterP;

                case "th":
                case "td":
                    return ResetterThTd;

                default:
                    return null;
            }
        }

        private void IncrementPosition()
        {
            _index++;
            _maxlineposition = _lineposition;
            if (_c == 10)
            {
                _lineposition = 1;
                _line++;
            }
            else
            {
                _lineposition++;
            }
        }

        private bool NewCheck()
        {
            if (_c != '<')
            {
                return false;
            }
            if (_index < Text.Length)
            {
                if (Text[_index] == '%')
                {
                    switch (_state)
                    {
                        case ParseState.AttributeAfterEquals:
                            PushAttributeValueStart(_index - 1);
                            break;

                        case ParseState.BetweenAttributes:
                            PushAttributeNameStart(_index - 1);
                            break;

                        case ParseState.WhichTag:
                            PushNodeNameStart(true, _index - 1);
                            _state = ParseState.Tag;
                            break;
                    }
                    _oldstate = _state;
                    _state = ParseState.ServerSideCode;
                    return true;
                }
            }

            if (!PushNodeEnd(_index - 1, true))
            {
                // stop parsing
                _index = Text.Length;
                return true;
            }
            _state = ParseState.WhichTag;
            if ((_index - 1) <= (Text.Length - 2))
            {
                if (Text[_index] == '!')
                {
                    PushNodeStart(HtmlNodeType.Comment, _index - 1);
                    PushNodeNameStart(true, _index);
                    PushNodeNameEnd(_index + 1);
                    _state = ParseState.Comment;
                    if (_index < (Text.Length - 2))
                    {
                        if ((Text[_index + 1] == '-') &&
                            (Text[_index + 2] == '-'))
                        {
                            _fullcomment = true;
                        }
                        else
                        {
                            _fullcomment = false;
                        }
                    }
                    return true;
                }
            }
            PushNodeStart(HtmlNodeType.Element, _index - 1);
            return true;
        }

        private void PushAttributeNameEnd(int index)
        {
            // TODO: Attribute Name
            _currentattribute._namelength = index - _currentattribute._namestartindex;
            _currentattribute._name = Text.Substring(_currentattribute._namestartindex, _currentattribute._namelength);
            _currentnode.Attributes.Add(_currentattribute);
        }

        private void PushAttributeNameStart(int index)
        {
            _currentattribute = CreateAttribute();
            _currentattribute._namestartindex = index;
            _currentattribute._line = _line;
            _currentattribute._lineposition = _lineposition;
        }

        private void PushAttributeValueEnd(int index)
        {
            // TODO: Attribute Value
            _currentattribute._valuelength = index - _currentattribute._valuestartindex;
            _currentattribute._value = Text.Substring(_currentattribute._valuestartindex, _currentattribute._valuelength);
        }

        private void PushAttributeValueStart(int index)
        {
            PushAttributeValueStart(index, 0);
        }

        private void PushAttributeValueStart(int index, int quote)
        {
            _currentattribute._valuestartindex = index;
            if (quote == '\'')
                _currentattribute.QuoteType = AttributeValueQuote.SingleQuote;
        }

        private bool PushNodeEnd(int index, bool close)
        {
            _currentnode._outerlength = index - _currentnode._outerstartindex;

            if (_currentnode._nodetype == HtmlNodeType.Text)
            {
                // forget about void nodes
                if (_currentnode._outerlength > 0)
                {
                    _currentnode._innerlength = _currentnode._outerlength;
                    _currentnode._innerstartindex = _currentnode._outerstartindex;
                    ((HtmlTextNode)_currentnode).Text = Text.Substring(_currentnode._innerstartindex, _currentnode._innerlength);
                    if (_lastparentnode != null)
                    {
                        _lastparentnode.AppendChild(_currentnode);
                    }
                }
            }
            else if (_currentnode._nodetype == HtmlNodeType.Comment)
            {
                // forget about void nodes
                if (_currentnode._outerlength > 0)
                {
                    _currentnode._innerlength = _currentnode._outerlength;
                    _currentnode._innerstartindex = _currentnode._outerstartindex;
                    ((HtmlCommentNode)_currentnode).Comment = Text.Substring(_currentnode._innerstartindex, _currentnode._innerlength);
                    if (_lastparentnode != null)
                    {
                        _lastparentnode.AppendChild(_currentnode);
                    }
                }
            }
            else
            {
                // TODO: Node Name
                _currentnode.Name = Text.Substring(_currentnode._namestartindex, _currentnode._namelength);

                if ((_currentnode._starttag) && (_lastparentnode != _currentnode))
                {
                    // add to parent node
                    if (_lastparentnode != null)
                    {
                        _lastparentnode.AppendChild(_currentnode);
                    }

                    ReadDocumentEncoding(_currentnode);

                    // remember last node of this kind
                    HtmlNode prev = Lastnodes.GetValueOrNull(_currentnode.Name);

                    _currentnode._prevwithsamename = prev;
                    Lastnodes[_currentnode.Name] = _currentnode;

                    // change parent?
                    if ((_currentnode.NodeType == HtmlNodeType.Document) ||
                        (_currentnode.NodeType == HtmlNodeType.Element))
                    {
                        _lastparentnode = _currentnode;
                    }

                    if (HtmlNode.IsCDataElement(CurrentNodeName()))
                    {
                        _state = ParseState.PcData;
                        return true;
                    }

                    if ((HtmlNode.IsClosedElement(_currentnode.Name)) ||
                        (HtmlNode.IsEmptyElement(_currentnode.Name)))
                    {
                        close = true;
                    }
                }
            }

            if ((close) || (!_currentnode._starttag))
            {
                if ((Options.StopperNodeName != null) && (_remainder == null) &&
                    (string.Compare(_currentnode.Name, Options.StopperNodeName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    _remainderOffset = index;
                    _remainder = Text.Substring(_remainderOffset);
                    CloseCurrentNode();
                    return false; // stop parsing
                }
                CloseCurrentNode();
            }
            return true;
        }

        private void PushNodeNameEnd(int index)
        {
            _currentnode._namelength = index - _currentnode._namestartindex;
            if (Options.FixNestedTags)
            {
                FixNestedTags();
            }
        }

        private void PushNodeNameStart(bool starttag, int index)
        {
            _currentnode._starttag = starttag;
            _currentnode._namestartindex = index;
        }

        private void PushNodeStart(HtmlNodeType type, int index)
        {
            _currentnode = CreateNode(type, index);
            _currentnode._line = _line;
            _currentnode._lineposition = _lineposition;
            if (type == HtmlNodeType.Element)
            {
                _currentnode._lineposition--;
            }
            _currentnode._streamposition = index;
        }

        private void ReadDocumentEncoding(HtmlNode node)
        {
            // Detected formats: 
            // <meta http-equiv="content-type" content="text/html;charset=name" />
            // <meta charset="name" />

            // when we append a child, we are in node end, so attributes are already populated

            if (!Options.ReadEncoding) return;

            // quick check, avoids string alloc
            if (node._namelength != 4) return;

            // all nodes names are lowercase
            if (node.Name != "meta") return;

            var value = node.GetAttributeValue("charset");

            if (value == null)
            {
                value = node.GetAttributeValue("http-equiv");

                if (value == null) return;

                if (string.Compare(value, "content-type", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return;
                }

                value = NameValuePairList.GetNameValuePairsValue(node.GetAttributeValue("content"), "charset");
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (string.Equals(value, "utf8", StringComparison.OrdinalIgnoreCase))
            {
                value = "utf-8";
            }

            try
            {
                _declaredencoding = Encoding.GetEncoding(value);
            }
            catch (ArgumentException)
            {
                _declaredencoding = null;
            }

            var streamEncoding = _document.StreamEncoding;

            if (streamEncoding != null && _declaredencoding != null)
            {
#if SILVERLIGHT || PocketPC || METRO
                if (_declaredencoding.WebName != _streamencoding.WebName)
#else
                if (_declaredencoding.WindowsCodePage != streamEncoding.WindowsCodePage)
#endif
                {
                    AddError(
                        HtmlParseErrorCode.CharsetMismatch,
                        _line, _lineposition,
                        _index, //node.OuterHtml,
                        "Encoding mismatch between StreamEncoding: " +
                        streamEncoding.WebName + " and DeclaredEncoding: " +
                        _declaredencoding.WebName);
                }
            }
        }

        #endregion

        private enum ParseState
        {
            Text,
            WhichTag,
            Tag,
            BetweenAttributes,
            EmptyTag,
            AttributeName,
            AttributeBeforeEquals,
            AttributeAfterEquals,
            AttributeValue,
            Comment,
            QuotedAttributeValue,
            ServerSideCode,
            PcData
        }
    }
}
