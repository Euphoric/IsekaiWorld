namespace IsekaiWorld;

public class ConstructionDefinition
{
    public string Id { get; }
    public string Label { get; }
    public BuildingDefinition? PlaceBuilding { get; }
    public SurfaceDefinition? PlaceFloor { get; }
    public float ConstructionTime { get; }
    public ItemDefinition? Material { get; }

    public ConstructionDefinition(string id, string label, BuildingDefinition? placeBuilding, SurfaceDefinition? placeFloor, float constructionTime, ItemDefinition? material = null)
    {
        Id = id;
        Label = label;
        PlaceBuilding = placeBuilding;
        PlaceFloor = placeFloor;
        ConstructionTime = constructionTime;
        Material = material;
    }
}