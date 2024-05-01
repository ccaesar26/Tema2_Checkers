using Checkers.Models;
using Checkers.ViewModels;

namespace Checkers.Services;

public class GameOperations(GameViewModel gameViewModel)
{
    public void Restart()
    {
        gameViewModel.Restart();
    }
    
    public void SaveGame()
    {
        gameViewModel.SaveGame();
    }
    
    public void LoadGame()
    {
        gameViewModel.LoadGame();
    }
}