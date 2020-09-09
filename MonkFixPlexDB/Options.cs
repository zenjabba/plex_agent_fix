using System;
using CommandLine;

namespace MonkFixPlexDB
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('y', "dryrun", Required = false, HelpText = "Dry run don't actually perform any operations.")]
        public bool Dryrun { get; set; }

        [Option('s', "sectionremove", Required = false, HelpText = "Remove By Section Priority")]
        public bool SectionRemove { get; set; }

        [Option('u', "unmatchfix", Required = false, HelpText = "Fix Unmatched Files By Section")]
        public bool UnmatchFix { get; set; }

        [Option('a', "agentfix", Required = false, HelpText = "Fix Mismatched Agent Files By Section")]
        public bool AgentFix { get; set; }

        [Option('r', "remove", Required = false, HelpText = "Remove Plex Entries That Aren't In The File System")]
        public bool Remove { get; set; }

        [Option('p', "apiremove", Required = false, HelpText = "Remove Plex Entries That Aren't In The File System")]
        public bool APIRemove { get; set; }

        [Option('e', "pre", Required = false, HelpText = "Scan Plex DB Entries That Aren't In The File System")]
        public bool PreRemove { get; set; }

        [Option('t', "test", Required = false, HelpText = "Run Whatever Code Is In Test()")]
        public bool DoTest { get; set; }

        [Option('t', "post", Required = false, HelpText = "Remove Plex DB Entries That Aren't In The File System")]
        public bool PostRemove { get; set; }

        [Option('c', "config", Required = false, HelpText = "Point To A Config File Other Than 'config.json'")]
        public string ConfigFile { get; set; }

        [Option('d', "database", Required = false, HelpText = "Path Of Plex Database")]
        public string DBPath { get; set; }

    }
}
