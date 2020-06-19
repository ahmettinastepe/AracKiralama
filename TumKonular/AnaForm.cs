using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace TumKonular
{
    public partial class AnaForm : RibbonForm
    {
        public static string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB; Initial Catalog=ARACKIRALAMA; Integrated Security=true;";

        public AnaForm()
        {
            InitializeComponent();
        }

        private void barBtnKiralama_ItemClick(object sender, ItemClickEventArgs e)
        {
            KiralaForm form = new KiralaForm();
            form.MdiParent = this;
            form.Show();
        }

        private void barBtnAraclar_ItemClick(object sender, ItemClickEventArgs e)
        {
            AracForm form = new AracForm();
            form.MdiParent = this;
            form.Show();
        }

        private void barBtnMusteriler_ItemClick(object sender, ItemClickEventArgs e)
        {
            MusteriForm form = new MusteriForm();
            form.MdiParent = this;
            form.Show();
        }

        private void barBtnCikis_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dialog = MessageBox.Show("Programdan Çıkmak İstediğinize Emin Misiniz?", "Çıkış Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
                Application.ExitThread();
        }
    }
}