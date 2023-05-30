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
    internal class TKTG
    {
        public static WebClient webClient = new WebClient();
        public static string url = "https://danso.org/cac-quoc-gia-tren-the-gioi-theo-dan-so/";
        public static string html = webClient.DownloadString(url);
        public static Dictionary<string, int> Nations = ExtractLinkData();
        public static string[] SampleTK = {"Xếp hạng: ","Tên Quốc gia: ","Dân số: ","%Thay đổi: ","Thay đổi: ","Mật độ: ",
        "Diện tích(km²): ","Di cư: ","Tỉ lệ sinh: ","Tuổi trung bình: ","%Thành thị: ","%Thế giới"};
        public static Dictionary<string, int> ExtractLinkData()
        {
            Dictionary<string, int> Dic = new Dictionary<string, int>();
            Dic.Add("trung quốc", 1);
            Dic.Add("ấn độ", 13);
            Dic.Add("usa", 25);
            Dic.Add("ấn độ", 37);

            return Dic;
        }

        public static string ThongKe(string a)
        {
            string tt = "Thống Kê theo năm 2020:" + "\n";
            string pattern = @"<td[^>]*>(.*?)<\/td>";
            MatchCollection matches = Regex.Matches(html, pattern);

            foreach (KeyValuePair<string, int> Dic in Nations)
            {
                int ios = 0;
                string nation = Dic.Key;
                int index = Dic.Value;
                if (a == nation)
                {
                    for (int i = index - 1; i < index + 11; i++)
                    {
                        string content = matches[i].Groups[1].Value;
                        byte[] bytes = Encoding.Default.GetBytes(content);
                        content = Encoding.UTF8.GetString(bytes).ToLower();
                        tt += SampleTK[ios] + content + '\n';
                        ios++;
                    }
                    break;
                }                 
            }
            
            /*
            for (int i = 0; i < 235; i++)
            {
                if(matches[i].Groups[1].Value.ToLower() == a)
                {
                    int iii = i + 11;
                        
                }
                
            }*/

            return tt;
        }
    }
}
