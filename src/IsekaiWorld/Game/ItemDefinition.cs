namespace IsekaiWorld.Game;

public class ItemDefinition
{
    public string Id { get; }
    public string Label { get; }
    public string TextureResource { get; }

    public ItemDefinition(string id, string label, string textureResource)
    {
        Id = id;
        Label = label;
        TextureResource = textureResource;
    }

    public override string ToString()
    {
        return $"[{Label}]";
    }
}