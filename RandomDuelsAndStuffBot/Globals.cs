using DSharpPlus.Entities;
using System;
using System.Text.RegularExpressions;

namespace RandomDuelsAndStuffBot
{
    // globals, generally used stuff and references
    public static class Globals
    {
        private static readonly Regex _whitespace = new(@"\s+");

        public static string RemoveWhitespace(this string s)
        {
            return _whitespace.Replace(s, string.Empty);
        }

        public static Random Rng { get; } = new();

        // NL = NewLine
        public static string NL { get; } = Environment.NewLine;

        public static DiscordColor ColorGreen { get; } = new(0x37FF77);
        public static DiscordColor ColorRed { get; } = new(0xFF2D00);
        public static DiscordColor ColorBlue { get; } = new(0x00BFFF);

        public static DiscordEmbedBuilder AppendToDescription(this DiscordEmbedBuilder builder, string toAppend)
        {
            builder.Description += NL + toAppend;
            return builder;
        }
    }
}
