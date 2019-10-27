using System;
using System.Collections.Generic;
using DataModel;
using ConsoleSimulationEngine2000;
using System.Threading.Tasks;
using Pastel;
using System.Drawing;
using System.Text;

namespace Simulation_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = new TextInput();
            var gui = new ConsoleGUI() { Input = input };
            var sim = new MySimulation(gui, input);
            await gui.Start(sim);

            Console.ReadKey();
        }
    }

    public class MySimulation : Simulation
    {
        private RollingDisplay log = new RollingDisplay(0, 0, -1, 12);
        private BorderedDisplay clockDisplay = new BorderedDisplay(0, 11, 20, 3) { };
        private BorderedDisplay messageDisplay = new BorderedDisplay(20, 11, 60, 3) { };
        private BorderedDisplay pokémon1 = new BorderedDisplay(0, 14, 40, 8) { };
        private BorderedDisplay pokémon2 = new BorderedDisplay(40, 14, 40, 8) { };
        private readonly ConsoleGUI gui;
        private readonly TextInput input;
        public Tournament tournament = new Tournament();

        public override List<BaseDisplay> Displays => new List<BaseDisplay>() {
        log,
        messageDisplay,
        clockDisplay,
        pokémon1,
        pokémon2,
        input.CreateDisplay(0, -3, -1) };

        public MySimulation(ConsoleGUI gui, TextInput input)
        {
            this.gui = gui;
            this.input = input;
        }

        /// <summary>
        /// Creates a lifebar based on life value. 
        /// </summary>
        /// <param name="life"></param>
        /// <returns></returns>
        public string UpdateLifeBar(byte life)
        {
            if (life == 0)
            {
                return "";
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string upperBeginning = "╔";
                string lowerBeginning = "╚";
                string upperEnd = "╗";
                string lowerEnd = "╝";
                string middle = "═";
                int length = life / 8;

                builder.Append(upperBeginning);

                for (int i = 0; i < length; i++)
                {
                    builder.Append($"{middle}");
                }

                builder.Append($"{upperEnd}\r\n");
                builder.Append(lowerBeginning);

                for (int i = 0; i < length; i++)
                {
                    builder.Append($"{middle}");
                }

                builder.Append($"{lowerEnd}\r\n");

                return builder.ToString().Pastel(Color.FromArgb(255, life, life));
            }
        }

        /// <summary>
        /// Reads the input and reacts to it. 
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(string command)
        {
            // If there are no Pokémon in the tournament the program assumes that you want to add Pokémon. 
            if (tournament.Size() < 1 && Int32.TryParse(command, out int result))
            {
                tournament.AddPokémonToTournament(result);
            }
            else if (tournament.Size() > 1)
            {
                if (command.ToLower().Contains("sudden") || command.ToLower().Contains("death") || command.ToLower() == "sd")
                {
                    tournament.TransitionTo(new SuddenDeath());
                }
                // If you didn't envoke sudden death the program assumes that you are trying to catch Pokémon. 
                // Because that the only command left. 
                else
                {
                    log.Log(tournament.CatchPokémon(command.ToLower()));
                }
            }
        }

        void UpdateMessageDisplay()
        {
            if (tournament.Size() < 1)
            {
                messageDisplay.Value = "Enter the amount of Pokémon you want in the tournament";
            }
            else if (tournament.Size() > 1)
            {
                messageDisplay.Value = "Write name of Pokémon or 'all' to catch";
            }
        }

        public override void PassTime(int deltaTime)
        {
            if (tournament.Size() > 1)
            {
                log.Log(tournament.ExecuteNextMove());

                // Updating the values of the Pokémon boxes. 
                string name1 = tournament.NameOfPokémon(1) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                string name2 = tournament.NameOfPokémon(2) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                pokémon1.Value = name1 + UpdateLifeBar(tournament.LifeOfPokémon(1));
                pokémon2.Value = name2 + UpdateLifeBar(tournament.LifeOfPokémon(2));

               string whoWon = tournament.CheckWin();

                if (whoWon != null)
                {
                    log.Log(whoWon);
                }
            }

            UpdateMessageDisplay();
            clockDisplay.Value = DateTime.Now.ToString("HH:mm:ss");

            while (input.HasInput)
            {
                string command = input.Consume();
                log.Log("Input: " + command);
                ExecuteCommand(command);
            }
        }
    }
}


