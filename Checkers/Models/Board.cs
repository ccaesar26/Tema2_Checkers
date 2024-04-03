using System;
using System.Collections;
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
            Initialize();
        }

        private void Initialize()
        {
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    if (row < 3 && (row + column) % 2 == 1)
                    {
                        Pieces[row, column] = new Man(EPieceColor.White);
                    }
                    else if (row > Size - 4 && (row + column) % 2 == 1)
                    {
                        Pieces[row, column] = new Man(EPieceColor.Black);
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

            var upgradeNeeded = false;
            if (IsSimpleMove(currentPosition, newPosition))
            {
                upgradeNeeded = piece.IsUpgradeable(newPosition);
            }
            else
            {
                var captures = FindJumpCaptures(currentPosition, newPosition);
                foreach (var capture in captures)
                {
                    Pieces[capture.Row, capture.Column] = null;
                    if (piece.IsUpgradeable(new Position(capture.Row + piece.GetWays().First(), capture.Column)))
                    {
                        upgradeNeeded = true;
                    }
                }
            }
            
            Pieces[newPosition.Row, newPosition.Column] = piece;
            Pieces[currentPosition.Row, currentPosition.Column] = null;
            
            if (upgradeNeeded)
            {
                UpdatePiece(newPosition);
            }
        }

        public IEnumerable<Position> GetPossibleMoves(Position position)
        {
            var piece = Pieces[position.Row, position.Column];
            var moves = new List<Position>();
            var ways = piece.GetWays();

            foreach (var way in ways)
            {
                // Right side
                if (position.Column + 1 < Size && position.Row + way >= 0 && position.Row + way < Size)
                {
                    var rightPosition = new Position(position.Row + way, position.Column + 1);
                    if (Pieces[rightPosition.Row, rightPosition.Column] == null)
                    {
                        moves.Add(rightPosition);
                    }
                    else if (IsPositionOccupied(rightPosition, piece.Color.Opposite()))
                    {
                        moves.AddRange(GetJumps(position, piece));
                    }
                }

                // Left side
                if (position.Column - 1 >= 0 && position.Row + way >= 0 && position.Row + way < Size)
                {
                    var leftPosition = new Position(position.Row + way, position.Column - 1);
                    if (Pieces[leftPosition.Row, leftPosition.Column] == null)
                    {
                        moves.Add(leftPosition);
                    }
                    else if (IsPositionOccupied(leftPosition, piece.Color.Opposite()))
                    {
                        moves.AddRange(GetJumps(position, piece));
                    }
                }
            }

            return moves;
        }

        private IEnumerable<Position> GetJumps(Position position, Piece piece, ISet<Position> visited = null)
        {
            var jumps = new List<Position>();
            
            visited ??= new HashSet<Position>();

            if (piece.IsUpgradeable(position))
            {
                piece = new King(piece.Color);
            }

            var ways = piece.GetWays();

            foreach (var way in ways)
            {
                // Right side
                if (position.Column + 2 < Size && position.Row + way * 2 >= 0 && position.Row + way * 2 < Size)
                {
                    var opponentPosition = new Position(position.Row + way, position.Column + 1);
                    if (Pieces[opponentPosition.Row, opponentPosition.Column] != null &&
                        IsPositionOccupied(opponentPosition, piece.Color.Opposite()))
                    {
                        var rightJumpPosition = new Position(position.Row + way * 2, position.Column + 2);
                        if (Pieces[rightJumpPosition.Row, rightJumpPosition.Column] == null && !visited.Contains(rightJumpPosition))
                        {
                            jumps.Add(rightJumpPosition);
                            visited.Add(rightJumpPosition);
                            jumps.AddRange(GetJumps(rightJumpPosition, piece, visited));
                        }
                    }
                }

                // Left side
                if (position.Column - 2 >= 0 && position.Row + way * 2 >= 0 && position.Row + way * 2 < Size)
                {
                    var opponentPosition = new Position(position.Row + way, position.Column - 1);
                    if (Pieces[opponentPosition.Row, opponentPosition.Column] != null &&
                        IsPositionOccupied(opponentPosition, piece.Color.Opposite()))
                    {
                        var leftJumpPosition = new Position(position.Row + way * 2, position.Column - 2);
                        if (Pieces[leftJumpPosition.Row, leftJumpPosition.Column] == null && !visited.Contains(leftJumpPosition))
                        {
                            jumps.Add(leftJumpPosition);
                            visited.Add(leftJumpPosition);
                            jumps.AddRange(GetJumps(leftJumpPosition, piece, visited));
                        }
                    }
                }
            }

            return jumps;
        }

        private IEnumerable<Position> FindJumpCaptures(Position from, Position to)
        {
            var captures = new HashSet<Position>();
            var jumps = GetJumps(from, Pieces[from.Row, from.Column]);
            var path = new List<Position> {from};
            
            var found = true;
            while (found)
            {
                found = false;
                var jumpsCopy = jumps.ToList();
            
                // Iterate over the possible jumps
                foreach (var next in from next in jumpsCopy where Math.Abs(next.Row - path.Last().Row) == 2 && Math.Abs(next.Column - path.Last().Column) == 2 let captured = new Position((next.Row + path.Last().Row) / 2, (next.Column + path.Last().Column) / 2) where captures.Add(captured) select next)
                {
                    path.Add(next);
                    jumpsCopy.Remove(next);
                    found = true;
                    break;
                }
            }
            
            return captures;
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

        private static bool IsSimpleMove(Position currentPosition, Position newPosition)
        {
            return System.Math.Abs(currentPosition.Row - newPosition.Row) == 1 &&
                   System.Math.Abs(currentPosition.Column - newPosition.Column) == 1;
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