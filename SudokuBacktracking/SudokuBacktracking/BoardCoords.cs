using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuBacktracking
{
    internal class BoardCoords
    {
        // Matrix textBoxes
        private readonly TextBox[,] textBoxes = new TextBox[9,9];

        /// <summary>
        /// Acessores por coordenadas: (linha, coluna).
        /// </summary>
        /// <param name="row">linhas de 0 a 8</param>
        /// <param name="col">colunas de 0 a 8</param>
        /// <returns></returns>
        public int? this[int row, int col]
        {
            get => GetCoordinate(row, col);
            set => SetCoordinate(row, col, value);
        }
        /// <summary>
        /// Acessor por coordenada: (linha, coluna, bloco)
        /// </summary>
        /// <param name="row">linhas de 0 a 2</param>
        /// <param name="col">colunas de 0 a 2</param>
        /// <param name="block">Blocos de 1 a 9</param>
        /// <returns></returns>
        public int? this[int row, int col, int block] 
        {
            get => GetCoordByBlock(row, col, block);
        }

        /// <summary>
        /// Construtor: Realiza link entre interface e dados internos de processamento
        /// </summary>
        /// <param name="sudokuBoard"></param>
        public BoardCoords(TableLayoutPanel sudokuBoard) 
        {
            // Para todos sub-TableLayoutPanel em sudokuBoard
            foreach (Control control in sudokuBoard.Controls)
            {
                if (control is TableLayoutPanel block)
                {
                    // Recupera número do bloco no nome do componente
                    int blockNumber = int.Parse(block.Name.Split('_')[1]);
                    foreach (Control textBox in block.Controls)
                    {
                        TextBox tmp = (TextBox)textBox;
                        // Utiliza propriedade TabIndex para indicar posição dentro do bloco
                        ParseCoord(tmp.TabIndex, out int i, out int j);
                        i += AddToRow(blockNumber);
                        j += AddToCol(blockNumber);
                        textBoxes[i, j] = tmp;
                        textBoxes[i, j].Text = "";
                    }
                }
            }
        }

        #region Public methods
        public void SetColor(int row, int col, Color color)
        {
            textBoxes[row, col].ForeColor = color;
        }

        public void Focus(TextBox textBox, Keys keys)
        {
            FindByName(textBox, out int row, out int col);
            switch (keys)
            {
                case Keys.Up:
                    if (row > 0) row--;
                    else row = 8;
                    break;
                case Keys.Down:
                    if (row < 8) row++;
                    else row = 0;
                    break;
                case Keys.Right:
                    if (col < 8) col++;
                    else col = 0;
                    break;
                case Keys.Left:
                    if (col > 0) col--;
                    else col = 8;
                    break;
                default:
                    return;
            }
            textBoxes[row, col].Focus();
        }

        public int[,] GetBoardMatrix()
        {
            int[,] board = new int[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (this[row, col] != null)
                    {
                        board[row, col] = (int)this[row, col];
                    }
                    else
                    {
                        board[row, col] = 0;
                    }
                }
            }
            return board;
        }

        public void SetBoardMatrix(int[,] matrix)
        {
            for (int i=0; i<matrix.GetLength(0); i++)
            {
                for (int j=0; j<matrix.GetLength(1); j++) 
                {
                    if (matrix[i,j] == 0)
                    {
                        this[i, j] = null;
                    }
                    else
                    {
                        this[i, j] = matrix[i, j];
                    }
                }
            }
        }
        #endregion

        #region Private methods
        private void SetCoordinate(int row, int col, int? value)
        {
            if (textBoxes[row, col].ForeColor != Color.Cyan)
            {
                if (value is null)
                {
                    textBoxes[row, col].Text = "";
                }
                else
                {
                    textBoxes[row, col].Text = value.ToString();
                }
            }
        }

        private int? GetCoordinate(int row, int col)
        {
            if (int.TryParse(textBoxes[row, col].Text, out int value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        private int? GetCoordByBlock(int row, int col, int block)
        {
            if (row < 0 || row > 2 || col < 0 || col > 2 || block < 1 || block > 9)
            {
                throw new Exception("Invalid value.");
            }
            row += AddToRow(block);
            col += AddToCol(block);
            return GetCoordinate(row, col);
        }

        private void FindByName(TextBox textBox, out int row, out int col)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (textBoxes[i,j].Name == textBox.Name)
                    {
                        row = i;
                        col = j;
                        return;
                    }
                }
            }
            throw new Exception("TextBox not found.");
        }

        private void ParseCoord(int value, out int row, out int col)
        {
            switch (value)
            {
                case 1:
                    row = 0;
                    col = 0;
                    break;
                case 2:
                    row = 0;
                    col = 1;
                    break;
                case 3:
                    row = 0;
                    col = 2;
                    break;
                case 4:
                    row = 1;
                    col = 0;
                    break;
                case 5:
                    row = 1;
                    col = 1;
                    break;
                case 6:
                    row = 1;
                    col = 2;
                    break;
                case 7:
                    row = 2;
                    col = 0;
                    break;
                case 8:
                    row = 2;
                    col = 1;
                    break;
                case 9:
                    row = 2;
                    col = 2;
                    break;
                default:
                    throw new Exception("Invalid value. only numbers between 1 and 9.");
            }
        }

        private int AddToRow(int block)
        {
            if (block <= 3)
            {
                return 0;
            }
            else if (block <= 6)
            {
                return 3;
            }
            else
            {
                return 6;
            }
        }

        private int AddToCol(int block)
        {
            if (block % 3 == 1)
            {
                return 0;
            }
            else if (block % 3 == 2)
            {
                return 3;
            }
            else
            {
                return 6;
            }
        }
        #endregion
    }
}