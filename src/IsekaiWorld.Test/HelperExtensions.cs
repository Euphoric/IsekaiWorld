using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        private class MessageParser
        {
            private readonly Dictionary<string, HexCubeCoord?> _characterPositions = new Dictionary<string, HexCubeCoord?>();

            public List<string> logs = new List<string>();
            
            public void MessageHandler(IEntityMessage evnt)
            {
                if (evnt is CharacterUpdated cu)
                {
                    _characterPositions.TryGetValue(cu.Id, out var previousPosition);

                    if (previousPosition != cu.Position)
                    {
                        logs.Add($"Character {cu.Id} : {cu.Position}");
                    }
                    
                    _characterPositions[cu.Id] = cu.Position;
                }else if (evnt is BuildingUpdated bu)
                {
                    logs.Add($"Building {bu.EntityId}");
                }
                else
                {
                    logs.Add($"Event: {evnt.GetType().Name}");
                }
            }   
        }
        
        public static void UpdateUntil(this GameEntity game, Func<GameTestStep, bool> check, int maxSteps = 1000)
        {
            var messaging = new EntityMessaging();
            game.Messaging.Register(messaging);
            var timedOut = UpdateUntilInner(game, check, maxSteps);
            game.Messaging.Unregister(messaging);

            var parser = new MessageParser();
            messaging.HandleMessages(parser.MessageHandler);
            
            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }

        public static bool UpdateUntilInner(this GameEntity game, Func<GameTestStep, bool> check, int maxSteps = 1000)
        {
            int steps = 0;
            while (!check(new GameTestStep(game)))
            {
                if (steps >= maxSteps)
                {
                    return true;
                }
                steps++;
                game.Update();
                
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();
            }

            return false;
        }

        public static Func<GameTestStep, bool> Reaches(this CharacterEntity character, HexCubeCoord position)
        {
            return gts => character.Position == position;
        }
    }

    public class GameTestStep
    {
        public GameEntity Game { get; }

        public GameTestStep(GameEntity game)
        {
            Game = game;
        }
    }
}