using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SudokuGame
{
    public partial class MainForm : Form
    {
        SudokuClass game = new SudokuClass();
        AboutForm about;

        public MainForm()
        {
            InitializeComponent();
            this.PuzzlePanel.Controls.Add(game.mainTable);
            SetStyle(ControlStyles.UserPaint |
                      ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.OptimizedDoubleBuffer,
                      true);
        }

        private void yeniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.NewGame();
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new AboutForm();
            }

            about.ShowDialog();
        }
    }
}