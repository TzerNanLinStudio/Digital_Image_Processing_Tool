using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CameraSetting
{
    public delegate void ButtonClickEventHandler(object value);
    //public event ButtonClickEventHandler ButtonShotClickEvent;

    public partial class UserControlOfCameraSetting : UserControl
    {
        //public delegate void ButtonClickEventHandler(object value);
        public event ButtonClickEventHandler ButtonShotClickEvent;

        public UserControlOfCameraSetting()
        {
            InitializeComponent();
        }

        private void button_Test001_Click(object sender, EventArgs e)
        {
            //ButtonShotClickEvent.Invoke(this);

            /*訂閱者 農夫 = new 訂閱者() { 名字 = "農夫" };
            訂閱者 商人 = new 訂閱者() { 名字 = "商人" };
            誹謗者 騎士 = new 誹謗者() { 名字 = "騎士" };

            報社 王國日報 = new 報社();
            //訂閱
            王國日報.最新新聞 += 農夫.通知我;
            王國日報.最新新聞 += 商人.通知我;
            王國日報.最新新聞 += 騎士.通知我;

            int i = 3;
            while (i-- > 0)
            {
                Console.WriteLine("請輸入最新消息：");

                string 消息 = Console.ReadLine();

                王國日報.投稿新聞(消息, i);
            }*/

        }

        private void numericUpDown_Exposure_ValueChanged(object sender, EventArgs e)
        {
            ButtonShotClickEvent.Invoke(this);
        }
    }

    class 報社
    {
        public delegate void 通知對象(string 新聞報導, int 新聞關鍵數字);

        public 通知對象 最新新聞;

        public void 投稿新聞(string 新聞稿, int 關鍵數字)
        {
            //觸發事件
            最新新聞.Invoke(新聞稿, 關鍵數字);
        }
    }

    class 訂閱者
    {
        public string 名字;

        public void 通知我(string 訊息, int 數字)
        {
            Console.WriteLine($"我是{名字}，我已經收到最新新聞：{訊息}");
            Console.WriteLine(數字);
        }
    }

    class 誹謗者
    {
        public string 名字;

        public void 通知我(string 訊息, int 數字)
        {
            Console.WriteLine($"我是{名字}，我不想收到最新新聞：{訊息}");
            Console.WriteLine(數字);
        }
    }
}
