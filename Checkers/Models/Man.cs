using System;
using System.Collections.Generic;

namespace Checkers.Models
{
    public class Man(EPieceColor color) : Piece(EPieceType.Man, color)
    {
        public override IEnumerable<int> GetWays()
        {
            return Color == EPieceColor.Black ? new[] { -1 } : new[] { 1 };
        }
    }
}