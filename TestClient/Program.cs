/*
 * Program:     TestClient.exe
 * Module:      Program.cs
 * Developer:   Dianne Corpuz
 * Date:        April 9, 2021
 * Summary:     A simple console Bingo game called "NumBingo"
 *              This game use System.Threading.EvenrWaitHandle that helps the player to wait for their turn, pause other client if it's not yet their turn
 *              This game use some functions and code from "WCF Example with callbacks and console client demonstrating 'turn-taking'"
 *              like a function that allows the client to "unregister" from the callbacks gracefully. And "unmanaged" region at the bottom was included 
 *  
 */

using System;
using System.Collections.Generic;
using TestLibrary;                      // Service contract and implementation
using System.ServiceModel;              // WCF types
using System.Threading;
using System.Runtime.InteropServices;   // DllImport()

namespace TestClient
{
    public class Program
    {
        private static IPlayerTurn player = null;           // service object reference
        public static int clientId, activeClientId = 0;     //client and active id of the client currently playing the game
        private static CallbackObject callbackObject = new CallbackObject();
        public static EventWaitHandle handleWait = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static bool gameOver = false;                //game over flag

        public static Dictionary<int, NumBingoCard> listObj = new Dictionary<int, NumBingoCard>();  //list of NumBingoCard/clients that will be used in the callbacks update


        static void Main(string[] args)
        {
            //prompt player if it can't connect or start the game, if there is no error with the service, start the game
            if (startGame())
            {
                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
                
                do
                {
                    handleWait.WaitOne();

                    if (gameOver)
                        Console.ReadKey();
                    else
                    {
                        Console.ReadLine();
                        player.NextPlayerTurn();
                        handleWait.Reset();
                    }


                } while (!gameOver);

                player.LeaveGame();
            }
            else
            {
                Console.WriteLine("ERROR: Unable to connect to the service!"); 
            }
        }

        //helper method to prompt player to input 10 numbers
        public static List<uint> UserNumbersChoice()
        {
            List<uint> numberList = new List<uint>();
                string userInput;
                uint inputnmr;

                const uint MIN = 1;
                const uint MAX = 25;
                const uint ALLOWED_NUMBERS = 10;

                Console.WriteLine("+~~~~~~~~~~~~~~ Your Turn ~~~~~~~~~~~~~+");
                Console.WriteLine($"\nChoose {ALLOWED_NUMBERS} numbers between {MIN} and {MAX}: ");
                bool isValid;
                for (int numberCounter = 0; numberCounter < ALLOWED_NUMBERS; ++numberCounter)
                {
                    isValid = false;
                    do
                    {
                        Console.Write($"[{(numberCounter + 1)}]: ");
                        userInput = Console.ReadLine();
                        if (uint.TryParse(userInput, out inputnmr))
                        {
                            if (inputnmr <= MAX && inputnmr >= MIN)
                            {
                                if (!numberList.Contains(inputnmr))
                                {
                                    numberList.Add(inputnmr);
                                    isValid = true;
                                }
                                else Console.WriteLine("Please choose another number!");
                            }
                            else Console.WriteLine($"Must Choose between  {MIN} and {MAX}!");
                        }
                        else Console.WriteLine("Invalid Number!");
                    }
                    while (!isValid);
                }

                Console.WriteLine();
            return numberList;
        }


       
        //connects and start the game
        private static bool startGame()
        {
            try
            {
                DuplexChannelFactory<IPlayerTurn> channel = new DuplexChannelFactory<IPlayerTurn>(callbackObject, "NumBingoEndpoint");
                player = channel.CreateChannel();

                clientId = player.JoinGame();  // Register for the callbacks 

                Console.WriteLine("+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+");
                Console.WriteLine("+              Welcome to NumBingo!               +");
                Console.WriteLine("+  A number bingo game developed by Dianne Corpuz +");
                Console.WriteLine("+~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~+");
                Console.WriteLine("");

                if (clientId == 1)
                {
                    HandleNumberOfPlayer(player);
                    callbackObject.Update(null, null, 1, 1, false); //callbacks update
                }
                else if (clientId > player.GetNumberOfPlayers()) //if client connected is more than the game allowed, prompt warning message
                    Console.WriteLine($"\n-------------WARNING: ONLY {player.GetNumberOfPlayers()} PLAYERS ALLOWED--------------");

                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //validate number of players input
        public static void HandleNumberOfPlayer(IPlayerTurn player)
        {
            string input;
            int playerMIN = 2;
            int playerMAX = 8;
            bool isValid = false;
            uint inputnmr;
            do
            {
                Console.Write($"Choose number of players [{playerMIN} - {playerMAX}]: ");
                input = Console.ReadLine();
                if (uint.TryParse(input, out inputnmr))
                {
                    if (inputnmr <= playerMAX && inputnmr >= playerMIN)
                    {
                        player.SetNumberPlayerAllowed(inputnmr);
                        isValid = true;
                    }
                    else Console.WriteLine($"Must Choose between  {playerMIN} and {playerMAX}!");
                }
                else Console.WriteLine("Invalid Number!");
            }
            while (!isValid);
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                    break;
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    player?.LeaveGame();
                    break;
            }
            return true;
        }

        #region unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]

        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages sent to the handler routine.
        public enum CtrlTypes 
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion

    }
}
