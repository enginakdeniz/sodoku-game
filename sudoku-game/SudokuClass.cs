using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace SudokuGame
{

    public class SudokuCell : Label
    {
        public int col { get; set; }
        public int row { get; set; }
        public int val { get; set; }

        public SudokuCell(int c, int r)
        {
            Dock = DockStyle.Fill;
            AutoSize = false;
            TextAlign = ContentAlignment.MiddleCenter;
            Margin = Padding.Empty;
            Padding = Padding.Empty;

            col = c;
            row = r;
            this.HelpRequested += new HelpEventHandler(SudokuCell_HelpRequested);
        }

        void SudokuCell_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("C:" + col.ToString() + ", R:" + row.ToString());
        }

        public void SetValue(int v, bool h)
        {
            this.val = v;
            if (!h)
            {
                this.Text = v.ToString();
                this.Font = new Font(this.Font, FontStyle.Bold);
            }
            else
            {
                this.Font = this.Font = new Font(this.Font, FontStyle.Regular);
            }
        }

        public void ClearValue()
        {
            Text = "";
            val = 0;
        }
    }

    public class SudokuClass
    {
        private const int max_col_count = 9;
        private const int max_row_count = 9;

        private int[,] solved = new int[max_col_count, max_row_count]
            {  //1  2  3  4  5  6  7  8  9
                {8, 2, 7, 1, 5, 4, 3, 9, 6}, //1
                {9, 6, 5, 3, 2, 7, 1, 4, 8}, //2
                {3, 4, 1, 6, 8, 9, 7, 5, 2}, //3
                {5, 9, 3, 4, 6, 8, 2, 7, 1}, //4
                {4, 7, 2, 5, 1, 3, 6, 8, 9}, //5
                {6, 1, 8, 9, 7, 2, 4, 3, 5}, //6
                {7, 8, 6, 2, 3, 5, 9, 1, 4}, //7
                {1, 5, 4, 7, 9, 6, 8, 2, 3}, //8
                {2, 3, 9, 8, 4, 1, 5, 6, 7}  //9
            };

        private int[,] puzzle = new int[max_col_count, max_row_count];
        private bool[,] hidden = new bool[max_col_count, max_row_count];

        private ContextMenuStrip SudokuInputMenu = new ContextMenuStrip();
        private ToolStripTextBox InputBox = new ToolStripTextBox("InputBox");

        public TableLayoutPanel mainTable = new TableLayoutPanel();
        private TableLayoutPanel[,] subTables = new TableLayoutPanel[3, 3];

        public SudokuClass()
        {
            InputBox.TextBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            SudokuInputMenu.ShowImageMargin = false;
            SudokuInputMenu.Items.Add(InputBox);
            mainTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            mainTable.RowCount = 3;
            mainTable.ColumnCount = 3;
            mainTable.Dock = DockStyle.Fill;
            for (int i = 0; i < 3; i++)
            {
                mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    subTables[i, j] = new TableLayoutPanel();
                    subTables[i, j].Dock = DockStyle.Fill;
                    subTables[i, j].CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                    subTables[i, j].Margin = Padding.Empty;
                    subTables[i, j].Padding = Padding.Empty;
                    subTables[i, j].RowCount = 3;
                    subTables[i, j].ColumnCount = 3;

                    for (int a = 0; a < 3; a++)
                    {
                        subTables[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 33));
                        subTables[i, j].ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
                        for (int b = 0; b < 3; b++)
                        {
                            SudokuCell cell = new SudokuCell((i * 3) + b,
                                                             (j * 3) + a);
                            cell.Click += new EventHandler(cell_Click);
                            subTables[i, j].Controls.Add(cell, a, b);
                        }
                    }

                    mainTable.Controls.Add(subTables[i, j]);
                }
            }
        }

        void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                ((SudokuCell)SudokuInputMenu.Tag).Text = InputBox.TextBox.Text;
                InputBox.TextBox.Text = "";
                SudokuInputMenu.Hide();
                e.Handled = true;
            }
            else if ((int)e.KeyChar >= 47 && (int)e.KeyChar <= 57)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        void cell_Click(object sender, EventArgs e)
        {
            if (hidden[((SudokuCell)sender).col, ((SudokuCell)sender).row])
            {
                SudokuInputMenu.Tag = sender;
                SudokuInputMenu.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        public void NewGame()
        {
            ShufflePuzzle();
            foreach (TableLayoutPanel subTable in mainTable.Controls)
            {
                foreach (SudokuCell item in subTable.Controls)
                {
                    item.ClearValue();
                    item.SetValue(puzzle[item.col, item.row], hidden[item.col, item.row]);
                }
            }
        }

        public string ShufflePuzzle()
        {
            string str = "123456789";
            Random rand = new Random(DateTime.Now.Millisecond);

            int val;
            str = new string(str.ToCharArray().OrderBy(s => (rand.Next(2) % 2) == 0).ToArray());

            for (int i = 0; i < max_col_count; i++)
            {
                for (int j = 0; j < max_row_count; j++)
                {
                    val = int.Parse(str.Substring(solved[i, j] - 1, 1));
                    puzzle[i, j] = val;

                    switch (rand.Next(2))
                    {
                        case 1:
                            hidden[i, j] = true;
                            break;
                        default:
                            hidden[i, j] = false;
                            break;
                    }
                }
            }

            return str;
        }

        private int GetReginStartPoint(int num)
        {
            if (num <= 2)
            {
                return 0;
            }
            else if (num <= 5)
            {
                return 3;
            }
            else
            {
                return 6;
            }
        }

        private List<int> GetPossibleValues(int c, int r)
        {
            List<int> possibleValues = new List<int>();
            possibleValues.Add(1);
            possibleValues.Add(2);
            possibleValues.Add(3);
            possibleValues.Add(4);
            possibleValues.Add(5);
            possibleValues.Add(6);
            possibleValues.Add(7);
            possibleValues.Add(8);
            possibleValues.Add(9);

            for (int i = 0; i < 9; i++)
            {
                possibleValues.Remove(puzzle[i, r]);
                possibleValues.Remove(puzzle[c, i]);
            }

            // Bulunduğu bölgeye göre uygunlugunu kontrol edelim..
            for (int i = GetReginStartPoint(c); i < GetReginStartPoint(c) + 3; i++)
            {
                for (int j = GetReginStartPoint(r); j < GetReginStartPoint(r) + 3; j++)
                {
                    possibleValues.Remove(puzzle[i, j]);
                }
            }

            return possibleValues;
        }
    }
}
