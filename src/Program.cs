var NormalWords = new Dictionary<string, WordInfo>();
var SpamlWords = new Dictionary<string, WordInfo>();

int totalSpanWords = 0;
int totalNormalWords = 0;

var csvPath = "spam.csv";

var lines = File.ReadAllLines(csvPath);
int maxLine = (int)Math.Floor(lines.Length * 0.75);
int validationLineAfter = maxLine + 1;

TraningModel(lines, maxLine);

ValidateModel(lines, validationLineAfter);

void ValidateModel(string[] lines, int validationLineAfter)
{
    Console.WriteLine();
    for (int i = validationLineAfter; i < lines.Length; i++)
    {
        var splitedLine = lines[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
        var flag = splitedLine[0];
        var message = splitedLine[1].ToLower();

        message = RemovePonctuations(message);
        message = RemoveStopWords(message);

        if (string.IsNullOrEmpty(message))
            continue;

        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var position = 0;
        var firstWord = words[position];

        while (NormalWords.ContainsKey(firstWord) == false && position < words.Length)
            firstWord = words[position++];

        if (position == words.Length)
            continue;

        var countOfFirstWordInNormal = (double)NormalWords[firstWord].Quantity;
        var countOfFirstWordInSpam = (double)SpamlWords[firstWord].Quantity;

        double probabilitieOfFirstWordBeNormal = countOfFirstWordInNormal / (countOfFirstWordInNormal + countOfFirstWordInSpam);

        double probToBeNormal = probabilitieOfFirstWordBeNormal;

        foreach (var word in words)
        {
            if (NormalWords.TryGetValue(word, out var value))
                probToBeNormal *= value.Probability;
        }

        double probabilitieOfFirstWordBeSpam = countOfFirstWordInSpam / (countOfFirstWordInSpam + countOfFirstWordInNormal);

        double probToBeSpam = probabilitieOfFirstWordBeSpam;
        foreach (var word in words)
        {
            if (SpamlWords.TryGetValue(word, out var value))
                probToBeSpam *= value.Probability;
        }

        string result = string.Empty;
        if (probToBeNormal > probToBeSpam)
        {
            result = "ham";
        }
        else
        {
            result = "spam";
        }

        Console.WriteLine($"Expected: {flag} -> Result: {result}");
    }
}

void TraningModel(string[] lines, int maxLine)
{
    foreach (var line in lines)
    {
        var splitedLine = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var flag = splitedLine[0];
        var message = splitedLine[1].ToLower();

        message = RemovePonctuations(message);
        message = RemoveStopWords(message);
        AddWordsToDictionaries(message, flag);

        Console.WriteLine(flag + " " + message);



        maxLine--;
        if (maxLine <= 0)
            break;
    }

    CalculateProbabilites();
}

void CalculateProbabilites()
{
    Console.WriteLine("Calculating probabilities");
    foreach (var spamWord in SpamlWords)
    {
        Console.Write(".");
        spamWord.Value.Probability = (double)spamWord.Value.Quantity / (double)totalNormalWords;
    }

    foreach (var normalWord in NormalWords)
    {
        Console.Write(".");
        normalWord.Value.Probability = (double)normalWord.Value.Quantity / (double)totalNormalWords;
    }
}

void AddWordsToDictionaries(string message, string flag)
{
    flag = flag.ToLower().Trim();
    if (flag == "spam")
    {
        var splitedWords = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in splitedWords)
        {
            totalSpanWords++;
            if (NormalWords.ContainsKey(word) == false)
            {
                totalNormalWords++;
                NormalWords.Add(word, new WordInfo { Quantity = 1 });
            }

            if (SpamlWords.ContainsKey(word))
            {
                SpamlWords[word].Quantity += 1;
            }
            else
            {
                SpamlWords.Add(word, new WordInfo { Quantity = 1 });
            }
        }
    }

    if (flag == "ham")
    {
        var splitedWords = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in splitedWords)
        {
            totalNormalWords++;
            if (SpamlWords.ContainsKey(word) == false)
            {
                totalSpanWords++;
                SpamlWords.Add(word, new WordInfo { Quantity = 1 });
            }

            if (NormalWords.ContainsKey(word))
            {
                NormalWords[word].Quantity += 1;
            }
            else
            {
                NormalWords.Add(word, new WordInfo { Quantity = 1 });
            }
        }
    }
}

string RemoveStopWords(string message)
{
    var splitedWords = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    var nonStopWords = splitedWords.Except(StopWords.ENG).ToArray();
    return string.Join(" ", nonStopWords);
}

string RemovePonctuations(string message)
{
    return new string(message.Where(_ => char.IsPunctuation(_) == false).ToArray());
}

public class WordInfo
{
    public required int Quantity { get; set; }
    public double Probability { get; set; }
}