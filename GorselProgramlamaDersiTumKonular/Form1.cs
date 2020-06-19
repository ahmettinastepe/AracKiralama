using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using GorselProgramlamaDersiTumKonular.Model;

namespace GorselProgramlamaDersiTumKonular
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OleDbConnection baglanti = new OleDbConnection();
        OleDbCommand komut = new OleDbCommand();
        DataSet dataSet = new DataSet();
        Adres _adres = new Adres();

        private void VerileriGetir()
        {
            try
            {
                baglanti.Open();
                OleDbDataAdapter veriAl = new OleDbDataAdapter("Select * From Adresler", baglanti);
                dataSet.Clear();
                veriAl.Fill(dataSet, "Adresler");
                dataGridVeri.DataSource = dataSet.Tables["Adresler"];
                baglanti.Close();

            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Bağlantı Hatası:" + exception);
            }
        }

        private void VeriEkle()
        {
            komut.Connection = baglanti;
            komut.CommandText = "insert into Adresler values('" + txtTcNo.Text + "','" + txtAd.Text + "','" +
                                txtSoyad.Text + "','" + txtAdres.Text + "')";
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }

        void KayitSil(string tcNo)
        {
           var mesaj = MessageBox.Show(@"Seçili Kaydı Silmek İstediğinize Emin Misiniz?", @"Sil Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (mesaj == DialogResult.Yes)
            {
                komut.Connection = baglanti;
                komut.CommandText = "delete from Adresler" +
                                    " where TcNo='" + tcNo + "'";
                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show(@"Seçili Kayıt Silindi");
            }
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=AdresDefteri.accdb;" +
                                        "Persist Security Info=False;";

            VerileriGetir();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            VeriEkle();
            VerileriGetir();
        }

        private void btnKayitSil_Click(object sender, EventArgs e)
        {
            string tcNo = txtTcNo.Text;
            KayitSil(tcNo);
            VerileriGetir();
        }

        private void dataGridVeri_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            _adres.TcNo = dataGridVeri.Rows[e.RowIndex].Cells[0].Value.ToString();
            _adres.Ad = dataGridVeri.Rows[e.RowIndex].Cells[1].Value.ToString();
            _adres.Soyad = dataGridVeri.Rows[e.RowIndex].Cells[2].Value.ToString();
            _adres.Adresi = dataGridVeri.Rows[e.RowIndex].Cells[3].Value.ToString();

            VerileriDoldur(_adres);
        }

        private void VerileriDoldur(Adres adres)
        {
            txtTcNo.Text = adres.TcNo;
            txtAd.Text = adres.Ad;
            txtSoyad.Text = adres.Soyad;
            txtAdres.Text = adres.Adresi;
        }
    }
}