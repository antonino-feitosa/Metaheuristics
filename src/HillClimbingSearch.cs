
namespace meta;

public interface StartSolveComponent
{
    public Solve CreateSolve();
}

public interface LocalSearchComponent
{

    public List<int> GetMoves();

    public void ApplyMove(int move, Solve solve);

    public void UndoMove(int move, Solve solve);
}

public class HillClimbingSearch : Optimizer
{
    protected LocalSearchComponent _localSearch = new LocalSearchComponentAdapter();
    protected StartSolveComponent _startSolve = new StartSolveComponentAdapter();
    protected Solve current = Solve.DEFAULT;

    public HillClimbingSearch(Problem problem, StopCondition stopCondition) : base(problem, stopCondition) { }

    protected override void Configure()
    {
        _localSearch = GetComponent<LocalSearchComponent>();
        _startSolve = GetComponent<StartSolveComponent>();
    }

    protected override void Start()
    {
        current = _startSolve.CreateSolve();
        current.Evaluate();
    }

    protected override void Iteration()
    {
        int best = -1;
        Solve n = current.Clone();
        foreach (int index in _localSearch.GetMoves())
        {
            _localSearch.ApplyMove(index, n);
            n.Evaluate();
            if (n.CompareTo(current) < 0)
            {
                best = index;
            }
            _localSearch.UndoMove(index, n);
        }
        if (best != -1)
        {
            _localSearch.ApplyMove(best, current);
            RegisterEvaluation(current);
        }
        else
        {
            _stopCondition.Stop();
        }
    }
}


public class StartSolveComponentAdapter : StartSolveComponent
{
    public Solve CreateSolve()
    {
        return new Solve(new Problem((a, b) => 0));
    }
}

public class LocalSearchComponentAdapter : LocalSearchComponent
{
    public void ApplyMove(int move, Solve solve) { }

    public List<int> GetMoves() { return new List<int>(); }

    public void UndoMove(int move, Solve solve) { }
}
