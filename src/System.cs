
namespace meta {

    public class System {
        private HashSet <object> _components;

        public System(){
            _components = new HashSet <object>();
        }

        public T? GetComponent<T>(){
            return _components.OfType<T>().First();
        }
    }

    // representation and evaluation results
    public class Solve : System, IComparable<Solve> {

        internal IComparable<Solve> _costFunction;

        internal Solve(IComparable<Solve> costFunction){
            _costFunction = costFunction;
        }

        public int CompareTo(Solve? other)
        {
            return _costFunction.CompareTo(other);
        }
    }

    // problem formulation and evaluation function
    public class Problem : System {

        protected IComparable<Solve> _costFunction;
        internal StopCondition? _stopCondition;

        public Problem(IComparable<Solve> costFunction){
            _costFunction = costFunction;
        }

        public void Evaluate(Solve solve){
            DoEvaluate(solve);
            _stopCondition?.RegisterEvaluation(solve);
        }

        public Solve CreateSolve(){
            var solve = new Solve(_costFunction);
            DoConfigureSolve(solve);
            return solve;
        }

        public virtual void Start(){}
        public virtual void Finish(){}
        protected virtual void DoEvaluate (Solve solve){}
        protected virtual void DoConfigureSolve(Solve solve){}
    }

    // metaheuristic stop condition
    public class StopCondition {
        public void Start(){}
        public void Finish(){}
        public bool IsRunning(){return true;}

        public void RegisterEvaluation(Solve solve){}
        public void RegisterIteration(){}
    };

    // metaheuristic algorithm
    public class Optimizer : System {

        protected Problem _problem;
        protected StopCondition _stopCondition;

        public Optimizer(Problem problem, StopCondition stopCondition){
            _problem = problem;
            _stopCondition = stopCondition;
        }

        public void Run(){
            _problem.Start();
            _stopCondition.Start();
            _problem._stopCondition = _stopCondition;
            while(_stopCondition.IsRunning()){
                DoIteration();
                _stopCondition.RegisterIteration();
            }
            _stopCondition.Finish();
            _problem.Finish();
        }

        protected virtual void DoIteration(){

        }
    }
}
