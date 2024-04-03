using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public abstract class Piece
    {
        public EPieceType Type { get; }
        public EPieceColor Color { get; }
        
        protected Piece(EPieceType type, EPieceColor color)
        {
            Type = type;
            Color = color;
        }

        public bool IsUpgradeable(Position position)
        {
            return Color == EPieceColor.Black ? position.Row == 0 : position.Row == 7;
        }

        public abstract IEnumerable<int> GetWays();
    }
}