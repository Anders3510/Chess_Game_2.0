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
		//Flag for player-switching
		// 1 : player 1 (white)
		//-1 : player 2 (black)
		int f_player = 1;
		

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
				board[1, i] = new Pawn(PieceColor.white, board);
			}

			for (int i = 0; i < 8; i += 1)
			{
				board[6, i] = new Pawn(PieceColor.black, board);
			}
		}

		/// <summary>
		/// Prints the chessboard and a guide to the console.
		/// </summary>
		public void PrintBoard()
		{
			Console.WriteLine(" _______________________________________");
			//Iterate Rows
			for (int i = 0; i < board.GetLength(0); i += 1)
			{
				Console.Write("|");
				//Iterate Columns
				for (int j = 0; j < board.GetLength(1); j += 1)
				{
					if (board[i, j] == null)
						Console.Write("    |");
					else
						if (board[i, j].GetColor() == PieceColor.white)
						Console.Write($" W{board[i, j].GetID()} |");
					else
						Console.Write($" B{board[i, j].GetID()} |");
				}
				Console.Write($"          {((i + 1) * -1) + 9}");
				Console.Write("\n|_______________________________________|\n");
			}
			Console.WriteLine($"                                                    A    B    C    D    E    F    G    H");
		}

		/// <summary>
		/// Helper function that parses the letters
		/// A-H into a corresponding index position
		/// </summary>
		/// <param name="input">String in which the first character is parsed</param>
		/// <returns>An index number, or -1 if the character could not be parsed</returns>
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

		/// <summary>
		/// Helper function that iterates from a Piece's starting position
		/// towards the target position, and determines if the move is legal.
		/// </summary>
		/// <param name="coords">An array of array coordinates.</param>
		/// <returns>A boolean value indicating if the move is legal.</returns>
		private bool CheckMovement(int[] coords)
		{
			Knight k = new Knight(PieceColor.white);
			if (ReferenceEquals(board[coords[0], coords[2]].GetType(), k.GetType()))
				return true;

			int yChange = Math.Sign(coords[1] - coords[0]);
			int xChange = Math.Sign(coords[3] - coords[2]);
			//i = y axis
			//j = x axis

			int i = coords[0] + yChange;
			int j = coords[2] + xChange;

			while (i != coords[1] || j != coords[3])
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

		/// <summary>
		/// Receives start and end positions, and tells the main program
		/// if a Piece's movement is legal. Switches player if the move
		/// is successful.
		/// </summary>
		/// <param name="pieceToMove">A string containing the starting position of the piece</param>
		/// <param name="newLocation">A string containing the target position of a piece</param>
		/// <returns>A boolean value indicating if the move is legal</returns>
		public bool PieceMovement(string pieceToMove, string newLocation)
		{
			//Get start and end coordinates
			int y = int.Parse(pieceToMove[1].ToString()) * -1 + 8;
			int x = ParseLetter(pieceToMove);

			int newY = int.Parse(newLocation[1].ToString()) * -1 + 8;
			int newX = ParseLetter(newLocation);

			//If the starting position is null, or the start and end positions match
			if (board[y, x] == null || (x == newX && y == newY))
				return false; //If the piece the player is attempting to move does not match the current player
			else if (board[y, x].GetColor() == PieceColor.white && f_player == -1 || (board[y, x].GetColor() == PieceColor.black && f_player == 1))
				return false;
			
			//Create coordinate array
			int[] coords = new int[] { y, newY, x, newX };

			//Check if move is legal
			if (board[y, x].CalculateAngle(coords) && CheckMovement(coords))
			{
				//Move piece
				board[newY, newX] = board[y, x];
				board[y, x] = null;
				//Switch player
				f_player *= -1;
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

			/// <summary>
			/// Calculates movement angles and other unique characteristics
			/// of Piece's derived types.
			/// </summary>
			/// <param name="coords">An array of piece coordinates</param>
			/// <returns>A boolean value indicating whether the attempted move is legal.</returns>
			public abstract bool CalculateAngle(int[] coords);

			/// <summary>
			///	Returns a derived type of Piece's identifying character.
			/// </summary>
			/// <returns>A character used for identifying the type of Piece the object is on the board</returns>
			public abstract char GetID();
		}

		class Rook : Piece
		{
			public Rook(PieceColor color) : base(color) { }

			public override bool CalculateAngle(int[] coords)
			{
				if (coords[0] - coords[1] == 0)
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

				if (yOffset == 1 || yOffset == 0)
					yApproved = true;
				else
					yApproved = false;

				if (xOffset == 1 || xOffset == 0)
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
			public Pawn(PieceColor color, Piece[,] board) : base(color) { this.board = board; }

			Piece[,] board; 
			bool hasMoved = false;

			public override bool CalculateAngle(int[] coords)
			{
				if (hasMoved) //If the piece has moved at least once during the game
				{
					int offset = 0;
					if (GetColor() == PieceColor.white) //If piece is white
						offset = 1;
					else
						offset = -1;

					if (coords[1] != coords[0] + offset) //If piece is not moving in the correct direction
						return false;

					int xOffset = Math.Abs(coords[2] - coords[3]);
					if (xOffset == 1 || xOffset == -1) //If offset on the x-axis is 1 or -1
					{
						PieceColor tempColor;
						if (board[coords[1], coords[3]] != null)
							tempColor = board[coords[1], coords[3]].GetColor();
						else
							tempColor = GetColor();
						//If target position is occupied and the color does not match
						if (board[coords[1], coords[3]] != null && GetColor() != tempColor)
							return true;
						else
							return false;
					}
					else if (xOffset == 0 && board[coords[1], coords[3]] == null)
						return true;
					else
						return false;
				}
				else
				{
					
					hasMoved = true;
				}
				return true;
			}

			public override char GetID()
			{
				return 'P';
			}
		}
	}
}
