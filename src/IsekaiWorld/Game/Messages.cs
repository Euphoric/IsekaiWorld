namespace IsekaiWorld.Game;

public record SpawnBuilding(HexCubeCoord Position, HexagonDirection Rotation, BuildingDefinition Building) : IEntityMessage;

public record Designate(HexCubeCoord Position, DesignationDefinition Designation) : IEntityMessage;

public record SpawnItem(HexCubeCoord Position, ItemDefinition Definition, int Count) : IEntityMessage;

public record StartConstruction(HexCubeCoord Position, HexagonDirection Rotation, ConstructionDefinition Definition) : IEntityMessage;

record SetSpeed(int Speed) : IEntityMessage;

record TogglePause : IEntityMessage;

public record AddCraftingBill(CraftingDefinition CraftingDefinition) : IEntityMessage;