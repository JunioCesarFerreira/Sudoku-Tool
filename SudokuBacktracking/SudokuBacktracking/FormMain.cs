using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SudokuBacktracking
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Objeto de controle e manipulação dos dados no tabuleiro.
        /// </summary>
        private readonly BoardCoords boardCoords;

        /// <summary>
        /// Construtor
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            // Prepara tabuleiro
            foreach (Control control in sudokuBoard.Controls)
            {
                if (control is TableLayoutPanel block)
                {
                    foreach (Control textBox in block.Controls)
                    {
                        TextBox tmp = (TextBox)textBox;
                        tmp.TextChanged += TextBoxes_TextChanged;
                        tmp.KeyDown += TextBoxes_KeyDown;
                    }
                }
            }
            boardCoords = new BoardCoords(sudokuBoard);
        }

        /// <summary>
        /// Click do botão Solve: Valida dados de entrada e aplica algoritmo de resolução.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Run_Click(object sender, EventArgs e)
        {
            List<string> listFails = new List<string>();
            if (!Algorithms.CheckBlocks(boardCoords, out List<string> tmpList))
            {
                listFails.AddRange(tmpList);
            }

            if (!Algorithms.CheckRows(boardCoords, out tmpList))
            {
                listFails.AddRange(tmpList);
            }

            if (!Algorithms.CheckCols(boardCoords, out tmpList))
            {
                listFails.AddRange(tmpList);
            }

            if (listFails.Count == 0)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (boardCoords[row, col] != null)
                        {
                            boardCoords.SetColor(row, col, Color.Cyan);
                        }
                    }
                }
                if (!Algorithms.BacktrackingSolveSudoku(boardCoords))
                {
                    MessageBox.Show("O tabuleiro atual não tem solução.");
                }
            }
            else
            {
                MessageBox.Show("Verifique os valores preenchidos:\r\n" + string.Join("\r\n", listFails));
            }
        }

        /// <summary>
        /// Evento de texto modificado das células: Permite entradas apenas de 1 a 9.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text != "")
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    if (value < 1 || value > 9)
                    {
                        textBox.Text = textBox.Text.Substring(textBox.Text.Length-1, 1);
                        textBox.Select(0, 1);
                    }
                }
                else textBox.Text = "";
            }
        }

        /// <summary>
        /// Evento de tecla das células: Movimento utilizando as setas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            boardCoords.Focus((TextBox)sender, e.KeyCode);
        }

        /// <summary>
        /// Click do botão Clear: Limpa todas células.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clear_Click(object sender, EventArgs e)
        {
            for (int i=0; i<9; i++)
            {
                for (int j=0; j<9; j++)
                {
                    boardCoords.SetColor(i, j, Color.LightGreen);
                    boardCoords[i, j] = null;
                }
            }
        }

        /// <summary>
        /// Click do botão Open: Abre um arquivo com dados numéricos de um tabuleiro de Sudoku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Open matrix file",
                DefaultExt = "*.txt",
                Filter = "Text(*.txt)|*.txt|All files(*.*)|*.*",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sw = new StreamReader(openFileDialog.FileName);
                    int[,] matrix = new int[9, 9];
                    int row = 0;
                    while (!sw.EndOfStream)
                    {
                        string[] values = sw.ReadLine().Split(' ');
                        if (values.Length != 9)
                        {
                            MessageBox.Show("Dados do arquivo são inválidos. Quantidade de colunas inválida.");
                            sw.Close();
                            return;
                        }
                        for (int col=0; col<9; col++)
                        {
                            if (int.TryParse(values[col], out int tmp))
                            {
                                matrix[row,col] = tmp;
                            }
                            else
                            {
                                MessageBox.Show("Dados do arquivo são inválidos. Valor não numérico detectado.");
                                sw.Close();
                                return;
                            }
                        }
                        row++;
                        if (row == 10)
                        {
                            MessageBox.Show("Dados do arquivo são inválidos. Quantidade de linhas inválida.");
                            sw.Close();
                            return;
                        }
                    }
                    sw.Close();
                    boardCoords.SetBoardMatrix(matrix);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Click do botão Save: Salva um arquivo com dados numéricos do tabuleiro atual.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Registrar request",
                DefaultExt = "*.txt",
                Filter = "Text(*.txt)|*.txt|All files(*.*)|*.*",
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int[,] matrix = boardCoords.GetBoardMatrix();
                    string textContents = "";
                    for (int row=0; row<9; row++)
                    {
                        textContents += matrix[row, 0].ToString();
                        for (int col=1; col<9; col++)
                        {
                            textContents += " " + matrix[row, col].ToString(); 
                        }
                        if (row < 8)
                        {
                            textContents += "\r\n";
                        }
                    }
                    File.WriteAllText(saveFileDialog.FileName, textContents);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}