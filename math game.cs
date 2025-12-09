using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
class Mathgame
{
    private int diff;
    private int life;
    private int score;
    private int maxnum;
    private int timelimit;
    private static int hscore;

    private Random random;

    public Mathgame(int diff) // Constructor takes difficulty from main
    {
        random = new Random();
        this.diff = diff;

        if (diff == 1) { life = 4; maxnum = 30; timelimit = 15; }
        else if (diff == 2) { life = 3; maxnum = 100; timelimit = 12; }
        else { life = 2; maxnum = 250; timelimit = 10; }

        score = 0;

        Console.WriteLine($"You have {life} lives. You get {timelimit} seconds per question.");
        Console.WriteLine($"\nYou can skip qns but be ready for the concequences.");
    }

    public void Start()
    {

        while (life > 0)
        {
            PlayRound();
        }
        Console.WriteLine($"Game Over! Your final score is: {score}");
    }

    private void PlayRound()
    {
        int num1 = random.Next(1, maxnum + 1);
        int num2 = random.Next(1, maxnum + 1);
        int correctanswer = 0;
        string op = "";
        switch (random.Next(1, 5))
        {
            case 1:
                op = "+";
                correctanswer = num1 + num2;
                break;

            case 2:
                op = "-";
                correctanswer = num1 - num2;
                break;

            case 3:
                op = "*";
                while (num1 * num2 > maxnum) // Ensure product does not exceed maxnum to make the game a bit easier
                {
                    num1 = random.Next(1, maxnum + 1);
                    num2 = random.Next(1, maxnum + 1);
                }
                correctanswer = num1 * num2;
                break;

            case 4:
                op = "/";
                while (num2 == 0 || num1 % num2 != 0) // Ensure no division by zero and result is an integer
                {
                    num1 = random.Next(1, maxnum + 1);
                    num2 = random.Next(1, maxnum + 1);
                }
                correctanswer = num1 / num2;
                break;
        }
            ;


        Console.Write($"What is {num1} {op} {num2}? ");

        string userInput = GetAnsweWithTimer(timelimit);


        if (userInput == null)
        {
            life--;
            Console.WriteLine($"\n⏰ Time's up! Lives left: {life}");
            return;
        }
        
        
        if (!int.TryParse(userInput, out int userAnswer))
        {
            Console.WriteLine("Please enter a valid number.");
            PlayRound();
        }

        if (userAnswer == correctanswer)
        {
            score++;
            Console.WriteLine("Correct! Your score is now: " + score);
        }

        else
        {
            life--;
            Console.WriteLine($"Wrong! The correct answer was {correctanswer}. You have {life} lives left.");
        }
    }



    public void ShowHighScore()
    {
        if (score > hscore)
        {
            hscore = score;
            var json = JsonSerializer.Serialize(hscore);
            File.WriteAllText("Highscore.json", json);
            Console.WriteLine("New high score!");
        }
        else
        {
            var hscore = File.ReadAllText("Highscore.json");
            Console.WriteLine("High score : " + hscore);
        }
    }

    private string GetAnsweWithTimer(int timelimit)
    {
        // Use a non-blocking character loop so we don't leave a lingering
        // Console.ReadLine running in the background. Console.ReadLine is
        // not cancellable so the previous approach could block future input
        // after a timeout. This loop collects key presses until Enter or
        // the timeout expires.
        var deadline = DateTime.Now.AddSeconds(timelimit);
        var input = string.Empty;

        while (DateTime.Now < deadline)
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0)
                    {
                        // remove last char from input and erase from console
                        input = input.Substring(0, input.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            // small sleep to avoid busy-waiting
            System.Threading.Thread.Sleep(40);
        }

        // Timeout expired
        return null;
    }


    class Execution
    {
        static string PlayAgain;
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("===Math Game===");

                Console.WriteLine("For each correct answer, you earn a point. For each wrong answer, you lose a life.");
                Console.WriteLine("c1hoose your difficulty: \n 1. easy \n 2. medium \n 3. hard");

                int diff;
                while (!int.TryParse(Console.ReadLine(), out diff) || diff < 1 || diff > 3)
                {
                    Console.WriteLine("Invalid input. Please enter 1, 2, or 3.");
                }

                Mathgame game = new Mathgame(diff);
                game.ShowHighScore();
                game.Start();
                game.ShowHighScore();

                Console.WriteLine("play again? (y/n)");
                PlayAgain = Console.ReadLine().ToLower();
            } while (PlayAgain == "y");
        }

    }
}