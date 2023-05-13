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
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);
            game.GameMap.SetCellSurface(character.Position, SurfaceDefinitions.Empty);

            game.Update(); // send msg
            game.Update(); // receive msg

            var issues = game.CheckForIssues().ToList();
            var characterStuckIssue = issues.Any(s =>
                s == $"Character '{character.Label}' stuck on impassable surface on {character.Position}");
            Assert.True(characterStuckIssue);
        }

        [Fact]
        public void Character_stuck_in_wall_issue_verification()
        {
            var game = CreateGame();

            var position = HexCubeCoord.Zero;
            game.SpawnBuilding(position, HexagonDirection.Left, BuildingDefinitions.StoneWall);
            var character = game.AddCharacter("Test guy", position);

            game.Update(); // send msg
            game.Update(); // receive msg

            var issues = game.CheckForIssues().ToList();
            var characterStuckIssue = issues.Any(s =>
                s == $"Character '{character.Label}' stuck on impassable surface on {position}");
            Assert.True(characterStuckIssue);
        }

        [Fact]
        public void Construction_test()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var position = new HexCubeCoord(-4, 3, 1);
            var construction =
                game.StartConstruction(position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);

            game.UpdateUntil(_ => character.ActivityName == "ConstructionActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(construction.Position));
            game.UpdateUntil(_ => character.IsIdle);

            game.Buildings
                .Select(x => new { x.Definition, x.Position })
                .Should().Contain(new { Definition = BuildingDefinitions.StoneWall, Position = position });
        }

        [Fact]
        public void Construction_test_many()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var constructions = new List<ConstructionTestView>();
            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.R == cell.Position.Q && cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 6)
                {
                    var construction = game.StartConstruction(cell.Position, HexagonDirection.Left,
                        ConstructionDefinitions.StoneWall);
                    constructions.Add(construction);
                }
            }

            game.UpdateUntil(NoActiveConstructions, maxSteps: 10000);

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();

            buildingPositions.Should().BeEquivalentTo(constructions.Select(x => x.Position));
        }

        [Fact]
        public void Construction_pawn_starts_at_same_position()
        {
            var game = CreateGame();

            var position = new HexCubeCoord(-4, 3, 1);

            var character = game.AddCharacter("Test guy", position);
            var construction =
                game.StartConstruction(position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);

            game.UpdateUntil(_ => character.ActivityName == "ConstructionActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(construction.Position));
            game.UpdateUntil(_ => character.IsIdle);

            game.Buildings
                .Select(x => new { x.Definition, x.Position })
                .Should().Contain(new { Definition = BuildingDefinitions.StoneWall, Position = position });
        }

        private bool NoActiveConstructions(GameTestStep gts)
        {
            return !gts.Game.Constructions.Any();
        }

        private bool NoUncutTrees(GameTestStep gts)
        {
            var trees = gts.Game.Buildings.Where(x => x.Definition == BuildingDefinitions.Plant.TreeOak);
            return !trees.Any();
        }

        private bool NoUngatheredPlants(GameTestStep gts)
        {
            var plants = gts.Game.Buildings.Where(x => x.Definition == BuildingDefinitions.Plant.WildRice);
            return !plants.Any();
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

            var stockpile = game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);

            game.UpdateUntil(NoItemsOutsideStockpile);
            game.UpdateUntil(NoCarriedItems);

            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { stockpile.Position, Definition = ItemDefinitions.Wood, Count = 1 });
        }

        [Fact]
        public void Items_hauling_stack()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var stockpile = game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 5);

            game.UpdateUntil(NoItemsOutsideStockpile);
            game.UpdateUntil(NoCarriedItems);

            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { stockpile.Position, Definition = ItemDefinitions.Wood, Count = 5 });
        }

        private bool NoItemsOutsideStockpile(GameTestStep gts)
        {
            return !ItemsOutsideStockpiles(gts.Game).Any();
        }

        private bool NoCarriedItems(GameTestStep gts)
        {
            return !gts.Game.Characters.SelectMany(x => x.CarriedItems).Any();
        }

        [Fact]
        public void Items_hauling_test_stacking()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero, disableHunger: true);

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

            var totalItemCountStart = game.Items.GroupBy(x => x.Definition)
                .Select(grp => new { Definition = grp.Key, Count = grp.Sum(x => x.Count) }).ToList();

            game.UpdateUntil(NoItemsOutsideStockpile, maxSteps: 10000);
            game.UpdateUntil(NoCarriedItems);

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
            game.UpdateUntil(NoCarriedItems);
        }

        private static List<ItemTestView> ItemsOutsideStockpiles(GameTestInstance game)
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

            var tree = game.SpawnBuilding(new HexCubeCoord(5, -3, -2), HexagonDirection.Left,
                BuildingDefinitions.Plant.TreeOak);
            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.Designate(tree.Position, DesignationDefinitions.CutWood);

            tree.Designation.Should().Be(DesignationDefinitions.CutWood);

            game.UpdateUntil(_ => character.ActivityName == "CutTreeActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(tree.Position));
            game.UpdateUntil(_ => character.IsIdle);

            game.Buildings.Should().NotContain(x => x.Position == tree.Position);
            game.Items.Should().Contain(x =>
                x.Position == tree.Position && x.Definition == ItemDefinitions.Wood && x.Count == 5);
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

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left,
                ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right,
                HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(
                HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right + HexagonDirection.Right,
                HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);

            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left, ItemDefinitions.Wood, 1);
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left + HexagonDirection.Left, ItemDefinitions.Wood, 1);
            game.SpawnItem(HexCubeCoord.Zero + HexagonDirection.Left + HexagonDirection.Left + HexagonDirection.Left,
                ItemDefinitions.Wood, 1);

            game.UpdateUntil(NoActiveConstructions);

            var remainingItems = game.Items.Any();
            remainingItems.Should().BeFalse();
        }

        [Fact]
        public void Construction_new_resource_spawned()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left,
                ConstructionDefinitions.TestWoodenWall);

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

            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right, HexagonDirection.Left,
                ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right,
                HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            game.StartConstruction(
                HexCubeCoord.Zero + HexagonDirection.Right + HexagonDirection.Right + HexagonDirection.Right,
                HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);

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
            var building = game.SpawnBuilding(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.WoodenWall);

            game.Update(); // TODO Remove

            game.Designate(building.Position, DesignationDefinitions.Deconstruct);

            building.Designation.Should().Be(DesignationDefinitions.Deconstruct);

            game.UpdateUntil(_ => character.ActivityName == "DeconstructActivity");

            game.UpdateUntil(gts => !gts.Game.EntitiesOn(building.Position).Any());
        }

        [Fact]
        public void Can_designate_only_specific_entities()
        {
            var game = CreateGame();

            var building = game.SpawnBuilding(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.WoodenWall);
            var tree = game.SpawnBuilding(new HexCubeCoord(5, -3, -2), HexagonDirection.Left,
                BuildingDefinitions.Plant.TreeOak);

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
            game.UpdateUntil(NoCarriedItems);
        }

        [Fact]
        public void Pause_stops_construction()
        {
            var game = CreateGame();

            game.Paused = true;

            game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var construction = game.StartConstruction(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                ConstructionDefinitions.TestWoodenWall);
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

            game.UpdateUntil(_ => 0.98 < character.Hunger && character.Hunger < 1.0,
                because: "Character's hunger starts at 1");

            game.UpdateUntil(_ => 0.95 < character.Hunger && character.Hunger < 0.98,
                because: "Character's hunger must decrease");
        }

        [Fact]
        public void Character_hunger_decrease_stops_when_paused()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.UpdateUntil(_ => 0.98 < character.Hunger && character.Hunger < 1.0,
                because: "Character's hunger starts at 1");

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
            game.SpawnItem(new HexCubeCoord(-3, -2, 5), ItemDefinitions.Grains, 1);

            character.SetHungerTo(0.31);
            game.UpdateUntil(_ => 0.3 < character.Hunger && character.Hunger < 0.31,
                because: "Character should be close to hungry");

            game.UpdateUntil(_ => character.ActivityName == "EatActivity");

            game.UpdateUntil(_ => 0.98 < character.Hunger && character.Hunger < 1.0,
                because: "Character was unable to eat");
            game.Items.Should().BeEmpty();

            game.UpdateUntil(_ => character.IsIdle);
        }

        [Fact]
        public void Hungry_character_waits_for_food_to_be_available()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            character.SetHungerTo(0.31);

            game.UpdateUntil(_ => 0.27 < character.Hunger && character.Hunger < 0.28,
                because: "Character should be hungry without food.");
            character.ActivityName.Should().Be("IdleActivity", "Because there is no food to eat.");

            game.SpawnItem(HexCubeCoord.Zero, ItemDefinitions.Grains, 1);

            game.UpdateUntil(_ => character.ActivityName == "EatActivity");
            game.UpdateUntil(_ => 0.98 < character.Hunger && character.Hunger < 1.0,
                because: "Character was unable to eat");
            game.UpdateUntil(_ => character.IsIdle);

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
            game.UpdateUntil(_ => character.IsIdle);

            game.Items.Select(x => new { x.Position, x.Definition, x.Count })
                .Should()
                .Contain(new { Position = HexCubeCoord.Zero, Definition = ItemDefinitions.Grains, Count = 2 });
        }

        [Fact]
        public void Gather_plant()
        {
            var game = CreateGame();

            var riceEntity = game.SpawnBuilding(new HexCubeCoord(-2, 2, 0), HexagonDirection.Left,
                BuildingDefinitions.Plant.WildRice);
            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.Designate(riceEntity.Position, DesignationDefinitions.Gather);

            game.UpdateUntil(_ => riceEntity.Designation == DesignationDefinitions.Gather);

            game.UpdateUntil(_ => character.ActivityName == "GatherActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(riceEntity.Position));
            game.UpdateUntil(_ => character.IsIdle);

            game.Buildings.Should().NotContain(x => x.Position == riceEntity.Position);
            game.Items.Should().Contain(x =>
                x.Position == riceEntity.Position && x.Definition == ItemDefinitions.Grains && x.Count == 1);
        }

        [Fact]
        public void Remove_plant()
        {
            var game = CreateGame();

            var plantEntity = game.SpawnBuilding(new HexCubeCoord(-2, 2, 0), HexagonDirection.Left,
                BuildingDefinitions.Plant.Grass);
            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            game.Designate(plantEntity.Position, DesignationDefinitions.Gather);

            game.UpdateUntil(_ => plantEntity.Designation == DesignationDefinitions.Gather);

            game.UpdateUntil(_ => character.ActivityName == "GatherActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(plantEntity.Position));
            game.UpdateUntil(_ => character.IsIdle);

            game.Buildings.Should().NotContain(x => x.Position == plantEntity.Position);
            game.Items.Should().BeEmpty();
        }

        [Fact]
        public void Cannot_start_construction_on_existing_building()
        {
            var game = CreateGame();

            var position = new HexCubeCoord(-4, 3, 1);
            game.SpawnBuilding(position, HexagonDirection.Left, BuildingDefinitions.StoneWall);
            game.TryStartConstruction(position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);

            game.Update();

            game.Constructions
                .Select(x => new { x.Definition, x.Position })
                .Should()
                .NotContain(new { Definition = ConstructionDefinitions.StoneWall, Position = position });
        }

        [Fact]
        public void Constructing_Floor()
        {
            var position = new HexCubeCoord(-4, 3, 1);

            var game = CreateGame();

            var originalSurface = game.Surface.Single(x => x.Position == position);
            originalSurface.Surface.Should().Be(SurfaceDefinitions.Dirt);

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var construction =
                game.StartConstruction(position, HexagonDirection.Left, ConstructionDefinitions.StoneTileFloor);

            game.UpdateUntil(_ => character.ActivityName == "ConstructionActivity");
            game.UpdateUntil(_ => character.Position.IsNextTo(construction.Position));
            game.UpdateUntil(_ => character.IsIdle);

            var surfaceCell = game.Surface.Single(x => x.Position == position);
            surfaceCell.Surface.Should().Be(SurfaceDefinitions.StoneTileFloor);
        }

        [Fact]
        public void Cannot_construct_floor_on_same_floor()
        {
            var position = new HexCubeCoord(-4, 3, 1);

            var game = CreateGame();

            game.SetFloor(position, SurfaceDefinitions.StoneTileFloor);

            game.TryStartConstruction(position, HexagonDirection.Left, ConstructionDefinitions.StoneTileFloor);

            game.Update();

            game.Constructions
                .Select(x => new { x.Definition, x.Position })
                .Should()
                .NotContain(new { Definition = ConstructionDefinitions.StoneTileFloor, Position = position });
        }

        [Fact]
        public void Cut_trees_with_multiple_characters()
        {
            var game = CreateGame();

            var treeA = game.SpawnBuilding(new HexCubeCoord(5, -3, -2), HexagonDirection.Left,
                BuildingDefinitions.Plant.TreeOak);
            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.Designate(treeA.Position, DesignationDefinitions.CutWood);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Gather_with_multiple_characters()
        {
            var game = CreateGame();

            var riceEntity = game.SpawnBuilding(new HexCubeCoord(-2, 2, 0), HexagonDirection.Left,
                BuildingDefinitions.Plant.WildRice);
            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.Designate(riceEntity.Position, DesignationDefinitions.Gather);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Construct_without_resources_with_multiple_characters()
        {
            var game = CreateGame();

            game.StartConstruction(new HexCubeCoord(-4, 3, 1), HexagonDirection.Left,
                ConstructionDefinitions.StoneWall);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Construct_with_resources_with_multiple_characters()
        {
            var game = CreateGame();

            game.StartConstruction(new HexCubeCoord(-4, 3, 1), HexagonDirection.Left,
                ConstructionDefinitions.TestWoodenWall);
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);
            game.SpawnItem(new HexCubeCoord(1, 1, -2), ItemDefinitions.Wood, 1);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Deconstruct_building_with_multiple_characters()
        {
            var game = CreateGame();

            var building = game.SpawnBuilding(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.WoodenWall);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.Designate(building.Position, DesignationDefinitions.Deconstruct);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Hauling_with_multiple_characters()
        {
            var game = CreateGame();

            game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood, 1);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();
        }

        [Fact]
        public void Hauling_to_stockpile_complex_multiple_charas()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            var itemsPositions = new List<HexCubeCoord>();
            HexCubeCoord.FillRectangleBetweenHexes(new HexOffsetCoord(-5, -5).ToCube(),
                new HexOffsetCoord(-2, -2).ToCube(), itemsPositions);
            foreach (var coord in itemsPositions)
            {
                game.SpawnItem(coord, ItemDefinitions.Wood, 3);
            }

            game.SpawnStockpile(new HexOffsetCoord(2, 2).ToCube());

            var totalItemCountStart = game.Items.GroupBy(x => x.Definition)
                .Select(grp => new { Definition = grp.Key, Count = grp.Sum(x => x.Count) }).ToList();

            game.UpdateUntil(NoItemsOutsideStockpile, maxSteps: 10000);
            game.UpdateUntil(NoCarriedItems);

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
        public void Complex_simulation()
        {
            var game = CreateGame();

            game.AddCharacter("Test guy A", HexCubeCoord.Zero, disableHunger: true);
            game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left, disableHunger: true);

            var treesList = new List<HexCubeCoord>();
            HexCubeCoord.FillRectangleBetweenHexes(new HexOffsetCoord(-5, -5).ToCube(),
                new HexOffsetCoord(-2, -2).ToCube(), treesList);
            foreach (var coord in treesList)
            {
                game.SpawnBuilding(coord, HexagonDirection.Left, BuildingDefinitions.Plant.TreeOak);
                game.Designate(coord, DesignationDefinitions.CutWood);
            }

            var plantsList = new List<HexCubeCoord>();
            HexCubeCoord.FillRectangleBetweenHexes(new HexOffsetCoord(2, 2).ToCube(), new HexOffsetCoord(5, 5).ToCube(),
                plantsList);
            foreach (var coord in plantsList)
            {
                game.SpawnBuilding(coord, HexagonDirection.Left, BuildingDefinitions.Plant.WildRice);
                game.Designate(coord, DesignationDefinitions.Gather);
            }

            var constructionsList = new List<HexCubeCoord>();
            HexCubeCoord.LineBetweenHexes(new HexOffsetCoord(-2, 2).ToCube(), new HexOffsetCoord(-5, 5).ToCube(),
                constructionsList);
            foreach (var coord in constructionsList)
            {
                game.StartConstruction(coord, HexagonDirection.Left, ConstructionDefinitions.TestWoodenWall);
            }

            var stockpilesList = new List<HexCubeCoord>();
            HexCubeCoord.FillRectangleBetweenHexes(new HexOffsetCoord(2, -2).ToCube(),
                new HexOffsetCoord(5, -5).ToCube(), stockpilesList);
            foreach (var coord in stockpilesList)
            {
                game.StartConstruction(coord, HexagonDirection.Left, ConstructionDefinitions.StockpileZone);
            }

            game.UpdateUntil(NoUncutTrees, maxSteps: 10000);
            game.UpdateUntil(NoUngatheredPlants);
            game.UpdateUntil(NoActiveConstructions);
            game.UpdateUntil(NoItemsOutsideStockpile, maxSteps: 10000);
            game.UpdateUntil(NoCarriedItems);

            var itemsInGame =
                game.Items
                    .GroupBy(x => x.Definition)
                    .ToDictionary(x => x.Key, x => x.Sum(c => c.Count));
            itemsInGame.Should().ContainKey(ItemDefinitions.Wood).WhoseValue.Should()
                .Be(treesList.Count * 5 - constructionsList.Count * 1);
            itemsInGame.Should().ContainKey(ItemDefinitions.Grains).WhoseValue.Should()
                .Be(plantsList.Count * 1);
        }

        [Fact]
        public void Hungry_characters_eats_food()
        {
            var game = CreateGame();

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            characterA.SetHungerTo(0.2);
            var characterB = game.AddCharacter("Test guy B", new HexCubeCoord(-1, 2, -1));
            characterB.SetHungerTo(0.2);

            game.SpawnItem(new HexCubeCoord(-3, -2, 5), ItemDefinitions.Grains, 1);

            game.UpdateUntil(_ => characterA.Hunger > 0.5 || characterB.Hunger > 0.5);
            game.UpdateUntil(AllCharactersAreInactive);

            game.Items.Should().BeEmpty();
        }

        private bool AllCharactersAreInactive(GameTestStep gts)
        {
            return gts.Game.Characters.All(ch => ch.IsIdle);
        }

        [Fact]
        public void Crafting_simple()
        {
            var game = CreateGame();

            game.SpawnBuilding(new HexCubeCoord(5, 3, -8), HexagonDirection.Left, BuildingDefinitions.CraftingDesk);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);

            game.AddCraftingBill(CraftingDefinitions.WoodenSpear);

            game.UpdateUntil(_ => characterA.ActivityName == "CraftingActivity");
            var startTick = game.Ticks;
            game.UpdateUntil(_ => characterA.ActivityName != "CraftingActivity");
            var endTicks = game.Ticks;
            game.UpdateUntil(_ => characterA.IsIdle);
            
            game.UpdateUntil(gts => gts.Game.Items.Any(i => i.Definition == ItemDefinitions.WoodenSpear));
            var craftedItem = game.Items.First(i => i.Definition == ItemDefinitions.WoodenSpear);
            craftedItem.Position.Should().Be(new HexCubeCoord(5, 3, -8));

            var elapsedTicks = (endTicks-startTick);
            elapsedTicks.Should().Be(GameSpeed.BaseTps * 3);
        }

        [Fact]
        public void Crafting_multiple_bills_in_sequence()
        {
            var game = CreateGame();

            game.SpawnBuilding(new HexCubeCoord(5, 3, -8), HexagonDirection.Left, BuildingDefinitions.CraftingDesk);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);

            game.AddCraftingBill(CraftingDefinitions.WoodenSpear);

            game.UpdateUntil(_ => characterA.ActivityName == "CraftingActivity");
            game.UpdateUntil(_ => characterA.IsIdle);
            var craftedItem =
                game.Items
                    .Where(x => x.Definition == ItemDefinitions.WoodenSpear)
                    .Should().ContainSingle().Subject;
            craftedItem.Position.Should().Be(new HexCubeCoord(5, 3, -8));

            game.AddCraftingBill(CraftingDefinitions.WoodenSpear);

            game.UpdateUntil(_ => characterA.ActivityName == "CraftingActivity");
            game.UpdateUntil(_ => characterA.IsIdle);

            var spearCount = game.Items.Where(x => x.Definition == ItemDefinitions.WoodenSpear).Sum(x => x.Count);
            spearCount.Should().Be(2);
        }

        [Fact]
        public void Crafting_multiple_characters()
        {
            var game = CreateGame();

            game.SpawnBuilding(new HexCubeCoord(5, 3, -8), HexagonDirection.Left, BuildingDefinitions.CraftingDesk);

            var characterA = game.AddCharacter("Test guy A", HexCubeCoord.Zero);
            var characterB = game.AddCharacter("Test guy B", HexCubeCoord.Zero + HexagonDirection.Left);

            game.AddCraftingBill(CraftingDefinitions.WoodenSpear);

            game.UpdateUntil(_ => characterA.IsActive || characterB.IsActive);
            var activeCharacters = new[] { characterA, characterB }.Where(x => x.IsActive);
            activeCharacters.Should().ContainSingle();

            game.UpdateUntil(_ => characterA.IsIdle && characterB.IsIdle);
        }

        [Fact]
        public void Spawning_item_on_stockpile_makes_it_used()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);

            var stockpile = game.SpawnStockpile(new HexCubeCoord(1, 1, -2));
            game.SpawnItem(stockpile.Position, ItemDefinitions.Grains, 1);
            var originalPosition = new HexCubeCoord(-1, -1, 2);
            var item = game.SpawnItem(originalPosition, ItemDefinitions.Wood, 1);

            game.Update();
            game.UpdateUntil(_ => character.IsIdle);

            item.Position.Should().Be(originalPosition);
        }
        
        [Fact]
        public void Adding_item_to_stack_planned_to_haul()
        {
            var game = CreateGame();

            var character = game.AddCharacter("Test guy", HexCubeCoord.Zero);
            var breakPosition = character.Position + HexagonDirection.Left;
            var itemPosition = breakPosition + HexagonDirection.Left;
            var stockpilePosition = character.Position + HexagonDirection.Right;
            
            game.SpawnStockpile(stockpilePosition);
            var item = game.SpawnItem(itemPosition, ItemDefinitions.Wood, 1);

            game.UpdateUntil(_ => character.Position == breakPosition);
            
            game.SpawnItem(itemPosition, ItemDefinitions.Wood, 1);
            
            game.UpdateUntil(_ => character.IsIdle);

            item.Position.Should().Be(stockpilePosition);
            item.Count.Should().Be(2);
        }
    }
}