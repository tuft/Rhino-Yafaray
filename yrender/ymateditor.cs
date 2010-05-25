using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.OpenNURBS;
using System.IO;
using System.Xml;


namespace yrender
{
    partial class ymateditor : Form
    {
        private OnMaterial rhinoMaterial;
        private MaterialManager materials;
        private string currentMaterial;

        public ymateditor(ref OnMaterial material, ref MaterialManager materials)
        {
            this.rhinoMaterial = material;
            this.materials = materials;                   
            InitializeComponent();
        }
        private void ymateditor_Load(object sender, EventArgs e)
        {
            // load material settings
            matDefinitionText.Enabled = false;
            string materialName = "";
            this.rhinoMaterial.GetUserString("yafaray_material", ref materialName);
            if (materialName == "") materialName = "defaultMat";            
            this.materialName.Text = materialName;            
            this.updateMaterials();
            this.setUpForm();
        }

        private void setUpForm()
        {
            string posibleName = this.materialName.Text.Trim();
            if (posibleName == "") matDefinitionText.Enabled = false;
            else
            {
                matDefinitionText.Enabled = true;
                if (materials.exists(posibleName))
                {
                    this.currentMaterial = posibleName;
                    this.newButton.Enabled = false;
                    this.OKButton.Enabled = true;
                    matDefinitionText.Enabled = true;
                    matDefinitionText.Text = materials.get(posibleName).serialize();
                }
                else
                {
                    this.currentMaterial = "";
                    this.newButton.Enabled = true;
                    matDefinitionText.Enabled = false;
                    this.OKButton.Enabled = false;
                }
            }
        }
        private void updateMaterials() 
        {
            this.materialName.Items.Clear();
            this.materialName.Items.AddRange(materials.getNames().ToArray());
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            // save material settings
            string selectedMaterial = this.materialName.Text.Trim();
            this.rhinoMaterial.SetUserString("yafaray_material", selectedMaterial);
            this.Close();
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialName_TextChanged(object sender, EventArgs e)
        {
            this.setUpForm();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            materials.create(materialName.Text);
            materialName.Items.Add(materialName.Text);
            this.setUpForm();
        }

        private void matDefinitionText_TextChanged(object sender, EventArgs e)
        {
            materials.get(this.currentMaterial).setDefinition(this.matDefinitionText.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //todo: move loading part to MaterialManager
            
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            StreamReader reader = new StreamReader(myStream);
                            this.materials.load(reader);
                            this.updateMaterials();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }                
    }
}
