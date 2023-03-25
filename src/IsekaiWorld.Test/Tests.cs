using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace IsekaiWorld
{
    public class Tests
    {
        private static GameTestInstance CreateGame()
        {
            return new GameTestInstance();
        }

        [Fact(Skip = "Fix updating impassable from terrain")]
        public void Character_stuck_in_impassable_terrain_issue_verification()
        {
            //var game = CreateGame();

            // var character = game.AddCharacter("Test guy");
            // character.Position = HexCubeCoord.Zero;
            // game.GameMap.SetCellSurface(character.Position, SurfaceDefinitions.Empty);
            //
            // game.Update(); // send msg
            // game.Update(); // receive msg
            //
            // var issues = game.CheckForIssues().ToList();
            // var characterStuckIssue = issues.Any(s =>
            //     s == $"Character '{character.Label}' stuck on impassable surface on {character.Position}");
            // Assert.True(characterStuckIssue);
        }

        [Fact]
        public void Character_stuck_in_wall_issue_verification()
        {
            var game = CreateGame();
            
            var building = game.SpawnBuilding(HexCubeCoord.Zero, HexagonDirection.Left, BuildingDefinitions.StoneWall);

            var character = game.AddCharacter("Test guy", building.Position);

            game.Update(); // send msg
            game.Update(); // receive msg

            var issues = game.CheckForIssues().ToList();
            var characterStuckIssue = issues.Any(s => s == $"Character '{character.Label}' stuck on impassable surface on {character.Position}");
            Assert.True(characterStuckIssue);
        }

        [Fact]
        public void Construction_test()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.R == cell.Position.Q && cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 6)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            var constructionPositions = game.Constructions.Select(x => x.Position).ToHashSet();

            game.UpdateUntil(NoActiveConstructions);

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();

            buildingPositions.Should().BeEquivalentTo(constructionPositions);
        }

        [Fact(Skip = "Fix")]
        public void Construction_complex_test()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy 1", HexCubeCoord.Zero);
            game.AddCharacter("Test guy 2", HexCubeCoord.Zero);

            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 5)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            var constructionPositions = game.Constructions.Select(x => x.Position).ToHashSet();

            game.UpdateUntil(NoActiveConstructions);

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();
            buildingPositions.Should().BeEquivalentTo(constructionPositions);
        }

        private bool NoActiveConstructions(GameTestStep gts)
        {
            return !gts.Game.Constructions.Any();
        }

        [Fact]
        public void Item_spawned()
        {
            var game = CreateGame();

            var position = new HexCubeCoord(-1, -1, 2);
            game.SpawnItem(position, ItemDefinitions.Wood, 13);
            game.Update();
            
            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { Position = position, Definition = ItemDefinitions.Wood, Count = 13 });
        }
        
        [Fact]
        public void Items_hauling_test_simple()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);
            
            var stockpile=  game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);

            game.UpdateUntil(NoItemsOutsideStockpile);

            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { stockpile.Position, Definition = ItemDefinitions.Wood, Count = 1 });
        }

        private bool NoItemsOutsideStockpile(GameTestStep gts)
        {
            return !ItemsOutsideStockpiles(gts.Game).Any();
        }

        [Fact]
        public void Items_hauling_test_stacking()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.SpawnStockpile(new HexCubeCoord(0, 0, 0));
            game.SpawnStockpile(new HexCubeCoord(1, 0, -1));

            foreach (var mapCell in game.GameMap.Cells)
            {
                var zeroDistance = mapCell.Position.DistanceFrom(HexCubeCoord.Zero);
                var typeDivide = mapCell.Position.R >= 0;
                if (3 <= zeroDistance && zeroDistance <= 5)
                {
                    game.SpawnItem(mapCell.Position, typeDivide ? ItemDefinitions.Wood : ItemDefinitions.Grains, 1);
                }
            }
            game.Update();
            
            var totalItemCountStart = game.Items.GroupBy(x => x.Definition)
                .Select(grp => new { Definition = grp.Key, Count = grp.Sum(x => x.Count) }).ToList();

            game.UpdateUntil(NoItemsOutsideStockpile, maxSteps: 5000);

            var multipleItemsOnSamePositions =
                game.Items.GroupBy(it => it.Position)
                    .Where(grp => grp.Count() > 1)
                    .ToList();
            multipleItemsOnSamePositions.Should().BeEmpty();

            var totalItemCountEnd = game.Items.GroupBy(x => x.Definition)
                .Select(grp => new { Definition = grp.Key, Count = grp.Sum(x => x.Count) }).ToList();
            totalItemCountEnd.Should().BeEquivalentTo(totalItemCountStart);
        }

        [Fact]
        public void Items_hauling_add_new_stockpile()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.SpawnItem(new HexCubeCoord(3, 2, -5), ItemDefinitions.Wood, 1);

            // wait for game to stabilize (or wait for pawn to not have a valid job)
            for (int i = 0; i < 100; i++)
            {
                game.Update();
            }

            game.SpawnStockpile(new HexCubeCoord(1, 1, -2));

            game.UpdateUntil(NoItemsOutsideStockpile);
        }

        private static List<ItemEntity> ItemsOutsideStockpiles(GameEntity game)
        {
            var stockpilePositions =
                game.Buildings
                    .Where(b => b.Definition == BuildingDefinitions.StockpileZone)
                    .Select(x => x.Position).ToHashSet();

            return game.Items.Where(it => !stockpilePositions.Contains(it.Position)).ToList();
        }

        [Fact]
        public void Cut_trees()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var tree = game.SpawnBuilding(new HexCubeCoord(5, -3, -2), HexagonDirection.Left, BuildingDefinitions.TreeOak);

            game.Designate(tree.Position, DesignationDefinitions.CutWood);
            
            tree.Designation.Should().Be(DesignationDefinitions.CutWood);
            
            game.UpdateUntil(_ =>
            {
                var entitiesOn = game.EntitiesOn(tree.Position);
                var treesExist = entitiesOn.OfType<BuildingEntity>()
                    .Any(b => b.Definition == BuildingDefinitions.TreeOak);
                var woodSpawned = entitiesOn.OfType<ItemEntity>().Any(i => i.Definition == ItemDefinitions.Wood);
                return !treesExist && woodSpawned;
            });
        }

        [Fact(Skip = "TODO: Reimplement without touching character's activity directly")]
        public void Movement_blocked_by_construction_reroutes()
        {
            // var game = CreateGame();
            //
            // foreach (var cell in game.GameMap.Cells)
            // {
            //     if (cell.Position.DistanceFrom(HexCubeCoord.Zero) == 6)
            //     {
            //         game.SpawnBuilding(cell.Position, HexagonDirection.Left, BuildingDefinitions.StoneWall);
            //     }
            // }
            //
            // var charPos = HexCubeCoord.Zero + HexagonDirection.Left + HexagonDirection.Left;
            // var character = game.AddCharacter("Test guy", charPos);
            //
            // var target = HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right;
            //
            // character.StartActivity(new MovementActivity(game, game.Pathfinding, character, target, false));
            //
            // var checkpoint = HexCubeCoord.Zero + HexagonDirection.Left;
            //
            // game.UpdateUntil(character.Reaches(checkpoint));
            //
            // game.SpawnBuilding(HexCubeCoord.Zero, HexagonDirection.Left, BuildingDefinitions.StoneWall);
            //
            // game.UpdateUntil(character.Reaches(target));
        }

        [Fact]
        public void Construction_with_resources_test()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left, ItemDefinitions.Wood, 1);
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left + HexagonDirection.Left, ItemDefinitions.Wood, 1);
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left + HexagonDirection.Left + HexagonDirection.Left, ItemDefinitions.Wood, 1);
            
            game.UpdateUntil(NoActiveConstructions);

            var remainingItems = game.Items.Any();
            remainingItems.Should().BeFalse();
        }

        [Fact]
        public void Construction_new_resource_spawned()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);

            // wait for game to stabilize (or wait for pawn to not have a valid job)
            for (int i = 0; i < 100; i++)
            {
                game.Update();
            }

            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left, ItemDefinitions.Wood, 1);

            game.UpdateUntil(NoActiveConstructions);

            var remainingItems = game.Items.Any();
            remainingItems.Should().BeFalse();
        }
        
        [Fact]
        public void Multiple_constructions_from_single_item_stack()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right + HexagonDirection.Right, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left, ItemDefinitions.Wood, 3);

            game.UpdateUntil(NoActiveConstructions);

            var remainingItems = game.Items.Any();
            remainingItems.Should().BeFalse();
        }
        
        [Fact]
        public void Deconstruct_building()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var building = game.SpawnBuilding(new HexCubeCoord(1, 1, -2), HexagonDirection.Left, BuildingDefinitions.WoodenWall);
            
            game.Designate(building.Position, DesignationDefinitions.Deconstruct);
            
            building.Designation.Should().Be(DesignationDefinitions.Deconstruct);
            
            game.UpdateUntil(_=>character.ActivityName == "DeconstructActivity");
            
            game.UpdateUntil(gts => !gts.Game.EntitiesOn(building.Position).Any());
        }

        [Fact]
        public void Can_designate_only_specific_entities()
        {
            var game = CreateGame();

            var building = game.SpawnBuilding(new HexCubeCoord(1, 1, -2), HexagonDirection.Left, BuildingDefinitions.WoodenWall);
            var tree = game.SpawnBuilding(new HexCubeCoord(5, -3, -2), HexagonDirection.Left, BuildingDefinitions.TreeOak);

            game.Designate(building.Position, DesignationDefinitions.CutWood);
            
            building.Designation.Should().BeNull();

            game.Designate(tree.Position, DesignationDefinitions.Deconstruct);

            tree.Designation.Should().BeNull();
        }

        [Fact]
        public void Pause_stops_hauling()
        {
            var game = CreateGame();

            game.Paused = true;

            game.AddCharacter("Test guy", HexCubeCoord.Zero);
            
            var stockpile = game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);

            for (int i = 0; i < 100; i++)
            {
                game.Update();                
            }

            var itemInStockpile = game.Items.Any(x => x.Position == stockpile.Position);
            Assert.False(itemInStockpile);
            
            game.Paused = false;
            
            game.UpdateUntil(NoItemsOutsideStockpile);
        }
        
        [Fact]
        public void Pause_stops_construction()
        {
            var game = CreateGame();

            game.Paused = true;

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var construction = game.StartConstruction(new HexCubeCoord(1, 1, -2), HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall)!;
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);

            for (int i = 0; i < 100; i++)
            {
                game.Update();
            }

            Assert.Equal(0, construction.Progress);

            game.Paused = false;
            
            game.UpdateUntil(NoActiveConstructions);
        }

        [Fact]
        public void Character_hunger_decreases()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);
            
            game.UpdateUntil(_=>0.98 < character.Hunger && character.Hunger < 1.0, because: "Character's hunger starts at 1");

            game.UpdateUntil(_=>0.95 < character.Hunger && character.Hunger < 0.98, because: "Character's hunger must decrease");
        }
        
        [Fact]
        public void Character_hunger_decrease_stops_when_paused()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.UpdateUntil(_=>0.98 < character.Hunger && character.Hunger < 1.0, because: "Character's hunger starts at 1");
            
            game.Paused = true;

            var startHunger = character.Hunger;
            
            game.Update();
            
            Assert.Equal(startHunger, character.Hunger);
        }

        [Fact]
        public void Hungry_character_eats_food()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);
            game.SpawnItem(HexCubeCoord.Zero, ItemDefinitions.Grains, 1);

            character.SetHungerTo(0.31);
            game.UpdateUntil(_=>0.3 < character.Hunger && character.Hunger < 0.31, because: "Character should be close to hungry");

            game.UpdateUntil(_ => character.ActivityName == "EatActivity");
            
            game.UpdateUntil(_=>0.98 < character.Hunger && character.Hunger < 1.0, because: "Character was unable to eat");
            game.Items.Should().BeEmpty();
            
            game.UpdateUntil(_ => character.ActivityName == null);
        }
        
        [Fact]
        public void Hungry_character_waits_for_food_to_be_available()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            character.SetHungerTo(0.31);
            
            game.UpdateUntil(_=>0.27 < character.Hunger && character.Hunger < 0.28, because: "Character should be hungry without food.");
            character.ActivityName.Should().BeNull("Because there is no food to eat.");

            game.SpawnItem(HexCubeCoord.Zero, ItemDefinitions.Grains, 1);
            
            game.UpdateUntil(_ => character.ActivityName == "EatActivity");
            game.UpdateUntil(_=>0.98 < character.Hunger && character.Hunger < 1.0, because: "Character was unable to eat");
            game.UpdateUntil(_ => character.ActivityName == null);
            
            game.Items.Should().BeEmpty();
        }
        
        [Fact]
        public void Eating_consumes_only_one_piece()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);
            game.SpawnItem(HexCubeCoord.Zero, ItemDefinitions.Grains, 3);

            character.SetHungerTo(0.31);

            game.UpdateUntil(_ => character.ActivityName == "EatActivity");
            game.UpdateUntil(_ => character.ActivityName == null);

            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { Position = HexCubeCoord.Zero, Definition = ItemDefinitions.Grains, Count = 2 });
        }
    }
}