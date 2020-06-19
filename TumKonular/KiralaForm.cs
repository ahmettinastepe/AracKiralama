using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Linq;
using TumKonular.Table;
using DevExpress.XtraBars.Ribbon;
using TumKonular.Reports;

namespace TumKonular
{
    public partial class KiralaForm : RibbonForm
    {
        #region Variables

        public static int MusteriId;
        public static int AracId;
        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        private List<AracKirala> Kiralamalar = new List<AracKirala>();

        #endregion

        public KiralaForm()
        {
            InitializeComponent();
        }

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
                DataTable dataTable = new DataTable("ViewAracKira");
                SqlDataAdapter adaptor = new SqlDataAdapter("Select * from ViewAracKira", connection);
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
        private void AracBilgileriGetir(int aracId)
        {
            try
            {
                Connect();
                var idYeGoreGetir = "Select * From Araclar Where AracNo=" + aracId;
                var komut = new SqlCommand(idYeGoreGetir, connection);
                var dataReader = komut.ExecuteReader();
                while (dataReader.Read())
                {
                    btnAracSec.Text = dataReader["Marka"].ToString();
                    txtModel.Text = dataReader["Model"].ToString();
                    txtPlaka.Text = dataReader["Plaka"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir Hata Meydana Geldi:\n" + ex.Message);
            }
        }
        private void MusteriBilgileriGetir(int musteriId)
        {
            try
            {
                Connect();
                var idYeGoreGetir = "Select * From Musteri Where MKod=" + musteriId;
                var komut = new SqlCommand(idYeGoreGetir, connection);
                var dataReader = komut.ExecuteReader();
                while (dataReader.Read())
                {
                    btnMusteriSec.Text = dataReader["Ad"].ToString();
                    txtSoyadi.Text = dataReader["Soyad"].ToString();
                    txtTcNo.Text = dataReader["Tcno"].ToString();
                    txtTel.Text = dataReader["Tel"].ToString();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir Hata Meydana Geldi:\n" + ex.Message);
            }
        }
        private void Temizle()
        {
            btnMusteriSec.Text = "";
            txtSoyadi.Text = "";
            txtTcNo.Text = "";
            txtTel.Text = "";

            btnAracSec.Text = "";
            txtModel.Text = "";
            txtPlaka.Text = "";

            txtBaslamaTarihi.EditValue = DateTime.Now.Date;
            txtBitisTarihi.EditValue = DateTime.Now.Date.AddDays(1);
        }
        private void AracKirala()
        {
            var dialogMessage = MessageBox.Show(
                $"{btnMusteriSec.Text} {txtSoyadi.Text} Adlı Müşteriye {btnAracSec.Text} Markalı {txtModel.Text} Model Aracı {txtBaslamaTarihi.Text} Tarihinden {txtBitisTarihi.Text} Tarihine Kadar Kiralamak Üzeresiniz. İşlemi Onaylıyor Musunuz?", "Kiralama Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogMessage == DialogResult.Yes)
            {
                try
                {
                    Connect();
                    command.CommandText = "insert into Kiralama values(@MKod,@AracNo,@Tarih,@TesTarih)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@MKod", MusteriId);
                    command.Parameters.AddWithValue("@AracNo", AracId);
                    command.Parameters.AddWithValue("@Tarih", txtBaslamaTarihi.DateTime.Date);
                    command.Parameters.AddWithValue("@TesTarih", txtBitisTarihi.DateTime.Date);
                    MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Araç Müşteriye Kiralandı." : "Bir Hata Meydana Geldi. Araç Kiralanamadı.");
                    connection.Close();
                    VerileriGetir();
                    Temizle();
                    btnKirala.Enabled = btnMusteriSec.Text != "";
                    btnAracSec.Enabled = btnMusteriSec.Text != "";
                    btnMusteriSec.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void AracKiralaSorgu()
        {
            try
            {
                Connect();
                var veriyiGetir = "Select * From ViewAracKira where AracNo=" + AracId;
                var komut = new SqlCommand(veriyiGetir, connection);
                var dataReader = komut.ExecuteReader();
                while (dataReader.Read())
                {
                    var kira = new AracKirala
                    {
                        MusteriKod = Convert.ToInt32(dataReader["MKod"]),
                        AracNo = Convert.ToInt32(dataReader["AracNo"]),
                        MusteriAd = dataReader["Ad"].ToString(),
                        MusteriSoyad = dataReader["Soyad"].ToString(),
                        Telefon = dataReader["Tel"].ToString(),
                        Marka = dataReader["Marka"].ToString(),
                        Model = dataReader["Model"].ToString(),
                        Plaka = dataReader["Plaka"].ToString(),
                        BaslamaTarihi = Convert.ToDateTime(dataReader["Tarih"]),
                        TeslimTarihi = Convert.ToDateTime(dataReader["TesTarih"])
                    };

                    Kiralamalar.Add(kira);
                }
                connection.Close();

                if (Kiralamalar.Count > 0)
                {
                    foreach (var item in Kiralamalar)
                    {
                        if (item.AracNo == AracId && item.MusteriKod == MusteriId)
                        {
                            if (Kiralamalar.Last() == item)
                            {
                                if (txtBitisTarihi.DateTime.Date >= item.TeslimTarihi)
                                {
                                    Guncelle();
                                }
                                else
                                {
                                    MessageBox.Show("Kira Başlama Tarihi Teslim Tarihinden Küçük Olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtBaslamaTarihi.EditValue = item.BaslamaTarihi;
                                    txtBitisTarihi.EditValue = item.TeslimTarihi.AddDays(1);
                                }
                            }
                        }
                        else if (item.AracNo == AracId && item.MusteriKod != MusteriId)
                        {
                            if (Kiralamalar.Last() == item)
                            {
                                if (txtBaslamaTarihi.DateTime.Date < item.TeslimTarihi)
                                {
                                    MessageBox.Show($"Araç, {item.MusteriAd} {item.MusteriSoyad} Adlı Müşteriye Kiralanmış. Rezervasyon İşlemi Yapılabilir.\nAraç Teslim Tarihi:{item.TeslimTarihi}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtBaslamaTarihi.EditValue = item.TeslimTarihi.Date.AddDays(1);
                                    txtBitisTarihi.EditValue = item.TeslimTarihi.Date.AddDays(2);

                                }
                                else if (txtBaslamaTarihi.DateTime.Date > item.TeslimTarihi)
                                    AracKirala();
                            }
                        }
                    }
                }
                else
                    AracKirala();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir Hata Meydana Geldi:\n" + ex.Message);
            }

            Kiralamalar.Clear();
        }
        private void Guncelle()
        {
            var dialogMessage = MessageBox.Show(
               $"{btnMusteriSec.Text} {txtSoyadi.Text} Adlı Müşteriye {btnAracSec.Text} Markalı {txtModel.Text} Model Arac {txtBitisTarihi.Text} Tarihine Kadar Süresi Uzatılcaktır.. İşlemi Onaylıyor Musunuz?", "Kiralama Onay",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogMessage == DialogResult.Yes)
            {
                try
                {
                    Connect();
                    command.CommandText = "update Kiralama set TesTarih=@TesTarih where MKod=" + MusteriId + " and AracNo=" + AracId;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@TesTarih", txtBitisTarihi.DateTime.Date);
                    MessageBox.Show(command.ExecuteNonQuery() >= 1 ? "Araç Teslim Süresi Uzatıldı." : "Bir Hata Meydana Geldi");
                    connection.Close();
                    VerileriGetir();
                    Temizle();
                    btnKirala.Enabled = btnMusteriSec.Text != "";
                    btnAracSec.Enabled = btnMusteriSec.Text != "";
                    btnMusteriSec.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void KiralaForm_Load(object sender, EventArgs e)
        {
            connection.ConnectionString = AnaForm.ConnectionString;
            command.Connection = connection;
            VerileriGetir();
            txtBaslamaTarihi.EditValue = DateTime.Now.Date;
            txtBitisTarihi.EditValue = DateTime.Now.Date.AddDays(1);
        }
        private void btnMusteriSec_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            MusteriForm form = new MusteriForm(true);
            form.ShowDialog();
            MusteriBilgileriGetir(MusteriId);
            btnAracSec.Enabled = MusteriId > 0;
        }
        private void btnAracSec_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            AracForm form = new AracForm(true);
            form.ShowDialog();
            AracBilgileriGetir(AracId);
            btnKirala.Enabled = btnMusteriSec.Text != "" && btnAracSec.Text != "";
        }
        private void btnKirala_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AracKiralaSorgu();
        }
        private void btnKapat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void KiralaForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                AracKiralaSorgu();
        }

        private void barBtnKiralamaReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowReportForm report = new ShowReportForm(new KiralamaReport());
            report.Show();
        }
    }
}