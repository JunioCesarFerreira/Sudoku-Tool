using System.Collections.Generic;

namespace SudokuBacktracking
{
    internal static class Algorithms
    {
        /// <summary>
        /// Verifica todos blocos em busca de números repetidos.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public static bool CheckBlocks(BoardCoords board, out List<string> msgList)
        {
            msgList = new List<string>();
            for (int q = 1; q < 10; q++)
            {
                List<int?> listValues = new List<int?>();
                List<int> repetitions = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j, q] != null)
                        {
                            bool isDistinct = true;
                            foreach (int? point in listValues)
                            {
                                if (point == board[i, j, q])
                                {
                                    isDistinct = false;
                                    break;
                                }
                            }
                            if (isDistinct)
                            {
                                listValues.Add(board[i, j, q]);
                            }
                            else
                            {
                                repetitions.Add((int)board[i, j, q]);
                            }
                        }
                    }
                }
                if (repetitions.Count > 0)
                {
                    msgList.Add("Valores \'"
                        + string.Join(", ", repetitions)
                        + "\' repetidos no bloco "
                        + q.ToString()
                        + "."
                        );
                }
            }
            return msgList.Count == 0;
        }
        /// <summary>
        /// Verifica todas linhas em busca de números repetidos.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public static bool CheckRows(BoardCoords board, out List<string> msgList)
        {
            msgList = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                List<int?> listValues = new List<int?>();
                List<int> repetitions = new List<int>();
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] != null)
                    {
                        bool isDistinct = true;
                        foreach (int? point in listValues)
                        {
                            if (point == board[i, j])
                            {
                                isDistinct = false;
                                break;
                            }
                        }
                        if (isDistinct)
                        {
                            listValues.Add(board[i, j]);
                        }
                        else
                        {
                            repetitions.Add((int)board[i, j]);
                        }
                    }
                }
                if (repetitions.Count > 0)
                {
                    msgList.Add("Valores \'"
                        + string.Join(", ", repetitions)
                        + "\' repetidos na linha "
                        + i.ToString()
                        + "."
                        );
                }
            }
            return msgList.Count == 0;
        }
        /// <summary>
        /// Verifica todas colunas em busca de números repetidos.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public static bool CheckCols(BoardCoords board, out List<string> msgList)
        {
            msgList = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                List<int?> listValues = new List<int?>();
                List<int> repetitions = new List<int>();
                for (int j = 0; j < 9; j++)
                {
                    if (board[j, i] != null)
                    {
                        bool isDistinct = true;
                        foreach (int? point in listValues)
                        {
                            if (point == board[j, i])
                            {
                                isDistinct = false;
                                break;
                            }
                        }
                        if (isDistinct)
                        {
                            listValues.Add(board[j, i]);
                        }
                        else
                        {
                            repetitions.Add((int)board[j, i]);
                        }
                    }
                }
                if (repetitions.Count > 0)
                {
                    msgList.Add("Valores \'"
                        + string.Join(", ", repetitions)
                        + "\' repetidos na coluna "
                        + i.ToString()
                        + "."
                        );
                }
            }
            if (msgList.Count == 0) return true;
            else return false;
        }
        /// <summary>
        /// Verifica se todas regras são satisfeitas.
        /// </summary>
        /// <param name="bCoords"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool CheckRules(BoardCoords bCoords, int row, int col, int num)
        {
            int[,] bMatrix = bCoords.GetBoardMatrix();
            // Verifica linha
            for (int down = 0; down < 9; down++)
            {
                if (bMatrix[row, down] == num)
                {
                    return false;
                }
            }
            // Verifica coluna
            for (int right = 0; right < 9; right++)
            {
                if (bMatrix[right, col] == num)
                {
                    return false;
                }
            }
            // Verifica bloco
            int blockRowStart = row - row % 3;
            int blockColStart = col - col % 3;
            int blockRowStop = blockRowStart + 3;
            int blockColStop = blockColStart + 3;
            for (int right = blockRowStart; right < blockRowStop; right++)
            {
                for (int down = blockColStart; down < blockColStop; down++)
                {
                    if (bMatrix[right, down] == num)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Algoritmo de resolução do Sudoku utilizando backtracking 
        /// </summary>
        /// <param name="boardCoords"></param>
        /// <returns></returns>
        public static bool BacktrackingSolveSudoku(BoardCoords boardCoords)
        {
            int row = -1;
            int col = -1;
            bool completed = true;
            // Verifica se está completo, senão move escopo para (linha,coluna) vazia.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (boardCoords[i, j] is null)
                    {
                        row = i;
                        col = j;
                        completed = false;
                        break;
                    }
                }
                if (!completed)
                {
                    break;
                }
            }
            // Se completo, então acabou!
            if (completed)
            {
                return true;
            }
            // Prenche células, verifica regras e aplica recursividade. 
            for (int n = 1; n <= 9; n++)
            {
                if (CheckRules(boardCoords, row, col, n))
                {
                    boardCoords[row, col] = n;
                    if (BacktrackingSolveSudoku(boardCoords))
                    {
                        return true;
                    }
                    else
                    {
                        boardCoords[row, col] = null;
                    }
                }
            }
            return false;
        }
    }
}
