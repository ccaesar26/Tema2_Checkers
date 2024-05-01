using System;
using System.Collections.Generic;
using System.Linq;
using Checkers.Services;

namespace Checkers.Models
{
    public class Game
    {
        public Board Board { get; private set; } = new();

        private readonly List<IGameListener> _listeners = [];

        public bool IsNotStarted => State == EState.Initial;

        public EPieceColor CurrentPlayer { get; private set; } = EPieceColor.Black;
        public EState State { get; private set; } = EState.Initial;
        public bool MultiJumps
        {
            get => Board.MultiJump;
            set => Board.MultiJump = value;
        }

        public void Restart()
        {
            Board = new Board();
            CurrentPlayer = EPieceColor.Black;
            State = EState.Initial;
            NotifyGameRestarted();
        }

        public void SaveGame(string filePath)
        {
            var gameConfiguration = new GameConfiguration(this);
            JsonUtility.SerializeGame(gameConfiguration, filePath);
        }
        
        public void LoadGame(string filePath)
        {
            var gameConfiguration = JsonUtility.DeserializeGame(filePath);
            Board = new Board(gameConfiguration.Pieces);
            CurrentPlayer = gameConfiguration.CurrentPlayer;
            MultiJumps = gameConfiguration.MultiJumps;
            State = gameConfiguration.State;
            NotifyGameRestarted();
            NotifyPlayerChanged(CurrentPlayer);
            NotifyGameStarted();
        }
        
        public void MakeMove(Position from, Position to)
        {
            if (State == EState.Initial)
            {
                NotifyGameStarted();
            }
            
            State = EState.Playing;
            
            var piece = Board.GetPiece(from);
            if (piece == null)
            {
                throw new InvalidOperationException("There is no piece at the specified position.");
            }

            if (piece.Color != CurrentPlayer)
            {
                throw new InvalidOperationException("It is not the current player's turn.");
            }

            
            Board.MovePiece(from, to);
            
            NotifyMoveMade(from, to);
            
            if (Board.CheckForWin())
            {
                State = CurrentPlayer == EPieceColor.White ? EState.BlackWon : EState.WhiteWon;

                var winner = CurrentPlayer;
                Leaderboard.UpdateLeaderboard(winner);
                
                NotifyGameOver(CurrentPlayer);
            }
            else
            {
                SwitchPlayer();
            }
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == EPieceColor.White ? EPieceColor.Black : EPieceColor.White;
            NotifyPlayerChanged(CurrentPlayer);
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
        
        private void NotifyPlayerChanged(EPieceColor currentPlayer)
        {
            foreach (var listener in _listeners)
            {
                listener.OnPlayerChanged(currentPlayer);
            }
        }
        
        private void NotifyGameRestarted()
        {
            foreach (var listener in _listeners)
            {
                listener.OnGameRestarted();
            }
        }
        
        private void NotifyGameStarted()
        {
            foreach (var listener in _listeners)
            {
                listener.OnGameStarted();
            }
        }

        public Piece GetPiece(Position position)
        {
            return Board.GetPiece(position);
        }

        public IEnumerable<Position> GetPossibleMoves(Position position)
        {
            if (State == EState.Initial)
            {
                NotifyGameStarted();
            }
            
            State = EState.Playing;
            
            if (Board.GetPiece(position)?.Color != CurrentPlayer)
            {
                throw new InvalidOperationException("It is not the current player's turn.");
            }
            return Board.GetPossibleMoves(position);
        }
    }
}