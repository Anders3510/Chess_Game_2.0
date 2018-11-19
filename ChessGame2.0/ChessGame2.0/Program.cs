using System;
using System.Collections.Generic;
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

			for(int i = 0; i < 8; i += 1)
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

		public bool PieceMovement(string pieceToMove, string newLocation)
		{
			return true;
		}

		abstract class Piece
		{
			PieceColor color { get; }

			public Piece(PieceColor color)
			{
				this.color = color;
			}

			public abstract bool CalculateAngle(int[] coords);
			public abstract char GetID();

			//Work in progress
			public bool MovePiece(int[] coords)
			{

				return true;
			}
		}

		class Rook : Piece
		{
			public Rook(PieceColor color) : base(color) {	}

			public override bool CalculateAngle(int[] coords)
			{
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
			public Bishop(PieceColor color) : base(color) {	}

			public override bool CalculateAngle(int[] coords)
			{
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
				if (Math.Abs(coords[0] - coords[1]) == 1 && Math.Abs(coords[2] - coords[3]) == 1)
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

				if((xOffset == 1 || xOffset == 2) && xOffset != yOffset)
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
