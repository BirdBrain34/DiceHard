using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AC_Move
{
    internal class PlayerMoveSet//This class is for player to choose what move and what it does 
    {
        //PLAYER SPAWN LOCATION
        public static int playerX = 2;
        public static int playerY = 2;
        public int attackdmg;
        public int dmgDealt;
        public static char[,] currentRoom = Dungeons.room1;
        public static bool GameEnded { get; private set; }
        public int PlayerMove(int diffDice,int dicePoints, int diceUsed, Player player, Enemy enemy,DisplayAction display)   
        {
            bool gameEnded = false;
            while (dicePoints > 0 && !GameEnded)
            {
                Console.Clear();
                Dungeons.DisplayRoom(currentRoom, dicePoints,player,enemy);
                Console.WriteLine("==========[CHARACTER ELEMENTS]==========");
                Console.WriteLine("[@] = You The Player");
                Console.WriteLine("[E] = The ghost");
                Console.WriteLine("[S] = Stairs");
                Console.WriteLine("[$] = Chest");
                Console.WriteLine("[#] = Wall");

                Console.WriteLine("==========[PLAYER STATS]==========");
                Console.WriteLine("[HEALTH]: {0}", player.health);
                Console.WriteLine("[MANA] {0}", player.Mana);
                Console.WriteLine("\n==========[SKILLS]==========");
                string[] skills = { "[1. Heal]", "[2. Flash light]=Lowers Defence of Enemies", "[3. Talisman]=Increase the Defence", "[4. Skip]" };
                foreach (var skill in skills)
                {
                    Console.WriteLine(skill);
                }
                Console.WriteLine("\n==========[PLAYER MOVES]==========");
                Console.WriteLine("Turns: {0}", diceUsed);
                Console.WriteLine("DicePoints: {0}", dicePoints);
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                int newX = PlayerMoveSet.playerX;
                int newY = PlayerMoveSet.playerY;
                // CONTROLS
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        newY--;
                        dicePoints--;
                        break;
                    case ConsoleKey.A:
                        newX--;
                        dicePoints--;
                        break;
                    case ConsoleKey.S:
                        newY++;
                        dicePoints--;
                        break;
                    case ConsoleKey.D:
                        newX++;
                        dicePoints--;
                        break;
                    case ConsoleKey.Enter:
                        dicePoints = PlayerAttack(dicePoints,player,enemy);
                        Program.CheckHP(player);
                        
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                        dicePoints = PlayerSkill(dicePoints, diceUsed, keyInfo.Key, player, enemy, display);
                        break;
                    case ConsoleKey.D4:
                        dicePoints = PlayerSkip(dicePoints);
                        player.RegenMana(player.ManaCap);
                        ++diceUsed;
                        break;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                }
                player.RegenMana(player.ManaCap);
                if (Program.IsMoveValid(currentRoom, newX, newY,enemy))
                {
                    PlayerMoveSet.playerX = newX;
                    PlayerMoveSet.playerY = newY;
                    // CONTROLS
                    if (currentRoom[newY, newX] == 'S')
                    {
                        if (currentRoom == Dungeons.room1)
                        {
                            currentRoom = Dungeons.room2;
                            PlayerMoveSet.playerX = 2;
                            PlayerMoveSet.playerY = 1;
                            Enemy.ResetEnemies(Dungeons.room2,enemy);
                        }
                        else if (currentRoom == Dungeons.room2)
                        {
                            currentRoom = Dungeons.room3;
                            PlayerMoveSet.playerX = 5;
                            PlayerMoveSet.playerY = 10;
                            Enemy.ResetEnemies(Dungeons.room3, enemy);
                        }
                        else if (currentRoom == Dungeons.room3)
                        {
                            currentRoom = Dungeons.room4;
                            PlayerMoveSet.playerX = 5;
                            PlayerMoveSet.playerY = 10;
                            Enemy.ResetEnemies(Dungeons.room4, enemy);
                        }
                        else if (currentRoom == Dungeons.room4)
                        {
                            currentRoom = Dungeons.room5;
                            PlayerMoveSet.playerX = 5;
                            PlayerMoveSet.playerY = 10;
                            Enemy.ResetEnemies(Dungeons.room5, enemy);
                        }
                        else if(currentRoom == Dungeons.room5)
                        {
                            GameEnded = true;
                            Console.WriteLine("CONGRATS");
                            SoundPlayer venuPlayer = new SoundPlayer("Victory.wav");
                            venuPlayer.Play();
                            Console.ReadLine();
                        }
                    }
                    for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
                    {
                        enemy.EnemyAI(diffDice,currentRoom,i,player,enemy,display);
                    }
                }
                if (gameEnded) break;
                if (player.health <= 0)
                {
                    display.GameResult(player, enemy, diceUsed);
                    break;
                }
            }
            ++diceUsed;
            return dicePoints;
        }
        public int PlayerAttack(int dicePoints, Player player, Enemy enemy)
        {
            Random rngAtk = new Random(); // just to initialize rng
            if (dicePoints < 5)
            {
                Console.WriteLine("\nYou don't have enough points to attack\n");
                return dicePoints;
            }
            else
            {
                int attackdmg = 0, dmgDealt = 0;
                attackdmg += rngAtk.Next(1, 11); // player generates a rng for atk and will increment on attackdmg
                dmgDealt = (attackdmg * player.atk) / enemy.def; // displays the actual damage dealt
                int index = GetEnemyIndex(enemy);
                // Check if a valid enemy index is found
                if (index != -1)
                {
                    Console.WriteLine("\nYou have dealt {0} amounts of damage", dmgDealt);
                    DealDamageToEnemy(index, dmgDealt, player, enemy);
                }
                else
                {
                    Console.WriteLine("\nNo enemies in sight. No damage dealt.");
                    return dicePoints;
                }

                Console.WriteLine();
                return dicePoints - 5; // decreases the amount of points being used
            }
        }//this move cost 5 dicePoints
        //Removes the enemy if the player kills it
        public static void KillEnemy(int index, Enemy enemy)
        {
            enemy.EnemyXROOM1[index] = -1;
            enemy.EnemyYROOM1[index] = -1;
        }
        public static void DealDamageToEnemy(int index,int dmgDealt,Player player,Enemy enemy)
        {
            enemy.health -= dmgDealt;//formula for damaging the enemy

            // Check if the enemy is defeated
            if (enemy.health <= 0)
            {
                KillEnemy(index, enemy);
            }
        }
        public static bool IsNearby(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
        }
        private static int GetEnemyIndex(Enemy enemy)
        {
            for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
            {
                if (PlayerMoveSet.IsNearby(PlayerMoveSet.playerX, PlayerMoveSet.playerY, enemy.EnemyXROOM1[i], enemy.EnemyYROOM1[i]))
                {
                    return i;
                }
            }
            return -1; // If no nearby enemy is found, return -1 or handle it accordingly
        }
        public int PlayerSkill(int dicePoints,int diceUsed,ConsoleKey skillUse,Player player, Enemy enemy,DisplayAction display)
        {
            int manaCost;//ask player on what skill to use,has manaCost to make the user not spam the skills
            bool skip = false;
            string message = ""; // Variable to store the message

            switch (skillUse)
            {
                case ConsoleKey.D1: //skill: Heal
                    manaCost = 2;//amount to decreases mana
                    dicePoints = display.UseSkill(skillUse, dicePoints, manaCost, player, enemy);
                    break;
                case ConsoleKey.D2://skill: Flash light
                    manaCost = 5;//amount to decreases mana
                    dicePoints = display.UseSkill(skillUse, dicePoints, manaCost, player, enemy);
                    break;
                case ConsoleKey.D3://skill: Talisman
                    manaCost = 10;//amount to decreases mana
                    dicePoints = display.UseSkill(skillUse, dicePoints, manaCost, player, enemy);
                    break;
            }
            if (dicePoints < 0)
            {
                Console.WriteLine("You do not have enough points to use this skill");
                 // exit the loop if dicePoints is not enough
            }

            return dicePoints;
        }
        public int PlayerSkip(int dicePoints)
        {
            Console.WriteLine("Skipping a turn");
            Console.Write("You still have {0} points in reserve.\n", dicePoints);
            return dicePoints + 10;
        }
    }
}
