using Faker;
using System.Text;
using System.Text.RegularExpressions;
namespace Voting
{
    public class Program
    {
        #region TryParseInputFile - парсинг файла с входными данными
        /// <summary>
        /// Метод для парсинга файла с входными данными
        /// </summary>
        /// <param name="path">Путь к файлу с входными данными</param>
        /// <param name="blocksCandidates">Списки кандидатов для каждого блока</param>
        /// <param name="vote">Результат голосования для каждого блока</param>
        /// <returns></returns>
        static bool TryParseInputFile(string path, out Dictionary<int, string>[] blocksCandidates, out int[][,] vote)
        {
            blocksCandidates = null;
            vote = null;
            string exampleDataInput = @"Пример корректного ввода данных:
1

4
1 - Ashlynn Bogisich
2 - Grace Keebler
3 - Kristofer Hickle
4 - Danielle Abernathy
2 4 1 3
3 2 1 4
3 1 4 2
4 1 3 2
2 3 1 4
1 2 4 3
2 1 4 3

2

3
1 - Deron Keebler
2 - Gilda Feil
3 - Okey Davis
1 3 2
2 1 3
2 1 3
3 1 2
3 1 2

";
            int countGlobalLines = 0;  //глобальный счетчик строк из файла
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            if(lines.Length == 0)
            {
                Console.WriteLine($"Файл пустой\n" + exampleDataInput);
                return false;
            }
            List<string[]> candidatesPerBlock = new List<string[]>();  //количество кандидатов для каждого блока
            List<int[,]> votesPerBlock = new List<int[,]>();  // количество голосов для каждого блока
            int countBlocks = 0;    //счетчик блоков
            while(lines.Length > 0)
            {
                int countLines = 0;       //локальный счетчик строк (внутри блока)
                int countCandidatesPerBlock = 0;    
                var blockHeader = lines.Take(3).ToArray();
                var strHeader = string.Join("\n", blockHeader);
                if (Regex.IsMatch(strHeader, @"^\d+\n\n\d+$")) //проверка соответствия заголовка (первые 3 строки блока)
                {
                    countBlocks++;
                    int blocks = int.Parse(blockHeader[0]);  //парсинг кол-ва блоков
                    if (blocks == countBlocks) //проверка соблюдения последовательности блоков
                    {
                        countCandidatesPerBlock = int.Parse(blockHeader[2]);  //парсинг кол-ва кандидатов в блоке
                        countLines += 3;
                        countGlobalLines += 3;
                        string[] blockCandidates = null;
                        blockCandidates = lines.Take(new Range(countLines, countLines + countCandidatesPerBlock)).ToArray();
                        string[] s = new string[blockCandidates.Length];  //массив для хранения кандидатов в блоке
                        for (int i = 0; i < countCandidatesPerBlock; i++)
                        {
                            if (Regex.IsMatch(blockCandidates[i], @"^\d+\s?-\s?\w+")) //проверка построчно кандидатов в блоке
                            {
                                s[i] = blockCandidates[i];
                                string[] str = s[i].Split(" - ");
                                var numCand = int.Parse(str[0]);
                                if (numCand != i + 1)     //проверка нумерации кандидатов 
                                {
                                    Console.WriteLine($"Не соблюден порядок ввода кандидатов, смотри строку #{countGlobalLines + 1}\n"+exampleDataInput);
                                    return false;
                                }
                                countLines++;
                                countGlobalLines ++;
                            }
                            else
                            {
                                if (Regex.IsMatch(blockCandidates[i], @"\d+\s\d+\s\d+$"))
                                    Console.WriteLine($"Не соответствует указанное число кандидатов фактическому, смотри строку #{countGlobalLines+1}");
                                else
                                    Console.WriteLine($"Неверный формат ввода кандидатов, смотри строку #{countGlobalLines+1}\n"+exampleDataInput);
                                return false;
                            }
                        }
                        candidatesPerBlock.Add(s);     //добавляем массив кандидатов блока в список
                        int[,] z = null;  //массив для хранения голосов в блоке
                        List<int[]> srs = new List<int[]>(); // список голосов в блоке
                        var blockVotes = lines.Take(new Range(countLines, lines.Length)).ToArray(); //заранее не известно, сколько будет голосов
                        for (int i = 0; i < blockVotes.Length; i++)
                        {
                            if (blockVotes[i] == string.Empty) //если встречаем пустую строку, то считаем, что достигли конца блока
                            {
                                z = new int[srs.Count, countCandidatesPerBlock];  //добавляем массив голосов в блоке
                                var count = 0;
                                foreach (var val in srs)
                                {
                                    for (int j = 0; j < countCandidatesPerBlock; j++)
                                    {
                                        z[count, j] = val[j]; //делаем запись голосов в массив
                                    }
                                    count++;
                                }
                                countLines++;
                                countGlobalLines ++;
                                lines = lines.TakeLast(lines.Length - countLines).ToArray();
                                break;
                            }
                            if (Regex.IsMatch(blockVotes[i], @"\d+\s\d+\s\d+$"))   //проверка соответствия строк голосов формату данных
                            {
                                if (countLines == lines.GetLength(0)-1)
                                {
                                    Console.WriteLine($"В конце каждого блока должна быть пустая строка, смотри строку #{countGlobalLines+1}\n"+exampleDataInput);
                                    return false;
                                }
                                var bv = blockVotes[i].Substring(0, blockVotes[i].Length);
                                int[] sv = null;
                                try
                                {
                                    sv = bv.Split(' ').Select(x => int.Parse(x)).ToArray(); //получаем массив голосов после парсинга строки
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine($"Неверный формат ввода голосов, смотри строку #{countGlobalLines + 1}\n" + exampleDataInput);
                                    return false;
                                }
                                if (sv.Length == countCandidatesPerBlock)
                                {
                                    for (int e = 1; e < sv.Length; e++)
                                    {
                                        try
                                        {
                                            sv.Single(x => x == sv[e]);         // проверка на дублирование кандидатов в голосе
                                        }
                                        catch (InvalidOperationException)
                                        {
                                            Console.WriteLine($"Неверный формат ввода голосов - нельзя указывать кандидата 2 и более раз, смотри строку #{countGlobalLines + 1}\n");
                                            return false;
                                        }
                                        if (sv[e] <1 || sv[e]  > sv.Length)   //проверка на валидность указания номеров кандидатов в голосах
                                        {
                                            Console.WriteLine($"Неверный формат ввода голосов - число должно соответствовать кандидату, смотри строку #{countGlobalLines + 1}\n");
                                            return false;
                                        }
                                    }
                                    srs?.Add(sv);    //добавляем голос в списки голосов блока
                                    countLines++;
                                    countGlobalLines ++;
                                }
                                else
                                {
                                    Console.WriteLine($"не соответствует количесто кандидатов и списка проголосовавшего, смотри строку #{countGlobalLines+1}");
                                    return false;
                                }
                            }
                            else if (Regex.IsMatch(blockVotes[i], @"^\d+\s?-\s?\w+"))
                            {
                                Console.WriteLine($"Не соответствует указанное число кандидатов фактическому, смотри строку #{countGlobalLines+1}");
                                return false;
                            }
                            else if (Regex.IsMatch(blockVotes[i], @"^\d+$"))
                            {
                                Console.WriteLine($"В конце каждого блока должна быть пустая строка, смотри строку #{countGlobalLines+1}\n"+exampleDataInput);
                                return false;
                            }
                            else
                            {
                                Console.WriteLine($"Неверный формат ввода голосов, смотри строку #{countGlobalLines + 1}\n"+exampleDataInput);
                                return false;
                            } 
                        }
                        if (z != null)
                            votesPerBlock.Add(z);
                    }
                    else
                    {
                        Console.WriteLine($"Неверно введен номер блока, смотри строку #{countGlobalLines+1}");
                        return false;
                    }
                }
                else if (Regex.IsMatch(strHeader, @"\d+\s\d+\n"))
                {
                    Console.WriteLine($"Допущена лишняя пустая строка, смотри строку #{countGlobalLines + 1}\n"+exampleDataInput);
                    return false;
                }
                else
                {
                    Console.WriteLine($"Неверный формат заголовка (номер блока и кол-во кандидатов), смотри строку #{countGlobalLines+1}\n"+exampleDataInput);
                    return false;
                }
            }
            // формируем выходные данные после всех проверок
            blocksCandidates = new Dictionary<int, string>[countBlocks];
            vote = votesPerBlock.ToArray();
            int countCand = 0;
            foreach (var val in candidatesPerBlock)
            {
                blocksCandidates[countCand] = new Dictionary<int, string>();
                for (int i = 0; i < val.Length; i++)
                {
                    string[] str = val[i].Split(" - ");
                    blocksCandidates[countCand].Add(int.Parse(str[0]), str[1]);
                }
                countCand++;
            }
            return true;
        }
        #endregion

        static void Main(string[] args)
        {
            //InputDataGenerate();
            bool b = TryParseInputFile(args[0], out Dictionary<int, string>[] blocksCandidates, out int[][,] vote);
            //bool b = TryParseInputFile(Path.Combine("c:", "Spark") + "\\TestVote.txt", out Dictionary<int, string>[] blocksCandidates, out int[][,] vote);
            if (b)
            {
                var result = CalculateVote(blocksCandidates, vote);
                PrintResult(blocksCandidates, vote, result);
            }
            Console.ReadLine();
        }

        #region PrintResult - вывод результата на консоль
        /// <summary>
        /// Вывод на консоль результатов голосования
        /// </summary>
        /// <param name="blocksCandidates">Списки кандидатов для каждого блока</param>
        /// <param name="vote">Голоса для каждого блока</param>
        /// <param name="result">Результат голосования</param>
        static void PrintResult(Dictionary<int, string>[] blocksCandidates, int[][,] vote, Dictionary<int, string> result)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < blocksCandidates.Length; i++)
            {
                builder.Append($"{i + 1}\t\t\t\t\t\t{result[i+1]}\n\n");
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
        #endregion


        #region InputDataGenerate - Генератор случайных входных данных
        /// <summary>
        /// Генератор случайных входных данных
        /// </summary>
        static void InputDataGenerate()
        {
            // Случайное количество блоков
            var blocks = RandomNumber.Next(1, 10);
            Dictionary<int, string>[] blocksCandidates = new Dictionary<int, string>[blocks];
            int[][,] randomVote = new int[blocks][,];
            for (int i = 0; i < blocks; i++) 
            {
                var candidatesCountPerBlock = RandomNumber.Next(3, 20); //задаем случайное кол-во кандидатов в текущем блоке
                var voteCountPerBlock = RandomNumber.Next(10, 1000);  //задаем случайнок кол-во голосов в текущем блоке
                var vote = new int[candidatesCountPerBlock];
                randomVote[i] = new int[voteCountPerBlock, candidatesCountPerBlock];
                blocksCandidates[i] = new Dictionary<int, string>(candidatesCountPerBlock);
                for (int j = 0; j < candidatesCountPerBlock; j++)
                {
                    blocksCandidates[i].Add(j+1, Name.FullName(NameFormats.Standard));  //добавляем кандидатов в текущий блок
                    vote[j] = j+1;
                }
                //Перетасовываем голоса в каждой строчке голосов по алгортму Фишера-Йетса
                for (int q = 0; q < voteCountPerBlock; q++)
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
        #endregion

        #region FileInputDataCreate - Метод для формирования тестового файла с входными данными  
        /// <summary>
        /// Метод для формирования тестового файла с входными данными 
        /// </summary>
        /// <param name="blocksCandidates">Списки кандидатов для каждого блока</param>
        /// <param name="randomVote">Голоса для каждого блока</param>
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
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Create();
                using (FileStream fstream = File.Create(path + "\\TestVote.txt"))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(builder.ToString());
                    // запись массива байтов в файл
                    fstream.Write(buffer, 0, buffer.Length);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region CalculateVote - подсчет голосов
        /// <summary>
        /// Метод для вычисления победителей голосования в каждом блоке
        /// </summary>
        /// <param name="blocksCandidates">Списки кандидатов для каждого блока</param>
        /// <param name="vote">Голоса для каждого блока</param>
        /// <returns>Результат голосования</returns>
        static Dictionary<int, string> CalculateVote(Dictionary<int, string>[] blocksCandidates, int[][,] vote)
        {
            Dictionary<int, string> winners = new Dictionary<int, string>();  //словарь для хранения победителей голосования в каждом блоке
            for(int i = 0; i < blocksCandidates.Length; i++)
            {
                Dictionary<int, int> voteRating = new Dictionary<int, int>(); // словарь для хранения промежуточных результатов
                foreach(var candidates in blocksCandidates[i])
                {
                    voteRating.Add(candidates.Key, 0); //изначально у всех кандидатов 0 голосов
                }
                int k = 0; //кол-во туров голосования (максимальное их число равно количеству кандидатов)
                string strWinner = String.Empty;  
                List<int> strVote = new List<int>(); //список строк с учтенными голосами
                KeyValuePair<int, int> max = new KeyValuePair<int, int>();
                while(true)
                {
                    if (k > blocksCandidates[i].Count) //прошли все туры голосования, завершаем процесс
                        break;
                    StringBuilder strWinnerBuilder = new StringBuilder();
                    for (int j = 0; j < vote[i].GetLength(0); j++)
                    {
                        if (k > 0 && strVote.Contains(j)) //Результат этой строки уже учтен
                            continue;
                        if (k > 0 && voteRating.ContainsKey(vote[i][j, k-1]))
                        {
                            strVote.Add(j);  //заносим в список учтенных голосов фаворитов предыдущего тура
                            continue;
                        }
                        //Находим в словаре кандидата и прибавляем ему голос
                        var num = voteRating.FirstOrDefault(x => x.Key == vote[i][j, k]).Key; 
                        if(num != 0)
                            voteRating[num]++;
                    }
                    max = voteRating.MaxBy(x => x.Value); 
                    var averageValue = voteRating.Average(x => x.Value);
                    //Сокращаем список кандидатов
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
        #endregion
    }
}