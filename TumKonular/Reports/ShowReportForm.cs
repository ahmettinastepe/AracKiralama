using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;

namespace TumKonular.Reports
{
    public partial class ShowReportForm : DevExpress.XtraEditors.XtraForm
    {
        public ShowReportForm(XtraReport report)
        {
            InitializeComponent();
            documentViewer.DocumentSource = report;
        }
    }
}