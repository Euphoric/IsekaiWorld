public class ConstructionJob
{
    private readonly ConstructionEntity _construction;

    public bool InProgress { get; private set; }
    
    public ConstructionJob(ConstructionEntity construction)
    {
        _construction = construction;
    }

    public void StartWorking(CharacterEntity character)
    {
        InProgress = true;
        character.Construct(_construction);
    }
}