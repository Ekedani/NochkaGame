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
                        if (!card.IsVisible)
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
            for (int i = 0; i < AIHandView.ColumnCount; i++)
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
            _currentGameState = new GameState();
            InitializeTable();
            RenderTable();
            RenderAiHand();
            RenderHumanHand();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }
    }
}