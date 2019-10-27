using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DataModel
{
    public class Pokémon
    {
        internal string Name { get; private set; }
        internal Element Type { get; private set; }
        internal Element WeakTo { get; private set; }
        internal List<Ability> Abilities { get; set; }
        internal byte Life { get; set; } = 255;
        internal float Defence { get; set; }
        internal float Strength { get; set; }
        internal byte CatchableAt { get; set; }
        internal bool Caught { get; set; }
        internal bool Stunned { get; set; } = false;

        // So that the pokémon won't just stun or heal all the time and waste our time they always have to use
        // an attack after such an ability. This makes that possible. 
        internal bool RecentlyUsedSpecial { get; set; }

        /// <summary>
        /// Performs a specific ability
        /// </summary>
        /// <param name="targetPokémon"></param>
        /// <param name="abilityNr"></param>
        /// <returns></returns>
        internal string UseAbility(Pokémon targetPokémon, int abilityNr)
        {
            if (abilityNr < 0 || abilityNr >= Abilities.Count)
            {
                throw new IndexOutOfRangeException("Ability index was out of range of the list of Abilities");
            }
            return Abilities[abilityNr].Execute(this, targetPokémon);
        }

        /// <summary>
        /// Performs a random ability
        /// </summary>
        /// <param name="targetPokémon"></param>
        /// <returns></returns>
        internal string UseRandomAbility(Pokémon targetPokémon)
        {
            int abilityNr = 0;

            if (RecentlyUsedSpecial)
            {
                // Find all abilities that aren't special abilities and execute one of them. 
                var allowedAbilities = this.Abilities.FindAll(ability => !ability.IsSpecial);
                abilityNr = Common.random.Next(0, allowedAbilities.Count);
                this.RecentlyUsedSpecial = false;
                return allowedAbilities[abilityNr].Execute(this, targetPokémon);
            }
            else
            {
                abilityNr = Common.random.Next(0, this.Abilities.Count);

                if (Abilities[abilityNr].IsSpecial)
                {
                    this.RecentlyUsedSpecial = true;
                }

                return Abilities[abilityNr].Execute(this, targetPokémon);
            }
            
            
        }

        /// <summary>
        /// Restores the life of the Pokémon. Recommended after every battle. 
        /// </summary>
        internal void Restore()
        {
            this.Life = 255;
        }

        // abilities = null?! When creating templates for Pokémon (Check "PokémonCreator") you want to 
        // skip a number of class-members. Hence the need for optional parameters. And since you can't 
        // create a new list withing the parameters, that needed to be done within the constructor. 
        public Pokémon(string name, Element type, Element weakTo, List<Ability> abilities = null, float defence = 1, float strength = 1, byte catchableAt = 100)
        {
            Name = name;
            Type = type;
            WeakTo = weakTo;
            if (abilities == null)
            {
                Abilities = new List<Ability>();
            }
            else
            {
                Abilities = abilities;
            }
            Defence = defence;
            Strength = strength;
            CatchableAt = catchableAt;
        }
    }

    public class Ability
    {
        internal string Name { get; private set; }
        internal Element Type { get; private set; }
        internal Effects EffectsOnSelf { get; set; }
        internal Effects EffectsOnTarget { get; set; }
        internal Color Color { get; private set; }
        internal bool IsSpecial { get; private set; }

        // Every ability can be shown with a color and that color should be locked to the type of the ability.
        // This locks an element with a color. 
        private static Dictionary<Element, Color> colors = new Dictionary<Element, Color> {
            { Element.Fire, Color.OrangeRed },
            { Element.Water, Color.Aqua },
            { Element.Electric, Color.Yellow },
            { Element.None, Color.LawnGreen },
        };

        /// <summary>
        /// Calculates the new life value after an attack or heal. 
        /// </summary>
        /// <returns></returns>
        internal byte NewLifeValue(Pokémon acting, Pokémon target, Effects effect)
        {
            // An attack has a LifeEffectValue that is a negative number, while heal has a positive one.
            // Because of math a heal-ability and an attack can be calculated the same way in this case. 
            // For example 2 + -2 = 0 while 2 + 2 = 4.

            float fullEffect;

            // If the ability is a healing ability or an attack the the target is weak to, then ignore defence. 
            if (target.WeakTo == this.Type || effect.Life > 0)
            {
                fullEffect = effect.Life * acting.Strength;
            }
            else
            {
                fullEffect = (effect.Life * acting.Strength) / target.Defence;
            }

            int lifeAfterEffect = target.Life + (int)fullEffect;
                     
            // Sees to that the returned result is never below 0 and never higher than 255. Because byte. 
            return (byte)Math.Max(0, Math.Min(255, lifeAfterEffect));
        }

        internal string Execute(Pokémon acting, Pokémon target)
        {
            if (acting.Stunned == true)
            {
                acting.Stunned = false;
                return $"{acting.Name} is stunned";
            }

            // Dealing wwith eventual effects of ability on the pokémon who used it. 
            acting.Life = NewLifeValue(acting, acting, this.EffectsOnSelf);
            acting.Stunned = this.EffectsOnSelf.Stun;

            // Dealing with eventual effects of ability on the target. 
            target.Life = NewLifeValue(acting, target, this.EffectsOnTarget);
            target.Stunned = this.EffectsOnTarget.Stun;

            return $"{acting.Name} used {this.Name.Pastel(this.Color)}";
        }

        public Ability(string name, Element type, Effects effectsOnSelf = null, Effects effectsOnTarget = null, bool isSpecial = false)
        {
            Name = name;
            Type = type;
            Color = colors[type];
            EffectsOnSelf = effectsOnSelf;
            EffectsOnTarget = effectsOnTarget;
            IsSpecial = isSpecial;
        }

        /// <summary>
        /// Checks whether the current ability already exists in a given list of abilities. 
        /// </summary>
        /// <param name="abilityList"></param>
        /// <returns></returns>
        internal bool ExistsIn(List<Ability> abilityList)
        {
            string thisName = this.Name;
            bool result = false;

            foreach (Ability ability in abilityList)
            {
                if (thisName == ability.Name)
                {
                    result = true;
                }
            }

            return result;
        }
    }

    // Currently an ability can affect life and stun. 
    public class Effects
    {
        public sbyte Life { get; private set; }
        public bool Stun { get; private set; }
        public Effects(sbyte life, bool stun)
        {
            Life = life;
            Stun = stun;
        }
    }
}

