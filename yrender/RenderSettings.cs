using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yrender
{
    public partial class RenderSettings : Form
    {
        public RenderSettings()
        {
            InitializeComponent();
        }

        private void RenderSettings_Load(object sender, EventArgs e)
        {

        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
