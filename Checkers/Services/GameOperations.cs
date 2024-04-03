using Checkers.Models;
using Checkers.ViewModels;

namespace Checkers.Services;

public class GameOperations(GameViewModel gameViewModel)
{
    private GameViewModel _gameViewModel = gameViewModel;
    
    public void MakeMove(Position from, Position to)
    {
        _gameViewModel.MakeMove(from, to);
    }
}