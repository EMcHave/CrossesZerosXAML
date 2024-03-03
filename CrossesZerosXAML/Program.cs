namespace CrossesZeros
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int fieldSize = 3;
            
            ConsolePlayer player1 = new ConsolePlayer(Role.Crosses);
            //ConsolePlayer player2 = new ConsolePlayer(Role.Zeros);
            //StupidBotPlayer player2 = new StupidBotPlayer(Role.Zeros);
            SmarterBotPlayer player2 = new SmarterBotPlayer(Role.Zeros);
            //SmartBotPlayer player2 = new SmartBotPlayer(Role.Zeros);
            Game game = new Game(fieldSize, player1, player2);
            char[] separator = new char[1] { ',' };

            while (!game.GameCompleted)
            {
                int i = -1;
                int j = -1;
                RedrawField(game.Field);

                if (game.CurrentPlayer is ConsolePlayer)
                {
                    Console.WriteLine("\nВведите адрес ячейки в формате i,j");
                    string input = Console.ReadLine();
                    try
                    {
                        if (input != null)
                        {
                            i = int.Parse(input.Split(separator)[0]);
                            j = int.Parse(input.Split(separator)[1]);
                        }
                    }
                    catch
                    {
                        WarningMessage("Некорректный ввод");
                        continue;
                    }
                    if (!game.Field.CellExists(i, j))
                    {
                        WarningMessage("Ячейка не существует");
                        continue;
                    }
                }
                if (!game.MakeGameStep(i, j))
                    WarningMessage("Ячейка занята");
            }
            RedrawField(game.Field);
            Console.WriteLine("Игра окончена!");
            if (game.Winner != Role.None)
                Console.WriteLine($"Победитель - {game.Winner}");
            else
                Console.WriteLine("Ничья!");
        }

        static void RedrawField(GameField field)
        {
            Console.Clear();
            int size = field.FieldSize;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    char symbol;
                    switch (field.Cells[i, j].CellRole)
                    {
                        case Role.Crosses:
                            symbol = 'x';
                            break;
                        case Role.Zeros:
                            symbol = '0';
                            break;
                        default:
                            symbol = '_';
                            break;
                    }
                    Console.Write(string.Concat(symbol, ' '));
                }
                Console.Write('\n');
            }   
        }

        static void WarningMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Нажмите любую клавишу и повторите ввод");
            Console.ReadLine();
        }
    }
}