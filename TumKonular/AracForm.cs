using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.SqlClient;
using TumKonular.Reports;

namespace TumKonular
{
    public partial class AracForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        private bool _secim;

        public AracForm()
        {
            InitializeComponent();
        }
        public AracForm(params object[] prm)
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

        private void VerileriGetir()
        {
            try
            {
                DataTable dt_Adresler = new DataTable("Araclar");
                SqlDataAdapter adaptor = new SqlDataAdapter("Select * from Araclar", connection);
                Connect();
                adaptor.Fill(dt_Adresler);
                gridControl.DataSource = dt_Adresler;
                connection.Close();
            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata Oluştu.." + hata.Message);
            }
        }

        private void Temizle()
        {
            lblAracNo.Text = "";
            txtMarka.Text = "";
            txtModel.Text = "";
            txtPlaka.Text = "";
        }

        private void AracForm_Load(object sender, EventArgs e)
        {
            connection.ConnectionString = AnaForm.ConnectionString;
            command.Connection = connection;
            VerileriGetir();
        }

        private void btnKaydet_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool _eklemi = lblAracNo.Text == "";

            if (_eklemi)
            {
                try
                {
                    Connect();
                    command.CommandText = "insert into Araclar values(@Marka,@Model,@Plaka)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Marka", txtMarka.Text.Trim());
                    command.Parameters.AddWithValue("@Model", txtModel.Text.Trim());
                    command.Parameters.AddWithValue("@Plaka", txtPlaka.Text.Trim());
                    MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Araç Eklendi" : "Araç Ekleme Başarısız");
                    connection.Close();
                    VerileriGetir();
                    Temizle();
                    gridControl.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir Hata Meydana Geldi: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    Connect();
                    command.CommandText = "Update Araclar set Marka=@Marka,Model=@Model,Plaka=@Plaka Where AracNo=@AracNo";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Marka", txtMarka.Text.Trim());
                    command.Parameters.AddWithValue("@Model", txtModel.Text.Trim());
                    command.Parameters.AddWithValue("@Plaka", txtPlaka.Text.Trim());
                    command.Parameters.AddWithValue("@AracNo", lblAracNo.Text.Trim());
                    MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Araç Güncellendi" : "Güncelleme Başarısız");
                    connection.Close();
                    VerileriGetir();
                    Temizle();
                    txtMarka.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir Hata Meydana Geldi: " + ex.Message);
                }
            }
        }

        private void btnSil_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MessageBox.Show("Seçili Kaydı Silmek İstediğinize Emin Misiniz?", "Sil Onay", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                command.CommandText = "Delete From Araclar Where AracNo=@AracNo";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AracNo", lblAracNo.Text.Trim());
                Connect();
                MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Araç Silindi" : "Araç Bulunamadı.");
                connection.Close();
                VerileriGetir();
                Temizle();
                txtMarka.Focus();
            }
        }

        private void btnTemizle_ItemClick(object sender, ItemClickEventArgs e)
        {
            Temizle();
        }

        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            _seciliKayit = gridView.FocusedRowHandle;

            if (_secim)
            {
                KiralaForm.AracId = Convert.ToInt32(gridView.GetRowCellValue(_seciliKayit, "AracNo"));
                this.Close();
            }

            lblAracNo.Text = gridView.GetRowCellValue(_seciliKayit, "AracNo").ToString();
            txtMarka.Text = gridView.GetRowCellValue(_seciliKayit, "Marka").ToString();
            txtModel.Text = gridView.GetRowCellValue(_seciliKayit, "Model").ToString();
            txtPlaka.Text = gridView.GetRowCellValue(_seciliKayit, "Plaka").ToString();
        }

        private void barBtnAracRapor_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowReportForm report=new ShowReportForm(new AraclarReport());
            report.Show();
        }
    }
}