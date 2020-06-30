using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace sudoku
{
    public partial class Form1 : Form
    {

        TextBox[,] sudo = new TextBox[9, 9];

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            olustur();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            yeniOyun();
        }

        public void olustur()
        {           
            int locationa = 30;
            int locationb = 30;
            int sayaca = 0;
            int sayacb = 0;
            Font tfont = button1.Font;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudo[i, j] = new TextBox();
                    sudo[i, j].TextChanged += new EventHandler(giris);
                    Controls.Add(sudo[i, j]);
                    sudo[i, j].Width = 30;
                    sudo[i, j].Font = new Font(tfont.FontFamily, 15);
                    sudo[i, j].MaxLength = 1;
                    sudo[i, j].Location = new Point(locationa, locationb);
                    sudo[i, j].TextAlign = HorizontalAlignment.Center;
                    sayaca++;
                    locationa += 32;       
                    if (sayaca == 3) { sayaca = 0; locationa += 5; }
                }
                locationa = 30;
                locationb += 32;
                sayacb++;
                if (sayacb == 3) { sayacb = 0; locationb += 5; }
            }
            locationb = 30;
        }

        public void yeniOyun()
        {
            sifirla();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudo[i, j].Text = "";
                    sudo[i, j].Enabled = true;
                    sudo[i, j].BackColor = default;
                }
            }

            string dizi = ""; // sonuç stringi.
            bool sil = false; // ilk satır silmek için kontrol değişkeni
            String URLString = " https://www.noordpress.com/sudoku.php"; // xml link
            XmlTextReader reader = new XmlTextReader(URLString); // xml okumak için reader tanımladık.
            while (reader.Read()) // okuma işlemi
            {
                dizi += reader.Value.Replace(",", string.Empty); // veriyi dizi adlı string içinde birleştir.
                if (sil == false) { sil = true; dizi = ""; } // ilk satırla gelen sürüm bilgilerini dahil etme.
            }
            int sayac = 0; // hücrelere numaraları aktarırken kullanacağımız parametre değişkeni.
            for (int i = 0; i < 9; i++) // 9x9 hücre için for döngüsü
            {
                for (int j = 0; j < 9; j++)
                {
                    if (dizi.Substring(sayac, 1) != "0") // eğer numara 0 ise hücreyi boş bırak.
                    {
                        sudo[i, j].Text = dizi.Substring(sayac, 1); 
                        sudo[i, j].Enabled = false; // doldurulan hücreyi düzenlemeyi kapat
                    }
                    sayac++; // sayacı arttır.
                }
            }
            reader.Close();
        }

        public bool kontrol()
        {
            for (int i = 0; i < 9; i++) // her satır için tekrar kullanım kontrolü
            {
                int[] ctrl = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; 
                for (int j = 0; j < 9; j++)
                {
                    if (sudo[i, j].Text != "") 
                    {                   
                    ctrl[Convert.ToInt32(sudo[i, j].Text)]++;
                    // tekrar kullanımda false döndürür
                    if (ctrl.Contains(2) == true) { return (false); } 
                    }
                }
            }

            for (int i = 0; i < 9; i++) // her sütun için tekrar kullanım kontrolü
            {
                int[] ctrl = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int j = 0; j < 9; j++)
                {
                    if (sudo[j, i].Text != "")
                    {
                        ctrl[Convert.ToInt32(sudo[j, i].Text)]++;
                        // tekrar kullanımda false döndürür
                        if (ctrl.Contains(2) == true) { return (false); }
                    }
                }
            }

            // her 3x3 hücre grubu için döngü
            for (int y = 0; y < 7; y += 3)
            {
                for (int x = 0; x < 7; x += 3)
                {
                    int[] ctrl = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (sudo[(i + y), (j + x)].Text != "")
                            {
                                ctrl[Convert.ToInt32(sudo[(i + y), (j + x)].Text)]++;
                                // tekrar kullanımda false döndürür
                                if (ctrl.Contains(2) == true) { return (false); }
                            }
                        }
                    }
                }
            }
            // hatalara takılmaz ise true döndür
            return (true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            kaydet();         
        }

        public void kaydet()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "XML Dosyası|*.xml";
            save.OverwritePrompt = true;
            save.CreatePrompt = true;
            save.Title = "İlerlemeni Kaydet";

            if (save.ShowDialog() == DialogResult.OK)
            {
                string text = "";
                XmlWriter xmlYazici = XmlWriter.Create(save.FileName);

                xmlYazici.WriteStartDocument();
                xmlYazici.WriteStartElement("puzzle");
                xmlYazici.WriteStartElement("rows");
                for (int i = 0; i < 9; i++)
                {
                    xmlYazici.WriteStartElement("row");
                    for (int j = 0; j < 9; j++)
                    {
                        if (sudo[i, j].Text == "") {
                            text += "0";
                        }
                        else
                        {
                            text += sudo[i, j].Text;
                            if (j != 8) { text += ","; }
                        }
                    }
                    xmlYazici.WriteString(text);
                    xmlYazici.WriteEndElement();
                    text = "";
                }
                xmlYazici.WriteEndElement();
                xmlYazici.WriteEndElement();
                xmlYazici.WriteEndDocument();
                xmlYazici.Close();
            }           
        }

        public void kayitac()
        {          
            OpenFileDialog file = new OpenFileDialog();
            file.RestoreDirectory = true;
            file.Filter = "XML Dosyası|*.xml";
            file.Title = "Kayıtlı Oyunun XML Dosyasını Seçin.";

            if (file.ShowDialog() == DialogResult.OK)
            {
                string DosyaYolu = file.FileName;

                sifirla();
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        sudo[i, j].Text = "";
                        sudo[i, j].Enabled = true;
                    }
                }

                string dizi = "";
                bool sil = false;
                String URLString = file.FileName;
                XmlTextReader reader = new XmlTextReader(URLString);
                while (reader.Read())
                {
                    dizi += reader.Value.Replace(",", string.Empty);
                    if (sil == false) { sil = true; dizi = ""; } // ilk satırla gelen sürüm bilgilerini sildik.
                }
                int sayac = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (dizi.Substring(sayac, 1) != "0")
                        {
                            sudo[i, j].Text = dizi.Substring(sayac, 1);
                            sudo[i, j].Enabled = false;
                        }
                        sayac++;
                    }
                }
                reader.Close();
                panel1.Visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EGE ÜNİVERSİTESİ\nTire Kutsan Meslek Yüksekokulu\nBilgisayar Programcılığı\nGöksel Sina BİLECEN\n60180000018");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            yeniOyun();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult cikis = new DialogResult();
            cikis = MessageBox.Show("İlerlemenizi kaydetmediyseniz silinecek. Devam etmek istiyormusunuz ?", "Ana Menü", MessageBoxButtons.YesNo);
            if (cikis == DialogResult.Yes)
            {
                sifirla();
                panel1.Visible = true;
            }
        }
        protected void giris(object sender, EventArgs e)
        {
            int ti=0, tj=0; // mevcut textBox parametrelerini tutmak için değişken oluşturduk.
            for (int i = 0; i < 9; i++) // 9x9 hücre döngüsü
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudo[i, j].Focused) { ti = i; tj = j; break; } // odaklanmış ise parametreyi değişkenlere al
                }                 
            }
            
            // kullanıcının sayı dışında veri girmesine karşı..
            long val; // long değişşkeni oluşturduk
            try{
                // girilen değer boş değilse Int64 çevirme işlemi yap
                if (sudo[ti, tj].Text != "") { val = System.Convert.ToInt64(sudo[ti, tj].Text); } 
            }
            catch{ // hata veriyorsa sayı değildir
                MessageBox.Show("Sadece rakam girebilirsiniz."); // gerekli uyarıyı ver
                sudo[ti, tj].Clear(); // mevcut textBox verisini temizle
            }

            if (!kontrol()) { // kontrol metodu false değer döndürürse yani tekrar kullanım yapılmış ise
                MessageBox.Show("Girdiğiniz rakam bu hücre içerisinde tekrar kullanılamaz."); // uyarı ver
                sudo[ti, tj].Clear(); // mevcut textBox verisini temizle
            }

            // kullanıcı 0 girerse
            if (sudo[ti, tj].Text == "0") { MessageBox.Show("Yalnız 1'den 9'a kadar olan rakamları kullanabilirsiniz."); sudo[ti, tj].Clear(); }

            // tüm hücreler hatalara takılmadan doldurulmuş ise oyunBitti metodunu çalıştır 
            if (doluMu()) { MessageBox.Show("Tebrikler. Oyunu Kazandınız !"); oyunBitti(); }
        }

        public bool doluMu()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // textBox dolu mu kontrolü. 
                    if (sudo[i, j].Text == "") { return (false); }
                }
            }

            // tüm hücreler dolu ise true döndür
            return (true);
        }

        public void oyunBitti()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // oyun kazanıldıysa tüm hücreler için düzenlemeyi kapat
                    // tüm hücreleri yeşile boya :)
                    sudo[i, j].Enabled = false;
                    sudo[i, j].BackColor = Color.Green;
                }
            }
        }

        public void sifirla() // tüm hücreleri temizler
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudo[i, j].Clear();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            kayitac();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
