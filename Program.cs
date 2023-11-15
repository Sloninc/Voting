using Faker;
using System.Text;
namespace Voting
{
    public class Program
    {
        static void Main(string[] args)
        {

            InputDataGenerate();


            Console.ReadLine();
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
                    blocksCandidates[i].Add(j, Name.FullName(NameFormats.Standard));
                    vote[j] = j;
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
                var values = blocksCandidates[i].Values;
                foreach(var value in values)
                    builder.Append(value+"\n");                                                          
                for (int k = 0; k < randomVote[i].GetLength(0); k++)
                {
                    for (int j = 0; j < randomVote[i].GetLength(1); j++)
                    {
                        builder.Append($"{randomVote[i][k, j]+1} \t");
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
    }
}