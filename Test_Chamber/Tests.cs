using DataModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Test_Chamber
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Ability_WeakToAttack_TakesMoreDamage()
        {
            Pokémon charizard = CreateCharizard();
            Pokémon blastoise = CreateBlastoise();

            blastoise.UseAbility(charizard, 0);
            charizard.UseAbility(blastoise, 0);

            Assert.IsTrue(charizard.Life < blastoise.Life);
        }

        [Test]
        public void AddToTournament_Works()
        {
            Tournament stadium = new Tournament();
            stadium.AddPokémonToTournament(5);
            Assert.AreEqual(5, stadium.Size());
        }

        [Test]
        public void CatchPokémon_TournamentHasTwoOfTheSame_StillOnlyCatchesTheCatchable()
        {
            Tournament stadium = new Tournament();
            stadium.PokémonInTournament.Add(CreateCharizard());
            stadium.PokémonInTournament.Add(CreateCharizard());
            stadium.PokémonInTournament.Add(CreatePikachu());
            stadium.PokémonInTournament.Add(CreateCharizardNotCatchable());
            stadium.CatchPokémon("Charizard");

            List<Pokémon> caught = stadium.PokémonInTournament.FindAll(pokémon => pokémon.Caught);


            Assert.IsTrue(caught.Count == 2 && caught[0].Name == "Charizard" && caught[1].Name == "Charizard");
        }

        [Test]
        public void CatchPokémon_CatchAll_CatchesCorrectAmount()
        {
            Tournament stadium = new Tournament();
            stadium.PokémonInTournament.Add(CreateBlastoise());
            stadium.PokémonInTournament.Add(CreateCharizard());
            stadium.PokémonInTournament.Add(CreatePikachu());
            stadium.PokémonInTournament.Add(CreateCharizardNotCatchable());
            stadium.CatchPokémon("All");

            List<Pokémon> caught = stadium.PokémonInTournament.FindAll(pokémon => pokémon.Caught);

            Assert.IsTrue(caught.Count == 2);
        }

        [Test]
        public void CatchPokémon_SpecificDoesntExist_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            stadium.PokémonInTournament.Add(CreateBlastoise());
            stadium.PokémonInTournament.Add(CreateCharizard());

            string result = stadium.CatchPokémon("Pikachu");

            Assert.AreEqual("You caught 0 Pokémon named Pikachu. Try again when the Pokémon are weaker", result);
        }

        [Test]
        public void LifeOfPokémon_Works()
        {
            Tournament stadium = new Tournament();
            // Default life is 255
            stadium.PokémonInTournament.Add(CreateBlastoise());
            stadium.PokémonInTournament[0].Life = 133;
            stadium.AddPokémonToTournament(1);
            
            Assert.AreEqual(133, stadium.LifeOfPokémon(1));
            Assert.AreEqual(255, stadium.LifeOfPokémon(2));
        }
        [Test]
        public void LifeOfPokémon_PokémonDoesntExist_ThrowsException()
        {
            Tournament stadium = new Tournament();
            stadium.PokémonInTournament.Add(CreateBlastoise());

            Assert.Throws<ArgumentOutOfRangeException>(() => stadium.LifeOfPokémon(2));
        }

        [Test]
        public void NameOfPokémon_Works()
        {
            Tournament stadium = new Tournament();
            stadium.PokémonInTournament.Add(CreateBlastoise());

            Assert.AreEqual("Blastoise", stadium.NameOfPokémon(1));
        }

        [Test]
        public void NameOfPokémon_PokémonDoesntExist_ThrowsException()
        {
            Tournament stadium = new Tournament();
            stadium.AddPokémonToTournament(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => stadium.NameOfPokémon(2));
        }

        [Test]
        public void CheckWin_WonBattle_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            Pokémon charizard = CreateCharizard();
            Pokémon blastoise = CreateBlastoise();
            stadium.PokémonInTournament.Add(blastoise);
            stadium.PokémonInTournament.Add(charizard);
            stadium.AddPokémonToTournament(1);
            
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);

            Assert.AreEqual("Blastoise has won the battle! Restoring stats...", stadium.CheckWin());
        }

        [Test]
        public void CheckWin_WonTournament_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            Pokémon charizard = CreateCharizard();
            Pokémon blastoise = CreateBlastoise();
            stadium.PokémonInTournament.Add(blastoise);
            stadium.PokémonInTournament.Add(charizard);

            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);

            Assert.AreEqual("Blastoise has won the tournament", stadium.CheckWin());
        }

        [Test]
        public void CheckWin_BothWereCaught_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            Pokémon charizard = CreateCharizard();
            Pokémon blastoise = CreateBlastoise();
            stadium.PokémonInTournament.Add(blastoise);
            stadium.PokémonInTournament.Add(charizard);

            stadium.CatchPokémon("all");

            Assert.AreEqual("Battle nullified", stadium.CheckWin());
        }

        [Test]
        public void CheckWin_WonBattle_RemovesTheLosingPokémon()
        {
            Tournament stadium = new Tournament();
            Pokémon charizard = CreateCharizard();
            Pokémon blastoise = CreateBlastoise();
            Pokémon pikachu = CreatePikachu();

            stadium.PokémonInTournament.Add(blastoise);
            stadium.PokémonInTournament.Add(charizard);
            stadium.PokémonInTournament.Add(pikachu);
 
            int before = stadium.Size();

            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);
            blastoise.UseAbility(charizard, 0);

            stadium.CheckWin();

            int after = stadium.Size();

            // Since Charizard should have been removed these are the expected results. 
            Assert.IsTrue(before > after && after == 2);
            Assert.IsTrue(stadium.NameOfPokémon(1) == "Blastoise" && stadium.NameOfPokémon(2) == "Pikachu");
        }

        [Test]
        public void SuddenDeath_Works()
        {

            Tournament stadium = new Tournament();
            stadium.AddPokémonToTournament(2);
            stadium.TransitionTo(new SuddenDeath());
            
            while (stadium.LifeOfPokémon(1) == 255 && stadium.LifeOfPokémon(2) == 255)
            {
                stadium.ExecuteNextMove();
            }

            Assert.IsTrue(stadium.LifeOfPokémon(1) == 0 || stadium.LifeOfPokémon(2) == 0);
        }

        [Test]
        public void NormalBattle_OnePokémonStandingInTheEnd()
        {

            Tournament stadium = new Tournament();
            stadium.AddPokémonToTournament(2);

            while (stadium.LifeOfPokémon(1) > 1 || stadium.LifeOfPokémon(2) > 1)
            {
                stadium.ExecuteNextMove();
            }
            
            Assert.IsTrue(stadium.CheckWin().Contains("won the tournament"));
        }

        Pokémon CreateCharizard()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Fire breath", Element.Fire, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pokémon charizard = new Pokémon("Charizard", Element.Fire, Element.Water, abilities, 1.5f, 1.2f, 255);
            return charizard;
        }

        Pokémon CreateCharizardNotCatchable()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Fire breath", Element.Fire, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pokémon charizard = new Pokémon("Charizard", Element.Fire, Element.Water, abilities, 1.5f, 1.2f, 1);
            return charizard;
        }

        Pokémon CreateBlastoise()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Water whip", Element.Water, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pokémon blastoise = new Pokémon("Blastoise", Element.Water, Element.Electric, abilities, 1.5f, 1.2f, 255);
            return blastoise;
        }
        
        Pokémon CreatePikachu()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Thunder strike", Element.Electric, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pokémon pikachu = new Pokémon("Pikachu", Element.Electric, Element.Fire, abilities, 1.5f, 1.2f, 1);
            return pikachu;
        }
    }
}