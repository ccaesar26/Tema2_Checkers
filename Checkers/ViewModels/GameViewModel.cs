using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Checkers.Commands;
using Checkers.Models;

namespace Checkers.ViewModels
{
    public class GameViewModel : BaseViewModel, IGameListener
    {
        public ObservableCollection<BoardSquare> Board { get; }
        
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
        
        private readonly Game _game;
        
        private Position? _firstPosition;
        private Position? _secondPosition;
        
        public ICommand PieceClickedCommand { get; }
        private static bool CanExecutePieceClicked(object parameter)
        {
            //var square = (BoardSquare)parameter;
            /*if (_firstPosition == null)
            {
                return square.PieceImagePath != "/Assets/empty.png";
            }*/
            return true;
        }
        
        public GameViewModel()
        {
            _game = new Game();
            
            Board = [];
            InitializeBoard();
            
            PieceClickedCommand = new RelayCommand(o => OnPieceClicked((BoardSquare)o), CanExecutePieceClicked);
            
            _game.AddListener(this);
        }

        public void MakeMove(Position from, Position to)
        {
            _game.MakeMove(from, to);
        }

        public void OnMoveMade(Position from, Position to)
        {
            InitializeBoard();
        }

        public void OnGameOver(EPieceColor? winner)
        {
            throw new System.NotImplementedException();
        }

        public void OnPieceUpdated(Position position, EPieceColor color)
        {
            var square = Board[(position.Row * 8) + position.Column];
            square.PieceImagePath = color == EPieceColor.White ? "/Assets/white_man.png" : "/Assets/black_man.png";
        }

        public void OnPlayerChanged(EPieceColor currentPlayer)
        {
            CurrentPlayer = currentPlayer == EPieceColor.White ? "White Player" : "Black Player";
            OnPropertyChanged("CurrentPlayer");
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
                    Board.Add(piece == null
                        ? new BoardSquare { PieceImagePath = "/Assets/empty.png", Position = new Position(i, j)}
                        : new BoardSquare
                        {
                            PieceImagePath = piece.Color == EPieceColor.White
                                ? "/Assets/white_man.png"
                                : "/Assets/black_man.png",
                            Position = new Position(i, j)
                        });
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
    }
    
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
}