using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public abstract class Piece(EPieceType type, EPieceColor color)
    {
        public EPieceType Type { get; } = type;
        public EPieceColor Color { get; } = color;

        public static Piece Create(EPieceType type, EPieceColor color)
        {
            return type switch
            {
                EPieceType.Man => new Man(color),
                EPieceType.King => new King(color),
                _ => throw new NotSupportedException($"Unsupported piece type: {type}")
            };
        }

        public bool IsUpgradeable(Position position)
        {
            if (Type == EPieceType.King)
            {
                return false;
            }
            return Color == EPieceColor.Black ? position.Row == 0 : position.Row == 7;
        }

        public abstract IEnumerable<int> GetWays();
    }
}