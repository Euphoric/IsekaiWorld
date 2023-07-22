using System.Collections.Generic;
using Godot;

namespace IsekaiWorld.Game;

public class BuildingDefinition
{
    public string Id { get; }
    public string Label { get; }
    public Color Color { get; }
    public bool EdgeConnected { get; }
    public IReadOnlyDictionary<HexagonDirection, string> TextureResource { get; }
    public bool Impassable { get; }
    public ItemDefinition? GatherDrop { get; }
    public IReadOnlySet<DesignationDefinition> AllowedDesignations { get; }

    public BuildingDefinition(
        string id,
        string label,
        Color color,
        Dictionary<HexagonDirection, string> textureResource,
        bool edgeConnected = false,
        bool impassable = false,
        IReadOnlySet<DesignationDefinition>? allowedDesignations = null,
        ItemDefinition? gatherDrop = null)
    {
        Id = id;
        Label = label;
        Color = color;
        TextureResource = textureResource;
        EdgeConnected = edgeConnected;
        Impassable = impassable;
        GatherDrop = gatherDrop;
        AllowedDesignations = allowedDesignations ?? new HashSet<DesignationDefinition>();
    }

    public override string ToString()
    {
        return $"[{Id}]";
    }
}