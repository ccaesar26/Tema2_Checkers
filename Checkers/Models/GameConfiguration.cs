using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Checkers.Models;

public class GameConfiguration
{
    public int BoardSize { get; set; }
    public Piece[] Pieces { get; set; }
    public EPieceColor CurrentPlayer { get; set; }
    public EState State { get; set; }
    public bool MultiJumps { get; set; }

    public GameConfiguration(Game game)
    {
        BoardSize = game.Board.Size;
        Pieces = new Piece[BoardSize * BoardSize];
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                Pieces[row * BoardSize + col] = game.Board.GetPiece(new Position(row, col));
            }
        }
        CurrentPlayer = game.CurrentPlayer;
        State = game.State;
        MultiJumps = game.MultiJumps;
    }

    [JsonConstructorAttribute]
    public GameConfiguration(
        int boardSize,
        Piece[] pieces,
        EPieceColor currentPlayer,
        EState state,
        bool multiJumps
    )
    {
        BoardSize = boardSize;
        Pieces = pieces;
        CurrentPlayer = currentPlayer;
        State = state;
        MultiJumps = multiJumps;
    }
}