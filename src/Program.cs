var csvPath = "spam.csv";

var lines = File.ReadAllLines(csvPath);
int linha = 1;

foreach (var line in lines)
{
    var splitedLine = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
    var flag = splitedLine[0];
    var message = splitedLine[1];
    Console.WriteLine(flag + " " + message);
    linha++;
}