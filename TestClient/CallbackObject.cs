/*
 * Program:     TestClient.exe
 * Module:      CallbackObject.cs
 * Developer:   Dianne Corpuz
 * Date:        April 9, 2021
 * Summary:     CallbackObject class that handles the updates from the client, and handles/display result of the game
 */
using System;
using System.Collections.Generic;
using TestLibrary;

namespace TestClient
{
    public class CallbackObject : ICallback
    {
        //helper method(Generic) to convert List or arrays to multi dimensional array
       public static T[,] ConvertToMultiDArray<T>(IList<T[]> arrays)
        {
            int minorLength = arrays[0].Length;
            T[,] arrayToReturn = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                    throw new ArgumentException("All arrays must be the same length");
                
                for (int j = 0; j < minorLength; j++)
                    arrayToReturn[i, j] = array[j];
            }
            return arrayToReturn;
        }

        //helper method to Display Current Player on each console
        public static void DisplayCurrentPlayer(int id)
        {
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|                                      |");
            Console.WriteLine($"|\tCurrently playing: Player #{id}   |");
            Console.WriteLine("|                                      |");
            Console.WriteLine("+--------------------------------------+");
        }

        //helper method to display a generated bingo card
        public static void DisplayGeneratedBingoCard(List<uint[]> bingoCardList)
        {
            //checks first if bingoCard is not null
            if (bingoCardList != null)
            {
                NumBingoCard.NumBingoCardDisplay(ConvertToMultiDArray(bingoCardList));

                Console.WriteLine();
                Console.WriteLine("+~~~~~~~~~~~~~~~~~~Result~~~~~~~~~~~~~~~~+");
                //after displaying the number bingo card, checks if it match with the user input
                foreach (var item in Program.listObj)
                    Console.WriteLine(NumBingoCard.CheckForNumBingo(item.Value.getUserInputs(), ConvertToMultiDArray(bingoCardList)));

                Console.WriteLine("+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+");
                Console.WriteLine();
            }
        }

        //helper method to Display Game over
        public static void DisplayGameOver()
        {
            Console.WriteLine("+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+");
            Console.WriteLine("+   Game over! Press any key to quit     +");
            Console.WriteLine("+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+");
        }

        //helper method to Display user's choosen numbers
        public static void DisplayUserInput(List<uint> userInputs)
        {
            Console.WriteLine("+---------------------------------------------------+");
            Console.WriteLine("|                My Choosen Numbers                 |");
            Console.WriteLine("|                                                   |");
            Console.Write("| ");
            userInputs.ForEach(e => Console.Write($"[{e.ToString("00")}] "));
            Console.Write("|");
            Console.WriteLine("\n+---------------------------------------------------+");
        }

        public void Update(List<uint[]> bingoCardList, List<uint> currentList, int counter, int nextId, bool over)
            {
                //updates the client
                Program.activeClientId = nextId;
                Program.gameOver = over;

                //display who is currently playing on each users
                DisplayCurrentPlayer(Program.activeClientId);

            //if the game is over, dispay the generated number bingo card on each user, and the result
            if (Program.gameOver)
                {
                DisplayGeneratedBingoCard(bingoCardList);

                DisplayGameOver();
                Program.handleWait.Set(); // Release all clients so they can exit out

            }


            else if (Program.activeClientId == Program.clientId)
                {
                    //prompt user to input numbers. then store it in an object called "NumberBingoCard"
                    List<uint> userInputs = Program.UserNumbersChoice();
                    NumBingoCard nb = new NumBingoCard();
                    nb.setUserInput(userInputs);
                    Program.listObj.Add(Program.clientId, nb);
                    DisplayUserInput(userInputs);

                Program.handleWait.Set();
                }
            }

        
    }
}
