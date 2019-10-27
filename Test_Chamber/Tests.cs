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
            Pok�mon charizard = CreateCharizard();
            Pok�mon blastoise = CreateBlastoise();

            blastoise.UseAbility(charizard, 0);
            charizard.UseAbility(blastoise, 0);

            Assert.IsTrue(charizard.Life < blastoise.Life);
        }

        [Test]
        public void AddToTournament_Works()
        {
            Tournament stadium = new Tournament();
            stadium.AddPok�monToTournament(5);
            Assert.AreEqual(5, stadium.Size());
        }

        [Test]
        public void CatchPok�mon_TournamentHasTwoOfTheSame_StillOnlyCatchesTheCatchable()
        {
            Tournament stadium = new Tournament();
            stadium.Pok�monInTournament.Add(CreateCharizard());
            stadium.Pok�monInTournament.Add(CreateCharizard());
            stadium.Pok�monInTournament.Add(CreatePikachu());
            stadium.Pok�monInTournament.Add(CreateCharizardNotCatchable());
            stadium.CatchPok�mon("Charizard");

            List<Pok�mon> caught = stadium.Pok�monInTournament.FindAll(pok�mon => pok�mon.Caught);


            Assert.IsTrue(caught.Count == 2 && caught[0].Name == "Charizard" && caught[1].Name == "Charizard");
        }

        [Test]
        public void CatchPok�mon_CatchAll_CatchesCorrectAmount()
        {
            Tournament stadium = new Tournament();
            stadium.Pok�monInTournament.Add(CreateBlastoise());
            stadium.Pok�monInTournament.Add(CreateCharizard());
            stadium.Pok�monInTournament.Add(CreatePikachu());
            stadium.Pok�monInTournament.Add(CreateCharizardNotCatchable());
            stadium.CatchPok�mon("All");

            List<Pok�mon> caught = stadium.Pok�monInTournament.FindAll(pok�mon => pok�mon.Caught);

            Assert.IsTrue(caught.Count == 2);
        }

        [Test]
        public void CatchPok�mon_SpecificDoesntExist_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            stadium.Pok�monInTournament.Add(CreateBlastoise());
            stadium.Pok�monInTournament.Add(CreateCharizard());

            string result = stadium.CatchPok�mon("Pikachu");

            Assert.AreEqual("You caught 0 Pok�mon named Pikachu. Try again when the Pok�mon are weaker", result);
        }

        [Test]
        public void LifeOfPok�mon_Works()
        {
            Tournament stadium = new Tournament();
            // Default life is 255
            stadium.Pok�monInTournament.Add(CreateBlastoise());
            stadium.Pok�monInTournament[0].Life = 133;
            stadium.AddPok�monToTournament(1);
            
            Assert.AreEqual(133, stadium.LifeOfPok�mon(1));
            Assert.AreEqual(255, stadium.LifeOfPok�mon(2));
        }
        [Test]
        public void LifeOfPok�mon_Pok�monDoesntExist_ThrowsException()
        {
            Tournament stadium = new Tournament();
            stadium.Pok�monInTournament.Add(CreateBlastoise());

            Assert.Throws<ArgumentOutOfRangeException>(() => stadium.LifeOfPok�mon(2));
        }

        [Test]
        public void NameOfPok�mon_Works()
        {
            Tournament stadium = new Tournament();
            stadium.Pok�monInTournament.Add(CreateBlastoise());

            Assert.AreEqual("Blastoise", stadium.NameOfPok�mon(1));
        }

        [Test]
        public void NameOfPok�mon_Pok�monDoesntExist_ThrowsException()
        {
            Tournament stadium = new Tournament();
            stadium.AddPok�monToTournament(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => stadium.NameOfPok�mon(2));
        }

        [Test]
        public void CheckWin_WonBattle_ReturnsCorrectString()
        {
            Tournament stadium = new Tournament();
            Pok�mon charizard = CreateCharizard();
            Pok�mon blastoise = CreateBlastoise();
            stadium.Pok�monInTournament.Add(blastoise);
            stadium.Pok�monInTournament.Add(charizard);
            stadium.AddPok�monToTournament(1);
            
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
            Pok�mon charizard = CreateCharizard();
            Pok�mon blastoise = CreateBlastoise();
            stadium.Pok�monInTournament.Add(blastoise);
            stadium.Pok�monInTournament.Add(charizard);

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
            Pok�mon charizard = CreateCharizard();
            Pok�mon blastoise = CreateBlastoise();
            stadium.Pok�monInTournament.Add(blastoise);
            stadium.Pok�monInTournament.Add(charizard);

            stadium.CatchPok�mon("all");

            Assert.AreEqual("Battle nullified", stadium.CheckWin());
        }

        [Test]
        public void CheckWin_WonBattle_RemovesTheLosingPok�mon()
        {
            Tournament stadium = new Tournament();
            Pok�mon charizard = CreateCharizard();
            Pok�mon blastoise = CreateBlastoise();
            Pok�mon pikachu = CreatePikachu();

            stadium.Pok�monInTournament.Add(blastoise);
            stadium.Pok�monInTournament.Add(charizard);
            stadium.Pok�monInTournament.Add(pikachu);
 
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
            Assert.IsTrue(stadium.NameOfPok�mon(1) == "Blastoise" && stadium.NameOfPok�mon(2) == "Pikachu");
        }

        [Test]
        public void SuddenDeath_Works()
        {

            Tournament stadium = new Tournament();
            stadium.AddPok�monToTournament(2);
            stadium.TransitionTo(new SuddenDeath());
            
            while (stadium.LifeOfPok�mon(1) == 255 && stadium.LifeOfPok�mon(2) == 255)
            {
                stadium.ExecuteNextMove();
            }

            Assert.IsTrue(stadium.LifeOfPok�mon(1) == 0 || stadium.LifeOfPok�mon(2) == 0);
        }

        [Test]
        public void NormalBattle_OnePok�monStandingInTheEnd()
        {

            Tournament stadium = new Tournament();
            stadium.AddPok�monToTournament(2);

            while (stadium.LifeOfPok�mon(1) > 1 || stadium.LifeOfPok�mon(2) > 1)
            {
                stadium.ExecuteNextMove();
            }
            
            Assert.IsTrue(stadium.CheckWin().Contains("won the tournament"));
        }

        Pok�mon CreateCharizard()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Fire breath", Element.Fire, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pok�mon charizard = new Pok�mon("Charizard", Element.Fire, Element.Water, abilities, 1.5f, 1.2f, 255);
            return charizard;
        }

        Pok�mon CreateCharizardNotCatchable()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Fire breath", Element.Fire, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pok�mon charizard = new Pok�mon("Charizard", Element.Fire, Element.Water, abilities, 1.5f, 1.2f, 1);
            return charizard;
        }

        Pok�mon CreateBlastoise()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Water whip", Element.Water, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pok�mon blastoise = new Pok�mon("Blastoise", Element.Water, Element.Electric, abilities, 1.5f, 1.2f, 255);
            return blastoise;
        }
        
        Pok�mon CreatePikachu()
        {
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(-50, false);
            Ability attack = new Ability("Thunder strike", Element.Electric, effectsOnSelf, effectsOnTarget, false);
            List<Ability> abilities = new List<Ability> { attack };
            Pok�mon pikachu = new Pok�mon("Pikachu", Element.Electric, Element.Fire, abilities, 1.5f, 1.2f, 1);
            return pikachu;
        }
    }
}