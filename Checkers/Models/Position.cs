﻿namespace Checkers.Models
{
    public readonly struct Position(int row, int column)
    {
        public int Row { get; } = row;
        public int Column { get; } = column;
    }
}