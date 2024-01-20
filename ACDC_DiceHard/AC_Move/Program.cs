using AC_Move;
using System;
using System.Media;
using System.Net.Mail;

namespace AC_Move
{
    internal class Program
    {
        //static bool[,] collectedChests = new bool[room1.GetLength(0), room1.GetLength(1)];
        static int ChooseDifficulty(ref int dicePoints, int chooseDiff, Player player, Enemy enemy,Dungeons dungeons ,PlayerMoveSet moveSet, DisplayAction display)//will chooose max value based on player choosing diff on main
        {
            int diffDice = 0;//max roll
            switch (chooseDiff)//gets the value based on choose diff 
            {
                case 1: diffDice = 20; break;
                case 2: diffDice = 50; break;
            }
            dicePoints = player.UseDicePoints(dicePoints, diffDice, player, enemy, dungeons, moveSet, display);//gets diffDice for player use
            return dicePoints;
        }
        //MAIN
        static void Main()
        {
            if (OperatingSystem.IsWindows())
            {
                SoundPlayer menuPlayer = new SoundPlayer("Main Menu.wav");
                menuPlayer.Play();
            }
            Dungeons dungeon = new Dungeons();
            Player player = new Player();
            Enemy enemy = new Enemy();
            PlayerMoveSet moveSet = new PlayerMoveSet();
            DisplayAction display = new DisplayAction();
            string[] chooseDiffs = { "1. Normal", "2. Hard" };//diff choices
            int diffChosen, diceUsed = 0;//holds the value for diff choses, increments a turn if a dice is used
            int dicePoints = 0;
            bool playerTurn = true;

            Console.WriteLine("=====[DICE HARD]=====");
            Console.WriteLine("Here are the difficulty options.");//loads the diff choices available
            foreach (string choice in chooseDiffs)
            {
                Console.WriteLine(choice);
            }
            Console.Write("Please input a number to choose diff: ");//ask player to choose difficulty
            diffChosen = Convert.ToUInt16(display.ValidInput(Console.ReadLine(), chooseDiffs.Length));
            Console.Clear();
            do
            {
                dicePoints = ChooseDifficulty(ref dicePoints, diffChosen, player, enemy, dungeon, moveSet, display);
            } while (!PlayerMoveSet.GameEnded);
            //regains Mana after a turn
            //enemy.EnemyAI(diffChosen, player, moveSet, display);//calls the method "EnemyAi" from enemy object
            //display.GameResult(player, enemy, diceUsed);//shows result
        }
        public static bool IsMoveValid(char[,] room, int x, int y,Enemy enemy)
        {
            int width = room.GetLength(1);
            int height = room.GetLength(0);

            if (x < 0 || x >= width || y < 0 || y >= height || room[y, x] == '#' || room[y, x] == '@' || room[y, x] == '$')
            {
                return false; // Check if the move hits a wall or the player
            }

            // Check if the move hits an enemy
            for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
            {
                if (x == enemy.EnemyXROOM1[i] && y ==enemy.EnemyYROOM1[i])
                {
                    return false; // Collision with an enemy
                }
            }

            return true;
        }
        //public static void Attack()
        //{
        //    // Check for nearby enemies and attack them
        //    for (int i = 0; i < enemyXROOM1.Length; i++)
        //    {
        //        if (IsNearby(PlayerMoveSet.playerX, PlayerMoveSet.playerY, enemyXROOM1[i], enemyYROOM1[i]))
        //        {
        //            DealDamageToEnemy(i);
        //        }
        //    }
        //}
        public static void CheckHP(Player player)
        {
            // Check if the player's health is zero
            if (player.health <= 0)
            {
                if (OperatingSystem.IsWindows())
                {
                    SoundPlayer menuPlayer = new SoundPlayer("Game Over.wav");
                    menuPlayer.Play();
                }
                Console.WriteLine("Game over. You have been defeated.");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
        
    }
}
