public class DesignationDefinition
{
    public DesignationDefinition(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public static class DesignationDefinitions
{
    public static DesignationDefinition CutWood { get; } = new("CutWood");
    public static DesignationDefinition Deconstruct { get; } = new("Deconstruct");
}