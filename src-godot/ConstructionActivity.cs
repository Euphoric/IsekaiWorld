public class ConstructionActivity : IActivity
{
    private readonly HexagonalMap _map;
    public CharaterEntity Character { get; }
    public ConstructionEntity Construction { get; }

    public bool IsFinished { get; private set; }
    
    public ConstructionActivity(HexagonalMap map, CharaterEntity character, ConstructionEntity construction)
    {
        _map = map;
        Character = character;
        Construction = construction;
    }

    public void Update(float delta)
    {
        if (IsFinished)
            return;
        
        Construction.Progress += delta;

        if (Construction.Progress > 3)
        {
            IsFinished = true;
            _map.RemoveConstruction(Construction);
        }
    }
}