using Checkers.Models;

namespace Checkers.ViewModels;

public class BoardSquare : BaseViewModel
{
    private string _pieceImagePath;
    public string PieceImagePath
    {
        get => _pieceImagePath;
        set
        {
            if (_pieceImagePath == value) return;
            _pieceImagePath = value;
            OnPropertyChanged(nameof(PieceImagePath));
        }
    }

    private Position _position;
    public Position Position
    {
        get => _position;
        set
        {
            if (Equals(_position, value)) return;
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }

    private bool _isPossibleMove = false;
    public bool IsPossibleMove
    {
        get => _isPossibleMove;
        set
        {
            if (_isPossibleMove == value) return;
            _isPossibleMove = value;
            OnPropertyChanged(nameof(IsPossibleMove));
        }
    }
}