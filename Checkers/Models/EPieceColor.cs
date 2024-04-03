namespace Checkers.Models
{
    public enum EPieceColor
    {
        Black,
        White
    }
    
    public static class EPieceColorExtensions
    {
        public static EPieceColor Opposite(this EPieceColor color)
        {
            return color == EPieceColor.Black ? EPieceColor.White : EPieceColor.Black;
        }
        
        public static int GetDirection(this EPieceColor color)
        {
            return color == EPieceColor.Black ? -1 : 1;
        }
        
        public static bool IsUpgradable(this EPieceColor color, Position position)
        {
            return color == EPieceColor.Black ? position.Row == 0 : position.Row == 7;
        }
    }
}