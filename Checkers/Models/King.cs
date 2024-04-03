using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public class King : Piece
    {
        public King(EPieceColor color) : base(EPieceType.King, color)
        {
        }
        
        public override IEnumerable<int> GetWays(Position currentPosition)
        {
            return new[] { -1, 1 };
        }
    }
}