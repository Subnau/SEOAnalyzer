using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WordCounter.UnitTests
{
    [TestFixture]
    public class WordCounterTest
    {


        private class CreatedEmptyWordsKeywordsStopwordsDic
        {
            public Dictionary<string, int> WordsDictionary = new Dictionary<string, int>();
            public Dictionary<string, int> KeywordsDictionary = new Dictionary<string, int>();
            public Dictionary<string, int> StopWordsDictionary = new Dictionary<string, int>();
            public char[] WordsSeparator = { ' ', ',', '.', '\r', '\n', '\t', '/' };
        }

        [TestCase("word&nbsp;word", "word word")]
        [TestCase("&#556;word&#556;word222&#556;", " word word222 ")]
        public void RemoveSpecialCharacters_TextWithCharacters_TextWithReplacedCharacters(string text, string etalon)
        {

            string str = WordCounter.ReplaceSpecialCharacters(text);

            Assert.AreEqual(str, etalon);
        }

        [TestCase("word word&")]
        [TestCase("word& nbsp;word")]
        public void RemoveSpecialCharacters_TextWithoutCharacters_TextNotChanged(string text)
        {
            string etalon = text;

            string str = WordCounter.ReplaceSpecialCharacters(text);

            Assert.AreEqual(str, etalon);
        }

        [TestCase("word11 __word**", "word word ")]
        [TestCase("word word", "word word")]
        [TestCase("word  word", "word word")]
        public void ReplaceNotLetters_SomeText_TextWithLettersAndOneSpaceInsteadNotLettersGroup(string text, string etalon)
        {
            string str = WordCounter.ReplaceNotLetters(text);

            Assert.AreEqual(str, etalon);
        }

        [TestCase("<[cdata word11wwwwwword**")]
        [TestCase("<ww[cdata word11wwwwwword**")]
        [TestCase("dd<[Cdata word11wwwwwword**")]
        public void IsCDATA_TextWithCDATA_True(string text)
        {
            bool res = WordCounter.IsCDATA(text);

            Assert.IsTrue(res);
        }

        [TestCase("<cdata word11wwwwwword**")]
        [TestCase("<ww[cdata")]
        public void IsCDATA_TextWithoutCDATAOrTextLenghtSmallerTen_False(string text)
        {
            bool res = WordCounter.IsCDATA(text);

            Assert.IsFalse(res);
        }

        [TestCase("            ")]
        [TestCase("")]
        [TestCase(null)]
        public void SplitTextAndCount_EmptyText_WordsDictionaryEmpty(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, null, null, true);
            int length = dics.WordsDictionary.Count;

            Assert.AreEqual(length, 0);
        }

        [TestCase("abc def")]
        public void SplitTextAndCount_TextWithTwoDifferentWords_WordsDictionaryWithTwoWordsAndOneOccurences(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, null, null, true);
            int length = dics.WordsDictionary.Count;

            Assert.AreEqual(length, 2);
        }

        [TestCase("abc Abc")]
        public void SplitTextAndCount_TextWithTwoIdenticalWords_WordsDictionaryWithOneWords(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, null, null, true);
            int length = dics.WordsDictionary.Count;

            Assert.AreEqual(length, 1);
        }

        [TestCase("abc Abc")]
        public void SplitTextAndCount_TextWithTwoIdenticalWords_WordsDictionaryWithOneWordsAndTwoOccurences(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, null, null, true);
            int occurences = dics.WordsDictionary["abc"];

            Assert.AreEqual(occurences, 2);
        }

        [TestCase("abc def")]
        public void SplitTextAndCount_TextWithTwoWordsAndOneInDicAndCanCountOnlyExistsInDic_WordInDicOccurencesIsTwo(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();

            dics.WordsDictionary.Add("abc", 1);

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, null, null, false);
            int occurences = dics.WordsDictionary["abc"];

            Assert.AreEqual(occurences, 2);
        }

        [TestCase("abc def")]
        public void SplitTextAndCount_TextWithTwoDifferentWordsAndOneInStopwords_WordsDictionaryWithOneWord(string text)
        {
            CreatedEmptyWordsKeywordsStopwordsDic dics = new CreatedEmptyWordsKeywordsStopwordsDic();
            dics.StopWordsDictionary.Add("abc", 0);

            WordCounter.SplitTextAndCount(text, dics.WordsDictionary, dics.StopWordsDictionary, null, true);
            int length = dics.WordsDictionary.Count;

            Assert.AreEqual(length, 1);
        }

    }
}
