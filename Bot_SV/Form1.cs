
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Linq;

namespace Bot_SV
{

    public partial class Form1 : Form
    {

        public TelegramBotClient botClient;
        //HoiDataBase HoiData;
        //6052997336
        public long chatId = 6052997336; // Mk fix trước 1 cái chat id là tài khuản của mk! -> cái này liên quan đến việc nhúng ở bên app

        int logCounter = 0;

        void AddLog(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke((MethodInvoker)delegate ()
                {
                    AddLog(msg);
                });
            }
            else
            {
                logCounter++;
                if (logCounter > 100)
                {
                    txtLog.Clear();
                    logCounter = 0;
                }
                txtLog.AppendText(msg + "\r\n");
            }
            Console.WriteLine(msg);
        }

        /// <summary>
        /// hàm tạo: ko kiểu, trùng tên với class
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            //HoiData = new HoiDataBase();
            // Thằng QuanLyBanHanglv1_bot
            string token = "6229327254:AAEofhwe182OnlI20rZ_jGX1rjJ_UyZbcEk";

            //Console.WriteLine("my token=" + token);

            botClient = new TelegramBotClient(token);  // Tạo 1 thằng bot 

            CancellationTokenSource cts = new CancellationTokenSource();  // Thằng này để hủy j đó kiểm soát chương trình
            // CancellationTokenSource cts = new CancellationTokenSource(); LÀm như này cũng đc nè??

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,  //hàm xử lý khi có người chát đến được gọi mỗi khi có cập nhật mới từ telegram API -> nó xử lý và trả về kq  
                pollingErrorHandler: HandlePollingErrorAsync,   // Hàm này sử lý lỗi -> có lỗi là gọi thằng này
                receiverOptions: receiverOptions,  // Thằng này đc new ở trên kìa tham số cài đặt về việc cập nhật mới
                cancellationToken: cts.Token    // Thằng này là hủy cts.Token  -> hủy nó làm j ?
                                                // Túm lại: bắt đầu quá trình nhận cập nhật từ Telegram API bằng cách kích hoạt botClient
                                                // các cập nhật sẽ được xử lý bởi hàm HandleUpdateAsync.
                                                // Nếu xảy ra lỗi trong quá trình nhận cập nhật, hàm HandlePollingErrorAsync sẽ được gọi để xử lý lỗi. 
                                                // 2 thằng sau là tùy chọn cập nhật.
            );

            Task<User> me = botClient.GetMeAsync(); // Được sử dụng để gửi một yêu cầu đến Telegram API để lấy thông tin về bot hiện tại.
            // => Nắm đầu thằng bot rồi.
            AddLog($"Thằng bot: @{me.Result.Username}");

            //async lập trình bất đồng bộ
            // Trả về đối tượng Task ?? 
            // Vậy là form 
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // botClient: Thằng này mk tạo ở trên rồi: được sử dụng để gửi các yêu cầu tới Telegram API
                // update: chứa thông tin về cập nhật mới nhận được từ Telegram API. Update chứa các thông tin như tin nhắn, sự kiện nhóm, thay đổi trạng thái, v.v.
                //          Vậy là thằng botClient yêu cầu -> trả kết quả về thằng Update!
                // cancellationToken: Thằng này nó sử lý khi có lỗi -> Nó k thấy đc gọi nhưng k có nó là lỗi ?<>??? nani
                // Only process Message updates: https://core.telegram.org/bots/api#message
                bool ok = false;

                //kdl? biến <=> biến đó có thể nhận NULL

                // Telegram.Bot.Types.Message là một lớp đại diện cho một tin nhắn trong Telegram.
                // Lớp này chứa các thông tin về tin nhắn, bao gồm nội dung, người gửi, người nhận, thời gian gửi, vị trí, hình ảnh, v.v.
                Telegram.Bot.Types.Message message = null; // dấu ? để có thể gán null 

                // update.Message là người dùng nhắn 1 tin nhắn mới tới bot
                if (update.Message != null)  // Nếu tin nếu thằng update không phải là null => có cập nhật mới:
                {
                    // message không phải là string -> nó là đối tượng đại diện cho 1 tin nhắn
                    message = update.Message;   // Và tao gán thông tin update vào thằng đại diện cho tin nhắn này
                    ok = true;
                }
                // update.EditedMessage là có 1 tin nhắn đã gửi từ trước rồi => song giờ nó click phải chuột sửa tin nhắn -> tao cũng nắm đầu ra xử lý
                if (update.EditedMessage != null)
                {
                    message = update.EditedMessage;
                    ok = true;
                }

                // Nó k chui vào 2 if ở trên <=> !false hoặc message == null => return; -> thầy kiểm tra kỹ quá cơ!
                if (!ok || message == null) return; //thoát ngay

                string messageText = message.Text;
                if (messageText == null) return;  //ko chơi với null

                chatId = message.Chat.Id;  //id của người chát với bot

                AddLog($"{chatId}: {messageText}");  //show lên để xem -> chứ k phải gửi về telegram

                string reply = "";  //đây là text trả lời

                string messLow = messageText.ToLower(); // Có lẽ k cần thiết!




                // ----------- BẮT ĐẦU XỬ LÝ -----------------------------------------------------------------------------
                // -> bot này là xử lý chủ động khi người chat đến ở đây!
                // Còn xử lý mà tự động BÁO CÁO 1 cái j đó khi Database thay đổi thì gọi con bot ở chỗ thay đổi đó!
                // -> Bây giờ chỉ cần Xử lý dữ liệu để tạo ra thằng reply

                // 1. khi hỏi về an Cốp:
                if (messLow.StartsWith("gv"))
                {
                    reply = "FeedBack Giáo viên:🥲 Môn học lập trình Windows thầy Đỗ Duy Cốp. Giảng quá xá là HAY!😍😍";
                }
                else if (new[] { "sv", "sinhvien", "hocsinh" }.Any(keyword => messLow.Contains(keyword)))
                {
                    string tenSV = messageText.Substring(messageText.IndexOf(' ') + 1);
                    reply = TuongTacDB.timSV("%" + tenSV.Replace(' ', '%') + "%");
                }
                else if (new[] { "hi", "hello", "chao", "allo" }.Any(keyword => messLow.Contains(keyword)))
                {
                    reply = "Chào bạn, tôi có thể giúp gì cho bạn?";
                }
                else if (messLow.StartsWith("việt"))
                {
                    reply = " Anh Việt Đẹp trai vãi nồi";
                }
                else if (new[] { "duy", "thằng duy", "anh duy" }.Any(keyword => messLow.Contains(keyword)))
                {
                    reply = "Ý bạn là thằng Phương Minh Duy, Bắc Giang, học 56kmt trường DH Công nghiệp Thái Nguyên phải không. " +
                        "Tôi thề với bạn, Thằng lùn đấy xấu như chó, " +
                        "Mặc dù là 1 AI nhưng tôi chưa thể tìm ra được ai Xấu hơn thằng cờ hó này";
                }
                else if (messLow.StartsWith("thế định"))
                {
                    reply = " ý bạn là thế định rickid, bắc ninh phải không ";
                }
                else if (messLow.StartsWith("/tktg"))
                {
                    reply = "ok";
                    string input = messLow.Substring(6);
                    reply = TKTG.ThongKe(input);
                }
                else if (messLow.StartsWith("/thoitiet"))
                {
                    
                    string input = messLow.Substring(10);
                    if (input.Contains(","))
                    {
                        string[] parts = input.Split(',');
                        reply = ThoiTiet.GetThoiTiet(parts[0], parts[1]);
                    }
                    else
                    {
                        reply = "Bạn nên nhập theo cú pháp: Địa điểm + ',' + thời gian";
                    }
              
                }
                else
                {
                    reply = "🤡Tôi không thể tìm thấy: " + messageText;
                }

                // ----------- KẾT THÚC XỬ LÝ -----------------------------------------------------------------------
                AddLog(reply); //show log to see




                // Echo received message text
                // => botClient.SendTextMessageAsync: => cái hàm này là hàm gửi tin nhắn về telegram
                // Nó đc gọi vào đoạn cuối của hàm HandleUpdateAsync mà hàm HandleUpdateAsync được khởi tạo khi form_Load rồi.
                // Mỗi khi có tin nhắn đến hàm HandleUpdateAsync -> sẽ đc gọi
                // Nếu -> ngon thì nó chạy đến đây và rep lại bên telegram còn nếu k ổn thì nó chạy về hàm lỗi HandlePollingErrorAsync
                Telegram.Bot.Types.Message sentMessage = await botClient.SendTextMessageAsync(
                           // Hàm gửi tin nhắn đi này cần setting như sau:
                           chatId: chatId, // chatId biến này lấy ở trên kia rồi -> lưu id thằng chat với mk để bây giờ trả lời lại nó chứ! chuẩn chưa
                           text: reply,    // rep lại bên telegram thì gán vào thuộc tính text => ở đây là biến reply mk đã xử lý dữ liệu ở trên rồi <>
                           parseMode: ParseMode.Html  // =>  Bro dùng cách đánh dấu văn bản HTML để thể hiện text.
                                                      //parseMode: ParseMode.Markdown => thì nó cũng là 1 cách đánh dấu văn bản nhưng nó k phong phú như html
                      );

                //đọc thêm về ParseMode.Html tại: https://core.telegram.org/bots/api#html-style
            }

            // Đây là hàm sử lý lỗi -> có lỗi nó chui vào hàm này
            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                //var ErrorMessage = exception switch
                //{
                //    ApiRequestException apiRequestException
                //        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n LỖI NHƯ SAU:\n{apiRequestException.Message}",
                //     => exception.ToString()
                //};

                //AddLog(ErrorMessage);
                Console.WriteLine("Looi roi anh oi");
                AddLog("----       Lỗi rồi -> K rõ lỗi j  -----------");
                return Task.CompletedTask;
            }
        }

        private void formBot_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

