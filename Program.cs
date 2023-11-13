using System;
namespace Voting
{
    enum InputDataText
    {
        Blocks,
        Candidates
    }
    public class Program
    {
        static int blocks;
        static int candidateCount;
        static void Main()
        {
            InputData(InputDataText.Blocks);
            InputData(InputDataText.Candidates);
            Console.ReadLine();
        }
        static void InputData(InputDataText inputData)
        {
            var isBlocks = inputData == InputDataText.Blocks ? true : false;
            var blocksText = "Введите количество тестовых блоков: ";
            var candodateText = "Введите количество кандидатов от 1 до 20: ";
            var inputText = inputData== InputDataText.Blocks ? blocksText: candodateText;
            Console.WriteLine(inputText);
            Console.CursorTop = 0;
            while (true)
            {
                Console.CursorLeft = inputText.Length;
                var q = int.TryParse(Console.ReadLine(), out var inputValue);
                if (q&&isBlocks&&inputValue>0)
                {
                    blocks = inputValue;
                    Console.Clear();
                    Console.WriteLine($"Вы ввели {blocks} тестовых блоков");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                }
                else if(q && !isBlocks&& inputValue is (<=20 and >0))
                {
                    candidateCount = inputValue;
                    Console.Clear();
                    Console.WriteLine($"Вы ввели {candidateCount} кандидатов");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    if(isBlocks)
                        Console.WriteLine("Количество тестовых блоков должно быть целым положительным числом\n" + inputText);
                    else Console.WriteLine("Количество кандидатов должно быть целым положительным числом от 1 до 20\n" + inputText);
                    Console.CursorTop = 1;
                }
            }
            
        }
    }
}