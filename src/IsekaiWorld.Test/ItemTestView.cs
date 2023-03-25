namespace IsekaiWorld;

public class ItemTestView
{
    public string Id { get; }

    public ItemTestView(string id)
    {
        Id = id;
    }

    public ItemDefinition Definition { get; private set; } = null!;
    public HexCubeCoord Position { get; private set; }
    public int Count { get; private set; }

    public void UpdateFrom(ItemUpdated evnt)
    {
        Definition = evnt.Definition;
        Position = evnt.Position;
        Count = evnt.Count;
    }
}