using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test_Chamber")]
namespace DataModel
{
    // Templates and elements all free to expand of course.
    public enum Element
    {
        Fire,
        Water,
        Electric,
        Ground,
        None
    }

    public class PokémonCreator
    {
        private List<Pokémon> GetPokémonTemplates()
        {
            List<Pokémon> pokéTemplates = new List<Pokémon>
            {
            new Pokémon("Blastoise", Element.Water, Element.Electric),
            new Pokémon("Squirtle", Element.Water, Element.Electric),
            new Pokémon("Psyduck", Element.Water, Element.Electric),
            new Pokémon("Charizard", Element.Fire, Element.Water),
            new Pokémon("Flareon", Element.Fire, Element.Water),
            new Pokémon("Rapidash", Element.Fire, Element.Water),
            new Pokémon("Pikachu", Element.Electric, Element.Ground),
            new Pokémon("Zapdos", Element.Electric, Element.Ground),
            new Pokémon("Electabuzz", Element.Electric, Element.Ground)
            };

            return pokéTemplates;
        }


        internal Pokémon CreatePokémon(string name)
        {
            // Deciding what Pokémon to get. 
            var pokéTemplates = GetPokémonTemplates();

            // IndexOf() returns -1 if it isn't found so we might aswell go with that here too.
            int nrInList = -1;

            if (name == "Random")
            {
                nrInList = Common.random.Next(0, pokéTemplates.Count);
            }
            else
            {
                nrInList = pokéTemplates.IndexOf(pokéTemplates.Find(p => p.Name == name));

                if (nrInList == -1)
                {
                    throw new InvalidOperationException("There is no such Pokémon in this list");
                }
            }

            Pokémon newPokémon = pokéTemplates[nrInList];


            // Randomizing and assigning stats of pokémon
            float defence = (float)Common.random.NextDouble();
            // +1 because the NextDouble() returns a value from 0.0 to 1.0. I need it to be between 1.0 and 2.0
            defence += 1;

            float strength = (float)Common.random.NextDouble();
            strength += 1;

            // Don't want it to be catchable att full life and if life is 0 it's dead so...
            byte catchableAt = (byte)Common.random.Next(1, 246);

            newPokémon.Defence = defence;
            newPokémon.Strength = strength;
            newPokémon.CatchableAt = catchableAt;


            // Assign abilities to Pokémon. Two attacks and one uncategorized ability.
            AttackCreator attack = new AttackCreator();
            OtherAbilityCreator other = new OtherAbilityCreator();

            newPokémon.Abilities.Add(attack.CreateAbility(newPokémon));
            newPokémon.Abilities.Add(attack.CreateAbility(newPokémon));
            newPokémon.Abilities.Add(other.CreateAbility(newPokémon));

            return newPokémon;
        }
    }

    public abstract class AbilityCreator
    {
        public AbilityCreator()
        {

        }

        // Different types of abilities require different kinds of templates. So this needs to be abstract. 
        protected abstract List<Ability> GetAbilityTemplates();

        // This is abstract too because the effects are applied differently depending on the ability. 
        // Notice that this method is used in the CreateAbility() method. This way I don't have to repeat
        // unnecessary code. 
        protected abstract Ability ApplyEffects(Ability ability);
        internal Ability CreateAbility(Pokémon pokémon)
        {
            // The new ability for a fire type cannot be a water attack, nor an ability that is already added.
            List<Ability> approvedAbilities = GetAbilityTemplates().FindAll(a => a.Type == pokémon.Type || a.Type == Element.None);
            approvedAbilities = approvedAbilities.FindAll(a => !a.ExistsIn(pokémon.Abilities));
            int abilityNr = Common.random.Next(0, approvedAbilities.Count);

            Ability ability = approvedAbilities[abilityNr];
            ability = ApplyEffects(ability);

            return ability;
        }
    }

    // Creator for normal attacks
    public class AttackCreator : AbilityCreator
    {
        public AttackCreator() : base()
        {

        }
        protected override List<Ability> GetAbilityTemplates()
        {
            List<Ability> abilityTemplates = new List<Ability>
            {
            new Ability ("Fire breath", Element.Fire),
            new Ability ("Fire punch", Element.Fire),
            new Ability ("Water blast", Element.Water),
            new Ability ("Water whip", Element.Water),
            new Ability ("Lightning strike", Element.Electric),
            new Ability ("Electricity bolt", Element.Electric),
            };

            return abilityTemplates;
        }

        protected override Ability ApplyEffects(Ability ability)
        {
            sbyte effectOnLife = (sbyte)Common.random.Next(-50, -9);
            Effects effectsOnSelf = new Effects(0, false);
            Effects effectsOnTarget = new Effects(effectOnLife, false);

            ability.EffectsOnSelf = effectsOnSelf;
            ability.EffectsOnTarget = effectsOnTarget;

            return ability;
        }
    }

    // Other Ability Creator
    public class OtherAbilityCreator : AbilityCreator
    {
        public OtherAbilityCreator() : base()
        {

        }
        protected override List<Ability> GetAbilityTemplates()
        {
            EffectsTemplates effects = new EffectsTemplates();

            // These are already full attacks with the help of effects templates. This needs to be the case
            // since they are so different from each other.
            List<Ability> abilityTemplates = new List<Ability>
            {
            new Ability ("Stun", Element.None, effects.CreateNoEffect(), effects.CreateStunEffect(), true),
            new Ability ("Heal", Element.None, effects.CreateHealEffect(), effects.CreateNoEffect(), true),
            new Ability ("Kamikaze", Element.None, effects.CreateHurtEffect(), effects.CreateHurtEffect(), true)
            };

            return abilityTemplates;

        }

        protected override Ability ApplyEffects(Ability ability)
        {
            return ability;
        }
    }


    public class EffectsTemplates
    {
        // I didn't have these in methods first but I needed the random number to be 
        // different every time it created an effect. 
        internal Effects CreateHurtEffect()
        {
            sbyte random = (sbyte)Common.random.Next(-50, -9);
            return new Effects(random, false);
        }
        internal Effects CreateHealEffect()
        {
            sbyte random = (sbyte)Common.random.Next(10, 51);
            return new Effects(random, false);
        }
        internal Effects CreateNoEffect()
        {
            return new Effects(0, false);
        }

        internal Effects CreateStunEffect()
        {
            return new Effects(0, true);
        }
    }
}

