using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public class King(EPieceColor color) : Piece(EPieceType.King, color)
    {
        public override IEnumerable<int> GetWays()
        {
            return new[] { -1, 1 };
        }
    }
}