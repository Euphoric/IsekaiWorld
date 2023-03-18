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
            private readonly Dictionary<string, HexCubeCoord?> _characterPositions = new();

            private readonly List<string> _logs = new();
            
            public void MessageHandler(IEntityMessage evnt)
            {
                if (evnt is CharacterUpdated cu)
                {
                    _characterPositions.TryGetValue(cu.EntityId, out var previousPosition);

                    if (previousPosition != cu.Position)
                    {
                        _logs.Add($"Character {cu.EntityId} : {cu.Position}");
                    }
                    
                    _characterPositions[cu.EntityId] = cu.Position;
                }else if (evnt is BuildingUpdated bu)
                {
                    _logs.Add($"Building {bu.EntityId}");
                }
                else
                {
                    _logs.Add($"Event: {evnt.GetType().Name}");
                }
            }   
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