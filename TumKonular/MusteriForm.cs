using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.SqlClient;
using TumKonular.Reports;

namespace TumKonular
{
    public partial class MusteriForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private bool _secim;

        public MusteriForm()
        {
            InitializeComponent();
        }

        public MusteriForm(params object[] prm)
        {
            InitializeComponent();
            _secim = (bool)prm[0];
        }

        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        private int _seciliKayit;

        private void Connect()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        private void Temizle()
        {
            lblKod.Text = "";
            txtTcNo.Text = "";
            txtAd.Text = "";
            txtSoyad.Text = "";
            txtAdres.Text = "";
            txtTel.Text = "";
        }

        private void VerileriGetir()
        {
            try
            {
                DataTable dataTable = new DataTable("Musteri");
                SqlDataAdapter adaptor = new SqlDataAdapter("Select * from Musteri", connection);
                Connect();
                adaptor.Fill(dataTable);
                gridControl.DataSource = dataTable;
                connection.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata Oluştu.." + hata.Message);
            }
        }

        private void MusteriForm_Load(object sender, EventArgs e)
        {
            connection.ConnectionString = AnaForm.ConnectionString;
            command.Connection = connection;
            VerileriGetir();
        }

        private void btnKaydet_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _eklemi = lblKod.Text == "";

            if (_eklemi)
            {
                Connect();
                command.CommandText = "insert into Musteri values(@TcNo,@Ad,@Soyad,@Adres,@Tel)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@TcNo", txtTcNo.Text.Trim());
                command.Parameters.AddWithValue("@Ad", txtAd.Text.Trim());
                command.Parameters.AddWithValue("@Soyad", txtSoyad.Text.Trim());
                command.Parameters.AddWithValue("@Adres", txtAdres.Text.Trim());
                command.Parameters.AddWithValue("@Tel", txtTel.Text.Trim());
                MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Müşteri Eklendi" : "Müşteri Ekleme Başarısız");
                connection.Close();
                VerileriGetir();
                Temizle();
                txtTcNo.Focus();
            }
            else
            {
                Connect();
                command.CommandText = "Update Musteri set Tcno=@TcNo,Ad=@Ad,Soyad=@Soyad,Adres=@Adres,Tel=@Tel Where MKod=@Mkod";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@TcNo", txtTcNo.Text.Trim());
                command.Parameters.AddWithValue("@Ad", txtAd.Text.Trim());
                command.Parameters.AddWithValue("@Soyad", txtSoyad.Text.Trim());
                command.Parameters.AddWithValue("@Adres", txtAdres.Text.Trim());
                command.Parameters.AddWithValue("@Tel", txtTel.Text.Trim());
                command.Parameters.AddWithValue("@Mkod", lblKod.Text.Trim());
                MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Müşteri Güncellendi" : "Güncelleme Başarısız");
                connection.Close();
                VerileriGetir();
                Temizle();
                txtTcNo.Focus();
            }
        }

        private void btnSil_ItemClick(object sender, ItemClickEventArgs e)
        {
            var message = MessageBox.Show("Seçili Kaydı Silmek İstediğinize Emin Misiniz?", "Sil Onay",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question) == DialogResult.Yes;

            if (message)
            {
                command.CommandText = "Delete From Musteri Where MKod=@Mkod";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Mkod", lblKod.Text.Trim());
                Connect();
                MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Müşteri Silindi" : "Müşteri Bulunamadı.");
                connection.Close();
                VerileriGetir();
                Temizle();
                txtTcNo.Focus();
            }
        }

        private void btnTemizle_ItemClick(object sender, ItemClickEventArgs e)
        {
            Temizle();
        }

        private void btnKapat_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            _seciliKayit = gridView.FocusedRowHandle;

            if (_secim)
            {
                KiralaForm.MusteriId = Convert.ToInt32(gridView.GetRowCellValue(_seciliKayit, "MKod"));
                this.Close();
            }

            lblKod.Text = gridView.GetRowCellValue(_seciliKayit, "MKod").ToString();
            txtTcNo.Text = gridView.GetRowCellValue(_seciliKayit, "Tcno").ToString();
            txtAd.Text = gridView.GetRowCellValue(_seciliKayit, "Ad").ToString();
            txtSoyad.Text = gridView.GetRowCellValue(_seciliKayit, "Soyad").ToString();
            txtTel.Text = gridView.GetRowCellValue(_seciliKayit, "Tel").ToString();
            txtAdres.Text = gridView.GetRowCellValue(_seciliKayit, "Adres").ToString();
        }

        private void barBtnMusteriReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowReportForm report=new ShowReportForm(new MusteriReport());
            report.Show();
        }
    }
}