using System;
using System.Collections.ObjectModel;
using Checkers.Models;

namespace Checkers.ViewModels
{
    public class GameViewModel : BaseViewModel, IGameListener
    {
        public ObservableCollection<string> Board { get; }
        private readonly Game _game;
        
        public GameViewModel()
        {
            _game = new Game();
            
            Board = new ObservableCollection<string>();
            InitializeBoard();
        }

        public void MakeMove(Position from, Position to)
        {
            _game.MakeMove(from, to);
        }

        public void OnMoveMade(Position from, Position to)
        {
            throw new System.NotImplementedException();
        }

        public void OnGameOver(EPieceColor? winner)
        {
            throw new System.NotImplementedException();
        }

        public void OnPieceUpdated(Position position, EPieceColor color)
        {
            var index = position.Row * 8 + position.Column;
            Board[index] = color == EPieceColor.White ? "Assets/white_king.png" : "Assets/black_king.png";
        }
        
        private void InitializeBoard()
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var piece = _game.GetPiece(new Position(i, j));
                    if (piece == null)
                    {
                        Board.Add("Assets/empty.png");
                    }
                    else
                    {
                        Board.Add(piece.Color == EPieceColor.White ? "Assets/white_man.png" : "Assets/black_man.png");
                    }
                }
            }
        }
    }
}