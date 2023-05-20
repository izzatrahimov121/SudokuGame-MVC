namespace Sudoku.MVC.HelperService;


/* C# program for Sudoku generator  */
public class CreatedSudoku
{
	#region old

	int[,] mat;
	int N; // number of columns/rows.
	int SRN; // square root of N
	int K; // No. Of missing digits

	// Constructor
	public CreatedSudoku(int N, int K)
	{
		this.N = N;
		this.K = K;

		// Compute square root of N
		double SRNd = Math.Sqrt(N);
		SRN = (int)SRNd;

		mat = new int[N, N];
	}
	static int[,] fullmat = new int[9, 9];
	// Sudoku Generator
	public void fillValues()
	{
		// Fill the diagonal of SRN x SRN matrices
		fillDiagonal();

		// Fill remaining blocks
		fillRemaining(0, SRN);

		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				fullmat[i, j] = mat[i, j];
			}
		}


		// Remove Randomly K digits to make game
		removeKDigits();
	}

	public static int[,] fullsudoku()
	{
		return fullmat;
	}

	// Fill the diagonal SRN number of SRN x SRN matrices
	void fillDiagonal()
	{

		for (int i = 0; i < N; i = i + SRN)

			// for diagonal box, start coordinates->i==j
			fillBox(i, i);
	}

	// Returns false if given 3 x 3 block contains num.
	bool unUsedInBox(int rowStart, int colStart, int num)
	{
		for (int i = 0; i < SRN; i++)
			for (int j = 0; j < SRN; j++)
				if (mat[rowStart + i, colStart + j] == num)
					return false;

		return true;
	}

	// Fill a 3 x 3 matrix.
	void fillBox(int row, int col)
	{
		int num;
		for (int i = 0; i < SRN; i++)
		{
			for (int j = 0; j < SRN; j++)
			{
				do
				{
					num = randomGenerator(N);
				}
				while (!unUsedInBox(row, col, num));

				mat[row + i, col + j] = num;
			}
		}
	}

	// Random generator
	int randomGenerator(int num)
	{
		Random rand = new Random();
		return (int)Math.Floor((double)(rand.NextDouble() * num + 1));
	}

	// Check if safe to put in cell
	bool CheckIfSafe(int i, int j, int num)
	{
		return (unUsedInRow(i, num) &&
				unUsedInCol(j, num) &&
				unUsedInBox(i - i % SRN, j - j % SRN, num));
	}

	// check in the row for existence
	bool unUsedInRow(int i, int num)
	{
		for (int j = 0; j < N; j++)
			if (mat[i, j] == num)
				return false;
		return true;
	}

	// check in the row for existence
	bool unUsedInCol(int j, int num)
	{
		for (int i = 0; i < N; i++)
			if (mat[i, j] == num)
				return false;
		return true;
	}

	// A recursive function to fill remaining
	// matrix
	bool fillRemaining(int i, int j)
	{
		//  System.out.println(i+" "+j);
		if (j >= N && i < N - 1)
		{
			i = i + 1;
			j = 0;
		}
		if (i >= N && j >= N)
			return true;

		if (i < SRN)
		{
			if (j < SRN)
				j = SRN;
		}
		else if (i < N - SRN)
		{
			if (j == (int)(i / SRN) * SRN)
				j = j + SRN;
		}
		else
		{
			if (j == N - SRN)
			{
				i = i + 1;
				j = 0;
				if (i >= N)
					return true;
			}
		}

		for (int num = 1; num <= N; num++)
		{
			if (CheckIfSafe(i, j, num))
			{
				mat[i, j] = num;
				if (fillRemaining(i, j + 1))
					return true;

				mat[i, j] = 0;
			}
		}
		return false;
	}

	// Remove the K no. of digits to
	// complete game
	public void removeKDigits()
	{
		int count = K;
		while (count != 0)
		{
			int cellId = randomGenerator(N * N) - 1;

			// System.out.println(cellId);
			// extract coordinates i  and j
			int i = (cellId / N);
			int j = cellId % 9;
			if (j != 0)
				j = j - 1;

			// System.out.println(i+" "+j);
			if (mat[i, j] != 0)
			{
				count--;
				mat[i, j] = 0;
			}
		}
	}

	//Print sudoku
	public int[,] printSudoku()
	{
		return mat;
	}



	

	// Driver code
	public static int[,] Create(string gameMode)
	{
		int k = 20;
		if (gameMode == "easy") { k = 15; }
		if (gameMode == "medium") { k = 25; }
		if (gameMode == "hard") { k = 40; }
		if (gameMode == "expert") { k = 50; }
		if (gameMode == "evil") { k = 60; }

		int N = 9, K = k;
		int[,] matrx = new int[N, N];


		CreatedSudoku sudoku = new CreatedSudoku(N, K);
		sudoku.fillValues();
		matrx = sudoku.printSudoku();
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				matrx[i, j] = sudoku.printSudoku()[i, j];
			}
		}
		return matrx;
		#endregion
	}
}


//static bool IsSolved(int[,] sudoku)
//{
//	// check rows
//	for (int i = 0; i < 9; i++)
//	{
//		HashSet<int> rowValues = new HashSet<int>();
//		for (int j = 0; j < 9; j++)
//		{
//			if (sudoku[i, j] == 0 || rowValues.Contains(sudoku[i, j]))
//			{
//				return false;
//			}
//			rowValues.Add(sudoku[i, j]);
//		}
//	}

//	// check columns
//	for (int j = 0; j < 9; j++)
//	{
//		HashSet<int> columnValues = new HashSet<int>();
//		for (int i = 0; i < 9; i++)
//		{
//			if (sudoku[i, j] == 0 || columnValues.Contains(sudoku[i, j]))
//			{
//				return false;
//			}
//			columnValues.Add(sudoku[i, j]);
//		}
//	}

//	// check 3x3 subgrids
//	for (int k = 0; k < 9; k++)
//	{
//		HashSet<int> subgridValues = new HashSet<int>();
//		int rowStart = (k / 3) * 3;
//		int columnStart = (k % 3) * 3;
//		for (int i = rowStart; i < rowStart + 3; i++)
//		{
//			for (int j = columnStart; j < columnStart + 3; j++)
//			{
//				if (sudoku[i, j] == 0 || subgridValues.Contains(sudoku[i, j]))
//				{
//					return false;
//				}
//				subgridValues.Add(sudoku[i, j]);
//			}
//		}
//	}

//	return true;
//}

