Dictionary<double, double> OriginalGraph = new Dictionary<double, double>();

for (int i = 0; i <= 30; i++)
    OriginalGraph.Add(i, function(i));

Dictionary<double, double> Columns = new Dictionary<double, double>();

for (int i = 8; i <= 24; i++)
    Columns.Add(i, OriginalGraph[i]);

Dictionary<double, List<double>> Probabilitis = new Dictionary<double, List<double>>();

foreach (double height in Columns.Values)
{
    var probability = height / Columns.Values.Sum();
    Probabilitis[probability] = new List<double>();
}

Random random = new Random(7);

Dictionary<double, double> Vs = new Dictionary<double, double>();
List<double> mediums = new List<double>();
double sum = 0;

for (int i = 0; i < 1000; i++)
{
    var randomNumber = random.NextDouble();
    sum += randomNumber;

    var targetRange = Probabilitis.First(kvp => randomNumber <= kvp.Key);
    var targetProbability = targetRange.Key;
    var targetList = targetRange.Value;

    targetList.Add(randomNumber);

    mediums.Add(randomNumber / sum);

    if (!Vs.ContainsKey(targetProbability))
        Vs[targetProbability] = 1;
    else
        Vs[targetProbability]++;
}

double vSum = Vs.Values.Sum();

foreach(var pair in new(Vs))
{
    var trueV = pair.Value / vSum;
    
    Vs[pair.Key] = true;
}

Dictionary<double, double> NewGraph = new Dictionary<double, double>();

for (int i = 8; i <= 24; i++)
{
    var height = OriginalGraph[i] * Vs[i] / Probabilitis[i];
    NewGraph[i] = height;
}

double function(double x) => Math.Abs(Math.Sin(x) * Math.Exp(Math.Cos(x))) * Math.Pow(Math.Tan(x * x -3), -1) 
* Math.Sin(5 * x) * Math.Sign(x - Math.PI);
