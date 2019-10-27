using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test_Chamber")]
namespace DataModel
{
    // The State-Datamodel structure of these classes are inspired by the following example code: 
    // https://refactoring.guru/design-patterns/state/csharp/example
    public class Tournament
    {
        private State state;
        internal int battleMoves = 0;
        private PokémonCreator pc = new PokémonCreator();
        internal List<Pokémon> PokémonInTournament { get; set; } = new List<Pokémon>();

        /// <summary>
        /// Returns the amount of Pokémon in the tournament.
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return PokémonInTournament.Count;
        }

        /// <summary>
        /// Returns the life value of a Pokémon in the tournament.  
        /// </summary>
        /// <param name="pokémonNr"></param>
        /// <returns></returns>
        public byte LifeOfPokémon(int pokémonNr)
        {
            // If the nr is wrong it should throw a out-of-bounds exception,
            // and I figure that the List-class is already programmed to do so.
            return PokémonInTournament[pokémonNr - 1].Life;
        }

        /// <summary>
        /// Returns the name of a Pokémon in the tournament.
        /// </summary>
        /// <param name="pokémonNr"></param>
        /// <returns></returns>
        public string NameOfPokémon(int pokémonNr)
        {
            return PokémonInTournament[pokémonNr - 1].Name;
        }

        public void AddPokémonToTournament(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Pokémon newPokémon = pc.CreatePokémon("Random");
                PokémonInTournament.Add(newPokémon);
            }
        }

        public string CatchPokémon(string name)
        {
            string whatHappened = "";
            List<Pokémon> caught;

            if (name.ToLower() == "all")
            {
                // Find all Pokémon with low enough life and set them to caught. 
                caught = PokémonInTournament.FindAll(pokémon =>
                {
                    if (pokémon.Life <= pokémon.CatchableAt)
                    {
                        pokémon.Caught = true;
                        return true;
                    }
                    return false;
                });

                whatHappened = $"You caught {caught.Count} Pokémon. ";
            }
            else
            {
                caught = PokémonInTournament.FindAll(pokémon =>
                {
                    // If the name of the pokémon matches the name we are looking for AND that pokémon is catchable then...
                    if (pokémon.Name.ToLower() == name.ToLower() && pokémon.Life <= pokémon.CatchableAt)
                    {
                        pokémon.Caught = true;
                        return true;
                    }
                    return false;
                });
                
                whatHappened = $"You caught {caught.Count} Pokémon named {name}. ";
            }

            if (caught.Count == 0)
            {
                whatHappened += "Try again when the Pokémon are weaker";
            }

            return whatHappened;
        }

        /// <summary>
        /// Deals with the after math of a battle move. If someone defeates the other one then it returns a string about who won. If nothing happened then it returns null.
        /// </summary>
        /// <returns></returns>
        public string CheckWin()
        {
            string whatHappened = null;
            string message = "has won the battle! Restoring stats...";

            if (PokémonInTournament[0].Caught && PokémonInTournament[1].Caught)
            {
                PokémonInTournament.RemoveAll(pokémon => pokémon.Caught);
                whatHappened = "Battle nullified";
            }
            else if (PokémonInTournament[0].Life == 0 || PokémonInTournament[0].Caught)
            {
                // Return what happened, restore the life of the winning Pokémon and remove the loser. 
                whatHappened = $"{PokémonInTournament[1].Name} {message}";
                PokémonInTournament[1].Restore();
                PokémonInTournament.RemoveAt(0);
            }
            else if (PokémonInTournament[1].Life == 0 || PokémonInTournament[1].Caught)
            {
                whatHappened = $"{PokémonInTournament[0].Name} {message}";
                PokémonInTournament[0].Restore();
                PokémonInTournament.RemoveAt(1);
            }

            if (PokémonInTournament.Count == 1)
            {
                whatHappened = $"{PokémonInTournament[0].Name} has won the tournament";
                PokémonInTournament.Clear();
                battleMoves = 0;
            }

            return whatHappened;
        }

        /// <summary>
        /// Use this to switch to sudden death mode. This mode will go back to normal mode onces a Pokémon has been defeated. 
        /// </summary>
        /// <param name="state"></param>
        public void TransitionTo(State state)
        {
            // Set the desired state to this class "The context" then update the State class with the
            // new context. The context and the state must stay connected. 
            this.state = state;
            this.state.SetContext(this);
        }

        public Tournament()
        {
            // This way the tournament always starts with normal battles. 
            this.TransitionTo(new NormalBattle());
        }

        public string ExecuteNextMove()
        {
            // Using the NextMove() of the current state "state.NextMove()" so that it executes the correct one.  
            return this.state.NextMove(PokémonInTournament[0], PokémonInTournament[1]);
        }
    }

    public abstract class State
    {
        protected Tournament tournament;
        internal void SetContext(Tournament tournament)
        {
            this.tournament = tournament;
        }
        internal abstract string NextMove(Pokémon pokémon1, Pokémon pokémon2);
    }

    public class NormalBattle : State
    {
        internal override string NextMove(Pokémon pokémon1, Pokémon pokémon2)
        {
            // This if-statement deals with whose turn it is based on the number of moves that has been done. 
            if (tournament.battleMoves % 2 == 0)
            {
                string whatHappened = pokémon1.UseRandomAbility(pokémon2);
                tournament.battleMoves++;
                return whatHappened;
            }
            else
            {
                string whatHappened = pokémon2.UseRandomAbility(pokémon1);
                tournament.battleMoves++;
                return whatHappened;
            }
        }
    }

    public class SuddenDeath : State
    {
        internal override string NextMove(Pokémon pokémon1, Pokémon pokémon2)
        {
            if (tournament.battleMoves % 2 == 0)
            {
                byte before = pokémon2.Life;
                string whatHappened = pokémon1.UseRandomAbility(pokémon2);
                byte after = pokémon2.Life;
                 
                // If the current Pokémon actually attacked it's opponent.
                // Then the battle is over in sudden death. 
                if (after < before)
                {
                    pokémon2.Life = 0;
                    tournament.TransitionTo(new NormalBattle());
                }
                else
                {
                    tournament.battleMoves++;
                }

                
                return whatHappened;
            }
            else
            {
                byte before = pokémon1.Life;
                string whatHappened = pokémon2.UseRandomAbility(pokémon1);
                byte after = pokémon1.Life;

                if (after < before)
                {
                    pokémon1.Life = 0;
                    tournament.TransitionTo(new NormalBattle());
                }
                else
                {
                    tournament.battleMoves++;
                }

                return whatHappened;
            }
        }
    }
}

