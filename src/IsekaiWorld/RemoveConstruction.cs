public class RemoveConstruction : INodeOperation
{
    private readonly ConstructionEntity _construction;

    public RemoveConstruction(ConstructionEntity construction)
    {
        _construction = construction;
    }

    public void Execute(GameNode gameNode)
    {
        gameNode.RemoveNodeFor(_construction);
    }
}