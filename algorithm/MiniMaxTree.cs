using System;
using System.Collections.Generic;
using NochkaGame.game;

namespace NochkaGame.algorithm
{
    public class MiniMaxNode
    {
        public readonly GameState State;
        public float AlphaEval;
        public float BetaEval;
        public readonly List<MiniMaxNode> ChildrenNodes;
        public readonly Move Move;

        public MiniMaxNode(GameState startState, Move move)
        {
            Move = move;
            State = startState.Clone();
            State.MakeMove(move);
            ChildrenNodes = new List<MiniMaxNode>();
            AlphaEval = float.MinValue;
            BetaEval = float.MaxValue;
        }

        public MiniMaxNode(GameState startState)
        {
            State = startState.Clone();
            ChildrenNodes = new List<MiniMaxNode>();
            AlphaEval = float.MinValue;
            BetaEval = float.MaxValue;
        }
    }

    public class MiniMaxTree
    {
        private readonly int _depthLimit;
        private readonly MiniMaxNode _rootNode;

        public MiniMaxTree(GameState rootState, int depthLimit = 1)
        {
            _rootNode = new MiniMaxNode(rootState);
            _depthLimit = depthLimit;
            GenerateTree();
        }

        private void GenerateTree()
        {
            ProcessNode(_rootNode, 0);
        }

        private void ProcessNode(MiniMaxNode node, int depth)
        {
            var isMin = depth % 2 == 0;
            if (node.State.IsTerminal() || depth == _depthLimit * 2)
            {
                if (isMin)
                {
                    node.BetaEval = EvaluateState(node.State, true);
                }
                else
                {
                    // This is opponent's victory, worst outcome
                    node.AlphaEval = float.MaxValue;
                }
            }
            else
            {
                var possibleMoves = GeneratePossibleMoves(node.State, isMin);
                foreach (var move in possibleMoves)
                {
                    var newChildrenNode = new MiniMaxNode(node.State, move)
                    {
                        AlphaEval = node.AlphaEval, BetaEval = node.BetaEval
                    };
                    ProcessNode(newChildrenNode, depth + 1);
                    if (isMin)
                    {
                        if (node.BetaEval > newChildrenNode.AlphaEval) node.BetaEval = newChildrenNode.AlphaEval;
                    }
                    else
                    {
                        if (node.AlphaEval < newChildrenNode.BetaEval) node.AlphaEval = newChildrenNode.BetaEval;
                    }

                    node.ChildrenNodes.Add(newChildrenNode);
                    if (node.AlphaEval >= node.BetaEval) break;
                }
            }
        }

        private static float EvaluateState(GameState state, bool isSecondPlayer)
        {
            var profitableCards = 0;
            var player = isSecondPlayer ? state.SecondPlayer : state.FirstPlayer;
            foreach (var playingCard in player.PlayerHand)
            {
                if (state.GameTable.AvailableMoves[(int) playingCard.Suit, playingCard.Value - 6])
                {
                    profitableCards++;
                }
            }

            if (profitableCards == 0) return 100 * player.PlayerHand.Count;
            var profitablePercent = (float) profitableCards / player.PlayerHand.Count;
            var evaluation = player.PlayerHand.Count / profitablePercent;
            return evaluation;
        }

        private static List<Move> GeneratePossibleMoves(GameState state, bool isSecondPlayer)
        {
            var possibleMoves = new List<Move>();
            var player = isSecondPlayer ? state.SecondPlayer : state.FirstPlayer;
            for (var i = 0; i < player.PlayerHand.Count; i++)
            {
                if (state.GameTable.AvailableMoves[(int) player.PlayerHand[i].Suit, player.PlayerHand[i].Value - 6])
                {
                    var place = new Tuple<int, int>((int) player.PlayerHand[i].Suit, player.PlayerHand[i].Value - 6);
                    possibleMoves.Add(new Move(place, isSecondPlayer, i));
                }
            }

            if (possibleMoves.Count == 0)
            {
                for (var i = 0; i < state.GameTable.Cards.GetLength(0); i++)
                {
                    for (var j = 0; j < state.GameTable.Cards.GetLength(1); j++)
                    {
                        if (state.GameTable.AvailableMoves[i, j] && state.GameTable.Cards[i, j] != null)
                        {
                            var place = new Tuple<int, int>(i, j);
                            possibleMoves.Add(new Move(place, isSecondPlayer));
                        }
                    }
                }
            }

            return possibleMoves;
        }

        public Move AiMakeMove()
        {
            Move bestMove = null;
            var minEval = float.MaxValue;
            foreach (var childrenNode in _rootNode.ChildrenNodes)
                if (minEval > childrenNode.AlphaEval)
                {
                    bestMove = childrenNode.Move;
                    minEval = childrenNode.AlphaEval;
                }

            return bestMove;
        }
    }
}