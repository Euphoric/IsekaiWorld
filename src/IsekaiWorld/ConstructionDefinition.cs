public class ConstructionDefinition
{
    public string Id { get; }
    public string Label { get; }
    public string PlaceBuildingId { get; }
    public string PlaceFloorId { get; }
    public int WorkRequired { get; }

    public ConstructionDefinition(string id, string label, string placeBuildingId, string placeFloorId, int workRequired)
    {
        Id = id;
        Label = label;
        PlaceBuildingId = placeBuildingId;
        PlaceFloorId = placeFloorId;
        WorkRequired = workRequired;
    }
}