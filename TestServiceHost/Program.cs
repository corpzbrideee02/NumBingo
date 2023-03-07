/*
 * Program:     TestServiceHost.exe
 * Module:      Program.cs
 * Developer:   Dianne Corpuz
 * Date:        April 9, 2021
 * Summary:     Service host for Liibray PLayerTurn
 */

using System;
using TestLibrary; // Service contract and implementation
using System.ServiceModel;  // WCF types

namespace TestServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost servHost = null;

            try
            {
                servHost = new ServiceHost(typeof(PlayerTurn));

                // Run the service
                servHost.Open();
                Console.WriteLine("Service started. Please any key to quit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Wait for a keystroke
                Console.ReadKey();
                if (servHost != null)
                    servHost.Close();
            }
        }
    }
}
