public class RemoveConstruction : INodeOperation
{
    private readonly ConstructionEntity _construction;

    public RemoveConstruction(ConstructionEntity construction)
    {
        _construction = construction;
    }

    public void Execute(HexagonalMap map)
    {
        map.RemoveNodeFor(_construction);
    }
}