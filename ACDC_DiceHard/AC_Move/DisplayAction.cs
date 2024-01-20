using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace AC_Move
{
    internal class DisplayAction
    {
        public string ValidInput(string input,int maxChoice)
        {
            int num;
            while (!int.TryParse(input, out num) || num < 1 || num > maxChoice)
            {
                Console.Write("Error! Please enter a valid number within the options: ");
                input = Console.ReadLine();
            }
            return input;

        }
        public int UseSkill(ConsoleKey skillUse, int dicePoints, int manaCost, Player player,Enemy enemy)
        {
            if (skillUse == ConsoleKey.D1)
            {
                if (dicePoints < 2 || player.Mana < 2)//dicePoints and Mana validation
                {
                    Console.WriteLine("You do not have enough points to use this skill");
                }
                else
                {
                    int hpRegen = 5;
                    if (player.health == player.HpCap)
                    {
                        Console.WriteLine("You are already at full health.");
                    }
                    else
                    {
                        dicePoints -= 2;
                        player.DecreaseMana(manaCost); // Decrease player's mana

                        if (player.health + hpRegen > player.HpCap)
                        {
                            hpRegen = player.HpCap - player.health;
                        }
                        Console.WriteLine("You use Heal to get your health back to {0} points.", hpRegen);
                        player.health += hpRegen; // gets health back by the calculated amount
                    }
                }

            }
            else if (skillUse == ConsoleKey.D2)
            {
                if (dicePoints < 5 || player.Mana < 5)//dicePoints and Mana validation
                {
                    Console.WriteLine("You do not have enough points to use this skill");
                }
                else
                {
                    dicePoints -= 5;
                    player.DecreaseMana(manaCost); // Decrease player's mana
                    Console.WriteLine("You use Flash light to blind nearby ghosts and decrease their defense.");
                    enemy.def -= 2;//decreases enemy def
                }
            }
            else if (skillUse == ConsoleKey.D3)
            {
                if (dicePoints < 10 || player.Mana < 10)//dicePoints and Mana validation
                {
                    Console.WriteLine("You do not have enough points to use this skill");
                }
                else
                {
                    dicePoints -= 10;
                    player.DecreaseMana(manaCost); // Decrease player's mana
                    Console.WriteLine("You use protected yourself with your Talisman to increase your defense.");
                    player.def += 5;//increases player def
                }
            }
            return dicePoints;
        }
        public void AttackConnect(int atkDmg)
        {
            if (atkDmg == 0)
                Console.WriteLine("The attack missed,");
            else if (atkDmg >= 8)
            {
                Console.WriteLine("The attack dealt a massive");
            }
        }
        public void GameResult(Player player, Enemy enemy, int diceUsed)//displays the result of the game
        {
            if (player.health <= 0)//if player loses
            {
                Console.WriteLine("GAME OVER.\nTotal dice used: {0}", diceUsed);
                SoundPlayer genuPlayer = new SoundPlayer("Game Over.wav");
                genuPlayer.Play();
                Console.ReadLine();
            }
            else if (enemy.health <= 0)//if enemy loses
            {
                Console.WriteLine("Congrats you have won.\nTotal dice used: {0}", diceUsed);
                SoundPlayer venuPlayer = new SoundPlayer("Victory.wav");
                venuPlayer.Play();
                Console.ReadLine();
            }
        }
    }
}
