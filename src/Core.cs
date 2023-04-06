
namespace meta;

public class System
{
    private HashSet<object> _components;

    public System()
    {
        _components = new HashSet<object>();
    }

    public T GetComponent<T>()
    {
        var comp = _components.OfType<T>().First();
        if (comp == null)
            throw new ArgumentNullException("Missing the component " + typeof(T).Name);
        return comp;
    }
}

// representation and evaluation results
public class Solve : System, IComparable<Solve>
{
    public static readonly Solve DEFAULT = new Solve(Problem.DEFAULT);
    private Problem _problem;

    internal Solve(Problem problem)
    {
        _problem = problem;
    }

    public int CompareTo(Solve? other)
    {
        return other != null ? _problem._costFunction(this, other) : -1;
    }

    public void Evaluate()
    {
        _problem.Evaluate(this);
    }

    public Solve Clone(){
        return new Solve(_problem);
    }
}

// problem formulation and evaluation function
public class Problem : System
{
    public static readonly Problem DEFAULT = new Problem((a, b) => 0);

    protected internal Comparison<Solve> _costFunction;
    internal Optimizer? _optimizer;

    public Problem(Comparison<Solve> costFunction)
    {
        _costFunction = costFunction;
    }

    public Solve CreateSolve()
    {
        var solve = new Solve(this);
        DoConfigureSolve(solve);
        return solve;
    }

    public virtual void Start() { }
    public virtual void Finish() { }
    public virtual void Evaluate(Solve solve){}
    protected virtual void DoConfigureSolve(Solve solve) { }
}

// metaheuristic stop condition
public interface StopCondition
{
    public void Start() { }
    public void Stop() { }
    public bool IsRunning() { return true; }

    public void RegisterEvaluation(Solve solve) { }
    public void RegisterIteration() { }
}

// metaheuristic algorithm
public class Optimizer : System
{

    protected Problem _problem;
    protected StopCondition _stopCondition;
    protected List<Solve> _bestSolves;

    public Optimizer(Problem problem, StopCondition stopCondition)
    {
        _problem = problem;
        _stopCondition = stopCondition;
        _bestSolves = new List<Solve>();
    }

    public void Run()
    {
        Configure();
        _problem._optimizer = this;
        _problem.Start();
        _stopCondition.Start();
        Start();
        while (_stopCondition.IsRunning())
        {
            Iteration();
            _stopCondition.RegisterIteration();
        }
        _stopCondition.Stop();
        _problem.Finish();
    }

    protected virtual void Configure(){}

    protected virtual void Start() { }

    protected virtual void Iteration() { }

    protected internal void RegisterEvaluation(Solve solve)
    {
        _stopCondition.RegisterEvaluation(solve);
        if (_bestSolves.Count == 0)
        {
            _bestSolves.Add(solve);
        }
        else
        {
            var best = _bestSolves[0];
            var cmp = best.CompareTo(solve);
            if (cmp < 0)
            {
                _bestSolves.Add(solve);
            }
            else if (cmp < 0)
            {
                _bestSolves.Clear();
                _bestSolves.Add(solve);
            }
        }
    }
}
