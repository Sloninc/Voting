using Faker;
using System.Text;
using System.Text.RegularExpressions;
namespace Voting
{
    public class Program
    {
        static bool TryParseInputFile(string path, out Dictionary<int, string>[] blocksCandidates, out int[][,] vote)
        {
            string[] lines = File.ReadAllLines(path);
            List<string[]> candidatesPerBlock = new List<string[]>();
            List<int[,]> votesPerBlock = new List<int[,]>();
            int countBlocks = 0;
            while(lines.Length > 0)
            {
                int countLines = 0;
                int countCandidatesPerBlock = 0;
                var blockHeader = lines.Take(3).ToArray();
                var strHeader = string.Join("\n", blockHeader);
                if (Regex.IsMatch(strHeader, @"^\d+\n\n\d+$"))
                {
                    countBlocks++;
                    int blocks = int.Parse(blockHeader[0]);
                    if (blocks == countBlocks)
                    {
                        countCandidatesPerBlock = int.Parse(blockHeader[2]);
                        countLines += 3;
                        var blockCandidates = lines.Take(new Range(countLines, countLines + countCandidatesPerBlock)).ToArray();
                        string[] s = new string[blockCandidates.Length];
                        for (int i = 0; i < countCandidatesPerBlock; i++)
                        {
                            if (Regex.IsMatch(blockCandidates[i], @"^\d+\s?-\s?\w+"))
                            {
                                s[i] = blockCandidates[i];
                                countLines++;
                            }
                            else
                            {
                                if (Regex.IsMatch(blockCandidates[i], @"\d+\s\d+\s\d+$"))
                                    Console.WriteLine("Не соответствует указанное число кандидатов фактическому");
                                else
                                    Console.WriteLine("Неверный формат ввода кандидатов");
                                blocksCandidates = null;
                                vote = null;
                                return false;
                            }
                        }
                        candidatesPerBlock.Add(s);
                        int[,] z = null;
                        List<int[]> srs = new List<int[]>();
                        var blockVotes = lines.Take(new Range(countLines, lines.Length)).ToArray();
                        for (int i = 0; i < blockVotes.Length; i++)
                        {
                            if (blockVotes[i] == string.Empty)
                            {
                                z = new int[srs.Count, countCandidatesPerBlock];
                                var count = 0;
                                foreach (var val in srs)
                                {
                                    for (int j = 0; j < countCandidatesPerBlock; j++)
                                    {
                                        z[count, j] = val[j];
                                    }
                                    count++;
                                }
                                countLines++;
                                lines = lines.TakeLast(lines.Length - countLines).ToArray();
                                break;
                            }
                            if (Regex.IsMatch(blockVotes[i], @"\d+\s\d+\s\d+$"))
                            {
                                if (blockVotes[i] == lines.Last())
                                {
                                    Console.WriteLine("В конце каждого блока должна быть пустая строка");
                                    blocksCandidates = null;
                                    vote = null;
                                    return false;
                                }
                                var bv = blockVotes[i].Substring(0, blockVotes[i].Length);
                                var sv = bv.Split(' ').Select(x => int.Parse(x)).ToArray();
                                if (sv.Length == countCandidatesPerBlock)
                                {
                                    srs?.Add(sv);
                                    countLines++;
                                }
                                else
                                {
                                    Console.WriteLine("не соответствует количесто кандидатов и списка проголосовавшего");
                                    blocksCandidates = null;
                                    vote = null;
                                    return false;
                                }
                            }
                            else if (Regex.IsMatch(blockVotes[i], @"^\d+\s?-\s?\w+"))
                            {
                                Console.WriteLine("Не соответствует указанное число кандидатов фактическому");
                                blocksCandidates = null;
                                vote = null;
                                return false;
                            }
                            else if (Regex.IsMatch(blockVotes[i], @"^\d+$"))
                            {
                                Console.WriteLine("В конце каждого блока должна быть пустая строка");
                                blocksCandidates = null;
                                vote = null;
                                return false;
                            }
                        }
                        if (z != null)
                            votesPerBlock.Add(z);
                    }
                    else
                    {
                        Console.WriteLine("Неверно введен номер блока");
                        blocksCandidates = null;
                        vote = null;
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Неверный формат заголовка");
                    blocksCandidates = null;
                    vote = null;
                    return false;
                }
            }
            blocksCandidates = new Dictionary<int, string>[countBlocks];
            vote = votesPerBlock.ToArray();
            int countt = 0;
            foreach (var val in candidatesPerBlock)
            {
                blocksCandidates[countt] = new Dictionary<int, string>();
                for (int i = 0; i < val.Length; i++)
                {
                    string[] str = val[i].Split(" - ");
                    blocksCandidates[countt].Add(int.Parse(str[0]), str[1]);
                }
                countt++;
            }
            return true;
        }
        static void Main(string[] args)
        { 
            //InputDataGenerate();
            bool b = TryParseInputFile(Path.Combine("c:", "Spark") + "\\TestVote.txt", out Dictionary<int, string>[] blocksCandidates, out int[][,] vote);
            //Print(blocksCandidates, vote);
            if (b)
            {
                var result = CalculateVote(blocksCandidates, vote);
                foreach (var v in result)
                {
                    Console.WriteLine($"Победитель в блоке {v.Key} - {v.Value}");
                }
            }
            Console.ReadLine();
        }
       static void Print(Dictionary<int, string>[] blocksCandidates, int[][,] vote)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < blocksCandidates.Length; i++)
            {
                builder.Append($"{i + 1}\n\n");
                builder.Append($"{blocksCandidates[i].Count}\n");
                var values = blocksCandidates[i].ToList();
                foreach (var value in values)
                    builder.Append(value.Key + " - " + value.Value + "\n");
                for (int k = 0; k < vote[i].GetLength(0); k++)
                {
                    for (int j = 0; j < vote[i].GetLength(1); j++)
                    {
                        builder.Append($"{vote[i][k, j]} ");
                    }
                    builder.Append("\n");
                }
                builder.Append("\n");
            }
            Console.Write(builder.ToString());
        }
        static void InputDataGenerate()
        {
            var blocks = RandomNumber.Next(1, 10);
            Dictionary<int, string>[] blocksCandidates = new Dictionary<int, string>[blocks];
            int[][,] randomVote = new int[blocks][,];
            for (int i = 0; i < blocks; i++)
            {
                var candidatesCountPerBlock = RandomNumber.Next(3, 20);
                var voteCountPerBlock = RandomNumber.Next(1, 1000);
                var vote = new int[candidatesCountPerBlock];
                randomVote[i] = new int[voteCountPerBlock, candidatesCountPerBlock];
                blocksCandidates[i] = new Dictionary<int, string>(candidatesCountPerBlock);
                for (int j = 0; j < candidatesCountPerBlock; j++)
                {
                    blocksCandidates[i].Add(j+1, Name.FullName(NameFormats.Standard));
                    vote[j] = j+1;
                }
                for(int q = 0; q < voteCountPerBlock; q++)
                {
                    for (int k = vote.Length - 1; k >= 1; k--)
                    {
                        int v = RandomNumber.Next(0, k + 1);
                        int tmp = vote[v];
                        vote[v] = vote[k];
                        vote[k] = tmp;
                    }
                    for(int z=0; z < vote.Length; z++)
                    {
                        randomVote[i][q,z] = vote[z];
                    } 
                } 
            }
            FileInputDataCreate(blocksCandidates, randomVote);
        }
        static void FileInputDataCreate(Dictionary<int, string>[] blocksCandidates, int[][,] randomVote)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < blocksCandidates.Length; i++)
            {
                builder.Append($"{i+1}\n\n");
                builder.Append($"{blocksCandidates[i].Count}\n");
                var values = blocksCandidates[i].ToList();
                foreach(var value in values)
                    builder.Append(value.Key+" - "+value.Value+"\n");                                                          
                for (int k = 0; k < randomVote[i].GetLength(0); k++)
                {
                    for (int j = 0; j < randomVote[i].GetLength(1); j++)
                    {
                        if (j == randomVote[i].GetLength(1)-1)
                            builder.Append($"{randomVote[i][k, j]}");
                        else
                            builder.Append($"{randomVote[i][k, j]} ");
                    }
                    builder.Append("\n");
                }
                builder.Append("\n");
            }
            string path = Path.Combine("c:", "Spark");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            directoryInfo.Create();
            using (FileStream fstream = File.Create(path + "\\TestVote.txt"))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(builder.ToString());
                // запись массива байтов в файл
                fstream.Write(buffer, 0, buffer.Length);
            }
        }

        static Dictionary<int, string> CalculateVote(Dictionary<int, string>[] blocksCandidates, int[][,] vote)
        {
            Dictionary<int, string> winners = new Dictionary<int, string>();
            for(int i = 0; i < blocksCandidates.Length; i++)
            {
                Dictionary<int, int> voteRating = new Dictionary<int, int>();
                foreach(var candidates in blocksCandidates[i])
                {
                    voteRating.Add(candidates.Key, 0);
                }
                int k = 0;
                string strWinner = String.Empty;
                List<int> strVote = new List<int>();
                KeyValuePair<int, int> max = new KeyValuePair<int, int>();
                while(true)
                {
                    if (k > blocksCandidates[i].Count)
                        break;
                    StringBuilder strWinnerBuilder = new StringBuilder();
                    for (int j = 0; j < vote[i].GetLength(0); j++)
                    {
                        if (k > 0 && strVote.Contains(j))
                            continue;
                        if (k > 0 && voteRating.ContainsKey(vote[i][j, k-1]))
                        {
                            strVote.Add(j);
                            continue;
                        }
                        var num = voteRating.FirstOrDefault(x => x.Key == vote[i][j, k]).Key;
                        if(num != 0)
                            voteRating[num]++;
                    }
                    max = voteRating.MaxBy(x => x.Value);
                    var averageValue = voteRating.Average(x => x.Value);
                    voteRating = voteRating.Where(x=>x.Value>=averageValue).ToDictionary(x => x.Key, x => x.Value);
                    var countMax = voteRating.Where(x => x.Value == max.Value);
                    if (countMax.Count() > 1)
                    {
                        foreach (var t in countMax)
                            strWinnerBuilder.Append(blocksCandidates[i].GetValueOrDefault(t.Key) + " ");
                        strWinner = strWinnerBuilder.ToString();
                    }
                    else
                    {
                        strWinnerBuilder.Append(blocksCandidates[i].GetValueOrDefault(max.Key));
                        strWinner = strWinnerBuilder.ToString();
                        if (max.Value > vote[i].GetLength(0) / 2)
                            break;
                    }
                    k++;
                }
                if(strWinner!=null)
                    winners.Add(i+1, strWinner);
            }
            return winners;
        }
    }
}