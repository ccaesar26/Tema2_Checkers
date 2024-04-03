namespace Checkers.Models
{
    public interface IGameListener
    {
        void OnMoveMade(Position from, Position to);
        void OnGameOver(EPieceColor? winner);
        void OnPieceUpdated(Position position, EPieceColor color);
    }
}