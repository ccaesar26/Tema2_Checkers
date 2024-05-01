namespace Checkers.Models
{
    public interface IGameListener
    {
        void OnMoveMade(Position from, Position to);
        void OnGameOver(EPieceColor? winner);
        void OnPlayerChanged(EPieceColor currentPlayer);
        void OnGameRestarted();
        void OnGameStarted();
    }
}