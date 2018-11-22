using System;
using System.Text.RegularExpressions;

namespace ChessGame2
{
	class Program
	{
		static void Main(string[] args)
		{
			var rx = new Regex(@"[A-H]{1}[1-8]{1}");
			ChessGame game = new ChessGame();

			bool isRunning = true;
			string input;

			string pieceToMove;
			string newLocation;

			while (isRunning)
			{
				//Print gameboard
				game.PrintBoard();

				//Prompt user and check input
				Console.WriteLine("What piece would you like to move?");
				input = Console.ReadLine();
				if (!rx.IsMatch(input) || input.Length > 2)
				{
					Console.WriteLine("Wrong input format.");
					continue;
				}
				pieceToMove = input;

				//Prompt user and check input
				Console.WriteLine("Where would you like to move it?");
				input = Console.ReadLine();
				while (!rx.IsMatch(input) || input.Length > 2)
				{
					Console.WriteLine("Wrong input format.");
					input = Console.ReadLine();
				}
				newLocation = input;

				if (game.PieceMovement(pieceToMove, newLocation))
					Console.WriteLine("Piece moved successfully!");
				else
					Console.WriteLine("Piece could not be moved.");
			}

		}
	}

	class ChessGame
	{
		Piece[,] board = new Piece[8, 8];

		//Coords array: { y1, y2, x1, x2 }
		/*
		 * {
		 * {0,0 0,1 0,2 0,3 0,4 0,5 0,6 0,7}
		 * {1,0 1,1 1,2 1,3 1,4 1,5 1,6 1,7}
		 * {2,0 2,1 2,2 2,3 2,4 2,5 2,6 2,7}
		 * {3,0 3,1 3,2 3,3 3,4 3,5 3,6 3,7}
		 * {4,0 4,1 4,2 4,3 4,4 4,5 4,6 4,7}
		 * {5,0 5,1 5,2 5,3 5,4 5,5 5,6 5,7}
		 * {6,0 6,1 6,2 6,3 6,4 6,5 6,6 6,7}
		 * {7,0 7,1 7,2 7,3 7,4 7,5 7,6 7,7}
		 * }
		*/
		enum PieceColor
		{
			white,
			black
		}

		public ChessGame()
		{
			board[0, 0] = new Rook(PieceColor.white);
			board[0, 1] = new Knight(PieceColor.white);
			board[0, 2] = new Bishop(PieceColor.white);
			board[0, 3] = new King(PieceColor.white);
			board[0, 4] = new Queen(PieceColor.white);
			board[0, 5] = new Bishop(PieceColor.white);
			board[0, 6] = new Knight(PieceColor.white);
			board[0, 7] = new Rook(PieceColor.white);

			board[7, 0] = new Rook(PieceColor.black);
			board[7, 1] = new Knight(PieceColor.black);
			board[7, 2] = new Bishop(PieceColor.black);
			board[7, 3] = new King(PieceColor.black);
			board[7, 4] = new Queen(PieceColor.black);
			board[7, 5] = new Bishop(PieceColor.black);
			board[7, 6] = new Knight(PieceColor.black);
			board[7, 7] = new Rook(PieceColor.black);

			for (int i = 0; i < 8; i += 1)
			{
				board[1, i] = new Pawn(PieceColor.white);
			}

			for (int i = 0; i < 8; i += 1)
			{
				board[6, i] = new Pawn(PieceColor.black);
			}
		}

		public void PrintBoard()
		{
			Console.WriteLine(" _______________________________");
			//Iterate Rows
			for (int i = 0; i < board.GetLength(0); i += 1)
			{
				Console.Write("|");
				//Iterate Columns
				for (int j = 0; j < board.GetLength(1); j += 1)
				{
					if (board[i, j] == null)
						Console.Write("   |");
					else
						Console.Write($" {board[i, j].GetID()} |");
				}
				Console.Write("\n|_______________________________|\n");
			}
		}

		private int ParseLetter(string input)
		{
			int x;
			switch (input[0])
			{
				case 'A': x = 0; break;

				case 'B': x = 1; break;

				case 'C': x = 2; break;

				case 'D': x = 3; break;

				case 'E': x = 4; break;

				case 'F': x = 5; break;

				case 'G': x = 6; break;

				case 'H': x = 7; break;

				default: x = -1; break;
			}
			return x;
		}

		private bool CheckMovement(int[] coords)
		{
			int yChange = Math.Sign(coords[1] - coords[0]);
			int xChange = Math.Sign(coords[3] - coords[2]);
			//i = y axis
			//j = x axis

			int i = coords[0] + yChange;
			int j = coords[2] + xChange;

			while(i != coords[1] || j != coords[3])
			{
				if (board[i, j] != null) //If there is a Piece present, then, if the color is not equal, and the target position matches the current
					if (board[coords[0], coords[2]].GetColor() != board[i, j].GetColor() && (i == coords[1] && j == coords[3]))
						return true;
					else
						return false;

				if (i != coords[1])
					i += yChange;

				if (j != coords[3])
					j += xChange;
			}

			return true;
		}

		public bool PieceMovement(string pieceToMove, string newLocation)
		{
			int y = int.Parse(pieceToMove[1].ToString()) - 1;
			int x = ParseLetter(pieceToMove);

			int newY = int.Parse(newLocation[1].ToString()) - 1;
			int newX = ParseLetter(newLocation);
			
			if (board[y, x] == null || (x == newX && y == newY))
				return false;

			int[] coords = new int[] { y, newY, x, newX };

			if (board[y, x].CalculateAngle(coords) && CheckMovement(coords))
			{
				board[newY, newX] = board[y, x];
				board[y, x] = null;
				return true;
			}
			else
				return false;
		}

		abstract class Piece
		{
			PieceColor color { get; }

			public Piece(PieceColor color)
			{
				this.color = color;
			}

			public PieceColor GetColor()
			{
				return color;
			}

			public abstract bool CalculateAngle(int[] coords);
			public abstract char GetID();
		}

		class Rook : Piece
		{
			public Rook(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				if(coords[0] - coords[1] == 0)
					return true;

				if (Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]) == 0)
					return true;
				else
					return false;
			}

			public override char GetID()
			{
				return 'R';
			}
		}

		class Bishop : Piece
		{
			public Bishop(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				if (coords[0] - coords[1] == 0)
					return false;

				if (Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]) == 1)
					return true;
				else
					return false;
			}

			public override char GetID()
			{
				return 'B';
			}
		}

		class Queen : Piece
		{
			public Queen(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				if (coords[0] - coords[1] == 0)
					return true;

				int angle = Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]);
				if (angle == 1 || angle == 0)
					return true;
				else
					return false;
			}

			public override char GetID()
			{
				return 'Q';
			}
		}

		class King : Piece
		{
			public King(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				bool xApproved;
				bool yApproved;
				int yOffset = Math.Abs(coords[0] - coords[1]);
				int xOffset = Math.Abs(coords[2] - coords[3]);

				if(yOffset == 1 || yOffset == 0)
					yApproved = true;
				else
					yApproved = false;

				if (xOffset == 1 || xOffset == 0)
					xApproved = true;
				else
					xApproved = false;

				if(yApproved && xApproved)
					return true;
				else
					return false;
			}

			public override char GetID()
			{
				return 'K';
			}
		}

		class Knight : Piece
		{
			public Knight(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				//Variable declarations
				bool xApproved;
				bool yApproved;
				int yOffset = Math.Abs(coords[0] - coords[1]);
				int xOffset = Math.Abs(coords[2] - coords[3]);

				if (yOffset == 1 || yOffset == 2)
					yApproved = true;
				else
					yApproved = false;

				if ((xOffset == 1 || xOffset == 2) && xOffset != yOffset)
					xApproved = true;
				else
					xApproved = false;

				if (yApproved && xApproved)
					return true;
				else
					return false;
			}

			public override char GetID()
			{
				return 'N';
			}
		}

		class Pawn : Piece
		{
			public Pawn(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				return true;
			}

			public override char GetID()
			{
				return 'P';
			}
		}
	}
}
