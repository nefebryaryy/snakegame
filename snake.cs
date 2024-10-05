using System;
using System.Collections.Generic;
using System.Threading;

class SnakeGame
{
    static int width = 30; // Szerokość planszy
    static int height = 20; // Wysokość planszy
    static int score;
    static int level = 1;
    static bool gameOver;
    static int snakeHeadX, snakeHeadY;
    static List<int> snakeX = new List<int>();
    static List<int> snakeY = new List<int>();
    static int foodX, foodY;
    static Random random = new Random();
    static ConsoleKey direction;

    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        // Oczekiwanie na naciśnięcie klawisza, aby rozpocząć grę
        Console.WriteLine("Naciśnij dowolny klawisz, aby rozpocząć grę...");
        Console.ReadKey(true);

        InitializeGame();

        while (!gameOver)
        {
            Draw();
            Input();
            Logic();
            Thread.Sleep(400 - (level * 10)); // Zmniejszamy prędkość gry, ustawiając opóźnienie na 400 ms
        }

        Console.Clear();
        Console.WriteLine("Gra zakończona! Twój wynik: " + score);
    }

    static void InitializeGame()
    {
        snakeHeadX = width / 2;
        snakeHeadY = height / 2;

        // Początkowe segmenty węża
        for (int i = 0; i < 3; i++)
        {
            snakeX.Add(snakeHeadX);
            snakeY.Add(snakeHeadY + i); // Zaczynamy od pozycji w dół
        }

        GenerateFood();
        score = 0;
        gameOver = false;
        direction = ConsoleKey.UpArrow; // Ustawienie początkowego kierunku
    }

    static void GenerateFood()
    {
        // Generujemy współrzędne jedzenia, dopóki nie znajdzie się na ciele węża
        do
        {
            foodX = random.Next(0, width);
            foodY = random.Next(0, height);
        } while (IsFoodOnSnake(foodX, foodY));
    }

    static bool IsFoodOnSnake(int x, int y)
    {
        // Sprawdzamy, czy jedzenie znajduje się na ciele węża
        for (int i = 0; i < snakeX.Count; i++)
        {
            if (snakeX[i] == x && snakeY[i] == y)
                return true;
        }
        return false;
    }

    static void Draw()
    {
        Console.Clear();
        for (int i = 0; i < width + 2; i++)
            Console.Write("#");
        Console.WriteLine();

        for (int y = 0; y < height; y++)
        {
            Console.Write("#");
            for (int x = 0; x < width; x++)
            {
                if (x == snakeHeadX && y == snakeHeadY)
                    Console.Write("O"); // Głowa węża
                else if (x == foodX && y == foodY)
                    Console.Write("*"); // Jedzenie
                else
                {
                    bool isSnakeBody = false;
                    for (int i = 0; i < snakeX.Count; i++)
                    {
                        if (snakeX[i] == x && snakeY[i] == y)
                        {
                            Console.Write("o"); // Ciało węża
                            isSnakeBody = true;
                            break;
                        }
                    }
                    if (!isSnakeBody)
                        Console.Write(" ");
                }
            }
            Console.WriteLine("#");
        }

        for (int i = 0; i < width + 2; i++)
            Console.Write("#");
        Console.WriteLine();
        Console.WriteLine("Wynik: " + score + " Poziom: " + level);
    }

    static void Input()
    {
        // Sprawdzamy, czy klawisz został naciśnięty
        if (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);
            // Aktualizujemy kierunek węża tylko wtedy, gdy nie jest on przeciwny do aktualnego
            if ((keyInfo.Key == ConsoleKey.LeftArrow && direction != ConsoleKey.RightArrow) ||
                (keyInfo.Key == ConsoleKey.RightArrow && direction != ConsoleKey.LeftArrow) ||
                (keyInfo.Key == ConsoleKey.UpArrow && direction != ConsoleKey.DownArrow) ||
                (keyInfo.Key == ConsoleKey.DownArrow && direction != ConsoleKey.UpArrow))
            {
                direction = keyInfo.Key; // Aktualizujemy kierunek
            }
        }
    }

    static void Logic()
    {
        int prevX = snakeHeadX;
        int prevY = snakeHeadY;

        // Aktualizujemy pozycję głowy węża w zależności od kierunku
        switch (direction)
        {
            case ConsoleKey.LeftArrow:
                snakeHeadX--;
                break;
            case ConsoleKey.RightArrow:
                snakeHeadX++;
                break;
            case ConsoleKey.UpArrow:
                snakeHeadY--;
                break;
            case ConsoleKey.DownArrow:
                snakeHeadY++;
                break;
        }

        // Ubywa sprawdzenie na kolizję z krawędziami
        // Możesz usunąć lub pozostawić, jeśli chcesz, aby wąż wychodził za krawędzie

        // Sprawdzenie kolizji z własnym ciałem
        for (int i = 1; i < snakeX.Count; i++) // Zaczynamy od 1, aby pominąć głowę
        {
            if (snakeX[i] == snakeHeadX && snakeY[i] == snakeHeadY)
            {
                gameOver = true; // Gra zakończona
                break;
            }
        }

        if (gameOver) return;

        // Dodajemy nową pozycję głowy na początek listy
        snakeX.Insert(0, snakeHeadX);
        snakeY.Insert(0, snakeHeadY);

        // Sprawdzamy, czy wąż zjadł jedzenie
        if (snakeHeadX == foodX && snakeHeadY == foodY)
        {
            score += 10; // Dodajemy punkty
            GenerateFood();
            if (score % 50 == 0) // Zwiększamy poziom trudności
            {
                level++;
            }
        }
        else
        {
            // Usuwamy ostatni segment, jeśli jedzenie nie zostało zjedzone
            snakeX.RemoveAt(snakeX.Count - 1);
            snakeY.RemoveAt(snakeY.Count - 1);
        }
    }
}
