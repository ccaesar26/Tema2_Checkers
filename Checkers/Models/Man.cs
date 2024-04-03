using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public class Man : Piece
    {
        public Man(EPieceColor color) : base(EPieceType.Man, color)
        {
        }

        public override IEnumerable<int> GetWays(Position currentPosition)
        {
            return Color == EPieceColor.Black ? new[] { -1 } : new[] { 1 };
        }
    }
}