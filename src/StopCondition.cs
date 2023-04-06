
namespace meta;

public class StopConditionComposite
{ // TODO

}

public class StopConditionIteration : StopCondition
{
    protected int _count;
    protected int _max;

    public StopConditionIteration(int max) { _max = max; }

    public int GetNumIterations() { return _count; }

    public void Start() { _count = 0; }

    public bool IsRunning() { return _count < _max; }
    public void RegisterIteration() { _count++; }
}
