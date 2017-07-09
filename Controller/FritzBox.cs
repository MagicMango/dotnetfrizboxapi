using FritzBoxAPI.Model.Constants;
using FritzBoxAPI.Model.Entities;
using FritzBoxAPI.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace FritzBoxAPI.Controller
{
    public class FritzBox
    {
        private String sid;

        public FritzBox(String user, String password)
        {
            this.sid = GetSessionId(user, password);
        }

        public Overview GetOverviewData()
        {
            String result = Http.Post(FritzBoxConstants.DATAPAGE, new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "page", "overview" },
            });
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(result);
            Overview overview = new Overview();
            foreach (dynamic call in data.data.foncalls.calls)
            {
                Call tmp = new Call() { Display = call.display, Name = call.name, Number = call.number };
                tmp.SetCallSatus(call.classes);
                tmp.SetTime(call.time);
                overview.Calls.Add(tmp);
            }
            foreach (dynamic device in data.data.net.devices)
            {
                Device tmp = new Device() { Name = device.name, Type = device.type, Url = device.url };
                tmp.SetDeviceSatus(device.classes);
                overview.Devices.Add(tmp);
            }
            return overview;
        }

        public PhoneBook GetCurrentPhoneBook()
        {
            String html = Http.Post(FritzBoxConstants.DATAPAGE, new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "page", "bookLi" },
            });
            return ParsePhoneBook(html);
        }

        public PhoneBook GetPhoneBookById(int Id)
        {
            return ParsePhoneBook(Http.ReadSite(FritzBoxConstants.GETPHONEBOOKBYID, "?sid=" + sid + "&bookid=" + Id));
        }

        public List<PhoneBook> GetAllPhoneBooks()
        {
            List<PhoneBook> result = new List<PhoneBook>();
            String html = Http.Post(FritzBoxConstants.DATAPAGE, new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "oldpage", "/fon_num/fonbook_select.lua" },
            });
            List<HtmlAgilityPack.HtmlNode> nodes = HtmlParser.GetIdDivs(html);
            foreach (HtmlAgilityPack.HtmlNode node in nodes)
            {
                result.Add(GetPhoneBookById(Int32.Parse((node.ChildNodes[1].Attributes.Count == 4) ? node.ChildNodes[1].Attributes[2].Value : node.ChildNodes[1].Attributes[3].Value)));
            }
            return result;
        }

        public void SetPhoneBookById(int Id)
        {
            Http.Post(FritzBoxConstants.DATAPAGE, new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "bookid", ""+Id },
                { "apply", "" },
                { "oldpage", "/fon_num/fonbook_select.lua" }
            });
        }

        public void AddPhoneBookEntry(int PhoneBookId, String Name, String Home, String Mobile = null, String Work = null, String Fax = null, String Email = null, Boolean Important = false)
        {
            NameValueCollection values = new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "idx", "" },
                { "uid", "" },
                { "prionumber", "none" },
                { "entryname", Name },
                { "bookid", ""+PhoneBookId },
                { "numbertypenew1", "home" },
                { "numbernew1", Home },
                { "apply", ""},
                { "back_to_page", "/fon_num/fonbook_list.lua"}
            };
            if (Mobile != null)
            {
                values.Add(new NameValueCollection() {
                    { "numbertypenew2", "mobile" },
                    { "numbernew2", Mobile }
                });
                if (Work != null)
                {
                    values.Add(new NameValueCollection() {
                    { "numbertypenew3", "work" },
                    { "numbernew3", Work }
                });
                    if (Fax != null)
                    {
                        values.Add(new NameValueCollection() {
                    { "numbertypenew4", "fax_work" },
                    { "numbernew4", Work }
                });
                    }
                }
            }
            if (Email != null)
            {
                values.Add(new NameValueCollection() {
                    { "emailnew1", Email },
                });
            }
            if (Important)
            {
                values.Add(new NameValueCollection() {
                    { "category", "on" },
                });
            }

            Http.Post(FritzBoxConstants.CREATEENTRYPAGE, values);
        }

        public void DeletePhoneBookEntry(int BookId, int Entry)
        {
            Http.Post(FritzBoxConstants.DATAPAGE, new NameValueCollection() {
                { "sid", sid },
                { "lang", "de" },
                { "bookid", ""+BookId },
                { "delete_entry", ""+Entry },
                { "oldpage", "/fon_num/fonbook_list.lua" }
            });
        }

        private PhoneBook ParsePhoneBook(String html)
        {
            PhoneBook Book = new PhoneBook();
            HtmlAgilityPack.HtmlNode metanode = HtmlParser.GetPhoneBookMeta(html);
            Book.Id = Int32.Parse(metanode.ChildNodes[3].Attributes["value"].Value);
            Book.Name = metanode.ChildNodes[5].Attributes["value"].Value;
            List<HtmlAgilityPack.HtmlNode> nodes = HtmlParser.GetPhoneBookTable(html).Descendants("tr").ToList();
            nodes = nodes.Take(nodes.Count - 1).ToList();
            for (int i = 0; i < nodes.Count; i++)
            {
                PhoneBookEntry entry = new PhoneBookEntry();
                entry.Id = Int32.Parse(nodes[i].SelectNodes(XPathConstants.PHONEBOOKTABLENODEID).FirstOrDefault().Attributes.Where(x => x.Name == "value").FirstOrDefault().Value);
                entry.Name = nodes[i].SelectNodes(XPathConstants.PHONEBOOKTABLENODENAME).FirstOrDefault().InnerText;
                entry.Number = nodes[i].SelectNodes(XPathConstants.PHONEBOOKTABLENODENUMBER).FirstOrDefault().InnerText;
                entry.Type = nodes[i].SelectNodes(XPathConstants.PHONEBOOKTABLENODETYPE).FirstOrDefault().InnerText;
                Book.PhoneBookEntries.Add(entry);
            }
            return Book;
        }

        private string GetSessionId(string benutzername, string kennwort)
        {
            XDocument doc = XDocument.Load(FritzBoxConstants.LOGINPAGE);
            string sid = XML.GetValue(doc, "SID");
            if (sid == "0000000000000000")
            {
                string challenge = XML.GetValue(doc, "Challenge");
                string uri = FritzBoxConstants.LOGINPAGE + "?username=" +
                benutzername + @"&response=" + challenge + "-" + Crypto.GetMD5Hash(challenge + "-" + kennwort);
                doc = XDocument.Load(uri);
                sid = XML.GetValue(doc, "SID");
            }
            return sid;
        }
    }
}
