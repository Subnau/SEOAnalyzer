using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

[assembly: InternalsVisibleTo("WordCounter.UnitTests")]

namespace WordCounter
{
    public class WordCounter
    {

        private readonly char[] _wordsSeparatorDefault = { ' ', ',', '.', '\r', '\n', '\t', '/' };
        private readonly char[] _wordsSeparator;
        private readonly HtmlNode _rootNode;
        private Dictionary<string, int> _wordsDictionary;
        private Dictionary<string, int> _keywordsDictionary;
        //Dictionary due framework 2.0 requirement, Hashset<string> will be better )
        private readonly Dictionary<string, int> _stopWordsDictionary;
        private int _extLinksNumber = -1;

        /// <summary>
        /// Create words counter 
        /// </summary>
        /// <param name="pageUrlOrText"></param>
        /// <param name="isUrl">Is in pageUrlOrText set url or some text</param>
        /// <param name="stopsWordsString">User's stop words</param>
        /// <param name="wordsSeparator">User's word separators</param>
        public WordCounter(string pageUrlOrText, bool isUrl, string stopsWordsString = null, char[] wordsSeparator = null)
        {
            if (wordsSeparator == null)
                _wordsSeparator = _wordsSeparatorDefault;

            if (!String.IsNullOrEmpty(stopsWordsString))
            {
                //insert stopWords in dictionary
                _stopWordsDictionary = new Dictionary<string, int>();
                SplitTextAndCount(stopsWordsString, _stopWordsDictionary, null, _wordsSeparator, true);
                if (_stopWordsDictionary.Count == 0)
                    _stopWordsDictionary = null;
            }

            HtmlDocument htmlDocument;

            try
            {
                if (isUrl)
                    htmlDocument = new HtmlWeb().Load(pageUrlOrText); //load web page by url
                else
                {
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(pageUrlOrText); // load web page/text from string
                }
            }
            catch (HtmlWebException ex) //HAP exception
            {
                throw new WebException("HtmlAgilityPack: " + ex.Message);
            }

            _rootNode = htmlDocument.DocumentNode;
        }

        /// <summary>
        /// Count word occurencies in document and insert word/occurencies in dictionary
        /// </summary>
        /// <returns>Dictionary of words</returns>
        public Dictionary<string, int> GetWordsDictionary()
        {
            if (_wordsDictionary != null)
                return _wordsDictionary; //return already counted dic

            _wordsDictionary = new Dictionary<string, int>();
            ProcessAllTextNodes(_rootNode, _wordsDictionary, _stopWordsDictionary, _wordsSeparator, true);

            if (_wordsDictionary.Count == 0)
                _wordsDictionary = null;

            return _wordsDictionary;
        }

        /// <summary>
        /// Try to get keywords from "meta keywords" and count this keywords in text
        /// If words are already counted (GetWordsDictionary() have been invoked), word occurences will get from dictionary of words
        /// Otherwise will be count by searching in text
        /// </summary>
        /// <returns>Keywords dictionary</returns>
        public Dictionary<string, int> GetKeywordsDictionary()
        {
            if (_keywordsDictionary != null)
                return _keywordsDictionary; //return already counted dic

            _keywordsDictionary = GetKeywordsDictionaryFromMetaTags(_rootNode, _wordsSeparator); 
            if (_keywordsDictionary != null) //are keywords exists in meta
                if (_wordsDictionary != null)
                { 
                    //get word occurencies from dicionary of words
                    var keywordsDictionaryTemp = new Dictionary<string, int>();
                    foreach (var keyword in _keywordsDictionary.Keys)
                    {
                        keywordsDictionaryTemp[keyword] = _wordsDictionary.ContainsKey(keyword) ? _wordsDictionary[keyword] : 0;
                    }
                    _keywordsDictionary = keywordsDictionaryTemp;
                }
                else
                {
                    //Full search in text, adding new words not allowed by last param
                    ProcessAllTextNodes(_rootNode, _keywordsDictionary, _stopWordsDictionary, _wordsSeparator, false);
                }
            return _keywordsDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>External links number in text</returns>
        public int GetExternalLinksNumber()
        {
            if (_extLinksNumber >= 0)
                return _extLinksNumber; //return already counted val

            _extLinksNumber = 0;
            var nodes = _rootNode.SelectNodes(@"//a[@href]");
            if (nodes != null)
                foreach (var link in nodes)
                {
                    var att = link.Attributes["href"];
                    if (att == null) continue;
                    var href = att.Value;
                    if (href.StartsWith("javascript", StringComparison.InvariantCultureIgnoreCase) ||
                        href.StartsWith("#", StringComparison.InvariantCultureIgnoreCase)) continue;

                    var uri = new Uri(href, UriKind.RelativeOrAbsolute);

                    if (uri.IsAbsoluteUri)
                    {
                        _extLinksNumber++;
                    }
                }
            return _extLinksNumber;
        }


        internal static void ProcessAllTextNodes(HtmlNode rootNode, Dictionary<string, int> dic, Dictionary<string, int> stopWordsDictionary,
            char[] wordsSeparator, bool isCanAddNewKeys)
        {
            foreach (var node in rootNode.Descendants("#text"))
            {
                if (String.Compare(node.ParentNode.Name, "script", true, CultureInfo.InvariantCulture) != 0) //ignore inner text in <script>abcabc</script>
                {
                    string s = node.InnerText;
                    if (!IsCDATA(s)) //ignore CDATA
                    {
                        s = ReplaceNotLetters(ReplaceSpecialCharacters(s)).Trim(); //clean text
                        if (!String.IsNullOrEmpty(s))
                        {
                            SplitTextAndCount(s, dic, stopWordsDictionary, wordsSeparator, isCanAddNewKeys); 
                        }
                    }
                }
            }
        }

        internal static Dictionary<string, int> GetKeywordsDictionaryFromMetaTags(HtmlNode rootNode, char[] wordsSeparator)
        {
            HtmlNode keywordsNode = rootNode.SelectSingleNode("//meta[@name='Keywords']");
            if (keywordsNode != null)
            {
                string keyWords = keywordsNode.GetAttributeValue("content", "");
                if (!String.IsNullOrEmpty(keyWords))
                {
                    string[] keyWordsSplitted = keyWords.Split(wordsSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (keyWordsSplitted.Length > 0)
                    {
                        var keywordDictionary = new Dictionary<string, int>();
                        foreach (var keyWord in keyWordsSplitted)
                        {
                            keywordDictionary[keyWord.ToLower()] = 0;
                        }
                        return keywordDictionary;
                    }
                }
            }
            return null;
        }

        internal static void SplitTextAndCount(string s, Dictionary<string, int> dic, Dictionary<string, int> stopWordsDictionary,
            char[] wordsSeparator, bool isCanAddNewKeys)
        {
            if (String.IsNullOrEmpty(s))
                return;

            string[] words = s.Split(wordsSeparator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
                if (dic.ContainsKey(words[i]))
                    dic[words[i]]++;
                else
                    if (isCanAddNewKeys)
                        if (stopWordsDictionary == null || !stopWordsDictionary.ContainsKey(words[i]))
                            dic[words[i]] = 1;
            }

        }

        // ReSharper disable once InconsistentNaming
        internal static bool IsCDATA(string s)
        {
            if (s.Length >= 10)
            {
                if (s.Substring(0, 10).ToLower().Contains("[cdata"))
                    return true;
            }
            return false;
        }

        internal static string ReplaceSpecialCharacters(string s)
        {
            return Regex.Replace(s, @"&[^\s;]+;", " ");
        }

        internal static string ReplaceNotLetters(string s)
        {
            return Regex.Replace(s, @"[^a-zA-Z]+", " ");
        }
    }

}
