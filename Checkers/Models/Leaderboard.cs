using Checkers.Services;

namespace Checkers.Models;

public class Leaderboard
{
    public int WhiteWins { get; set; }
    public int BlackWins { get; set; }
    
    public static void UpdateLeaderboard(EPieceColor winner)
    {
        var leaderboard = JsonUtility.DeserializeLeaderboard();
        
        if (winner == EPieceColor.White)
        {
            leaderboard.WhiteWins++;
        }
        else
        {
            leaderboard.BlackWins++;
        }
        
        JsonUtility.SerializeLeaderboard(leaderboard);
    }
}