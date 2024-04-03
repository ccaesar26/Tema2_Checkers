using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Models
{
    public class Game
    {
        private readonly Board _board;
        private readonly List<IGameListener> _listeners = new List<IGameListener>();
        
        private EPieceColor _currentPlayer;
        private EState _state;

        public Game()
        {
            _board = new Board();
            _board.Initialize();
            _currentPlayer = EPieceColor.White;
            _state = EState.Playing;
        }

        public void MakeMove(Position from, Position to)
        {
            var piece = _board.GetPiece(from);
            if (piece == null)
            {
                throw new InvalidOperationException("There is no piece at the specified position.");
            }

            if (piece.Color != _currentPlayer)
            {
                throw new InvalidOperationException("It is not the current player's turn.");
            }

            _board.MovePiece(from, to);
            
            if (_board.CheckForWin())
            {
                _state = _currentPlayer == EPieceColor.White ? EState.BlackWon : EState.WhiteWon;
            }

            SwitchPlayer();
        }

        private void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == EPieceColor.White ? EPieceColor.Black : EPieceColor.White;
        }
        
        public void AddListener(IGameListener listener)
        {
            _listeners.Add(listener);
        }
        
        public void RemoveListener(IGameListener listener)
        {
            _listeners.Remove(listener);
        }
        
        private void NotifyMoveMade(Position from, Position to)
        {
            foreach (var listener in _listeners)
            {
                listener.OnMoveMade(from, to);
            }
        }
        
        private void NotifyGameOver(EPieceColor? winner)
        {
            foreach (var listener in _listeners)
            {
                listener.OnGameOver(winner);
            }
        }
        
        private void NotifyPieceUpdated(Position position, EPieceColor color)
        {
            foreach (var listener in _listeners)
            {
                listener.OnPieceUpdated(position, color);
            }
        }

        public Piece GetPiece(Position position)
        {
            return _board.GetPiece(position);
        }
    }
}