using System;
using System.Collections.Generic;

namespace Juniper
{
    public class EmojiGroup : Emoji
    {
        private static readonly Random rand = new Random();

        public IReadOnlyList<Emoji> Alts { get; }

        /// <summary>
        /// Groupings of Unicode-standardized pictograms.
        /// </summary>
        /// <param name="value">a Unicode sequence</param>
        /// <param name="desc">an English text description of the pictogram</param>
        /// <param name="alts">Emojis in this group</param>
        public EmojiGroup(string value, string desc, params Emoji[] alts)
            : base(value, desc)
        {
            Alts = alts;
        }

        public EmojiGroup(Emoji archetype, params Emoji[] alts)
            : base(archetype)
        {
            Alts = alts;
        }

        /// <summary>
        /// Selects a random emoji out of the collection.
        /// </summary>
        /// <returns></returns>
        public override Emoji Random()
        {
            if (Alts.Count == 0)
            {
                return base.Random();
            }

            var idx = rand.Next(Alts.Count);

            var selection = Alts[idx];
            if (selection is EmojiGroup group)
            {
                return group.Random();
            }
            else
            {
                return selection;
            }
        }

        public override bool Contains(Emoji e)
        {
            if (base.Contains(e))
            {
                return true;
            }
            else
            {
                foreach (var alt in Alts)
                {
                    if (alt.Contains(e))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}