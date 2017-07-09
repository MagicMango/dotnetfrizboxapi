using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace FritzBoxAPI.Util
{
    public static class XML
    {
        public static string GetValue(XDocument doc, string name)
        {
            XElement info = doc.FirstNode as XElement;
            return info.Element(name).Value;
        }

        public static List<XElement> GetValues(XDocument doc, string name)
        {
            XElement info = doc.FirstNode as XElement;
            return info.Elements(name).ToList();
        }

        public static String RemoveScriptTag(String input)
        {
            Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            return rRemScript.Replace(input, "");
        }
        public static String RemoveStyleTag(String input)
        {
            Regex rRemScript = new Regex(@"<style[^>]*>[\s\S]*?</style>");
            return rRemScript.Replace(input, "");
        }
    }
}
