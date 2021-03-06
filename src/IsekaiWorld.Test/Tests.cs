using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace IsekaiWorld.Test
{
    public class Tests
    {
        private static GameEntity CreateGame()
        {
            return new GameEntity();
        }

        [Fact(Skip = "Fix updating passability from terrain")]
        public void Character_stuck_in_impassalbe_terrain_issue_verification()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;
            game.GameMap.SetCellSurface(character.Position, SurfaceDefinitions.Empty);

            game.Update(); // send msg
            game.Update(); // receive msg
            
            var issues = game.CheckForIssues().ToList();
            var characterStuckIssue = issues.Any(s => s == $"Character '{character.Label}' stuck on impassable surface on {character.Position}");
            Assert.True(characterStuckIssue);
        }
        
        [Fact]
        public void Character_stuck_in_wall_issue_verification()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var position = HexCubeCoord.Zero;
            game.SpawnBuilding(position, HexagonDirection.Left, BuildingDefinitions.StoneWall);

            var character = game.AddCharacter("Test guy");
            character.Position = position;
            
            game.Update(); // send msg
            game.Update(); // receive msg
            
            var issues = game.CheckForIssues().ToList();
            var characterStuckIssue = issues.Any(s => s == $"Character '{character.Label}' stuck on impassable surface on {character.Position}");
            Assert.True(characterStuckIssue);
        }

        [Fact(Skip = "Fix movement when it's path is blocked.")]
        public void Construction_test()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.R == cell.Position.Q && cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 6)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            var constructionPositions = game.Constructions.Select(x => x.Position).ToHashSet();

            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !game.Constructions.Any();
            });

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();

            buildingPositions.Should().BeEquivalentTo(constructionPositions);
        }
        
        [Fact(Skip = "Fix")]
        public void Construction_complex_test()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character1 = game.AddCharacter("Test guy 1");
            character1.Position = HexCubeCoord.Zero;
            var character2 = game.AddCharacter("Test guy 2");
            character2.Position = HexCubeCoord.Zero;
            
            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 5)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            var constructionPositions = game.Constructions.Select(x => x.Position).ToHashSet();

            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !game.Constructions.Any();
            });

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();
            buildingPositions.Should().BeEquivalentTo(constructionPositions);
        }

        [Fact]
        public void Items_hauling_test_simple()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            var stockpile = new BuildingEntity(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.StockpileZone);
            game.AddEntity(stockpile);
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood);

            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !ItemsOutsideStockpiles(game).Any();
            });

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }

        [Fact]
        public void Items_hauling_test_stacking()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            game.AddEntity(new BuildingEntity(new HexCubeCoord(0, 0, 0), HexagonDirection.Left, BuildingDefinitions.StockpileZone));
            game.AddEntity(new BuildingEntity(new HexCubeCoord(1, 0, -1), HexagonDirection.Left, BuildingDefinitions.StockpileZone));
            
            foreach (var mapCell in game.GameMap.Cells)
            {
                var zeroDistance = mapCell.Position.DistanceFrom(HexCubeCoord.Zero);
                var typeDivide = mapCell.Position.R >= 0;
                if (3 <= zeroDistance && zeroDistance <= 5)
                {
                    game.SpawnItem(mapCell.Position, typeDivide ? ItemDefinitions.Wood : ItemDefinitions.Grains);
                }
            }

            var totalItemCountStart = game.Items.GroupBy(x=>x.Definition).Select(grp=> new {Definition = grp.Key, Count = grp.Sum(x => x.Count)}).ToList();
            
            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !ItemsOutsideStockpiles(game).Any();
            });

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
            
            var multipleItemsOnSamePositions =
                game.Items.GroupBy(it => it.Position)
                    .Where(grp => grp.Count() > 1)
                    .ToList();
            multipleItemsOnSamePositions.Should().BeEmpty();
            
            var totalItemCountEnd = game.Items.GroupBy(x=>x.Definition).Select(grp=> new {Definition = grp.Key, Count = grp.Sum(x => x.Count)}).ToList();
            totalItemCountEnd.Should().BeEquivalentTo(totalItemCountStart);
        }

        [Fact(Skip = "TODO: Implement hauling job when stockpile is added")]
        public void Items_hauling_add_new_stockpile()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;
            
            game.SpawnItem(new HexCubeCoord(3, 2, -5), ItemDefinitions.Wood);
            
            game.Update();
            
            var stockpile = new BuildingEntity(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.StockpileZone);
            game.AddEntity(stockpile);
            
            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !ItemsOutsideStockpiles(game).Any();
            });

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }

        private static List<ItemEntity> ItemsOutsideStockpiles(GameEntity game)
        {
            var stockpilePositions =
                game.Buildings
                    .Where(b => b.Definition == BuildingDefinitions.StockpileZone)
                    .Select(x => x.Position).ToHashSet();

            var itemsOutsideStockpiles = game.Items.Where(it => !stockpilePositions.Contains(it.Position)).ToList();
            return itemsOutsideStockpiles;
        }

        [Fact]
        public void Cut_trees()
        {
            var game = CreateGame();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            var treePosition = new HexCubeCoord(5, -3, -2);
            game.AddEntity(new BuildingEntity(treePosition, HexagonDirection.Left, BuildingDefinitions.TreeOak));
            
            game.DesignateCutWood(treePosition);
            
            bool timedOut = game.UpdateUntil(() =>
            {
                var entitiesOn = game.EntitiesOn(treePosition);
                var treesExist = entitiesOn.OfType<BuildingEntity>().Any(b=>b.Definition == BuildingDefinitions.TreeOak);
                var woodSpawned = entitiesOn.OfType<ItemEntity>().Any(i => i.Definition == ItemDefinitions.Wood);
                return !treesExist && woodSpawned;
            }, 300);

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }
    }
}