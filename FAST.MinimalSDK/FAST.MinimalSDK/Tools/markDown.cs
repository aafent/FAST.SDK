using FAST.Strings;
using System.Text;
using System.Text.RegularExpressions;

namespace FAST.Tools
{
    /*
     * MarkdownSharp
     * -------------
     * a C# Markdown processor
     * 
     * Markdown is a text-to-HTML conversion tool for web writers
     * Copyright (c) 2004 John Gruber
     * https://daringfireball.net/projects/markdown/
     * 
     * Markdown.NET
     * Copyright (c) 2004-2009 Milan Negovan
     * https://www.aspnetresources.com
     * https://aspnetresources.com/blog/markdown_announced.aspx
     * 
     * MarkdownSharp
     * Copyright (c) 2009-2011 Jeff Atwood
     * https://stackoverflow.com
     * https://blog.codinghorror.com
     * https://github.com/StackExchange/MarkdownSharp
     * 
     * History: Milan ported the Markdown processor to C#. He granted license to me so I can open source it
     * and let the community contribute to and improve MarkdownSharp.
     * 
     */

    #region Copyright and license

    /*
    (!) downloaded from: https://github.com/StackExchange/MarkdownSharp
    Aug 2022 : change naming to camel style

    Copyright (c) 2009 - 2010 Jeff Atwood

    http://www.opensource.org/licenses/mit-license.php

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

    Copyright (c) 2003-2004 John Gruber
    <http://daringfireball.net/>   
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are
    met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.

    * Neither the name "Markdown" nor the names of its contributors may
      be used to endorse or promote products derived from this software
      without specific prior written permission.

    This software is provided by the copyright holders and contributors "as
    is" and any express or implied warranties, including, but not limited
    to, the implied warranties of merchantability and fitness for a
    particular purpose are disclaimed. In no event shall the copyright owner
    or contributors be liable for any direct, indirect, incidental, special,
    exemplary, or consequential damages (including, but not limited to,
    procurement of substitute goods or services; loss of use, data, or
    profits; or business interruption) however caused and on any theory of
    liability, whether in contract, strict liability, or tort (including
    negligence or otherwise) arising in any way out of the use of this
    software, even if advised of the possibility of such damage.
    */

    #endregion


    /// <summary>
    /// Options for configuring MarkdownSharp.
    /// see: https://commonmark.org/
    /// </summary>
    public class markdownOptions
    {
        /// <summary>
        /// when true, (most) bare plain URLs are auto-hyperlinked  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool autoHyperlink { get; set; }

        /// <summary>
        /// when true, RETURN becomes a literal newline  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool autoNewlines { get; set; }

        /// <summary>
        /// use ">" for HTML output, or " />" for XHTML output
        /// </summary>
        public string emptyElementSuffix { get; set; }

        /// <summary>
        /// when false, email addresses will never be auto-linked  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool linkEmails { get; set; }

        /// <summary>
        /// when true, bold and italic require non-word characters on either side  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool strictBoldItalic { get; set; }

        /// <summary>
        /// when true, asterisks may be used for intraword emphasis
        /// this does nothing if StrictBoldItalic is false
        /// </summary>
        public bool asteriskIntraWordEmphasis { get; set; }
    }

    /// <summary>
    /// Markdown is a text-to-HTML conversion tool for web writers. 
    /// Markdown allows you to write using an easy-to-read, easy-to-write plain text format, 
    /// then convert it to structurally valid XHTML (or HTML).
    /// for more see: https://commonmark.org/
    /// </summary>
    public class markdown
    {
        // last version before migration "1.13"
        private const string _version = "2.0";

        #region Constructors and Options

        /// <summary>
        /// Create a new Markdown instance using default options
        /// </summary>
        public markdown() : this(false)
        {
        }

        /// <summary>
        /// Create a new Markdown instance and optionally load options from a configuration
        /// file. There they should be stored in the appSettings section, available options are:
        ///     Markdown.StrictBoldItalic (true/false)
        ///     Markdown.EmptyElementSuffix (">" or " />" without the quotes)
        ///     Markdown.LinkEmails (true/false)
        ///     Markdown.AutoNewLines (true/false)
        ///     Markdown.AutoHyperlink (true/false)
        ///     Markdown.AsteriskIntraWordEmphasis (true/false)
        /// </summary>
        public markdown(bool loadOptionsFromConfigFile)
        {
            if (!loadOptionsFromConfigFile) return;
            
            throw new NotImplementedException("Load option from config file is not implemented yet");

            /* TODO (MIGRATION): 
            var settings = ConfigurationManager.AppSettings;
            foreach (string key in settings.Keys)
            {
                switch (key)
                {
                    case "Markdown.AutoHyperlink":
                        AutoHyperlink = Convert.ToBoolean(settings[key]);
                        break;
                    case "Markdown.AutoNewlines":
                        AutoNewLines = Convert.ToBoolean(settings[key]);
                        break;
                    case "Markdown.EmptyElementSuffix":
                        EmptyElementSuffix = settings[key];
                        break;
                    case "Markdown.LinkEmails":
                        LinkEmails = Convert.ToBoolean(settings[key]);
                        break;
                    case "Markdown.StrictBoldItalic":
                        StrictBoldItalic = Convert.ToBoolean(settings[key]);
                        break;
                    case "Markdown.AsteriskIntraWordEmphasis":
                        AsteriskIntraWordEmphasis = Convert.ToBoolean(settings[key]);
                        break;
                }
            }
            */
        }

        /// <summary>
        /// Create a new Markdown instance and set the options from the MarkdownOptions object.
        /// </summary>
        public markdown(markdownOptions options)
        {
            autoHyperlink = options.autoHyperlink;
            autoNewLines = options.autoNewlines;
            if (!string.IsNullOrEmpty(options.emptyElementSuffix))
                emptyElementSuffix = options.emptyElementSuffix;
            linkEmails = options.linkEmails;
            strictBoldItalic = options.strictBoldItalic;
            asteriskIntraWordEmphasis = options.asteriskIntraWordEmphasis;
        }

        /// <summary>
        /// use ">" for HTML output, or " />" for XHTML output
        /// </summary>
        public string emptyElementSuffix { get; set; } = " />";

        /// <summary>
        /// when false, email addresses will never be auto-linked  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool linkEmails { get; set; } = true;

        /// <summary>
        /// when true, bold and italic require non-word characters on either side  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool strictBoldItalic { get; set; } = false;

        /// <summary>
        /// when true, asterisks may be used for intraword emphasis
        /// this does nothing if StrictBoldItalic is false
        /// </summary>
        public bool asteriskIntraWordEmphasis { get; set; } = false;

        /// <summary>
        /// when true, RETURN becomes a literal newline  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool autoNewLines { get; set; } = false;

        /// <summary>
        /// when true, (most) bare plain URLs are auto-hyperlinked  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool autoHyperlink { get; set; } = false;

        #endregion

        private enum tokenType { Text, Tag }

        private struct token
        {
            public token(tokenType type, string value)
            {
                Type = type;
                Value = value;
            }

            public tokenType Type;
            public string Value;
        }

        /// <summary>
        /// maximum nested depth of [] and () supported by the transform; implementation detail
        /// </summary>
        private const int _nestDepth = 6;

        /// <summary>
        /// Tabs are automatically converted to spaces as part of the transform  
        /// this constant determines how "wide" those tabs become in spaces  
        /// </summary>
        private const int _tabWidth = 4;

        private const string _markerUL = "[*+-]";
        private const string _markerOL = @"\d+[.]";

        private static readonly Dictionary<string, string> _escapeTable;
        private static readonly Dictionary<string, string> _invertedEscapeTable;
        private static readonly Dictionary<string, string> _backslashEscapeTable;

        private readonly Dictionary<string, string> _urls = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _titles = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _htmlBlocks = new Dictionary<string, string>();

        private int _listLevel;
        private const string AutoLinkPreventionMarker = "\x1AP"; // temporarily replaces "://" where auto-linking shouldn't happen

        /// <summary>
        /// In the static constuctor we'll initialize what stays the same across all transforms.
        /// </summary>
        static markdown()
        {
            // Table of hash values for escaped characters:
            _escapeTable = new Dictionary<string, string>();
            _invertedEscapeTable = new Dictionary<string, string>();
            // Table of hash value for backslash escaped characters:
            _backslashEscapeTable = new Dictionary<string, string>();

            string backslashPattern = "";

            foreach (char c in @"\`*_{}[]()>#+-.!/:")
            {
                string key = c.ToString();
                string hash = getHashKey(key, isHtmlBlock: false);
                _escapeTable.Add(key, hash);
                _invertedEscapeTable.Add(hash, key);
                _backslashEscapeTable.Add(@"\" + key, hash);
                backslashPattern += Regex.Escape(@"\" + key) + "|";
            }

            _backslashEscapes = new Regex(backslashPattern.Substring(0, backslashPattern.Length - 1), RegexOptions.Compiled);
        }

        /// <summary>
        /// current version of MarkdownSharp;  
        /// see http://code.google.com/p/markdownsharp/ for the latest code or to contribute
        /// </summary>
        public string version
        {
            get { return _version; }
        }

        /// <summary>
        /// Transforms the provided Markdown-formatted text to HTML;  
        /// see http://en.wikipedia.org/wiki/Markdown
        /// </summary>
        /// <remarks>
        /// The order in which other subs are called here is
        /// essential. Link and image substitutions need to happen before
        /// EscapeSpecialChars(), so that any *'s or _'s in the a
        /// and img tags get encoded.
        /// </remarks>
        public string transform(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            setup();

            text = Normalize(text);

            text = hashHTMLBlocks(text);
            text = stripLinkDefinitions(text);
            text = runBlockGamut(text);
            text = Unescape(text);

            cleanup();

            return text + "\n";
        }




        /// <summary>
        /// Perform transformations that form block-level tags like paragraphs, headers, and list items.
        /// </summary>
        private string runBlockGamut(string text, bool unhash = true, bool createParagraphs = true)
        {
            text = doHeaders(text);
            text = doHorizontalRules(text);
            text = doLists(text);
            text = doCodeBlocks(text);
            text = doBlockQuotes(text);

            // We already ran HashHTMLBlocks() before, in Markdown(), but that
            // was to escape raw HTML in the original Markdown source. This time,
            // we're escaping the markup we've just created, so that we don't wrap
            // <p> tags around block-level tags.
            text = hashHTMLBlocks(text);

            return formParagraphs(text, unhash: unhash, createParagraphs: createParagraphs);
        }

        /// <summary>
        /// Perform transformations that occur *within* block-level tags like paragraphs, headers, and list items.
        /// </summary>
        private string runSpanGamut(string text)
        {
            text = doCodeSpans(text);
            text = EscapeSpecialCharsWithinTagAttributes(text);
            text = EscapeBackslashes(text);

            // Images must come first, because ![foo][f] looks like an anchor.
            text = doImages(text);
            text = doAnchors(text);

            // Must come after DoAnchors(), because you can use < and >
            // delimiters in inline links like [this](<url>).
            text = doAutoLinks(text);

            text = text.Replace(AutoLinkPreventionMarker, "://");

            // (v) process the tables
            text=doTables(text);

            text = EncodeAmpsAndAngles(text);
            text = doItalicsAndBold(text);
            return doHardBreaks(text);
        }

        private static readonly Regex _newlinesLeadingTrailing = new Regex(@"^\n+|\n+\z", RegexOptions.Compiled);
        private static readonly Regex _newlinesMultiple = new Regex(@"\n{2,}", RegexOptions.Compiled);
        private static readonly Regex _leadingWhitespace = new Regex("^[ ]*", RegexOptions.Compiled);

        private static readonly Regex _htmlBlockHash = new Regex("\x1AH\\d+H", RegexOptions.Compiled);

        /// <summary>
        /// splits on two or more newlines, to form "paragraphs";    
        /// each paragraph is then unhashed (if it is a hash and unhashing isn't turned off) or wrapped in HTML p tag
        /// </summary>
        private string formParagraphs(string text, bool unhash = true, bool createParagraphs = true)
        {
            // split on two or more newlines
            string[] grafs = _newlinesMultiple.Split(_newlinesLeadingTrailing.Replace(text, ""));

            for (int i = 0; i < grafs.Length; i++)
            {
                if (grafs[i].Contains("\x1AH"))
                {
                    // unhashify HTML blocks
                    if (unhash)
                    {
                        int sanityCheck = 50; // just for safety, guard against an infinite loop
                        bool keepGoing = true; // as long as replacements where made, keep going
                        while (keepGoing && sanityCheck > 0)
                        {
                            keepGoing = false;
                            grafs[i] = _htmlBlockHash.Replace(grafs[i], match =>
                            {
                                keepGoing = true;
                                return _htmlBlocks[match.Value];
                            });
                            sanityCheck--;
                        }
                        /* if (keepGoing)
                        {
                            // Logging of an infinite loop goes here.
                            // If such a thing should happen, please open a new issue on http://code.google.com/p/markdownsharp/
                            // with the input that caused it.
                        }*/
                    }
                }
                else
                {
                    // do span level processing inside the block, then wrap result in <p> tags
                    grafs[i] = _leadingWhitespace.Replace(runSpanGamut(grafs[i]), createParagraphs ? "<p>" : "") + (createParagraphs ? "</p>" : "");
                }
            }

            return string.Join("\n\n", grafs);
        }

        private void setup()
        {
            // Clear the global hashes. If we don't clear these, you get conflicts
            // from other articles when generating a page which contains more than
            // one article (e.g. an index page that shows the N most recent
            // articles):
            _urls.Clear();
            _titles.Clear();
            _htmlBlocks.Clear();
            _listLevel = 0;
        }

        private void cleanup()
        {
            setup();
        }

        private static string _nestedBracketsPattern;

        /// <summary>
        /// Reusable pattern to match balanced [brackets]. See Friedl's 
        /// "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string getNestedBracketsPattern()
        {
            // in other words [this] and [this[also]] and [this[also[too]]]
            // up to _nestDepth
            if (_nestedBracketsPattern == null)
            {
                _nestedBracketsPattern =
                    stringsHelper.repeatString(@"
                (?>              # Atomic matching
                    [^\[\]]+      # Anything other than brackets
                    |
                    \[
                        ", _nestDepth) + stringsHelper.repeatString(
                    @" \]
                )*"
                    , _nestDepth);
            }

            return _nestedBracketsPattern;
        }

        private static string _nestedParensPattern;

        /// <summary>
        /// Reusable pattern to match balanced (parens). See Friedl's 
        /// "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string getNestedParensPattern()
        {
            // in other words (this) and (this(also)) and (this(also(too)))
            // up to _nestDepth
            if (_nestedParensPattern == null)
            {
                _nestedParensPattern =
                    stringsHelper.repeatString(@"
                (?>              # Atomic matching
                    [^()\s]+      # Anything other than parens or whitespace
                    |
                    \(
                        ", _nestDepth) + stringsHelper.repeatString(
                    @" \)
                )*"
                    , _nestDepth);
            }

            return _nestedParensPattern;
        }

        private static readonly Regex _linkDef = new Regex(string.Format(@"
                    ^[ ]{{0,{0}}}\[([^\[\]]+)\]:  # id = $1
                        [ ]*
                        \n?                   # maybe *one* newline
                        [ ]*
                    <?(\S+?)>?              # url = $2
                        [ ]*
                        \n?                   # maybe one newline
                        [ ]*
                    (?:
                        (?<=\s)             # lookbehind for whitespace
                        [""(]
                        (.+?)               # title = $3
                        ["")]
                        [ ]*
                    )?                      # title is optional
                    (?:\n+|\Z)", _tabWidth - 1), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Strips link definitions from text, stores the URLs and titles in hash references.
        /// </summary>
        /// <remarks>
        /// ^[id]: url "optional title"
        /// </remarks>
        private string stripLinkDefinitions(string text) => _linkDef.Replace(text, new MatchEvaluator(linkEvaluator));

        private string linkEvaluator(Match match)
        {
            string linkID = match.Groups[1].Value.ToLowerInvariant();
            _urls[linkID] = EncodeAmpsAndAngles(match.Groups[2].Value);

            if (match.Groups[3]?.Length > 0)
                _titles[linkID] = match.Groups[3].Value.Replace("\"", "&quot;");

            return "";
        }

        // compiling this monster regex results in worse performance. trust me.
        private static readonly Regex _blocksHtml = new Regex(getBlockPattern(), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// derived pretty much verbatim from PHP Markdown
        /// </summary>
        private static string getBlockPattern()
        {
            // Hashify HTML blocks:
            // We only want to do this for block-level HTML tags, such as headers,
            // lists, and tables. That's because we still want to wrap <p>s around
            // "paragraphs" that are wrapped in non-block-level tags, such as anchors,
            // phrase emphasis, and spans. The list of tags we're looking for is
            // hard-coded:
            //
            // *  List "a" is made of tags which can be both inline or block-level.
            //    These will be treated block-level when the start tag is alone on 
            //    its line, otherwise they're not matched here and will be taken as 
            //    inline later.
            // *  List "b" is made of tags which are always block-level;
            //
            const string blockTagsA = "ins|del";
            const string blockTagsB = "p|div|h[1-6]|blockquote|pre|table|dl|ol|ul|address|script|noscript|form|fieldset|iframe|math";

            // Regular expression for the content of a block tag.
            const string attr = @"
        (?>				            # optional tag attributes
            \s			            # starts with whitespace
            (?>
            [^>""/]+	            # text outside quotes
            |
            /+(?!>)		            # slash not followed by >
            |
            ""[^""]*""		        # text inside double quotes (tolerate >)
            |
            '[^']*'	                # text inside single quotes (tolerate >)
            )*
        )?	
        ";

            string content = stringsHelper.repeatString(@"
            (?>
                [^<]+			        # content without tag
            |
                <\2			        # nested opening tag
                " + attr + @"       # attributes
                (?>
                    />
                |
                    >", _nestDepth) +   // end of opening tag
                        ".*?" +             // last level nested tag content
            stringsHelper.repeatString(@"
                    </\2\s*>	        # closing nested tag
                )
                |				
                <(?!/\2\s*>           # other tags with a different name
                )
            )*", _nestDepth);

            string content2 = content.Replace(@"\2", @"\3");

            // First, look for nested blocks, e.g.:
            // 	<div>
            // 		<div>
            // 		tags for inner block must be indented.
            // 		</div>
            // 	</div>
            //
            // The outermost tags must start at the left margin for this to match, and
            // the inner nested divs must be indented.
            // We need to do this before the next, more liberal match, because the next
            // match will start at the first `<div>` and stop at the first `</div>`.
            string pattern = @"
        (?>
                (?>
                (?<=\n)     # Starting at the beginning of a line
                |           # or
                \A\n?       # the beginning of the doc
                )
                (             # save in $1

                # Match from `\n<tag>` to `</tag>\n`, handling nested tags 
                # in between.
                      
                    <($block_tags_b_re)   # start tag = $2
                    $attr>                # attributes followed by > and \n
                    $content              # content, support nesting
                    </\2>                 # the matching end tag
                    [ ]*                  # trailing spaces
                    (?=\n+|\Z)            # followed by a newline or end of document

                | # Special version for tags of group a.

                    <($block_tags_a_re)   # start tag = $3
                    $attr>[ ]*\n          # attributes followed by >
                    $content2             # content, support nesting
                    </\3>                 # the matching end tag
                    [ ]*                  # trailing spaces
                    (?=\n+|\Z)            # followed by a newline or end of document
                      
                | # Special case just for <hr />. It was easier to make a special 
                # case than to make the other regex more complicated.
                  
                    [ ]{0,$less_than_tab}
                    <hr
                    $attr                 # attributes
                    /?>                   # the matching end tag
                    [ ]*
                    (?=\n{2,}|\Z)         # followed by a blank line or end of document
                  
                | # Special case for standalone HTML comments:
                  
                    (?<=\n\n|\A)            # preceded by a blank line or start of document
                    [ ]{0,$less_than_tab}
                    (?s:
                    <!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->
                    )
                    [ ]*
                    (?=\n{2,}|\Z)            # followed by a blank line or end of document
                  
                | # PHP and ASP-style processor instructions (<? and <%)
                  
                    [ ]{0,$less_than_tab}
                    (?s:
                    <([?%])                # $4
                    .*?
                    \4>
                    )
                    [ ]*
                    (?=\n{2,}|\Z)            # followed by a blank line or end of document
                      
                )
        )";

            pattern = pattern.Replace("$less_than_tab", (_tabWidth - 1).ToString());
            pattern = pattern.Replace("$block_tags_b_re", blockTagsB);
            pattern = pattern.Replace("$block_tags_a_re", blockTagsA);
            pattern = pattern.Replace("$attr", attr);
            pattern = pattern.Replace("$content2", content2);
            return pattern.Replace("$content", content);
        }

        /// <summary>
        /// replaces any block-level HTML blocks with hash entries
        /// </summary>
        private string hashHTMLBlocks(string text)
        {
            return _blocksHtml.Replace(text, new MatchEvaluator(htmlEvaluator));
        }

        private string htmlEvaluator(Match match)
        {
            string text = match.Groups[1].Value;
            string key = getHashKey(text, isHtmlBlock: true);
            _htmlBlocks[key] = text;

            return string.Concat("\n\n", key, "\n\n");
        }

        private static string getHashKey(string s, bool isHtmlBlock)
        {
            var delim = isHtmlBlock ? 'H' : 'E';
            return "\x1A" + delim + Math.Abs(s.GetHashCode()).ToString() + delim;
        }

        private static readonly Regex _htmlTokens = new Regex(@"
        (<!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->)|        # match <!-- foo -->
        (<\?.*?\?>)|                 # match <?foo?> " +
            stringsHelper.repeatString(@" 
        (<[A-Za-z\/!$](?:[^<>]|", _nestDepth - 1) + @" 
        (<[A-Za-z\/!$](?:[^<>]"
            + stringsHelper.repeatString(")*>)", _nestDepth) +
                                        " # match <tag> and </tag>",
            RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// returns an array of HTML tokens comprising the input string. Each token is 
        /// either a tag (possibly with nested, tags contained therein, such 
        /// as &lt;a href="&lt;MTFoo&gt;"&gt;, or a run of text between tags. Each element of the 
        /// array is a two-element array; the first is either 'tag' or 'text'; the second is 
        /// the actual value.
        /// </summary>
        private List<token> tokenizeHTML(string text)
        {
            int pos = 0;
            int tagStart = 0;
            var tokens = new List<token>();

            // this regex is derived from the _tokenize() subroutine in Brad Choate's MTRegex plugin.
            // http://www.bradchoate.com/past/mtregex.php
            foreach (Match m in _htmlTokens.Matches(text))
            {
                tagStart = m.Index;

                if (pos < tagStart)
                    tokens.Add(new token(tokenType.Text, text.Substring(pos, tagStart - pos)));

                tokens.Add(new token(tokenType.Tag, m.Value));
                pos = tagStart + m.Length;
            }

            if (pos < text.Length)
                tokens.Add(new token(tokenType.Text, text.Substring(pos, text.Length - pos)));

            return tokens;
        }

        private static readonly Regex _anchorRef = new Regex(string.Format(@"
        (                               # wrap whole match in $1
            \[
                ({0})                   # link text = $2
            \]

            [ ]?                        # one optional space
            (?:\n[ ]*)?                 # one optional newline followed by spaces

            \[
                (.*?)                   # id = $3
            \]
        )", getNestedBracketsPattern()), RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _anchorInline = new Regex(string.Format(@"
            (                           # wrap whole match in $1
                \[
                    ({0})               # link text = $2
                \]
                \(                      # literal paren
                    [ ]*
                    ({1})               # href = $3
                    [ ]*
                    (                   # $4
                    (['""])           # quote char = $5
                    (.*?)               # title = $6
                    \5                  # matching quote
                    [ ]*                # ignore any spaces between closing quote and )
                    )?                  # title is optional
                \)
            )", getNestedBracketsPattern(), getNestedParensPattern()),
                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _anchorRefShortcut = new Regex(@"
        (                               # wrap whole match in $1
            \[
                ([^\[\]]+)                 # link text = $2; can't contain [ or ]
            \]
        )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown link shortcuts into HTML anchor tags
        /// </summary>
        /// <remarks>
        /// [link text](url "title") 
        /// [link text][id] 
        /// [id] 
        /// </remarks>
        private string doAnchors(string text)
        {
            if (!text.Contains("["))
                return text;

            // First, handle reference-style links: [link text] [id]
            text = _anchorRef.Replace(text, new MatchEvaluator(anchorRefEvaluator));

            // Next, inline-style links: [link text](url "optional title") or [link text](url "optional title")
            text = _anchorInline.Replace(text, new MatchEvaluator(anchorInlineEvaluator));

            //  Last, handle reference-style shortcuts: [link text]
            //  These must come last in case you've also got [link test][1]
            //  or [link test](/foo)
            return _anchorRefShortcut.Replace(text, new MatchEvaluator(anchorRefShortcutEvaluator));
        }

        private string saveFromAutoLinking(string s)
        {
            return s.Replace("://", AutoLinkPreventionMarker);
        }

        private string anchorRefEvaluator(Match match)
        {
            string wholeMatch = match.Groups[1].Value;
            string linkText = saveFromAutoLinking(match.Groups[2].Value);
            string linkID = match.Groups[3].Value.ToLowerInvariant();

            string result;

            // for shortcut links like [this][].
            if (linkID?.Length == 0)
                linkID = linkText.ToLowerInvariant();

            if (_urls.ContainsKey(linkID))
            {
                string url = _urls[linkID];

                url = AttributeSafeUrl(url);

                result = "<a href=\"" + url + "\"";

                if (_titles.ContainsKey(linkID))
                {
                    string title = AttributeEncode(_titles[linkID]);
                    title = AttributeEncode(EscapeBoldItalic(title));
                    result += " title=\"" + title + "\"";
                }

                result += ">" + linkText + "</a>";
            }
            else
            {
                result = wholeMatch;
            }

            return result;
        }

        private string anchorRefShortcutEvaluator(Match match)
        {
            string wholeMatch = match.Groups[1].Value;
            string linkText = saveFromAutoLinking(match.Groups[2].Value);
            string linkID = Regex.Replace(linkText.ToLowerInvariant(), @"[ ]*\n[ ]*", " ");  // lower case and remove newlines / extra spaces

            string result;

            if (_urls.ContainsKey(linkID))
            {
                string url = _urls[linkID];

                url = AttributeSafeUrl(url);

                result = "<a href=\"" + url + "\"";

                if (_titles.ContainsKey(linkID))
                {
                    string title = AttributeEncode(_titles[linkID]);
                    title = EscapeBoldItalic(title);
                    result += " title=\"" + title + "\"";
                }

                result += ">" + linkText + "</a>";
            }
            else
            {
                result = wholeMatch;
            }

            return result;
        }

        private string anchorInlineEvaluator(Match match)
        {
            string linkText = saveFromAutoLinking(match.Groups[2].Value);
            string url = match.Groups[3].Value;
            string title = match.Groups[6].Value;
            string result;

            if (url.StartsWith("<") && url.EndsWith(">"))
                url = url.Substring(1, url.Length - 2); // remove <>'s surrounding URL, if present            

            url = AttributeSafeUrl(url);

            result = string.Format("<a href=\"{0}\"", url);

            if (!string.IsNullOrEmpty(title))
            {
                title = AttributeEncode(title);
                title = EscapeBoldItalic(title);
                result += string.Format(" title=\"{0}\"", title);
            }

            result += string.Format(">{0}</a>", linkText);
            return result;
        }

        private static readonly Regex _imagesRef = new Regex(@"
                (               # wrap whole match in $1
                !\[
                    (.*?)       # alt text = $2
                \]

                [ ]?            # one optional space
                (?:\n[ ]*)?     # one optional newline followed by spaces

                \[
                    (.*?)       # id = $3
                \]

                )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _imagesInline = new Regex(string.Format(@"
            (                     # wrap whole match in $1
            !\[
                (.*?)           # alt text = $2
            \]
            \s?                 # one optional whitespace character
            \(                  # literal paren
                [ ]*
                ({0})           # href = $3
                [ ]*
                (               # $4
                (['""])       # quote char = $5
                (.*?)           # title = $6
                \5              # matching quote
                [ ]*
                )?              # title is optional
            \)
            )", getNestedParensPattern()),
                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown image shortcuts into HTML img tags. 
        /// </summary>
        /// <remarks>
        /// ![alt text][id]
        /// ![alt text](url "optional title")
        /// </remarks>
        private string doImages(string text)
        {
            if (!text.Contains("!["))
                return text;

            // First, handle reference-style labeled images: ![alt text][id]
            text = _imagesRef.Replace(text, new MatchEvaluator(imageReferenceEvaluator));

            // Next, handle inline images:  ![alt text](url "optional title")
            // Don't forget: encode * and _
            return _imagesInline.Replace(text, new MatchEvaluator(imageInlineEvaluator));
        }

        // This prevents the creation of horribly broken HTML when some syntax ambiguities
        // collide. It likely still doesn't do what the user meant, but at least we're not
        // outputting garbage.
        private string escapeImageAltText(string s)
        {
            s = EscapeBoldItalic(s);
            return Regex.Replace(s, @"[\[\]()]", m => _escapeTable[m.ToString()]);
        }

        private string imageReferenceEvaluator(Match match)
        {
            string wholeMatch = match.Groups[1].Value;
            string altText = match.Groups[2].Value;
            string linkID = match.Groups[3].Value.ToLowerInvariant();

            // for shortcut links like ![this][].
            if (linkID?.Length == 0)
                linkID = altText.ToLowerInvariant();

            if (_urls.ContainsKey(linkID))
            {
                string url = _urls[linkID];
                string title = null;

                if (_titles.ContainsKey(linkID))
                    title = _titles[linkID];

                return imageTag(url, altText, title);
            }
            else
            {
                // If there's no such link ID, leave intact:
                return wholeMatch;
            }
        }

        private string imageInlineEvaluator(Match match)
        {
            string alt = match.Groups[2].Value;
            string url = match.Groups[3].Value;
            string title = match.Groups[6].Value;

            if (url.StartsWith("<") && url.EndsWith(">"))
                url = url.Substring(1, url.Length - 2);    // Remove <>'s surrounding URL, if present

            return imageTag(url, alt, title);
        }

        private string imageTag(string url, string altText, string title)
        {
            altText = escapeImageAltText(AttributeEncode(altText));
            url = AttributeSafeUrl(url);
            var result = string.Format("<img src=\"{0}\" alt=\"{1}\"", url, altText);
            if (!string.IsNullOrEmpty(title))
            {
                title = AttributeEncode(EscapeBoldItalic(title));
                result += string.Format(" title=\"{0}\"", title);
            }
            result += emptyElementSuffix;
            return result;
        }

        private static readonly Regex _headerSetext = new Regex(@"
            ^(.+?)
            [ ]*
            \n
            (=+|-+)     # $1 = string of ='s or -'s
            [ ]*
            \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _headerAtx = new Regex(@"
            ^(\#{1,6})  # $1 = string of #'s
            [ ]*
            (.+?)       # $2 = Header text
            [ ]*
            \#*         # optional closing #'s (not counted)
            \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown headers into HTML header tags
        /// </summary>
        /// <remarks>
        /// <para>
        /// Header 1  
        /// ========  
        /// </para>
        /// <para>
        /// Header 2  
        /// --------  
        /// </para>
        /// <para>
        /// # Header 1  
        /// ## Header 2  
        /// ## Header 2 with closing hashes ##  
        /// ...  
        /// ###### Header 6  
        /// </para>
        /// </remarks>
        private string doHeaders(string text)
        {
            text = _headerSetext.Replace(text, new MatchEvaluator(seTextHeaderEvaluator));
            return _headerAtx.Replace(text, new MatchEvaluator(atxHeaderEvaluator));
        }

        /// <summary>
        /// Processing the tables
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string doTables(string text)
        {
            //https://stackoverflow.com/questions/32719745/regex-for-markdown-table

            var tables=markdownHelper.extractTables(text);
            if (tables.Count == 0 ) return text; // no tables found

            StringBuilder html = new();
            var tableLines = tables[0];

            var lines = stringParsingHelper.toLinesArray(tableLines);
            var cells =markdownHelper.toTwoDimArray( lines.ToList() );
            
            if ( cells.Length < 1 ) return text;
           
            string htmlWhiteSpace= "&nbsp;";

            var rows=cells.GetLength(0);
            var cols=cells.GetLength(1);
            string dataStyle= "style=\"text-align:left\"";

            html.AppendLine("<table>");
            for(var row=0; row< rows ; row++)
            {
                if ( row==0) html.Append("<thead>");
                else if (row == 1) html.Append("<tbody>");
                html.Append("<tr>");
                for (var col=0; col<cols ; col++)
                {  
                    html.Append(row==0?$"<th ":"<td ");
                    html.Append(dataStyle);
                    html.Append(">");
                    if (cells[row, col]==null) cells[row, col]=string.Empty;
                    html.Append(string.IsNullOrWhiteSpace(cells[row,col].Trim())? htmlWhiteSpace : cells[row, col]);
                    html.Append(row==0?"</th>":"</td>");
                }
                html.AppendLine("</tr>");
                if (row == 0) html.Append("</thead>");
            }
            if (rows > 0) html.Append("</tbody>");
            html.AppendLine("</table>");

            return html.ToString();
        }




        private string seTextHeaderEvaluator(Match match)
        {
            string header = match.Groups[1].Value;
            int level = match.Groups[2].Value.StartsWith("=") ? 1 : 2;
            return string.Format("<h{1}>{0}</h{1}>\n\n", runSpanGamut(header), level);
        }

        private string atxHeaderEvaluator(Match match)
        {
            string header = match.Groups[2].Value;
            int level = match.Groups[1].Value.Length;
            return string.Format("<h{1}>{0}</h{1}>\n\n", runSpanGamut(header), level);
        }

        private static readonly Regex _horizontalRules = new Regex(@"
        ^[ ]{0,3}         # Leading space
            ([-*_])       # $1: First marker
            (?>           # Repeated marker group
                [ ]{0,2}  # Zero, one, or two spaces.
                \1        # Marker character
            ){2,}         # Group repeated at least twice
            [ ]*          # Trailing spaces
            $             # End of line.
        ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown horizontal rules into HTML hr tags
        /// </summary>
        /// <remarks>
        /// ***  
        /// * * *  
        /// ---
        /// - - -
        /// </remarks>
        private string doHorizontalRules(string text)
        {
            return _horizontalRules.Replace(text, "<hr" + emptyElementSuffix + "\n");
        }

        private static readonly string _wholeList = string.Format(@"
        (                               # $1 = whole list
            (                             # $2
            [ ]{{0,{1}}}
            ({0})                       # $3 = first list item marker
            [ ]+
            )
            (?s:.+?)
            (                             # $4
                \z
            |
                \n{{2,}}
                (?=\S)
                (?!                       # Negative lookahead for another list item marker
                [ ]*
                {0}[ ]+
                )
            )
        )", string.Format("(?:{0}|{1})", _markerUL, _markerOL), _tabWidth - 1);

        private static readonly Regex _listNested = new Regex("^" + _wholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _listTopLevel = new Regex(@"(?:(?<=\n\n)|\A\n?)" + _wholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown lists into HTML ul and ol and li tags
        /// </summary>
        private string doLists(string text)
        {
            // We use a different prefix before nested lists than top-level lists.
            // See extended comment in _ProcessListItems().
            if (_listLevel > 0)
            {
                return _listNested.Replace(text, new MatchEvaluator(listEvaluator));
            }
            else
            {
                return _listTopLevel.Replace(text, new MatchEvaluator(listEvaluator));
            }
        }

        private string listEvaluator(Match match)
        {
            string list = match.Groups[1].Value;
            string marker = match.Groups[3].Value;
            string listType = Regex.IsMatch(marker, _markerUL) ? "ul" : "ol";
            string result;
            string start = "";
            if (listType == "ol")
            {
                int.TryParse(marker.Substring(0, marker.Length - 1), out int firstNumber);
                if (firstNumber != 1 && firstNumber != 0)
                    start = " start=\"" + firstNumber + "\"";
            }

            result = processListItems(list, listType == "ul" ? _markerUL : _markerOL);

            return string.Format("<{0}{1}>\n{2}</{0}>\n", listType, start, result);
        }

        /// <summary>
        /// Process the contents of a single ordered or unordered list, splitting it
        /// into individual list items.
        /// </summary>
        private string processListItems(string list, string marker)
        {
            // The listLevel global keeps track of when we're inside a list.
            // Each time we enter a list, we increment it; when we leave a list,
            // we decrement. If it's zero, we're not in a list anymore.

            // We do this because when we're not inside a list, we want to treat
            // something like this:

            //    I recommend upgrading to version
            //    8. Oops, now this line is treated
            //    as a sub-list.

            // As a single paragraph, despite the fact that the second line starts
            // with a digit-period-space sequence.

            // Whereas when we're inside a list (or sub-list), that line will be
            // treated as the start of a sub-list. What a kludge, huh? This is
            // an aspect of Markdown's syntax that's hard to parse perfectly
            // without resorting to mind-reading. Perhaps the solution is to
            // change the syntax rules such that sub-lists must start with a
            // starting cardinal number; e.g. "1." or "a.".

            _listLevel++;

            // Trim trailing blank lines:
            list = Regex.Replace(list, @"\n{2,}\z", "\n");

            string pattern = string.Format(
                @"(^[ ]*)                    # leading whitespace = $1
            ({0}) [ ]+                 # list marker = $2
            ((?s:.+?)                  # list item text = $3
            (\n+))      
            (?= (\z | \1 ({0}) [ ]+))", marker);

            bool lastItemHadADoubleNewline = false;

            // has to be a closure, so subsequent invocations can share the bool
            string ListItemEvaluator(Match match)
            {
                string item = match.Groups[3].Value;

                bool endsWithDoubleNewline = item.EndsWith("\n\n");
                bool containsDoubleNewline = endsWithDoubleNewline || item.Contains("\n\n");

                var loose = containsDoubleNewline || lastItemHadADoubleNewline;
                // we could correct any bad indentation here..
                item = runBlockGamut(outdent(item) + "\n", unhash: false, createParagraphs: loose);

                lastItemHadADoubleNewline = endsWithDoubleNewline;
                return string.Format("<li>{0}</li>\n", item);
            }

            list = Regex.Replace(list, pattern, new MatchEvaluator(ListItemEvaluator),
                                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            _listLevel--;
            return list;
        }

        private static readonly Regex _codeBlock = new Regex(string.Format(@"
                (?:\n\n|\A\n?)
                (                        # $1 = the code block -- one or more lines, starting with a space
                (?:
                    (?:[ ]{{{0}}})       # Lines must start with a tab-width of spaces
                    .*\n+
                )+
                )
                ((?=^[ ]{{0,{0}}}[^ \t\n])|\Z) # Lookahead for non-space at line-start, or end of doc",
                    _tabWidth), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// /// Turn Markdown 4-space indented code into HTML pre code blocks
        /// </summary>
        private string doCodeBlocks(string text)
        {
            return _codeBlock.Replace(text, new MatchEvaluator(codeBlockEvaluator));
        }

        private string codeBlockEvaluator(Match match)
        {
            string codeBlock = match.Groups[1].Value;

            codeBlock = EncodeCode(outdent(codeBlock));
            codeBlock = _newlinesLeadingTrailing.Replace(codeBlock, "");

            return string.Concat("\n\n<pre><code>", codeBlock, "\n</code></pre>\n\n");
        }

        private static readonly Regex _codeSpan = new Regex(@"
                (?<![\\`])   # Character before opening ` can't be a backslash or backtick
                (`+)      # $1 = Opening run of `
                (?!`)     # and no more backticks -- match the full run
                (.+?)     # $2 = The code block
                (?<!`)
                \1
                (?!`)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown `code spans` into HTML code tags
        /// </summary>
        private string doCodeSpans(string text)
        {
            //    * You can use multiple backticks as the delimiters if you want to
            //        include literal backticks in the code span. So, this input:
            //
            //        Just type ``foo `bar` baz`` at the prompt.
            //
            //        Will translate to:
            //
            //          <p>Just type <code>foo `bar` baz</code> at the prompt.</p>
            //
            //        There's no arbitrary limit to the number of backticks you
            //        can use as delimters. If you need three consecutive backticks
            //        in your code, use four for delimiters, etc.
            //
            //    * You can use spaces to get literal backticks at the edges:
            //
            //          ... type `` `bar` `` ...
            //
            //        Turns to:
            //
            //          ... type <code>`bar`</code> ...         
            //

            return _codeSpan.Replace(text, new MatchEvaluator(codeSpanEvaluator));
        }

        private string codeSpanEvaluator(Match match)
        {
            string span = match.Groups[2].Value;
            span = Regex.Replace(span, "^[ ]*", ""); // leading whitespace
            span = Regex.Replace(span, "[ ]*$", ""); // trailing whitespace
            span = EncodeCode(span);
            span = saveFromAutoLinking(span); // to prevent auto-linking. Not necessary in code *blocks*, but in code spans.

            if ( span.StartsWith("csharp") )
            {
                span=span.Substring("csharp".Length).TrimStart();
                if (span.StartsWith("\n")) span = span.Substring("\n".Length).TrimStart();

                span=FAST.Html.htmlColorHelper.colorFormatByByLanguage(span);
                if ( stringParsingHelper.trySplitBetweenTokens(span,"<pre>","</pre>",out string tmpB, out string spanColor, out string tmpA) )
                {
                    spanColor = spanColor.Replace("<pre>\r\n", "").Replace("\r\n</pre>", "");
                    spanColor =spanColor.Replace("<pre>","").Replace("</pre>","");
                    spanColor=spanColor.Replace("\"keyword\"", "\"token keyword\"");
                    span = spanColor;
                }

                return string.Concat(
                    @"<pre class=""language-csharp"" tabindex=""0""><code class=""language-csharp"">",
                    span,
                    "</code></pre>");
            }

            return string.Concat("<code>", span, "</code>");
        }

        private static readonly Regex _bold = new Regex(@"(\*\*|__) (?=\S) (.+?[*_]*) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _semiStrictBold = new Regex(@"(?=.[*_]|[*_])(^|(?=\W__|(?!\*)[\W_]\*\*|\w\*\*\w).)(\*\*|__)(?!\2)(?=\S)((?:|.*?(?!\2).)(?=\S_|\w|\S\*\*(?:[\W_]|$)).)(?=__(?:\W|$)|\*\*(?:[^*]|$))\2",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _strictBold = new Regex(@"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)\2(?=\S)(.*?\S)\2\2(?!\2)(?=[\W_]|$)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _italic = new Regex(@"(\*|_) (?=\S) (.+?) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _semiStrictItalic = new Regex(@"(?=.[*_]|[*_])(^|(?=\W_|(?!\*)(?:[\W_]\*|\D\*(?=\w)\D)).)(\*|_)(?!\2\2\2)(?=\S)((?:(?!\2).)*?(?=[^\s_]_|(?=\w)\D\*\D|[^\s*]\*(?:[\W_]|$)).)(?=_(?:\W|$)|\*(?:[^*]|$))\2",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _strictItalic = new Regex(@"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)(?=\S)((?:(?!\2).)*?\S)\2(?!\2)(?=[\W_]|$)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown *italics* and **bold** into HTML strong and em tags
        /// </summary>
        private string doItalicsAndBold(string text)
        {
            if (!(text.Contains("*") || text.Contains("_")))
                return text;
            // <strong> must go first, then <em>
            if (strictBoldItalic)
            {
                if (asteriskIntraWordEmphasis)
                {
                    text = _semiStrictBold.Replace(text, "$1<strong>$3</strong>");
                    text = _semiStrictItalic.Replace(text, "$1<em>$3</em>");
                }
                else
                {
                    text = _strictBold.Replace(text, "$1<strong>$3</strong>");
                    text = _strictItalic.Replace(text, "$1<em>$3</em>");
                }
            }
            else
            {
                text = _bold.Replace(text, "<strong>$2</strong>");
                text = _italic.Replace(text, "<em>$2</em>");
            }
            return text;
        }

        /// <summary>
        /// Turn markdown line breaks (two space at end of line) into HTML break tags
        /// </summary>
        private string doHardBreaks(string text)
        {
            if (autoNewLines)
            {
                return Regex.Replace(text, @"\n", string.Format("<br{0}\n", emptyElementSuffix));
            }
            else
            {
                return Regex.Replace(text, @" {2,}\n", string.Format("<br{0}\n", emptyElementSuffix));
            }
        }

        private static readonly Regex _blockquote = new Regex(@"
        (                           # Wrap whole match in $1
            (
            ^[ ]*>[ ]?              # '>' at the start of a line
                .+\n                # rest of the first line
            (.+\n)*                 # subsequent consecutive lines
            \n*                     # blanks
            )+
        )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown > quoted blocks into HTML blockquote blocks
        /// </summary>
        private string doBlockQuotes(string text)
        {
            return _blockquote.Replace(text, new MatchEvaluator(blockQuoteEvaluator));
        }

        private string blockQuoteEvaluator(Match match)
        {
            string bq = match.Groups[1].Value;

            bq = Regex.Replace(bq, "^[ ]*>[ ]?", "", RegexOptions.Multiline);       // trim one level of quoting
            bq = Regex.Replace(bq, "^[ ]+$", "", RegexOptions.Multiline);           // trim whitespace-only lines
            bq = runBlockGamut(bq);                                                  // recurse

            bq = Regex.Replace(bq, "^", "  ", RegexOptions.Multiline);

            // These leading spaces screw with <pre> content, so we need to fix that:
            bq = Regex.Replace(bq, @"(\s*<pre>.+?</pre>)", new MatchEvaluator(blockQuoteEvaluator2), RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            bq = string.Format("<blockquote>\n{0}\n</blockquote>", bq);
            string key = getHashKey(bq, isHtmlBlock: true);
            _htmlBlocks[key] = bq;

            return "\n\n" + key + "\n\n";
        }

        private string blockQuoteEvaluator2(Match match)
        {
            return Regex.Replace(match.Groups[1].Value, "^  ", "", RegexOptions.Multiline);
        }

        private const string _charInsideUrl = @"[-A-Z0-9+&@#/%?=~_|\[\]\(\)!:,\.;" + "\x1a]";
        private const string _charEndingUrl = "[-A-Z0-9+&@#/%=~_|\\[\\])]";

        private static readonly Regex _autolinkBare = new Regex(@"(<|="")?\b(https?|ftp)(://" + _charInsideUrl + "*" + _charEndingUrl + ")(?=$|\\W)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex _endCharRegex = new Regex(_charEndingUrl, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string handleTrailingParens(Match match)
        {
            // The first group is essentially a negative lookbehind -- if there's a < or a =", we don't touch this.
            // We're not using a *real* lookbehind, because of links with in links, like <a href="http://web.archive.org/web/20121130000728/http://www.google.com/">
            // With a real lookbehind, the full link would never be matched, and thus the http://www.google.com *would* be matched.
            // With the simulated lookbehind, the full link *is* matched (just not handled, because of this early return), causing
            // the google link to not be matched again.
            if (match.Groups[1].Success)
                return match.Value;

            var protocol = match.Groups[2].Value;
            var link = match.Groups[3].Value;
            if (!link.EndsWith(")"))
                return "<" + protocol + link + ">";
            var level = 0;
            foreach (Match c in Regex.Matches(link, "[()]"))
            {
                if (c.Value == "(")
                {
                    if (level <= 0)
                        level = 1;
                    else
                        level++;
                }
                else
                {
                    level--;
                }
            }
            var tail = "";
            if (level < 0)
            {
                link = Regex.Replace(link, @"\){1," + -level + "}$", m => { tail = m.Value; return ""; });
            }
            if (tail.Length > 0)
            {
                var lastChar = link[link.Length - 1];
                if (!_endCharRegex.IsMatch(lastChar.ToString()))
                {
                    tail = lastChar + tail;
                    link = link.Substring(0, link.Length - 1);
                }
            }
            return "<" + protocol + link + ">" + tail;
        }

        private static readonly Regex _autoEmailBare = new Regex(@"(<|="")?(?:mailto:)?([-.\w]+\@[-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string emailBareLinkEvaluator(Match match)
        {
            // We matched an opening <, so it's already enclosed
            if (match.Groups[1].Success)
            {
                return match.Value;
            }
            return "<" + match.Value + ">";
        }

        private readonly static Regex _linkEmail = new Regex(@"<
                    (?:mailto:)?
                    (
                    [-.\w]+
                    \@
                    [-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+
                    )
                    >", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Turn angle-delimited URLs into HTML anchor tags
        /// </summary>
        /// <remarks>
        /// &lt;http://www.example.com&gt;
        /// </remarks>
        private string doAutoLinks(string text)
        {
            if (autoHyperlink)
            {
                // fixup arbitrary URLs by adding Markdown < > so they get linked as well
                // note that at this point, all other URL in the text are already hyperlinked as <a href=""></a>
                // *except* for the <http://www.foo.com> case
                text = _autolinkBare.Replace(text, handleTrailingParens);
            }

            // Hyperlinks: <http://foo.com>
            text = Regex.Replace(text, "<((https?|ftp):[^'\">\\s]+)>", new MatchEvaluator(hyperlinkEvaluator));

            if (linkEmails)
            {
                // Email addresses: <address@domain.foo> or <mailto:address@domain.foo>
                // Also allow "address@domain.foo" and "mailto:address@domain.foo", without the <>
                //text = _autoEmailBare.Replace(text, EmailBareLinkEvaluator);
                text = _linkEmail.Replace(text, new MatchEvaluator(emailEvaluator));
            }

            return text;
        }

        private string hyperlinkEvaluator(Match match)
        {
            string link = match.Groups[1].Value;
            string url = AttributeSafeUrl(link);
            return string.Format("<a href=\"{0}\">{1}</a>", url, link);
        }

        private string emailEvaluator(Match match)
        {
            string email = Unescape(match.Groups[1].Value);

            //
            //    Input: an email address, e.g. "foo@example.com"
            //
            //    Output: the email address as a mailto link, with each character
            //            of the address encoded as either a decimal or hex entity, in
            //            the hopes of foiling most address harvesting spam bots. E.g.:
            //
            //      <a href="&#x6D;&#97;&#105;&#108;&#x74;&#111;:&#102;&#111;&#111;&#64;&#101;
            //        x&#x61;&#109;&#x70;&#108;&#x65;&#x2E;&#99;&#111;&#109;">&#102;&#111;&#111;
            //        &#64;&#101;x&#x61;&#109;&#x70;&#108;&#x65;&#x2E;&#99;&#111;&#109;</a>
            //
            //    Based by a filter by Matthew Wickline, posted to the BBEdit-Talk
            //    mailing list: <http://tinyurl.com/yu7ue>
            //
            email = "mailto:" + email;

            // leave ':' alone (to spot mailto: later) 
            email = EncodeEmailAddress(email);

            email = string.Format("<a href=\"{0}\">{0}</a>", email);

            // strip the mailto: from the visible part
            return Regex.Replace(email, "\">.+?:", "\">");
        }

        private static readonly Regex _outDent = new Regex("^[ ]{1," + _tabWidth + "}", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Remove one level of line-leading spaces
        /// </summary>
        private string outdent(string block)
        {
            return _outDent.Replace(block, "");
        }

        #region Encoding and Normalization

        /// <summary>
        /// encodes email address randomly  
        /// roughly 10% raw, 45% hex, 45% dec 
        /// note that @ is always encoded and : never is
        /// </summary>
        private string EncodeEmailAddress(string addr)
        {
            var sb = new StringBuilder(addr.Length * 5);
            var rand = new Random();
            int r;
            foreach (char c in addr)
            {
                r = rand.Next(1, 100);
                if ((r > 90 || c == ':') && c != '@')
                    sb.Append(c);                         // m
                else if (r < 45)
                    sb.AppendFormat("&#x{0:x};", (int)c); // &#x6D
                else
                    sb.AppendFormat("&#{0};", (int)c);    // &#109
            }
            return sb.ToString();
        }

        private static readonly Regex _codeEncoder = new Regex(@"&|<|>|\\|\*|_|\{|\}|\[|\]", RegexOptions.Compiled);

        /// <summary>
        /// Encode/escape certain Markdown characters inside code blocks and spans where they are literals
        /// </summary>
        private string EncodeCode(string code)
        {
            return _codeEncoder.Replace(code, EncodeCodeEvaluator);
        }

        private string EncodeCodeEvaluator(Match match)
        {
            switch (match.Value)
            {
                // Encode all ampersands; HTML entities are not
                // entities within a Markdown code span.
                case "&":
                    return "&amp;";
                // Do the angle bracket song and dance
                case "<":
                    return "&lt;";
                case ">":
                    return "&gt;";
                // escape characters that are magic in Markdown
                default:
                    return _escapeTable[match.Value];
            }
        }

        private static readonly Regex _amps = new Regex("&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex _angles = new Regex(@"<(?![A-Za-z/?\$!])", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        /// <summary>
        /// Encode any ampersands (that aren't part of an HTML entity) and left or right angle brackets
        /// </summary>
        private string EncodeAmpsAndAngles(string s)
        {
            s = _amps.Replace(s, "&amp;");
            return _angles.Replace(s, "&lt;");
        }

        private static readonly Regex _backslashEscapes;

        /// <summary>
        /// Encodes any escaped characters such as \`, \*, \[ etc
        /// </summary>
        private string EscapeBackslashes(string s) => _backslashEscapes.Replace(s, new MatchEvaluator(EscapeBackslashesEvaluator));

        private string EscapeBackslashesEvaluator(Match match) => _backslashEscapeTable[match.Value];

        // note: this space MATTERS - do not remove (hex / unicode) \|/
        private static readonly Regex _unescapes = new Regex("\x1A" + "E\\d+E", RegexOptions.Compiled);

        /// <summary>
        /// swap back in all the special characters we've hidden
        /// </summary>
        private string Unescape(string s) => _unescapes.Replace(s, new MatchEvaluator(UnescapeEvaluator));

        private string UnescapeEvaluator(Match match) => _invertedEscapeTable[match.Value];

        /// <summary>
        /// escapes Bold [ * ] and Italic [ _ ] characters
        /// </summary>
        private string EscapeBoldItalic(string s)
        {
            s = s.Replace("*", _escapeTable["*"]);
            return s.Replace("_", _escapeTable["_"]);
        }

        private static string AttributeEncode(string s)
        {
            return s.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&#39;");
        }

        private static string AttributeSafeUrl(string s)
        {
            s = AttributeEncode(s);
            foreach (var c in "*_:()[]")
                s = s.Replace(c.ToString(), _escapeTable[c.ToString()]);
            return s;
        }

        /// <summary>
        /// Within tags -- meaning between &lt; and &gt; -- encode [\ ` * _] so they 
        /// don't conflict with their use in Markdown for code, italics and strong. 
        /// We're replacing each such character with its corresponding hash 
        /// value; this is likely overkill, but it should prevent us from colliding 
        /// with the escape values by accident.
        /// </summary>
        private string EscapeSpecialCharsWithinTagAttributes(string text)
        {
            var tokens = tokenizeHTML(text);

            // now, rebuild text from the tokens
            var sb = new StringBuilder(text.Length);

            foreach (var token in tokens)
            {
                string value = token.Value;

                if (token.Type == tokenType.Tag)
                {
                    value = value.Replace(@"\", _escapeTable[@"\"]);

                    if (autoHyperlink && value.StartsWith("<!")) // escape slashes in comments to prevent autolinking there -- https://meta.stackexchange.com/questions/95987/html-comment-containing-url-breaks-if-followed-by-another-html-comment
                        value = value.Replace("/", _escapeTable["/"]);

                    value = Regex.Replace(value, "(?<=.)</?code>(?=.)", _escapeTable["`"]);
                    value = EscapeBoldItalic(value);
                }

                sb.Append(value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// convert all tabs to _tabWidth spaces; 
        /// standardizes line endings from DOS (CR LF) or Mac (CR) to UNIX (LF); 
        /// makes sure text ends with a couple of newlines; 
        /// removes any blank lines (only spaces) in the text
        /// </summary>
        private string Normalize(string text)
        {
            var output = new StringBuilder(text.Length);
            var line = new StringBuilder();
            bool valid = false;

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '\n':
                        if (valid) output.Append(line);
                        output.Append('\n');
                        line.Length = 0; valid = false;
                        break;
                    case '\r':
                        if (i < text.Length - 1 && text[i + 1] != '\n')
                        {
                            if (valid) output.Append(line);
                            output.Append('\n');
                            line.Length = 0; valid = false;
                        }
                        break;
                    case '\t':
                        int width = _tabWidth - line.Length % _tabWidth;
                        for (int k = 0; k < width; k++)
                            line.Append(' ');
                        break;
                    case '\x1A':
                        break;
                    default:
                        if (!valid && text[i] != ' ') valid = true;
                        line.Append(text[i]);
                        break;
                }
            }

            if (valid) output.Append(line);
            output.Append('\n');

            // add two newlines to the end before return
            return output.Append("\n\n").ToString();
        }

        #endregion


    }
}
