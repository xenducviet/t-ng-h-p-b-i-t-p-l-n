using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Windows.Forms.LinkLabel;

namespace Bot_SV
{
    internal class ThoiTiet
    {
        public static WebClient webClient = new WebClient();
        public static string url = "https://www.nchmf.gov.vn/kttv/";
        public static string html = webClient.DownloadString(url);
        public static Dictionary<string, string> linkDataDictionary = ExtractLinkData(html);
        public static string[] SampleTT = { "🌡Nhiệt độ ", "Thời tiết ", "💧Độ ẩm ", "💨Hướng gió " };

        public static Dictionary<string, string> ExtractLinkData(string websiteContent)
        {
            Dictionary<string, string> linkDataDictionary = new Dictionary<string, string>();

            string pattern = @"<a\s+class=""name-wt-city""\s+href=""([^""]*?)""[^>]*>(.*?)</a>";
            MatchCollection matches = Regex.Matches(websiteContent, pattern);

            foreach (Match match in matches)
            {
                string href = match.Groups[1].Value;
                string content = match.Groups[2].Value;
                byte[] bytes = Encoding.Default.GetBytes(content);
                content = Encoding.UTF8.GetString(bytes).ToLower();

                linkDataDictionary.Add(content, href);
            }

            return linkDataDictionary;
        }
        public static string Check(string a, string b)
        {
            string tt = "";
            foreach (KeyValuePair<string, string> linkData in linkDataDictionary)
            {
                string content = linkData.Key;
                string href = linkData.Value;
                tt += content + ".|" + href;
            }
            return tt;
        }

        public static string GetThoiTiet(string a, string b)
        {
            string link = "";
            string total_content = "$\"Không tồn tại địa điểm {a} này trên Việt Nam\";";

            foreach (KeyValuePair<string, string> linkData in linkDataDictionary)
            {
                string content = linkData.Key;
                string href = linkData.Value;

                if (a == content)
                {
                    total_content = "Kết quả là: \n";
                    link = href;
                    WebClient webClient = new WebClient();
                    string html = webClient.DownloadString(link);
                    if (b == "hôm nay")
                    {
                        string pattern = @"<div\s+class=""uk-width-3-4"">(.*?)<\/div>";
                        MatchCollection matches = Regex.Matches(html, pattern);
                        for (int i = 0; i < 4; i++)
                        {

                            string content_of_web = matches[i].Groups[1].Value;
                            byte[] bytes = Encoding.Default.GetBytes(content_of_web);
                            content_of_web = Encoding.UTF8.GetString(bytes);
                            total_content += SampleTT[i] + content_of_web + "\n";
                        }
                    }
                    else if (b == "1 ngày tới" || b == "một ngày tới")
                    {
                        total_content += ForeCast(link, 0, 0);
                    }
                    else if (b == "2 ngày tới" || b == "hai ngày tới")
                    {
                        total_content += ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4);
                    }
                    else if (b == "3 ngày tới" || b == "ba ngày tới")
                    {
                        total_content += ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4) + "/n" +
                        ForeCast(link, 2, 8);
                    }
                    else if (b == "4 ngày tới" || b == "bốn ngày tới")
                    {
                        total_content = ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4) + "/n" +
                                        ForeCast(link, 2, 8) + "/n" + ForeCast(link, 3, 12);
                    }
                    else if (b == "5 ngày tới" || b == "năm ngày tới")
                    {
                        total_content = ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4) + "/n" +
                                        ForeCast(link, 2, 8) + "/n" + ForeCast(link, 3, 12) + "/n" +
                                        ForeCast(link, 4, 16);
                    }
                    else if (b == "6 ngày tới" || b == "sáu ngày tới")
                    {
                        total_content = ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4) + "/n" +
                                        ForeCast(link, 2, 8) + "/n" + ForeCast(link, 3, 12) + "/n" +
                                        ForeCast(link, 4, 16) + "/n" + ForeCast(link, 5, 20);
                    }
                    else if (b == "6 ngày tới" || b == "sáu ngày tới")
                    {
                        total_content = ForeCast(link, 0, 0) + "/n" + ForeCast(link, 1, 4) + "/n" +
                                        ForeCast(link, 2, 8) + "/n" + ForeCast(link, 3, 12) + "/n" +
                                        ForeCast(link, 4, 16) + "/n" + ForeCast(link, 5, 20) + "/n" +
                                        ForeCast(link, 6, 24);
                    }
                    else
                    {
                        total_content = "Có vấn đề về cú pháp nhập. Vui lòng thử lại";
                    }

                    break;
                }
            }
            return total_content;
        }

        public static string ForeCast(string a, int index1, int index2)
        {
            string total_content = "";
            string[] title = { "❄️Nhiệt độ thấp nhất: ", "", "🌧Tỉ lệ mưa:", "💨Tốc độ gió: " };
            int iot = 0;
            WebClient webClient = new WebClient();
            string html = webClient.DownloadString(a);

            string ngay = @"<div\s+class=""date-wt"">(.*?)<span>(.*?)<\/span><\/div>";
            MatchCollection getNgay = Regex.Matches(html, ngay);
            string Datee = getNgay[index1].Groups[1].Value;
            string Timee = getNgay[index1].Groups[2].Value;

            byte[] b = Encoding.Default.GetBytes(Datee);
            Datee = Encoding.UTF8.GetString(b);
            byte[] b2 = Encoding.Default.GetBytes(Timee);
            Timee = Encoding.UTF8.GetString(b2);


            total_content += Datee + " " + Timee + "\n";

            string pattern1 = @"<span\s+class=""large-temp"">(.*?)<\/span>";
            string pattern2 = @"<span\s+class=""small-temp"">(.*?)<\/span>";
            string pattern3 = @"<div\s+class=""text-temp"">(.*?)<\/div>";

            MatchCollection matches1 = Regex.Matches(html, pattern1);
            string content_of_web1 = matches1[index1].Groups[1].Value;
            byte[] bytes1 = Encoding.Default.GetBytes(content_of_web1);
            content_of_web1 = Encoding.UTF8.GetString(bytes1);
            total_content += "Nhiệt độ cao nhất:" + content_of_web1 + "\n";

            int index3 = index2 + 4;
            for (int i = index2; i < index3; i++)
            {
                MatchCollection matches2 = Regex.Matches(html, pattern2);
                string content_of_web2 = matches2[i].Groups[1].Value;
                byte[] bytes2 = Encoding.Default.GetBytes(content_of_web2);
                content_of_web2 = Encoding.UTF8.GetString(bytes2);
                if (content_of_web2.Length == 0 && i == index2 + 1)
                {
                    content_of_web2 = "";
                }
                else if (content_of_web2.Length == 0)
                {
                    content_of_web2 = "Không có dữ liệu.";
                }
                else if (content_of_web2.Contains(";"))
                {
                    string[] strings = content_of_web2.Split(';');
                    content_of_web2 = strings[1];
                    total_content += title[iot] + content_of_web2 + "\n";
                }
                else
                {
                    total_content += title[iot] + content_of_web2 + "\n";
                }

                iot++;

            }

            MatchCollection matches3 = Regex.Matches(html, pattern3);
            string content_of_web3 = matches3[index1].Groups[1].Value;
            byte[] bytes3 = Encoding.Default.GetBytes(content_of_web3);
            content_of_web3 = Encoding.UTF8.GetString(bytes3);
            total_content += "Tình trạng thời tiết: " + content_of_web3 + "\n";

            return total_content;
        }
    }
}

