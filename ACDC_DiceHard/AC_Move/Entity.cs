using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AC_Move
{
    internal class Entity//This is the class that can morph the stats of player and enemy
    {
        public int health { get; set; }//for health
        public int atk { get; set; }//for atk
        public int def { get; set; }//for def
        public Entity(int hpstat, int atkstat, int defstat)//method to set the stats
        { health = hpstat; atk = atkstat; def = defstat; }
    }
    class Player : Entity//Player inherits the stats from entity
    {
        static int diceUsed = 0;//holds the amount of dice used per turn
        private int mana = 20;//default mana at the start of the game
        private int hpCap = 50;//health cap 
        private int manaCap = 20;//mana cap
        public Player() : base(50, 7, 7) { }//default stats for player
        public int Mana//gets and sets mana
        {
             get { return mana; }
            private set { mana = value; }
        }
        public int ManaCap//gets and sets the value for hpCap
        {
            get { return manaCap; }
            set { manaCap = value; }
        }
        public void DecreaseMana(int manaCost)//decrease mana based on skillUse
        {
            if (manaCost > 0)//will decrease mana if it is higher than 0
            {
                mana -= manaCost;
                if (mana < 0)
                {
                    mana = 0; // ensure mana doesn't go below 0
                }
            }
        }
        public void RegenMana(int manaCap)//regen mana every time the player ends a turn
        {
            mana += 2;//gains mana back by 2
            if (mana > manaCap)//limits mana gain back to 20;
                mana = manaCap;
        }
        public int HpCap//gets and sets the value for hpCap
        {
            get { return hpCap; }
            set { hpCap = value; }
        }
        public void PlayerStats(string stat, int statIncrease)//Player gains stats from ItemGen() WIP
        {
            if (stat == "health")//will increase if the stat is "health"
                hpCap += statIncrease;
            else if (stat == "atk")//will increase if the stat is "atk"
                atk += statIncrease;
            else if (stat == "def")//will increase if the stat is "def"
                def += statIncrease;
            else if (stat == "mana")//will increase if the stat is "def"
                manaCap += statIncrease;
        }
        public int UseDicePoints(int dicePoints, int diffDice, Player player, Enemy enemy,Dungeons dungeons,PlayerMoveSet moveSet,DisplayAction display)//calls Enemy in Entity
        {
            Random rngDice = new Random();//calls Random for rng
            int diceMove = 0;//choose what move to use
            //player skipping is false by default
            dicePoints += rngDice.Next(1, diffDice);//the dicePoints will hold the value that the player can use
            do//will loop until the dicePoints reaches 0 or if the player skips a turn
            {
                Console.WriteLine("Turn: {0}", diceUsed);
                Console.Write("Dice Points: " + dicePoints);//player displays the amount of dicePoints available
                dicePoints = moveSet.PlayerMove(diffDice,dicePoints,diceUsed, player, enemy,display);

            } while (dicePoints > 0 && !PlayerMoveSet.GameEnded);
            if (dicePoints <= 0)
            {
                Console.Write("You used all your points\n");
                ++diceUsed;
            }
            return dicePoints;
        }//player will use the dice to allocate points into specific moves
    }
    class Enemy : Entity//Enemy inherits the stats from entity
    {
        //ENEMY SPAWN LOCATION
        public static int[] enemyXROOM1 = { 4, 14, 18, 10, 6 };
        public int[] EnemyXROOM1
        {
            get { return enemyXROOM1; }
        }
        public static int[] enemyYROOM1 = { 16, 8, 5, 12, 15 };
        public int[] EnemyYROOM1
        {
            get { return enemyYROOM1; }
        }

        private int enemyDistance  = 0;//resets distance by 0 everytime the enemy turn is finished WIP
        public Enemy() : base(20, 5, 5) { }//default stats for player
        public int EnemyDistance
        {
            get { return enemyDistance; }
            private set { enemyDistance = value; }
        }//get and set enemtDistance
        public void EnemyAI(int diffDice, char[,] currentRoom, int index,Player player, Enemy enemy,DisplayAction display)
        {
            int enHp = health, enAtk = atk, enDef = def, dicePoints = 0, attackdmg = 0, dmgDealt;
            Random rngDice = new Random(); //holds value to roll the dice

            switch (diffDice) //will hold max value for dice based on chosen diff
            {
                case 1: diffDice = 20; break;
                case 2: diffDice = 50; break;
            }
            dicePoints += rngDice.Next(1, diffDice);//enemy gains dicePoints
            MoveEnemy(currentRoom,index, dicePoints, rngDice, player, enemy, display);
        }
        public static void MoveEnemy(char[,] currentRoom, int index,int dicePoints, Random rngDice, Player player,Enemy enemy,DisplayAction display)
        {
            int newEnemyX = enemyXROOM1[index];
            int newEnemyY = enemyYROOM1[index];

            if (newEnemyX < PlayerMoveSet.playerX && Program.IsMoveValid(Dungeons.room1, newEnemyX + 1, newEnemyY,enemy))
            {
                newEnemyX++;
            }
            else if (newEnemyX > PlayerMoveSet.playerX && Program.IsMoveValid(Dungeons.room1, newEnemyX - 1, newEnemyY, enemy))
            {
                newEnemyX--;
            }
            if (newEnemyY < PlayerMoveSet.playerY && Program.IsMoveValid(Dungeons.room1, newEnemyX, newEnemyY + 1, enemy))
            {
                newEnemyY++;
            }
            else if (newEnemyY > PlayerMoveSet.playerY && Program.IsMoveValid(Dungeons.room1, newEnemyX, newEnemyY - 1, enemy))
            {
                newEnemyY--;
            }
            //Enemy Attack
            if(Program.IsMoveValid(currentRoom, newEnemyX, newEnemyY,enemy))
            {
                if(newEnemyX == PlayerMoveSet.playerX && newEnemyY == PlayerMoveSet.playerY)
                {
                    Console.WriteLine("Enemy Attacks");

                    if (dicePoints > 5)//check if the enemy has enough dicePoints to attack
                    {
                        int attackdmg = 0, dmgDealt;
                        attackdmg += rngDice.Next(1, 11);//attack will be rng based from 1,10
                        player.health -= (attackdmg * enemy.atk) / player.def;
                        dmgDealt = (attackdmg * enemy.atk) / player.def;
                        display.AttackConnect(attackdmg);
                        Console.WriteLine("Ghost have dealt {0} amounts of damage", dmgDealt);
                        dicePoints -= 5; // decreases dicePoints after the enemy attacks
                    }
                    Console.WriteLine();
                }
            }

            if (newEnemyX != PlayerMoveSet.playerX || newEnemyY != PlayerMoveSet.playerY)
            {
                enemyXROOM1[index] = newEnemyX;
                enemyYROOM1[index] = newEnemyY;
            }
        }
        public static void ResetEnemies(char[,] room, Enemy enemy)
        {
            // Reset existing enemies
            for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
            {
                enemy.EnemyXROOM1[i] = -1;
                enemy.EnemyYROOM1[i] = -1;
            }

            Random rand = new Random();
            for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
            {
                int enemyX = rand.Next(1, room.GetLength(1) - 1);
                int enemyY = rand.Next(1, room.GetLength(0) - 1);

                // Ensure that the enemy does not spawn on the player or a wall
                while (room[enemyY, enemyX] == '#' || (enemyX == PlayerMoveSet.playerX && enemyY == PlayerMoveSet.playerY))
                {
                    enemyX = rand.Next(1, room.GetLength(1) - 1);
                    enemyY = rand.Next(1, room.GetLength(0) - 1);
                }

                enemy.EnemyXROOM1[i] = enemyX;
                enemy.EnemyYROOM1[i] = enemyY;
            }
        }
        // Check for nearby enemies and kill them
        public static void KillEnemyIfNear(Enemy enemy)
        {
            for (int i = 0; i < enemy.EnemyXROOM1.Length; i++)
            {
                if (PlayerMoveSet.IsNearby(PlayerMoveSet.playerX, PlayerMoveSet.playerY, enemy.EnemyXROOM1[i], enemy.EnemyYROOM1[i]))
                {
                    PlayerMoveSet.KillEnemy(i,enemy);
                }
            }
        }

        
    }
}
