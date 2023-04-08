using System.Collections.Generic;

public class DesignationDefinition
{
    public DesignationDefinition(string id, string texturePath, string title)
    {
        Id = id;
        TexturePath = texturePath;
        Title = title;
    }

    public string Id { get; }
    public string TexturePath { get; }
    public string Title { get; }

    public override string ToString()
    {
        return $"[{Id}]";
    }
}

public static class DesignationDefinitions
{
    public static DesignationDefinition CutWood { get; } = new("Core.Designation.CutWood", "Textures/Designation/CutPlant.png", "Cut wood");
    public static DesignationDefinition Deconstruct { get; } = new("Core.Designation.Deconstruct", "Textures/Designation/Deconstruct.png", "Deconstruct");

    public static IReadOnlyList<DesignationDefinition> All { get; } = new[]
    {
        CutWood, 
        Deconstruct
    };
}