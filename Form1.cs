using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using static System.Net.WebRequestMethods;
using System.Web;
using System.Speech.Synthesis;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace LabelingNumbers
{
    public partial class Form1 : Form
    {

        string[] VN = { "mươi", "trăm", "ngàn", "triệu", "tỉ", "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín"};
        string[] EN1 = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};
        string[] EN2 = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        string[] EN3 = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninty" };
        string[] EN4 = { "hundred", "thousand", "million", "billion", "trillion" ,"quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecellion", "quattuordecillion", "quindecillion", "sexdecillion", "septendecillion", "octodecillion", "novemdecillion", "vigintillion"};
        string numbers = "0123456789";
        bool IsVietnamese = true;
        bool CheckIfPressed = false;
        SpeechSynthesizer saysth = new SpeechSynthesizer();

        private string tts_api_url = "http://api.voicerss.org/";
        private string tts_api_key = "d4c101ae10e64ca2a65bca79e70c0a60";

        private string queryBuilder()
        {
            var uriBuilder = new UriBuilder(tts_api_url);
            var query = System.Net.WebUtility
            query["key"] = tts_api_key;
            if (IsVietnamese) query["hl"] = "vi-vn"; else query["hl"] = "en";
            query["src"] = textBox2.Text;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void process()
        {
            TextBox tb = txbx_input; int ch;
            tb.SelectionStart = tb.Text.Length;
            if (!CheckIfPressed) return;
            do
            {
                ch = tb.Text.IndexOf(',');
                if (ch != -1) CheckIfPressed = false;
                tb.Text = tb.Text.Substring(0, Math.Max(0, ch)) + tb.Text.Substring(ch + 1);
            }
            while (ch != -1);
            string display = "", tbaf = "";
            if (IsVietnamese)
            {
                textBox2.Text = "không";
                bool hasNumber = false, sfn1 = false, rest = false;
                for (int i = 0; i < tb.Text.Length; ++i)
                {
                    tbaf += tb.Text[i].ToString();
                    int j = 5 + numbers.IndexOf(tb.Text[i]);
                    switch ((tb.Text.Length - i) % 3)
                    {
                        case 0:
                            if (!(tb.Text[i] == '0' && tb.Text[i + 1] == '0' && tb.Text[i + 2] == '0'))
                            {
                                display += VN[j] + " " + VN[1] + " ";
                                hasNumber = true;
                            }
                            break;
                        case 2:
                            if (tb.Text[i] > '1')
                            {
                                display += VN[j] + " " + VN[0] + " ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] == '0')
                            { sfn1 = true; }
                            else
                            {
                                display += "mười" + " ";
                                hasNumber = true;
                            }
                            break;
                        case 1:
                            if (i != tb.Text.Length - 1)
                            {
                                tbaf += ",";
                            }
                            if (tb.Text[i] == '0') { rest = true; break; }
                            else if (sfn1)
                            {
                                display += "lẻ " + VN[j] + " ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] != '5' && tb.Text[i] >= '1')
                            {
                                display += VN[j] + " ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] == '5' && i != 0)
                            {
                                display += "lăm ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] == '5')
                            {
                                display += "năm ";
                                hasNumber = true;
                            }
                            rest = true;
                            break;

                    }
                    if ((tb.Text.Length - i - 1) % 3 == 0 && hasNumber)
                    {
                        int k = ((tb.Text.Length - i - 1) / 3);
                        while (k > 0)
                        {
                            int sfn = Math.Min((k + 2) % 3, 3) + 1;
                            k -= sfn;
                            display += VN[1 + sfn] + " ";
                        }
                    }
                    if (rest)
                    {
                        rest = false;
                        hasNumber = false;
                        sfn1 = false;
                    }

                }
            }
            else
            {
                textBox2.Text = "zero";
                bool hasNumber, Scnd, sfn1, rest;
                hasNumber = false; Scnd = false; sfn1 = false; rest = false;
                for (int i = 0; i < tb.Text.Length; ++i)
                {
                    tbaf += tb.Text[i];
                    int j = numbers.IndexOf(tb.Text[i]);
                    switch ((tb.Text.Length - i) % 3)
                    {
                        case 0:
                            if (!(tb.Text[i] == '0' && tb.Text[i + 1] == '0' && tb.Text[i + 2] == '0'))
                            {
                                display += EN1[j] + " " + EN4[0] + " ";
                                hasNumber = true;
                            }
                            break;
                        case 2:
                            if (tb.Text[i] > '1')
                            {
                                display += EN3[j - 2] + " ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] == '0')
                            { sfn1 = true; }
                            else
                            {
                                Scnd = true;
                                if (tb.Text[i + 1] == '0')
                                {
                                    display += "ten "; hasNumber = true;
                                }
                            }
                            break;
                        case 1:
                            if (i != tb.Text.Length - 1)
                            {
                                tbaf += ",";
                            }
                            if (tb.Text[i] == '0') { rest = true; break; }
                            else if (Scnd)
                            {
                                display += EN2[j] + " ";
                                hasNumber = true;
                            }
                            else if (sfn1)
                            {
                                display += "and " + EN1[j] + " ";
                                hasNumber = true;
                            }
                            else if (tb.Text[i] >= '1')
                            {
                                display += EN1[j] + " ";
                                hasNumber = true;
                            }
                            rest = true;
                            break;

                    }
                    if ((tb.Text.Length - i - 1) % 3 == 0 && hasNumber)
                    {
                        int k = ((tb.Text.Length - i - 1) / 3);
                        if (k > 0) display += EN4[k] + " ";
                    }
                    if (rest)
                    {
                        rest = false;
                        hasNumber = false;
                        Scnd = false;
                        sfn1 = false;
                    }

                }
            }
            textBox2.Text = display;
            CheckIfPressed = false;
            tb.Text = tbaf;
        }
        
        private void txbx_input_textChanged(object sender, EventArgs e)
        {
            process();
        }

        private void txbx_input_keyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txbx_input_keyPress(object sender, KeyPressEventArgs e)
        {
            string l = ",+-";
            TextBox tb = (sender as TextBox);
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && l.IndexOf(e.KeyChar) == -1) e.Handled = true;
            if (l.IndexOf(e.KeyChar) > 0 && tb.Text.IndexOf(e.KeyChar) != -1) e.Handled = true;
            if (l.IndexOf(e.KeyChar) > 0 && tb.SelectionStart > 0) e.Handled = true;
            CheckIfPressed = true;
        }

        private void VN_Click(object sender, EventArgs e)
        {
            
        }

        private void EN_Click(object sender, EventArgs e)
        {
            
        }

        private void TTS(object sender, EventArgs e)
        {
            if (IsVietnamese)
            {
                string request = queryBuilder();
                MediaFoundationReader mf = null;
                while (true)
                {
                    try
                    {
                        using (mf = new MediaFoundationReader(request))
                            break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }
                using (var wo = new WasapiOut())
                {
                    wo.Init(mf);
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }

            }
            else
            {
                saysth.Speak(textBox2.Text);
            }
        }

        private void EN(object sender, MouseEventArgs e)
        {
            IsVietnamese = false;
            CheckIfPressed = true;
            process();
        }

        private void vn(object sender, MouseEventArgs e)
        {
            IsVietnamese = true;
            CheckIfPressed = true;
            process();
        }
    }
}
