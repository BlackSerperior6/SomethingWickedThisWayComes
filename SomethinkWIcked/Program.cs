using ScottPlot;

Dictionary<double, double> OriginalGraph = new Dictionary<double, double>();

var randFunction = new Random(12);

for (int i = 0; i <= 30; i++)
    OriginalGraph.Add(i, randFunction.Next(5, 15));

Dictionary<double, double> TheoreticalGraph = new Dictionary<double, double>();

for (int i = 8; i <= 24; i++)
{
    if (i % 2 != 0)
        TheoreticalGraph.Add(i, OriginalGraph[i]);
}

List<double> columnKeys = TheoreticalGraph.Keys.ToList();
List<double> theoreticalHeights = TheoreticalGraph.Values.ToList();
double totalTheoreticalHeight = theoreticalHeights.Sum();

List<double> theoreticalProbabilities = new List<double>();
List<double> cumulativeProbabilities = new List<double>();
double cumulative = 0;

for (int i = 0; i < theoreticalHeights.Count; i++)
{
    double prob = theoreticalHeights[i] / totalTheoreticalHeight;
    theoreticalProbabilities.Add(prob);
    cumulative += prob;
    cumulativeProbabilities.Add(cumulative);
}

cumulativeProbabilities[cumulativeProbabilities.Count - 1] = 1.0;

Dictionary<double, int> experimentalCounts = new Dictionary<double, int>();

for (int i = 0; i < columnKeys.Count; i++)
    experimentalCounts[columnKeys[i]] = 0;

Random random = new Random(2);
List<double> mediums = new List<double>();
double sum = 0;
int totalSamples = 1000;

for (int i = 0; i < totalSamples; i++)
{
    double randomNumber = random.NextDouble();
    sum += randomNumber;
    mediums.Add(sum / (i + 1));

    for (int j = 0; j < cumulativeProbabilities.Count; j++)
    {
        if (randomNumber <= cumulativeProbabilities[j])
        {
            experimentalCounts[columnKeys[j]]++;
            break;
        }
    }
}

Dictionary<double, double> experimentalProbabilities = new Dictionary<double, double>();

for (int i = 0; i < columnKeys.Count; i++)
    experimentalProbabilities[columnKeys[i]] = experimentalCounts[columnKeys[i]] / (double)totalSamples;

Dictionary<double, double> AdjustedGraph = new Dictionary<double, double>();

for (int i = 0; i < columnKeys.Count; i++)
{
    double key = columnKeys[i];
    double theoreticalProb = theoreticalProbabilities[i];
    double experimentalProb = experimentalProbabilities[key];

    if (experimentalProb > 0)
        AdjustedGraph[key] = theoreticalHeights[i] * (theoreticalProb / experimentalProb);
    else
        AdjustedGraph[key] = 0;
}

var plt = new Plot();

var originalSignal = plt.Add.SignalXY(OriginalGraph.Keys.ToArray(), OriginalGraph.Values.ToArray());
originalSignal.Color = Colors.Grey.WithAlpha(0.7);
originalSignal.LineWidth = 1;
originalSignal.LegendText = "Основной граф";

var theoreticalBars = plt.Add.Bars(values: TheoreticalGraph.Values, positions: TheoreticalGraph.Keys);

foreach (var bar in theoreticalBars.Bars)
{
    bar.Size = 2d;
    bar.FillColor = Colors.Blue.WithAlpha(1);
}

var adjustedBars = plt.Add.Bars(values: AdjustedGraph.Values, positions: AdjustedGraph.Keys);
foreach (var bar in adjustedBars.Bars)
{
    bar.Size = 2d;
    bar.FillColor = Colors.Red.WithAlpha(1);
}

var theoreticalScatter = plt.Add.SignalXY(TheoreticalGraph.Keys.ToArray(), TheoreticalGraph.Values.ToArray());
theoreticalScatter.Color = Colors.Blue;
theoreticalScatter.LineWidth = 2;
theoreticalScatter.LegendText = "Теоретический граф";

var adjustedScatter = plt.Add.SignalXY(AdjustedGraph.Keys.ToArray(), AdjustedGraph.Values.ToArray());
adjustedScatter.Color = Colors.Red;
adjustedScatter.LineWidth = 2;
adjustedScatter.LegendText = "Экспериментальный граф";

plt.Axes.SetLimitsX(8, 24);
plt.XLabel("Величина X");
plt.YLabel("Высота");
plt.Legend.IsVisible = true;
plt.SaveSvg("Test.svg", 800, 1000);


var newPlt = new Plot();
double[] x = Enumerable.Range(0, mediums.Count).Select(i => (double)i).ToArray();
double[] y = mediums.ToArray();

newPlt.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(0.05);

newPlt.Add.Scatter(x, y);
newPlt.Title("Сходимость");
newPlt.YLabel("Среднее");
newPlt.XLabel("Число итераций");
newPlt.Add.HorizontalLine(0.45, color: Colors.Red);
newPlt.Add.HorizontalLine(0.55, color: Colors.Red);
newPlt.SaveSvg("Rng.svg", 800, 1000);

Console.WriteLine("Итоги");
double totalError = 0;
for (int i = 0; i < columnKeys.Count; i++)
{
    double error = Math.Abs(theoreticalHeights[i] - AdjustedGraph[columnKeys[i]]);
    totalError += error;
    Console.WriteLine($"Ключ {columnKeys[i]}: Ошибка = {error:F2}");
}
Console.WriteLine($"Суммарная ошибка: {totalError:F2}");