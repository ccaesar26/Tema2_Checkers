using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    public class Board
    {
        public int Size { get; }

        private readonly Piece[,] _pieces;
        public int BlackPieces => _pieces.Cast<Piece>().Count(piece => piece?.Color == EPieceColor.Black);
        public int WhitePieces => _pieces.Cast<Piece>().Count(piece => piece?.Color == EPieceColor.White);
        public bool MultiJump { get; set; } = true;

        public Board(int size = 8)
        {
            Size = size;
            _pieces = new Piece[size, size];
            Initialize();
        }
        public Board(IReadOnlyList<Piece> pieces)
        {
            Size = (int)Math.Sqrt(pieces.Count);
            _pieces = new Piece[Size, Size];
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    _pieces[row, column] = pieces[row * Size + column];
                }
            }
        }

        private void Initialize()
        {
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    if (row < 3 && (row + column) % 2 == 1)
                    {
                        _pieces[row, column] = new Man(EPieceColor.White);
                    }
                    else if (row > Size - 4 && (row + column) % 2 == 1)
                    {
                        _pieces[row, column] = new Man(EPieceColor.Black);
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

            var piece = _pieces[currentPosition.Row, currentPosition.Column];
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
                    _pieces[capture.Row, capture.Column] = null;
                    if (piece.IsUpgradeable(new Position(capture.Row + piece.GetWays().First(), capture.Column)))
                    {
                        upgradeNeeded = true;
                    }
                }
            }
            
            _pieces[newPosition.Row, newPosition.Column] = piece;
            _pieces[currentPosition.Row, currentPosition.Column] = null;
            
            if (upgradeNeeded)
            {
                UpdatePiece(newPosition);
            }
        }

        public IEnumerable<Position> GetPossibleMoves(Position position)
        {
            var piece = _pieces[position.Row, position.Column];
            var moves = new List<Position>();
            var ways = piece.GetWays();

            foreach (var way in ways)
            {
                // Right side
                if (position.Column + 1 < Size && position.Row + way >= 0 && position.Row + way < Size)
                {
                    var rightPosition = new Position(position.Row + way, position.Column + 1);
                    if (_pieces[rightPosition.Row, rightPosition.Column] == null)
                    {
                        moves.Add(rightPosition);
                    }
                    else if (IsPositionOccupied(rightPosition, piece.Color.Opposite()))
                    {
                        var paths = GetJumps(position, piece);
                        foreach (var path in paths)
                        {
                            moves.AddRange(path);
                        }
                    }
                }

                // Left side
                if (position.Column - 1 >= 0 && position.Row + way >= 0 && position.Row + way < Size)
                {
                    var leftPosition = new Position(position.Row + way, position.Column - 1);
                    if (_pieces[leftPosition.Row, leftPosition.Column] == null)
                    {
                        moves.Add(leftPosition);
                    }
                    else if (IsPositionOccupied(leftPosition, piece.Color.Opposite()))
                    {
                        //moves.AddRange(GetJumps(position, piece));
                        var paths = GetJumps(position, piece);
                        foreach (var path in paths)
                        {
                            moves.AddRange(path);
                        }
                    }
                }
            }

            return moves;
        }

        private IEnumerable<List<Position>> GetJumps(Position position, Piece piece, ISet<Position> visited = null, List<Position> path = null)
        {
            var jumpPaths = new List<List<Position>>();

            if (!MultiJump)
            {
                return jumpPaths;
            }
            
            visited ??= new HashSet<Position>();
            path ??= [];
            
            if (piece == null || !IsPositionValid(position))
            {
                return jumpPaths; // Return an empty list if the piece is null or the position is invalid
            }

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
                    if (_pieces[opponentPosition.Row, opponentPosition.Column] != null &&
                        IsPositionOccupied(opponentPosition, piece.Color.Opposite()))
                    {
                        var rightJumpPosition = new Position(position.Row + way * 2, position.Column + 2);
                        if (_pieces[rightJumpPosition.Row, rightJumpPosition.Column] == null && !visited.Contains(rightJumpPosition))
                        {
                            var newPath = new List<Position>(path);
                            newPath.Add(rightJumpPosition);
                            jumpPaths.Add(newPath);
                            visited.Add(rightJumpPosition);
                            jumpPaths.AddRange(GetJumps(rightJumpPosition, piece, visited, newPath));
                        }
                    }
                }

                // Left side
                if (position.Column - 2 >= 0 && position.Row + way * 2 >= 0 && position.Row + way * 2 < Size)
                {
                    var opponentPosition = new Position(position.Row + way, position.Column - 1);
                    if (_pieces[opponentPosition.Row, opponentPosition.Column] != null &&
                        IsPositionOccupied(opponentPosition, piece.Color.Opposite()))
                    {
                        var leftJumpPosition = new Position(position.Row + way * 2, position.Column - 2);
                        if (_pieces[leftJumpPosition.Row, leftJumpPosition.Column] == null && !visited.Contains(leftJumpPosition))
                        {
                            var newPath = new List<Position>(path) { leftJumpPosition };
                            jumpPaths.Add(newPath);
                            visited.Add(leftJumpPosition);
                            jumpPaths.AddRange(GetJumps(leftJumpPosition, piece, visited, newPath));
                        }
                    }
                }
            }

            return jumpPaths;
        }

        private IEnumerable<Position> FindJumpCaptures(Position from, Position to)
        {
            var captures = new HashSet<Position>();
            var paths = GetJumps(from, _pieces[from.Row, from.Column]);
            var longestPath = new List<Position>();

            foreach (var path in paths)
            {
                path.Insert(0, from);
            }
            
            foreach (var path in paths)
            {
                if (path.Count != path.Distinct().ToList().Count)
                {
                    continue;
                }
                
                var reversedPath = new List<Position>(path);
                reversedPath.Reverse();
                if (path.Last().Equals(to) && path.Count > longestPath.Count)
                {
                    longestPath = path;
                }
                if (reversedPath.Last().Equals(to) && reversedPath.Count > longestPath.Count)
                {
                    longestPath = reversedPath;
                }
            }
            
            for (var i = 0; i < longestPath.Count - 1; i++)
            {
                var capture = new Position((longestPath[i].Row + longestPath[i + 1].Row) / 2, (longestPath[i].Column + longestPath[i + 1].Column) / 2);
                captures.Add(capture);
            }
            
            return captures;
        }

        public bool CheckForWin()
        {
            return BlackPieces == 0 || WhitePieces == 0;
        }

        public Piece GetPiece(Position from)
        {
            return _pieces[from.Row, from.Column];
        }

        private bool IsPositionValid(Position position)
        {
            return position.Row >= 0 && position.Row < Size && position.Column >= 0 && position.Column < Size;
        }

        private bool IsPositionEmpty(Position position)
        {
            return _pieces[position.Row, position.Column] == null;
        }

        private static bool IsSimpleMove(Position currentPosition, Position newPosition)
        {
            return System.Math.Abs(currentPosition.Row - newPosition.Row) == 1 &&
                   System.Math.Abs(currentPosition.Column - newPosition.Column) == 1;
        }

        private bool IsPositionOccupied(Position position, EPieceColor color)
        {
            return _pieces[position.Row, position.Column] != null &&
                   _pieces[position.Row, position.Column].Color == color;
        }

        private void UpdatePiece(Position position)
        {
            _pieces[position.Row, position.Column] = new King(_pieces[position.Row, position.Column].Color);
        }
    }
}