using System;

[Obsolete("Use messaging to update UI instead of node operations.")]
public interface INodeOperation
{
    void Execute(GameNode gameNode);
}