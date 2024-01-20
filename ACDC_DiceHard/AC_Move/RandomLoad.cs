using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Move
{
    internal class RandomLoad
    {
        public void ItemGen(Player player)
        {
            string items = "item 1;health;10;item 2;atk;1;item 3;def;1;item 4;atk;2;item 5;def;2;item 6;mana;10";//list of items 
            string[] sItems = items.Split(";");//creates an array to splits them into 1D array
            int chLen = sItems.Length / 3, chIndex = 0, chItemGet;//gets the length of the 1D array and splits it into 3,holds the index for the 2D array,holds index value to get the item
            string[,] chItems = new string[chLen, 3];//creates a 2D array from chLen and the attributes of stats to 3
            Random rngItem = new Random();

            for (int i = 0; i < chLen; i++)//appends the 1D array and integrate it into the 2D array
            {
                //ex: chItems[item 1[item 1,health,10],item 2[item 2,atk,1],...]
                chItems[i, 0] = sItems[0 + chIndex];//gets item name
                chItems[i, 1] = sItems[1 + chIndex];//gets what stat to increase
                chItems[i, 2] = sItems[2 + chIndex];//gets value of said stat in "string"
                chIndex += 3;//increments it by 3 to start the chain till the list is not less than chLen
            }
            chItemGet = rngItem.Next(0, chLen);//gets index value to get the specific item

            Console.WriteLine("You have obtained {0}",chItems[chItemGet, 0]);//item name
            player.PlayerStats(chItems[chItemGet, 1], Convert.ToUInt16(chItems[chItemGet, 2]));//stat increase and amount
        }//player will get a random item and gets stats from said item
    }
}
