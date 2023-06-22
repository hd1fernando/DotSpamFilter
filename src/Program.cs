var NormalWords = new Dictionary<string, WordInfo>();
var SpamlWords = new Dictionary<string, WordInfo>();

int totalSpanWords = 0;
int totalNormalWords = 0;

var csvPath = "spam.csv";

var lines = File.ReadAllLines(csvPath);
int maxLine = (int)Math.Floor(lines.Length * 0.75);
int validationLineAfter = maxLine + 1;

foreach (var line in lines)
{
    var splitedLine = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
    var flag = splitedLine[0];
    var message = splitedLine[1];

    message = RemovePonctuations(message);
    message = RemoveStopWords(message);
    AddWordsToDictionaries(message, flag);

    Console.WriteLine(flag + " " + message);

    maxLine--;
    if (maxLine <= 0)
        break;
}

void AddWordsToDictionaries(string message, string flag)
{
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