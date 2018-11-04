using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class Form1 : Form
    {
        Controller controller = new Controller();

        DataGridViewCell current;
        public Form1()
        {
            InitializeComponent();

            //TODO: Make this dynamic to add rows and columns dynamically based on Model.tileH and Model.tileW
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();

            FillTileSet();

        }

        private void FillTileSet()
        {
            //Fills Tile Options
            //TODO: Make this more dynamic. Get a list of tiles and dynamically fill this datagridview with them
            //That will make it easier to add tiles in the future
            Image i = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\1.jpg");
            Image j = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\2.jpg");
            Image k = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\3.jpg");
            Image l = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\4.jpg");
            Image m = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\7.jpg");
            Image n = Image.FromFile(@"C:\Users\Austin\Pictures\TestIcons\6.jpg");

            dataGridView3.Rows.Add();

            dataGridView3.Rows[0].Cells[0].Value = i;
            dataGridView3.Rows[0].Cells[0].Tag = 1;
            dataGridView3.Rows[0].Cells[1].Value = j;
            dataGridView3.Rows[0].Cells[1].Tag = 2;
            dataGridView3.Rows[0].Cells[2].Value = k;
            dataGridView3.Rows[0].Cells[2].Tag = 3;
            dataGridView3.Rows[0].Cells[3].Value = l;
            dataGridView3.Rows[0].Cells[3].Tag = 4;
            dataGridView3.Rows[0].Cells[4].Value = m;
            dataGridView3.Rows[0].Cells[4].Tag = 5;
            dataGridView3.Rows[0].Cells[5].Value = n;
            dataGridView3.Rows[0].Cells[5].Tag = 6;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            current = dataGridView1.CurrentCell;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "bork")
            {
                Image i = Image.FromFile(@"C:\Users\Austin\Pictures\Sasha\4.jpg");
                dataGridView1.CurrentCell.Value = i;
            }
            else if (comboBox1.Text == "Tile1")
            {
                Image i = Image.FromFile(@"C:\Users\Austin\Pictures\Sasha\492.gif");
                dataGridView1.CurrentCell.Value = i;
            }

            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.save();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.setCurrentEditorLevel();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentCell.Value = dataGridView3.CurrentCell.Value;
            controller.SetTileID(dataGridView1.CurrentCell.RowIndex, dataGridView1.CurrentCell.ColumnIndex, (int)dataGridView3.CurrentCell.Tag);
        }

        //private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    controller.export();
        //}
    }
}
