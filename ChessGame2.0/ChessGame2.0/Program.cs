using System;
using System.Collections.Generic;

namespace ChessGame2
{
	class Program
	{
		static void Main(string[] args)
		{

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

		abstract class Piece
		{
			PieceColor color { get; }

			public abstract bool CalculateAngle(int[] coords);

			//Work in progress
			public bool MovePiece(int[] coords)
			{
				return true;
			}
		}

		class Rook : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				if (Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]) == 0)
					return true;
				else
					return false;
			}
		}

		class Bishop : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				if (Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]) == 1)
					return true;
				else
					return false;
			}
		}

		class Queen : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				int angle = Math.Abs(coords[2] - coords[3]) / Math.Abs(coords[0] - coords[1]);
				if(angle == 1 || angle == 0)
					return true;
				else
					return false;
			}
		}

		class King : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				if(Math.Abs(coords[0] - coords[1]) == 1 && Math.Abs(coords[2] - coords[3]) == 1)
					return true;
				else
					return false;
			}
		}

		class Knight : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				return true;
			}
		}

		class Pawn : Piece
		{
			public override bool CalculateAngle(int[] coords)
			{
				return true;
			}
		}
	}
}
