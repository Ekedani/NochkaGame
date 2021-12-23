using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NochkaGame.game;
using NochkaGame.game.card;

namespace NochkaGame
{
    public partial class GameForm : Form
    {
        private GameState _currentGameState;

        public GameForm()
        {
            InitializeComponent();
        }

        private void InitializeTable()
        {
            tableView.RowCount = 4;
            tableView.ColumnCount = 9;
            foreach (DataGridViewColumn column in tableView.Columns)
            {
                column.DividerWidth = 10;
                column.Width = 100;
            }

            foreach (DataGridViewRow row in tableView.Rows)
            {
                row.DividerHeight = 10;
            }
        }

        private void RenderGameState()
        {
            RenderTable();
            RenderAiHand();
            RenderHumanHand();
        }

        private void RenderTable()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tableView[j, i].Value = "";
                    var card = _currentGameState.GameTable.Cards[i, j];
                    if (card == null)
                    {
                        tableView[j, i].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        if (card.IsVisible)
                        {
                            tableView[j, i].Style.BackColor = Color.White;
                            tableView[j, i].Style.ForeColor = (int) card.Suit < 2 ? Color.Red : Color.Black;
                            tableView[j, i].Value = card.ToString();
                        }
                        else
                        {
                            tableView[j, i].Style.BackColor = Color.DarkGray;
                        }
                    }
                }
            }
        }

        private void RenderHumanHand()
        {
            playerHandView.RowCount = 1;
            playerHandView.ColumnCount = _currentGameState.FirstPlayer.PlayerHand.Count;
            for (int i = 0; i < playerHandView.ColumnCount; i++)
            {
                var card = _currentGameState.FirstPlayer.PlayerHand[i];
                playerHandView.Columns[i].DividerWidth = 10;
                playerHandView.Columns[i].Width = 80;
                playerHandView[i, 0].Style.BackColor = Color.White;
                playerHandView[i, 0].Style.ForeColor = (int) card.Suit < 2 ? Color.Red : Color.Black;
                playerHandView[i, 0].Value = card.ToString();
            }
        }

        private void RenderAiHand()
        {
            AIHandView.RowCount = 1;
            AIHandView.ColumnCount = _currentGameState.SecondPlayer.PlayerHand.Count;
            for (int i = 0; i < AIHandView.ColumnCount; i++)
            {
                AIHandView.Columns[i].DividerWidth = 10;
                AIHandView.Columns[i].Width = 80;
                AIHandView[i, 0].Style.BackColor = Color.DarkGray;
            }
        }

        private void StartNewGame()
        {
            skipTurnButton.Enabled = true;
            moveButton.Enabled = true;
            _currentGameState = new GameState();
            InitializeTable();
            RenderTable();
            RenderAiHand();
            RenderHumanHand();
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            try
            {
                MakeHumanMove();
                skipTurnButton.Enabled = false;
                if (_currentGameState.IsTerminal())
                {
                    moveButton.Enabled = false;
                    MessageBox.Show("You have won, congrajulations!", "Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MakeAiMove();
                if (_currentGameState.IsTerminal())
                {
                    moveButton.Enabled = false;
                    MessageBox.Show("You have lost!", "Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void skipTurnButton_Click(object sender, EventArgs e)
        {
            MakeAiMove();
            skipTurnButton.Enabled = false;
        }

        private void MakeAiMove()
        {
            var move = _currentGameState.SecondPlayer.AssumeMoves(_currentGameState.GameTable);
            _currentGameState.MakeMove(move);
            RenderTable();
            RenderAiHand();
        }

        private void MakeHumanMove()
        {
            var selectedCells = tableView.SelectedCells;
            if (selectedCells.Count != 1) throw new Exception("Only one place must be selected!");
            var selectedCell = selectedCells[0];
            if (!_currentGameState.GameTable.AvailableMoves[selectedCell.RowIndex, selectedCell.ColumnIndex])
            {
                throw new Exception("You can't make this move!");
            }

            Move move = null;
            if (_currentGameState.FirstPlayer.HasMoves(_currentGameState.GameTable))
            {
                var card = new PlayingCard(selectedCell.ColumnIndex + 6, (CardSuit) selectedCell.RowIndex);
                if (!_currentGameState.FirstPlayer.HasCard(card)) throw new Exception("You haven't needed card!");
                var idx = _currentGameState.FirstPlayer.PlayerHand.FindIndex(playingCard =>
                    playingCard.Suit == card.Suit && playingCard.Value == card.Value);
                move = new Move(new Tuple<int, int>(selectedCell.RowIndex, selectedCell.ColumnIndex), false, idx);
                
            }
            else
            {
                move = new Move(new Tuple<int, int>(selectedCell.RowIndex, selectedCell.ColumnIndex), false, -1);
            }
            _currentGameState.MakeMove(move);
            RenderTable();
            RenderHumanHand();
        }
    }
}