public interface IActivity
{
    void Update(float delta);
    bool IsFinished { get; }
}