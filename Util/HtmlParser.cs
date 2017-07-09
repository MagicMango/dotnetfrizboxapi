using FritzBoxAPI.Model.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FritzBoxAPI.Util
{
    public static class HtmlParser
    {
        public static HtmlAgilityPack.HtmlNode GetPhoneBookTable(String html)
        {
            String removed = XML.RemoveScriptTag("<root>" + html + "</root>");
            removed = XML.RemoveStyleTag(removed);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(removed);
            return doc.DocumentNode.Descendants("table").ToList().Last();
        }

        public static HtmlAgilityPack.HtmlNode GetPhoneBookMeta(String html)
        {
            String removed = XML.RemoveScriptTag("<root>" + html + "</root>");
            removed = XML.RemoveStyleTag(removed);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
            doc.LoadHtml(removed);
            //FirstChild because of root append!
            return doc.DocumentNode.FirstChild.SelectNodes(XPathConstants.PHONEBOOKMETADATA).ToList().Last();
        }

        public static List<HtmlAgilityPack.HtmlNode> GetIdDivs(String html)
        {
            String removed = XML.RemoveScriptTag("<root>" + html + "</root>");
            removed = XML.RemoveStyleTag(removed);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(removed);
            return doc.DocumentNode.FirstChild.SelectNodes(XPathConstants.PHONEBOOKISLIST).ToList();
        }
    }
}
