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

        public abstract IEnumerable<int> GetWays(Position currentPosition);
    }
}