using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Checkers.Commands;
using Checkers.Models;
using Checkers.Services;
using Microsoft.Win32;

namespace Checkers.ViewModels;

public class GameViewModel : BaseViewModel, IGameListener
{
    public ObservableCollection<BoardSquare> Board { get; }
    public ObservableCollection<string> Leaderboard { get; }
    private GameOperations _gameOperations;
        
    private string _currentPlayer = "Black Player";
    public string CurrentPlayer 
    { 
        get => _currentPlayer; 
        set
        {
            _currentPlayer = value; 
            OnPropertyChanged(nameof(CurrentPlayer)); 
        }
    }
    public bool MultiJumps
    {
        get => _game.MultiJumps;
        set
        {
            if (IsNotStarted) _game.MultiJumps = value;
            OnPropertyChanged(nameof(MultiJumps));
        }
    }
    
    private bool _isMultiJumpEnabled = true;
    public bool IsMultiJumpEnabled
    {
        get => _isMultiJumpEnabled;
        set
        {
            _isMultiJumpEnabled = value;
            OnPropertyChanged(nameof(IsMultiJumpEnabled));
            IsMultiJumpDisabled = !value;
        }
    }

    private bool _isMultiJumpDisabled;
    public bool IsMultiJumpDisabled
    {
        get => _isMultiJumpDisabled;
        set
        {
            _isMultiJumpDisabled = value;
            OnPropertyChanged(nameof(IsMultiJumpDisabled));
        }
    }
    
    private readonly Game _game;
        
    private Position? _firstPosition;
    private Position? _secondPosition;

    private bool _isNotStarted = true;
    public bool IsNotStarted
    {
        get => _isNotStarted;
        set
        {
            _isNotStarted = value;
            OnPropertyChanged(nameof(IsNotStarted));
        }
    }
    public ICommand PieceClickedCommand { get; }
    private bool CanExecutePieceClicked(object parameter)
    {
        var square = (BoardSquare)parameter;
        if (_firstPosition == null)
        {
            return square.PieceImagePath != "/Assets/empty.png";
        }
        return true;
    }
        
    public ICommand RestartCommand { get; }
    private static bool CanExecuteRestart(object parameter) => true;
    
    public ICommand SaveGameCommand { get; }
    private static bool CanExecuteSaveGame(object parameter) => true;
    
    public ICommand LoadGameCommand { get; }
    private static bool CanExecuteLoadGame(object parameter) => true;
        
    public GameViewModel()
    {
        _game = new Game();
            
        Board = [];
        InitializeBoard();

        Leaderboard = [];
        UpdateLeaderboard();
            
        _gameOperations = new GameOperations(this);
            
        PieceClickedCommand = new RelayCommand(o => OnPieceClicked((BoardSquare)o), CanExecutePieceClicked);
        RestartCommand = new RelayCommand(o => _gameOperations.Restart(), CanExecuteRestart);
        SaveGameCommand = new RelayCommand(o => _gameOperations.SaveGame(), CanExecuteSaveGame);
        LoadGameCommand = new RelayCommand(o => _gameOperations.LoadGame(), CanExecuteLoadGame);
            
        _game.AddListener(this);
    }

    private void MakeMove(Position from, Position to)
    {
        _game.MakeMove(from, to);
    }

    public void OnMoveMade(Position from, Position to)
    {
        InitializeBoard();
    }

    public void OnGameOver(EPieceColor? winner)
    {
        var result = MessageBox.Show(winner switch
        {
            null => "It's a draw!",
            EPieceColor.White => "White Player Wins!",
            _ => "Black Player Wins!"
        }, "Game Over", MessageBoxButton.OK);
        switch (result)
        {
            case MessageBoxResult.OK:
                _game.Restart();
                break;
            case MessageBoxResult.Cancel:
            case MessageBoxResult.None:
            case MessageBoxResult.Yes:
            case MessageBoxResult.No:
            default:
                Application.Current.Shutdown();
                break;
        }
        
        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        var leaderboard = JsonUtility.DeserializeLeaderboard();
        Leaderboard.Clear();
        Leaderboard.Add($"White's wins: {leaderboard.WhiteWins}");
        Leaderboard.Add($"Black's wins: {leaderboard.BlackWins}");
    }

    public void OnPlayerChanged(EPieceColor currentPlayer)
    {
        CurrentPlayer = currentPlayer == EPieceColor.White ? "White Player" : "Black Player";
        OnPropertyChanged("CurrentPlayer");
    }

    public void OnGameRestarted()
    {
        InitializeBoard();
        OnPlayerChanged(EPieceColor.Black);
        IsMultiJumpEnabled = _game.MultiJumps;
        IsNotStarted = true;
    }

    public void OnGameStarted()
    {
        IsNotStarted = false;
        IsMultiJumpEnabled = _game.MultiJumps;
    }

    private void OnPieceClicked(BoardSquare square)
    {
        if (_firstPosition == null)
        {
            _firstPosition = square.Position;
            try
            {
                var moves = _game.GetPossibleMoves((Position)_firstPosition);
                UpdateMoves(moves);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _firstPosition = null;
            }
        }
        else
        {
            _secondPosition = square.Position;
            try
            {
                MakeMove((Position)_firstPosition, (Position)_secondPosition);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                foreach (var boardSquare in Board)
                {
                    boardSquare.IsPossibleMove = false;
                }
                _firstPosition = null;
                _secondPosition = null;
            }
        }
    }
        
    private void InitializeBoard()
    {
        Board.Clear();
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var piece = _game.GetPiece(new Position(i, j));
                if (piece == null)
                {
                    Board.Add(new BoardSquare
                        { PieceImagePath = "/Assets/empty.png", Position = new Position(i, j) });
                }
                else
                {
                    if (piece.Type == EPieceType.Man)
                    {
                        Board.Add(new BoardSquare
                        {
                            PieceImagePath = piece.Color == EPieceColor.White
                                ? "/Assets/white_man.png"
                                : "/Assets/black_man.png",
                            Position = new Position(i, j)
                        });
                    }
                    else
                    {
                        Board.Add(new BoardSquare
                        {
                            PieceImagePath = piece.Color == EPieceColor.White
                                ? "/Assets/white_king.png"
                                : "/Assets/black_king.png",
                            Position = new Position(i, j)
                        });
                    }
                }
            }
        }
    }
        
    private void UpdateMoves(IEnumerable<Position> moves)
    {
        foreach (var move in moves)
        {
            var square = Board[(move.Row * 8) + move.Column];
            square.IsPossibleMove = true;
        }
    }

    public void Restart()
    {
        _game.Restart();
    }
        
    public void SaveGame()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Checkers Game Files (*.chk)|*.chk",
            DefaultExt = "chk",
            AddExtension = true
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            _game.SaveGame(saveFileDialog.FileName);
        }
    }
    
    public void LoadGame()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Checkers Game Files (*.chk)|*.chk",
            DefaultExt = "chk",
            AddExtension = true
        };
        if (openFileDialog.ShowDialog() == true)
        {
            _game.LoadGame(openFileDialog.FileName);
        }
    }
}