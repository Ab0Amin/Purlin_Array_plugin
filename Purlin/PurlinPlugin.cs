using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tekla.Structures.Dialog;

namespace Purlin_Array
{
    public partial class PurlinPlugin : PluginFormBase
    {
        public PurlinPlugin()
        {
            InitializeComponent();
            imageComboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }
        private void okApplyModifyGetOnOffCancel1_ApplyClicked(object sender, EventArgs e)
        {
            this.Apply();
        }

        private void okApplyModifyGetOnOffCancel1_CancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okApplyModifyGetOnOffCancel1_GetClicked(object sender, EventArgs e)
        {
            this.Get();
        }

        private void okApplyModifyGetOnOffCancel1_ModifyClicked(object sender, EventArgs e)
        {
            this.Modify();
        }

        private void okApplyModifyGetOnOffCancel1_OkClicked(object sender, EventArgs e)
        {
            this.Apply();
            this.Close();
        }

        private void okApplyModifyGetOnOffCancel1_OnOffClicked(object sender, EventArgs e)
        {
            this.ToggleSelection();
        }

        private void profileCatalog1_SelectClicked(object sender, EventArgs e)
        {
            profileCatalog1.SelectedProfile = tx_profile.Text;
            
        }

        private void profileCatalog1_SelectionDone(object sender, EventArgs e)
        {
            SetAttributeValue(tx_profile, profileCatalog1.SelectedProfile);

        }

        private void materialCatalog1_SelectClicked(object sender, EventArgs e)
        {
            materialCatalog1.SelectedMaterial = tx_material.Text;

        }

        private void materialCatalog1_SelectionDone(object sender, EventArgs e)
        {
            SetAttributeValue(tx_material, materialCatalog1.SelectedMaterial);

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void profileCatalog1_DoubleClick(object sender, EventArgs e)
        {
            SetAttributeValue(tx_profile, profileCatalog1.SelectedProfile);
            profileCatalog1.Enabled = true;
            profileCatalog1.FindForm();
            profileCatalog1.Dispose();

        }

        private void imageComboBox1_TabIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void imageComboBox1_ImageCBSelectedIndexChanged(object sender, EventArgs e)
        {
            if (imageComboBox1.SelectedIndex == 2)
            {
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                textBox11.Enabled = true;
            }
            else
            {
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                textBox11.Enabled = false;

            }
        }
    }
}
