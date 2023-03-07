/*
 * Program:     TestLibrary.dll
 * Module:      PlayerTurn.cs
 * Developer:   Dianne Corpuz
 * Date:        April 9, 2021
 * Summary:     Defines and implements the IPlayerTurn service contract
 */

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace TestLibrary
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Update(List<uint[]> bingoCardList, List<uint> userNumberChoice, int counter, int nextClient, bool gameOver);

    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IPlayerTurn
    {
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract(IsOneWay = true)]
        void NextPlayerTurn();
        [OperationContract]
        uint GetNumberOfPlayers();

        [OperationContract(IsOneWay = true)]
        void SetNumberPlayerAllowed(uint number);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PlayerTurn : IPlayerTurn
    {
        private int turnCount;                                   // tracks game turn count
        private Dictionary<int, ICallback> callbacks = null;    //  manage callback and id of each client
        private int nextClientId;                               // next client id who joins the game
        private int clientIndex;                                // next client index
        private bool gameOver = false;                          // game over flag
        private List<uint> currentUserInputs = new List<uint>(); //stores user inputs(numbers)
        private NumBingoCard numberCard = null;                  //     
        private uint[,] numBingoCard = null;                    //variable to store generated bingo card numbers

        private uint PLAYERS_ALLOWED; // number of users allowed to play the game

        // C'tor 
        public PlayerTurn()
        { 
            turnCount = nextClientId = 1; // Initialize the game players turns or the game
            clientIndex = 0;
            callbacks = new Dictionary<int, ICallback>();
            numberCard = new NumBingoCard();        
            numBingoCard = numberCard.GenerateNumbBingoCard(); //generate random number bingo card once, NOTE: all players share the same bingocard at the end of game
        }

        //Getters - Numbers of players allowed 
        public uint GetNumberOfPlayers()
        {
            return PLAYERS_ALLOWED;
        }

        //Setters - Numbers of players allowed 
        public void SetNumberPlayerAllowed(uint number)
        {
            PLAYERS_ALLOWED = number;
        }


        //ServiceContract method - Register callbacks
        public int JoinGame()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>(); // Identify which client is calling this method

            if (callbacks.ContainsValue(callback))
            {
                int id = callbacks.Values.ToList().IndexOf(callback); // if client is already registered, return the id
                return callbacks.Keys.ElementAt(id);
            }

            callbacks.Add(nextClientId, callback);// Register client and return a new clientId

            return nextClientId++;
        }


        //ServiceContract method - unregister from callbacks
        public void LeaveGame()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (callbacks.ContainsValue(callback))
            {
                // Identify which client is currently calling this method
                // - Get the index of the client within the callbacks collection
                int thisIndex = callbacks.Values.ToList().IndexOf(callback);
                // - Get the unique id of the client as stored in the collection
                int uniqueid = callbacks.ElementAt(thisIndex).Key;

                // Remove this client from receiving callbacks from the service
                callbacks.Remove(uniqueid);

                // Make sure the counting sequence isn't disrupted by removing this client
                if (thisIndex == clientIndex)
                    // This client was supposed to count next but is exiting the game
                    // Need to signal the next client to count instead 
                    updateAllClients();
                else if (clientIndex > thisIndex)

                    // This prevents a player from being "skipped over" in the turn-taking of this "game"
                    clientIndex--;
            }
        }


        //ServiceContract method
        public void NextPlayerTurn()
        {
            clientIndex = ++clientIndex % callbacks.Count; //get index of the next client

            if (++turnCount == (PLAYERS_ALLOWED+1)) // this determine if the game is over
                gameOver = true;

            updateAllClients();  // Update all clients

        }

        //helper method that convert multidimensional array to List of unsigned integers array
        //this is needed because callbacks doesn't accept multidimensional array
        public static List<uint[]> MultiDArrayToList(uint[,] numBingoCard)
        {
            int count = 0;
            //LINQ was used to convert it to list
            return numBingoCard.Cast<uint>()
                               .GroupBy(x => count++ / numBingoCard.GetLength(1))
                               .Select(card => card.ToArray())
                               .ToList();
        }

        // Helper method that invokes the callback method in each client
        private void updateAllClients()
        {
            foreach (ICallback cb in callbacks.Values)
            {
                cb.Update(MultiDArrayToList(numBingoCard), currentUserInputs, turnCount, callbacks.Keys.ElementAt(clientIndex), gameOver);

            }
        }


    }
}
