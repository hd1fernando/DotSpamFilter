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