using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    public class Board
    {
        public int Size { get; }
        public Piece[,] Pieces { get; }

        public Board(int size = 8)
        {
            Size = size;
            Pieces = new Piece[size, size];
        }

        public void Initialize()
        {
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    if (row < 3 && (row + column) % 2 == 1)
                    {
                        Pieces[row, column] = new Man(EPieceColor.Black);
                    }
                    else if (row > Size - 4 && (row + column) % 2 == 1)
                    {
                        Pieces[row, column] = new Man(EPieceColor.White);
                    }
                }
            }
        }

        public void MovePiece(Position currentPosition, Position newPosition)
        {
            if (!IsPositionValid(currentPosition) || !IsPositionValid(newPosition))
            {
                throw new System.InvalidOperationException("Invalid position.");
            }

            var piece = Pieces[currentPosition.Row, currentPosition.Column];
            var moves = GetPossibleMoves(currentPosition).ToList();

            if (!moves.Contains(newPosition))
            {
                throw new System.InvalidOperationException("Invalid move.");
            }

            if (IsMoveJump(currentPosition, newPosition))
            {
                MakeJump(currentPosition, newPosition);
            }
            else
            {
                Pieces[newPosition.Row, newPosition.Column] = piece;
                Pieces[currentPosition.Row, currentPosition.Column] = null;
            }
        }

        public IEnumerable<Position> GetPossibleMoves(Position position)
        {
            var piece = Pieces[position.Row, position.Column];
            var moves = new List<Position>();
            var ways = piece.GetWays(position);

            foreach (var way in ways)
            {
                // Right side
                if (position.Column + 1 < Size && position.Row + way < Size)
                {
                    var rightPosition = new Position(position.Row + way, position.Column + 1);
                    if (Pieces[rightPosition.Row, rightPosition.Column] == null)
                    {
                        moves.Add(rightPosition);
                    }
                    else if (IsPositionOccupied(rightPosition, piece.Color.Opposite()))
                    {
                        moves.AddRange(GetJumps(rightPosition));
                    }
                }

                // Left side
                if (position.Column - 1 >= 0 && position.Row + way < Size)
                {
                    var leftPosition = new Position(position.Row + way, position.Column - 1);
                    if (Pieces[leftPosition.Row, leftPosition.Column] == null)
                    {
                        moves.Add(leftPosition);
                    }
                    else if (IsPositionOccupied(leftPosition, piece.Color.Opposite()))
                    {
                        moves.AddRange(GetJumps(leftPosition));
                    }
                }
            }

            return moves;
        }

        private IEnumerable<Position> GetJumps(Position position)
        {
            var piece = Pieces[position.Row, position.Column];
            var jumps = new List<Position>();

            if (piece.Color.IsUpgradable(position))
            {
                piece = new King(piece.Color);
            }

            var ways = piece.GetWays(position);

            foreach (var way in ways)
            {
                // Right side
                if (position.Column + 2 < Size && position.Row + way < Size)
                {
                    var rightPosition = new Position(position.Row + way, position.Column + 1);
                    if (Pieces[rightPosition.Row, rightPosition.Column] != null &&
                        IsPositionOccupied(rightPosition, piece.Color.Opposite()))
                    {
                        var rightJumpPosition = new Position(position.Row + way * 2, position.Column + 2);
                        if (Pieces[rightJumpPosition.Row, rightJumpPosition.Column] == null)
                        {
                            jumps.Add(rightJumpPosition);
                            jumps.AddRange(GetJumps(rightJumpPosition));
                        }
                    }
                }

                // Left side
                if (position.Column - 2 >= 0)
                {
                    var leftPosition = new Position(position.Row + way, position.Column - 1);
                    if (Pieces[leftPosition.Row, leftPosition.Column] == null &&
                        IsPositionOccupied(leftPosition, piece.Color.Opposite()))
                    {
                        var leftJumpPosition = new Position(position.Row + way * 2, position.Column - 2);
                        if (Pieces[leftJumpPosition.Row, leftJumpPosition.Column] == null)
                        {
                            jumps.Add(leftJumpPosition);
                            jumps.AddRange(GetJumps(leftJumpPosition));
                        }
                    }
                }
            }

            return jumps;
        }

        private void MakeJump(Position currentPosition, Position newPosition)
        {
            var piece = Pieces[currentPosition.Row, currentPosition.Column];
            var jumps = GetJumps(currentPosition).ToList();

            if (!jumps.Contains(newPosition))
            {
                throw new System.InvalidOperationException("Invalid jump.");
            }

            Pieces[newPosition.Row, newPosition.Column] = piece;
            Pieces[currentPosition.Row, currentPosition.Column] = null;
            Pieces[(currentPosition.Row + newPosition.Row) / 2, (currentPosition.Column + newPosition.Column) / 2] =
                null;

            if (piece.Color.IsUpgradable(newPosition))
            {
                UpdatePiece(newPosition);
            }
        }

        public bool CheckForWin()
        {
            var blackPieces = 0;
            var whitePieces = 0;

            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    if (Pieces[row, column] != null)
                    {
                        if (Pieces[row, column].Color == EPieceColor.Black)
                        {
                            blackPieces++;
                        }
                        else
                        {
                            whitePieces++;
                        }
                    }
                }
            }

            return blackPieces == 0 || whitePieces == 0;
        }

        public Piece GetPiece(Position from)
        {
            return Pieces[from.Row, from.Column];
        }

        private bool IsPositionValid(Position position)
        {
            return position.Row >= 0 && position.Row < Size && position.Column >= 0 && position.Column < Size;
        }

        private bool IsPositionEmpty(Position position)
        {
            return Pieces[position.Row, position.Column] == null;
        }

        private bool IsMoveJump(Position currentPosition, Position newPosition)
        {
            return System.Math.Abs(currentPosition.Row - newPosition.Row) == 2 &&
                   System.Math.Abs(currentPosition.Column - newPosition.Column) == 2;
        }

        public bool IsPositionOccupied(Position position, EPieceColor color)
        {
            return Pieces[position.Row, position.Column] != null &&
                   Pieces[position.Row, position.Column].Color == color;
        }

        private void UpdatePiece(Position position)
        {
            Pieces[position.Row, position.Column] = new King(Pieces[position.Row, position.Column].Color);
        }
    }
}