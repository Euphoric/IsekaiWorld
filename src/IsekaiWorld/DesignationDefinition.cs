public class DesignationDefinition
{
    public DesignationDefinition(string id, string texturePath)
    {
        Id = id;
        TexturePath = texturePath;
    }

    public string Id { get; }
    public string TexturePath { get; }
}

public static class DesignationDefinitions
{
    public static DesignationDefinition CutWood { get; } = new("CutWood", "Textures/Designation/CutPlant.png");
    public static DesignationDefinition Deconstruct { get; } = new("Deconstruct", "Textures/Designation/Deconstruct.png");
}