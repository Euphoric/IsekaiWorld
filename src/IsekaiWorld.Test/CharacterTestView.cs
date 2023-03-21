using System;

namespace IsekaiWorld.Test;

public class CharacterTestView
{
    public String Id { get; }
    public String Label { get; private set; } = null!;
    public HexCubeCoord Position { get; private set; }
    public string? ActivityName { get; private set; }

    public CharacterTestView(String id)
    {
        Id = id;
    }

    public void UpdateFrom(CharacterCreated characterCreated)
    {
        Label = characterCreated.Label;
    }
    
    public void UpdateFrom(CharacterUpdated characterUpdated)
    {
        Position = characterUpdated.Position;
        ActivityName = characterUpdated.ActivityName;
    }
}