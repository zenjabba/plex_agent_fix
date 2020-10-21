using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using SQLite;
using SQLiteConnection = SQLite.SQLiteConnection;

namespace MonkFixPlexDB
{
    class Program
    {

        public static string DBPath;
        public static string configFile = "config.json";

        private static List<string> doneIds = new List<string>();

        public static Configuration configuration;

        public static bool Verbose = false;

        public static bool Debug = false;

        public static bool DryRun = false;

        public static List<MediaPath> MediaPaths = new List<MediaPath>();

        private static SQLiteConnection plexDB;

        private static PlexUser plexUser;

        private static CookieContainer cookieJar = new CookieContainer();
        private static HttpClientHandler handler;


        static void Main(string[] args)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            //Test();



            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {

                       configuration = GetConfiguration();

                       if (o.Dryrun)
                       {
                           DryRun = true;
                       }
                       if (o.Verbose)
                       {
                           Verbose = o.Verbose;
                       }
                       if (o.Debug)
                       {
                           Debug = o.Debug;
                       }

                       if (!string.IsNullOrEmpty(o.ConfigFile))
                       {
                           configFile = o.ConfigFile;
                           configuration = GetConfiguration();
                       }
                       else
                       {
                           configuration = GetConfiguration();

                       }

                       plexUser = getPlexUser();

                       MediaPaths = configuration.MediaPaths;


                       if (!string.IsNullOrEmpty(o.DBPath))
                       {
                           DBPath = o.DBPath;
                           try
                           {
                               //plexDB = GetDatabase();
                               //FixPlexDBSort();
                           }
                           catch (Exception ex)
                           {

                           }
                       }
                       else
                       {
                           DBPath = configuration.PlexDatabasePath;
                           try
                           {
                               //plexDB = GetDatabase();
                               //FixPlexDBSort();
                           }
                           catch (Exception ex)
                           {

                           }
                       }

                       if (o.PreRemove)
                       {
                           File.WriteAllText("delete.ids", "");
                           ScanMissingFilesFromPlexDB();
                       }

                       if (o.SectionRemove)
                       {
                           foreach (var s in configuration.SectionsToProcess)
                           {
                               ScanAndRemoveMissingFilesFromPlexBySection(s.ToString());
                           }
                       }

                       if (o.UnmatchFix)
                       {
                           foreach (var s in configuration.SectionsToProcess)
                           {
                               MatchUnmatchedMediaInPlexBySection(s.ToString());
                           }
                       }

                       if (o.AgentFix)
                       {


                           if (plexUser != null)
                           {
                               if (!string.IsNullOrEmpty(plexUser.User.AuthToken))
                               {
                                   foreach (var s in configuration.SectionsToProcess)
                                   {
                                       fixAgentBySection(s);
                                   }
                               }
                               else
                               {
                                   WriteLog("No Valid Plex Token Aborting");
                               }
                           }

                       }

                       if (o.PostRemove)
                       {
                           RemoveMissingFilesFromListFromPlexDB();
                       }

                       if (o.Remove)
                       {



                           if (plexUser != null)
                           {
                               if (!string.IsNullOrEmpty(plexUser.User.AuthToken))
                               {
                                   try
                                   {
                                       foreach (var s in configuration.SectionsToProcess)
                                       {
                                           deleteFromSection(s);

                                       }
                                   }
                                   catch(Exception ex)
                                   {

                                   }
                                   try
                                   {
                                       foreach (var s in configuration.EpisodeSectionsToProcess)
                                       {
                                           deleteFromSection(s, "4");

                                       }
                                   }
                                   catch(Exception ex)
                                   {

                                   }
                               }
                               else
                               {
                                   WriteLog("No Valid Plex Token Aborting");
                               }
                           }


                       }

                       if (o.APIRemove)
                       {
                           File.WriteAllText("delete.ids", "");

                           ScanAndRemoveMissingFilesFromPlexViaAPI();
                       }


                       if (o.DoTest)
                       {
                           Test();
                       }



                   });

        }


        public static void doProcessItemsToDeleteFromPlexLibrary(PlexLibrary plexLibrary) {

            try
            {
                foreach (var i in plexLibrary.MediaContainer.Metadata)
                {

                    try
                    {
                        var mc = getPlexMediaContainer(i.RatingKey.ToString()).Result;

                        if (mc != null)
                        {
                            foreach (var m in mc.MediaContainer.Metadata)
                            {
                                foreach (var mm in m.Media)
                                {
                                    if (mm.Part.Count > 1)
                                    {
                                        foreach (var p in mm.Part)
                                        {
                                            WriteLog("Checking that " + p.File +" exists" );
                                                 
                                            if (p.Exists == false)
                                            {
                                                if(!DryRun)
                                                    doPlexDeleteItem(i.RatingKey.ToString(), mm.Id.ToString());

                                                WriteLog(p.File.ToString() + " is unavailable and has been deleted");

                                                System.Threading.Thread.Sleep(200);
                                                if(!DryRun)
                                                    doPlexMetadataDeleteUnavailableFiles(m.RatingKey.ToString());


                                            }
                                        }
                                    }
                                    else
                                    {
                                        var p = mm.Part.FirstOrDefault();

                                        if (string.IsNullOrEmpty(p.File.ToString()))
                                        {
                                            WriteLog(p.File.ToString() + " is unavailable but is only copy and HAS NOT been deleted.");

                                        }
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("ERROR:" + ex.Message.ToString());

                    }
                }
            }
            catch (Exception metadataEx)
            {
                WriteLog("ERROR:" + metadataEx.Message.ToString());

            }


        }

        public static void deleteFromSection(long s, string contentType = "1")
        {

            WriteLog("Initialization of Plex Section " + s);

            var c = configuration;

            SectionProgress sectionProgress = null;
            try
            {
                sectionProgress = configuration.SectionProgress.Where(x => x.sectionId == s).FirstOrDefault();

            }
            catch (Exception ex)
            {
                WriteLog("ERROR:" + ex.Message.ToString());

            }

            var startingPoint = 0;

            if (sectionProgress != null)
            {
                if (sectionProgress.lastKey > 0)
                {
                    startingPoint = sectionProgress.lastKey;
                }
            }
            else
            {
                sectionProgress = new SectionProgress() { sectionId = (int)s, lastKey = 0 };
                if (configuration.SectionProgress == null)
                {
                    configuration.SectionProgress = new List<SectionProgress>();
                    configuration.SectionProgress.Add(sectionProgress);

                }
                else
                {
                    configuration.SectionProgress.Add(sectionProgress);

                }

            }

            WriteLog("Starting Point for Plex Section " + s + " Set To Item: " + startingPoint);

            var plexLibrary = doPlexGetLibraryEntries(s.ToString(), contentType, startingPoint).Result;

            if(plexLibrary != null)
            {

                if (startingPoint > plexLibrary.MediaContainer.TotalSize)
                {
                    goto configuration_timeout;
                }
            }
            else
            {
                goto configuration_timeout;

            }

            doProcessItemsToDeleteFromPlexLibrary(plexLibrary);

            if (plexLibrary != null)
            {

                sectionProgress.lastKey = startingPoint + 100;

                foreach (var p in configuration.SectionProgress)
                {
                    if (p.sectionId == sectionProgress.sectionId)
                    {
                        p.lastKey = sectionProgress.lastKey;
                        WriteConfiguration();

                    }
                }

                var howMany = (plexLibrary.MediaContainer.TotalSize / 100) + 100;

                var totalSize = plexLibrary.MediaContainer.TotalSize;// + 100;
                WriteLog("Your Plex Section " + s + " has " + totalSize + " items.");

                for (int x = startingPoint + 100; x < totalSize; x += 100)
                {

                    try
                    {
                        WriteLog("Getting " + x + " of " + totalSize + " items from Plex Section " + s);
                        plexLibrary = doPlexGetLibraryEntries(s.ToString(), contentType, x).Result;

                       

                    }
                    catch (Exception plexLibEx)
                    {
                        WriteLog("ERROR:" + plexLibEx.Message.ToString());

                    }
                    if (plexLibrary != null)
                    {
                        doProcessItemsToDeleteFromPlexLibrary(plexLibrary);


                    }

                    sectionProgress.lastKey = x;

                    foreach (var p in configuration.SectionProgress)
                    {
                        if (p.sectionId == sectionProgress.sectionId)
                        {
                            p.lastKey = sectionProgress.lastKey;
                            WriteConfiguration();

                        }
                    }


                    System.Threading.Thread.Sleep(100);

                    if (configuration.Timeout > 0)
                    {
                        System.Threading.Thread.Sleep(configuration.Timeout);

                    }

                }

                if(sectionProgress.lastKey < totalSize)
                {
                    plexLibrary = doPlexGetLibraryEntries(s.ToString(), contentType, sectionProgress.lastKey).Result;

                    if (plexLibrary != null)
                    {
                        doProcessItemsToDeleteFromPlexLibrary(plexLibrary);

                    }

                    sectionProgress.lastKey = Convert.ToInt32(totalSize);

                    foreach (var p in configuration.SectionProgress)
                    {
                        if (p.sectionId == sectionProgress.sectionId)
                        {
                            p.lastKey = sectionProgress.lastKey;
                            WriteConfiguration();

                        }
                    }
                }

                WriteLog("Plex Section " + s.ToString() + " Done Processing");

                if (configuration.EmptyTrash)
                    doPlexEmptyTrashBySection(s.ToString());

            }


        configuration_timeout:

            if (configuration.Timeout > 0)
            {
                System.Threading.Thread.Sleep(configuration.Timeout);

            }

        }

        public static void fixAgentBySection(long s)
        {


            SectionProgress sectionProgress = null;
            try
            {
                sectionProgress = configuration.SectionProgress.Where(x => x.sectionId == s).FirstOrDefault();

            }
            catch (Exception ex)
            {

            }

            var startingPoint = 0;

            if (sectionProgress != null)
            {
                if (sectionProgress.lastKey > 0)
                {
                    startingPoint = sectionProgress.lastKey;
                }
            }
            else
            {
                sectionProgress = new SectionProgress() { sectionId = (int)s, lastKey = 0 };
                if (configuration.SectionProgress == null)
                {
                    configuration.SectionProgress = new List<SectionProgress>();
                    configuration.SectionProgress.Add(sectionProgress);

                }
                else
                {
                    configuration.SectionProgress.Add(sectionProgress);

                }
            }

            WriteLog("Starting Point for Plex Section " + s + " Set To Item: " + startingPoint);

            var plexLibrary = doPlexGetLibraryEntries(s.ToString(), startingPoint).Result;

            if(plexLibrary == null)
            {
                goto configuration_timeout;
            }
            else
            {
                if (startingPoint  > plexLibrary.MediaContainer.TotalSize)
                {
                    goto configuration_timeout;
                }
            }

            WriteLog("Initialization of Plex Section " + s);

            foreach (var i in plexLibrary.MediaContainer.Metadata)
            {

                if (i.Guid.Contains(configuration.PreferredAgent))
                {

                }
                else
                {

                    try
                    {
                        var matches = getPlexMatchesByMetadataIdX(i.RatingKey.ToString()).Result;

                        if (matches != null)
                        {
                            if (matches.MediaContainer.SearchResult != null)
                            {
                                var bestMatch = matches.MediaContainer.SearchResult.FirstOrDefault();

                                if (bestMatch != null)
                                {
                                    if (!string.IsNullOrEmpty(bestMatch.Name))
                                    {
                                        doPlexUnmatchItem(i.RatingKey.ToString());
                                        //System.Threading.Thread.Sleep(10000);

                                        WriteLog("Using " + bestMatch.Guid + " to match " + bestMatch.Name + " " + bestMatch.Year.ToString());
                                        doPlexMovieMatchItem(i.RatingKey.ToString(), WebUtility.UrlEncode(bestMatch.Guid), WebUtility.UrlEncode(bestMatch.Name), WebUtility.UrlEncode(bestMatch.Year.ToString()));
                                        System.Threading.Thread.Sleep(1000);


                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("ERROR:" + ex.Message.ToString());
                    }

                }
            }

            if (plexLibrary != null)
            {

                sectionProgress.lastKey = startingPoint + 100;

                foreach (var p in configuration.SectionProgress)
                {
                    if (p.sectionId == sectionProgress.sectionId)
                    {
                        p.lastKey = sectionProgress.lastKey;
                        WriteConfiguration();

                    }
                }



                var howMany = (plexLibrary.MediaContainer.TotalSize / 100) + 100;

                var totalSize = plexLibrary.MediaContainer.TotalSize;
                WriteLog("Your Plex Section " + s + " has " + totalSize + " items.");

                for (int x = startingPoint + 100; x < totalSize; x += 100)
                {

                    try
                    {
                        WriteLog("Getting " + x + " of " + totalSize + " items from Plex Section " + s);

                        plexLibrary = doPlexGetLibraryEntries(s.ToString(), x).Result;

                        if(x > plexLibrary.MediaContainer.TotalSize)
                        {
                            goto configuration_timeout;
                        }

                    }
                    catch (Exception plexLibEx)
                    {

                    }

                    if (plexLibrary != null)
                    {
                        try
                        {
                            foreach (var i in plexLibrary.MediaContainer.Metadata)
                            {

                                if (i.Guid.Contains(configuration.PreferredAgent))
                                {

                                }
                                else
                                {

                                    try
                                    {
                                        var matches = getPlexMatchesByMetadataIdX(i.RatingKey.ToString()).Result;

                                        if (matches != null)
                                        {
                                            if (matches.MediaContainer.SearchResult != null)
                                            {
                                                var bestMatch = matches.MediaContainer.SearchResult.FirstOrDefault();

                                                if (bestMatch != null)
                                                {
                                                    if (!string.IsNullOrEmpty(bestMatch.Name))
                                                    {
                                                        doPlexUnmatchItem(i.RatingKey.ToString());
                                                        //System.Threading.Thread.Sleep(10000);

                                                        WriteLog("Using " + bestMatch.Guid + " to match " + bestMatch.Name + " " + bestMatch.Year.ToString());
                                                        doPlexMovieMatchItem(i.RatingKey.ToString(), WebUtility.UrlEncode(bestMatch.Guid), WebUtility.UrlEncode(bestMatch.Name), WebUtility.UrlEncode(bestMatch.Year.ToString()));
                                                        System.Threading.Thread.Sleep(1000);
                                                        WriteLog("  https://zenjabba.digitalmonks.org/library/metadata/" + i.RatingKey.ToString() + "?checkFiles=0&includeAllConcerts=1&includeBandwidths=1&includeChapters=1&includeChildren=1&includeConcerts=1&includeExtras=1&includeFields=1&includeGeolocation=1&includeLoudnessRamps=1&includeMarkers=1&includeOnDeck=1&includePopularLeaves=1&includePreferences=1&includeRelated=1&includeRelatedCount=1&includeReviews=1&includeStations=1&X-Plex-Token=EtYC9xW_5g9Ht4sCfoac");

                                                    }
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog("ERROR:" + ex.Message.ToString());
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    sectionProgress.lastKey = x;

                    foreach (var p in configuration.SectionProgress)
                    {
                        if (p.sectionId == sectionProgress.sectionId)
                        {
                            p.lastKey = sectionProgress.lastKey;
                            WriteConfiguration();

                        }
                    }


                    System.Threading.Thread.Sleep(100);

                    if (configuration.Timeout > 0)
                    {
                        System.Threading.Thread.Sleep(configuration.Timeout);

                    }
                }

            }

            configuration_timeout:
            if (configuration.Timeout > 0)
            {
                System.Threading.Thread.Sleep(configuration.Timeout);

            }

        }

        public static async void Test()
        {
            Verbose = true;
            configuration = GetConfiguration();

            /*
            DBPath = configuration.PlexDatabasePath;
            try
            {
                plexDB = GetDatabase();
                //FixPlexDBSort();
            }
            catch (Exception ex)
            {

            }*/

            /*
            ScanDirectoryForPlexFiles("/Users/martinbowling/downloads/plex-test/");
            */

            var pu = getPlexUser();

            plexUser = pu;

            if (plexUser != null)
            {
                if (!string.IsNullOrEmpty(plexUser.User.AuthToken))
                {
                    foreach (var s in configuration.SectionsToProcess)
                    {
                        deleteFromSection(s);

                    }
                    foreach (var s in configuration.EpisodeSectionsToProcess)
                    {
                        deleteFromSection(s, "4");

                    }
                }
                else
                {
                    WriteLog("No Valid Plex Token Aborting");
                }
            }

            WriteLog("Test() Done!");


        }



        public static SQLiteConnection GetDatabase()
        {
            try
            {
                return new SQLiteConnection(DBPath);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }


        public static async void ScanAndRemoveMissingFilesFromPlex()
        {
            var plexFiles = plexDB.Table<media_parts>().ToList();

            foreach (var f in plexFiles)
            {
                long dirID = -1;
                try
                {
                    dirID = f.directory_id;
                }
                catch (Exception dirEx)
                {
                    dirID = -1;
                }


                if (dirID > -1)
                {
                    if (!string.IsNullOrEmpty(f.file))
                    {
                        if (!f.file.Contains("Transcode/Sync+"))
                        {

                            var fs = f.file.Replace(MediaPaths[0].DockerPath, MediaPaths[0].RealPath).ToString();

                            WriteLog("Checking that " + fs + " exists");


                            try
                            {
                                if (!File.Exists(fs))
                                {

                                    if (!DryRun)
                                    {
                                        var item = plexDB.Table<media_items>().Where(x => x.id == f.media_item_id).FirstOrDefault();
                                        if (item != null)
                                        {
                                            if (item.id > 0)
                                            {
                                                WriteLog("File " + fs.ToString() + " Does not exist, deleting media part id: " + f.media_item_id + "");

                                                await doPlexDeleteItem(item.metadata_item_id.ToString(), f.media_item_id.ToString());



                                                WriteLog("Refreshing & Checking Files Associated with Metadata ID:" + item.metadata_item_id.ToString());

                                                await doPlexRefreshMetadata(item.metadata_item_id.ToString());




                                            }
                                        }

                                    }



                                    //File.AppendAllText("delete.ids", f.id + Environment.NewLine);

                                    System.Threading.Thread.Sleep(12000);


                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLog(ex.Message + ex.InnerException + ex.Source);
                            }

                        }
                    }
                }
            }

            //WriteLog("Please shut down Plex and re-run with the --post flag");
        }



        public static async void ScanDirectoryForPlexFiles(string plexDirectory)
        {
            string[] files = Directory.GetFiles(plexDirectory, "*.*", SearchOption.AllDirectories)
                    .Where(f => configuration.MediaTypes.IndexOf(Path.GetExtension(f)) >= 0).ToArray();

            foreach (var f in files)
            {
                //if (configuration.MediaTypes.IndexOf(Path.GetExtension(f)) >=0)
                //{
                Console.WriteLine(f.ToString());

                //}
            }

        }

        public static async void MatchUnmatchedMediaInPlexBySection(string id)
        {

            //var plexFiles = plexDB.Table<media_parts>().Where(x => x.media_item_id == lid).ToList();

            var plexFiles = plexDB.Query<metadata_items>("SELECT * FROM metadata_items WHERE originally_available_at IS NULL AND library_section_id = " + id + " AND guid NOT LIKE '%collection%' AND  title <> '' AND ((metadata_type=1) OR (metadata_type=2) )");


            foreach (var f in plexFiles)
            {

                try
                {
                    if (!DryRun)
                    {
                        WriteLog("Unmatching Metadata Id: " + f.id.ToString() + " named " + f.title);
                        await doPlexUnmatchItem(f.id.ToString());

                        WriteLog("Finding Matches for Metadata Id: " + f.id.ToString() + " named " + f.title);

                        var matches = await getPlexMatchesByMetadataId(f.id.ToString());
                        if (matches != null)
                        {
                            if (matches.MediaContainer.Size > 0)
                            {
                                var match = matches.MediaContainer.SearchResult.First();

                                WriteLog("Matching Metadata Id: " + f.id.ToString() + " named " + f.title + " to Match " + match.Name);


                                await doPlexMatchItemByGUID(f.id.ToString(), match.Guid.ToString(), match.Name.ToString());

                                //doPlexMatchItemOld(f.id.ToString(), match.Guid.ToString(), match.Name.ToString());

                                WriteLog("Analyzing Metadata Metadata Id: " + f.id.ToString() + " named " + f.title);

                                await doPlexMetadataAnalyze(f.id.ToString());

                                WriteLog("Refreshing Metadata Metadata Id: " + f.id.ToString() + " named " + f.title);
                                await doPlexMetadataRefresh(f.id.ToString());

                            }
                        }
                    }



                    //File.AppendAllText("delete.ids", f.id + Environment.NewLine);

                    System.Threading.Thread.Sleep(100);

                    if (configuration.Timeout > 0)
                    {
                        System.Threading.Thread.Sleep(configuration.Timeout);

                    }
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message + ex.InnerException + ex.Source);
                }


            }

            if (configuration.EmptyTrash)
            {
                WriteLog("Emptying Trash For Section: " + id);
                doPlexEmptyTrashBySection(id);

            }

            //WriteLog("Please shut down Plex and re-run with the --post flag");
        }



        public static async void ScanAndRemoveMissingFilesFromPlexBySection(string id)
        {

            //var plexFiles = plexDB.Table<media_parts>().Where(x => x.media_item_id == lid).ToList();

            var plexFiles = plexDB.Query<media_parts>("select * from media_parts where media_item_id in (select id from media_items where library_section_id = " + id + ")");


            foreach (var f in plexFiles)
            {
                long dirID = -1;
                try
                {
                    dirID = f.directory_id;
                }
                catch (Exception dirEx)
                {
                    dirID = -1;
                    WriteLog("Invalid Directory Id Skipping...");
                }


                if (dirID > -1)
                {

                    if (!string.IsNullOrEmpty(f.file))
                    {

                        if (!f.file.Contains("Transcode/Sync+"))
                        {


                            var fs = f.file.Replace(MediaPaths[0].DockerPath, MediaPaths[0].RealPath).ToString();

                            WriteLog("Checking that " + fs + " exists");


                            try
                            {
                                if (!File.Exists(fs))
                                {

                                    if (!DryRun)
                                    {
                                        var item = plexDB.Table<media_items>().Where(x => x.id == f.media_item_id).FirstOrDefault();
                                        if (item != null)
                                        {
                                            if (item.id > 0)
                                            {
                                                WriteLog("File " + fs.ToString() + " Does not exist, deleting media part id: " + f.media_item_id + "");

                                                await doPlexDeleteItem(item.metadata_item_id.ToString(), f.media_item_id.ToString());



                                                WriteLog("Refreshing & Checking Files Associated with Metadata ID:" + item.metadata_item_id.ToString());

                                                await doPlexRefreshMetadata(item.metadata_item_id.ToString());




                                            }
                                        }

                                    }



                                    //File.AppendAllText("delete.ids", f.id + Environment.NewLine);

                                    System.Threading.Thread.Sleep(100);

                                    if (configuration.Timeout > 0)
                                    {
                                        System.Threading.Thread.Sleep(configuration.Timeout);

                                    }


                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLog(ex.Message + ex.InnerException + ex.Source);
                            }
                        }
                    }
                }
            }

            if (configuration.EmptyTrash)
            {
                WriteLog("Emptying Trash For Section: " + id);
                doPlexEmptyTrashBySection(id);

            }

            //WriteLog("Please shut down Plex and re-run with the --post flag");
        }



        public static async void ScanAndRemoveMissingFilesFromPlexById(string id)
        {

            long lid = Convert.ToInt64(id.ToString());
            var plexFiles = plexDB.Table<media_parts>().Where(x => x.media_item_id == lid).ToList();

            foreach (var f in plexFiles)
            {
                long dirID = -1;
                try
                {
                    dirID = f.directory_id;
                }
                catch (Exception dirEx)
                {
                    dirID = -1;
                    WriteLog("Invalid Directory Id Skipping...");
                }


                if (dirID > -1)
                {
                    if (!string.IsNullOrEmpty(f.file))
                    {



                        var fs = f.file.Replace(MediaPaths[0].DockerPath, MediaPaths[0].RealPath).ToString();

                        WriteLog("Checking that " + fs + " exists");


                        try
                        {
                            if (!File.Exists(fs))
                            {

                                if (!DryRun)
                                {
                                    var item = plexDB.Table<media_items>().Where(x => x.id == f.media_item_id).FirstOrDefault();
                                    if (item != null)
                                    {
                                        if (item.id > 0)
                                        {
                                            WriteLog("File " + fs.ToString() + " Does not exist, deleting media part id: " + f.media_item_id + "");

                                            await doPlexDeleteItem(item.metadata_item_id.ToString(), f.media_item_id.ToString());



                                            WriteLog("Refreshing & Checking Files Associated with Metadata ID:" + item.metadata_item_id.ToString());

                                            await doPlexRefreshMetadata(item.metadata_item_id.ToString());




                                        }
                                    }

                                }



                                //File.AppendAllText("delete.ids", f.id + Environment.NewLine);

                                System.Threading.Thread.Sleep(12000);


                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message + ex.InnerException + ex.Source);
                        }
                    }
                }
            }

            //WriteLog("Please shut down Plex and re-run with the --post flag");
        }


        public static async void ScanAndRemoveMissingFilesFromPlexViaAPI()
        {

            try
            {
                var plexFiles = plexDB.Table<metadata_items>().Where(x => x.metadata_type == 1 && x.id > 32).ToList();

                foreach (var f in plexFiles)
                {


                    WriteLog("Checking that " + f.title + " exists");


                    var mc = await getPlexMediaContainer(f.id.ToString());


                    if (mc == null)
                    {
                        goto next;
                    }

                    try
                    {


                        if (!DryRun)
                        {
                            var md = mc.MediaContainer.Metadata[0];

                            foreach (var m in md.Media)
                            {
                                foreach (var mp in m.Part)
                                {
                                    /*
                                    if ((mp.Accessible == false) && (mp.Exists == false))
                                    {
                                        WriteLog("File " + mp.File + " Does not exist, deleting media part id: " + mp.Id + "");

                                        await doPlexDeleteItem(f.id.ToString(), mp.Id.ToString());

                                        //File.AppendAllText("delete.ids", f.id.ToString() + "," + mp.Id.ToString() + Environment.NewLine);


                                        WriteLog("Refreshing & Checking Files Associated with Metadata ID:" + f.id.ToString());

                                        await doPlexRefreshMetadata(f.id.ToString());
                                    }*/
                                }
                            }




                            //File.AppendAllText("delete.ids", f.id + Environment.NewLine);


                            System.Threading.Thread.Sleep(12000);

                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message + ex.InnerException + ex.Source);
                    }
                next:
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message + ex.InnerException + ex.Source);

            }
            //WriteLog("Please shut down Plex and re-run with the --post flag");
        }

        public static void ScanMissingFilesFromPlexDB()
        {
            var plexFiles = plexDB.Table<media_parts>().ToList();

            foreach (var f in plexFiles)
            {

                var fs = f.file.Replace(MediaPaths[0].DockerPath, MediaPaths[0].RealPath).ToString();

                WriteLog("Checking that " + fs + " exists");


                if (!File.Exists(fs))
                {
                    /*
                    if (!DryRun)
                        plexDB.Delete<media_parts>(f.id); //doPlexDeleteItem(f.media_item_id.ToString()); 



                    */

                    var item = plexDB.Table<media_items>().Where(x => x.id == f.media_item_id).FirstOrDefault();

                    WriteLog("File " + fs.ToString() + " Does not exist, adding plex media part id: " + f.id + " to queue for deletion");

                    File.AppendAllText("delete.ids", item.metadata_item_id.ToString() + "," + f.media_item_id.ToString() + Environment.NewLine);

                    //System.Threading.Thread.Sleep(12000);
                }
            }

            WriteLog("Please shut down Plex and re-run with the --post flag");
        }


        public static void RemoveMissingFilesFromListFromPlexDB()
        {
            var ids = File.ReadAllLines("delete.ids");

            if (ids.Length < 1)
            {
                WriteLog("No Metadata Ids found to delete please run with the --pre flag first ");
            }
            else
            {
                foreach (var i in ids)
                {
                    if (!DryRun)
                    {
                        WriteLog("deleting plex media part id: " + i);
                        plexDB.Delete<media_parts>(i); //doPlexDeleteItem(f.media_item_id.ToString()); 

                    }

                }
            }
        }


        public static void RemoveMissingFilesFromPlexDB()
        {
            var plexFiles = plexDB.Table<media_parts>().ToList();

            foreach (var f in plexFiles)
            {

                var fs = f.file.Replace(MediaPaths[0].DockerPath, MediaPaths[0].RealPath).ToString();

                WriteLog("Checking that " + fs + " exists");


                if (!File.Exists(fs))
                {
                    if (!DryRun)
                        plexDB.Delete<media_parts>(f.id); //doPlexDeleteItem(f.media_item_id.ToString()); 


                    WriteLog("File " + fs.ToString() + " Does not exist, deleting plex media part id: " + f.id);
                    //System.Threading.Thread.Sleep(12000);
                }
            }
        }

        public static void WriteLog(string message)
        {
            if (Verbose)
            {
                File.AppendAllText("plex_fix.log", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " -> " + message.ToString() + Environment.NewLine);
                Console.WriteLine(message.ToString());
            }
            else
            {

            }
        }


        private static void doGetMatchPlexItemsNotPreferredAgent(long lib, string prefferedAgent)
        {

            Console.WriteLine("Processing Dupes For Section " + lib + " using " + prefferedAgent + " as the preffered agent");

            var items = plexDB.Table<metadata_items>().Where(x => x.library_section_id == lib && x.metadata_type == 1).OrderByDescending(x => x.year); //.Query<metadata_items>("select * from metadata_items where title <> '' and library_section_id = 7 and metadata_type = 1 order by title").ToList();

            List<metadata_items> itemsWithDupes = new List<metadata_items>();

            var targetId = 236224;

            var doIt = false;

            foreach (var i in items)
            {

                var min1 = i.year - 1;
                var plus1 = i.year + 1;
                var dupeItems = items.Where(x => x.title == i.title && ((x.year == i.year) || (x.year == min1) || (x.year == plus1))).ToList();

                //var dupeItems = items.Where(x => x.title == i.title && ((x.year == i.year) ||(x.year == min1 ) || (x.year == plus1) )).ToList();
                //var dupeItems = items.Where(x => x.title == i.title && (x.year == i.year)).ToList();

                if (dupeItems.Count >= 1202312)
                {
                    Console.Write(i.title + " has " + dupeItems.Count + ": ");
                    var baseItem = items.First();
                    foreach (var d in dupeItems)
                    {
                        Console.Write(d.id + ", ");

                        if (d.guid.Contains(prefferedAgent))
                        {
                            baseItem = d;
                        }
                    }


                    Console.WriteLine();
                    Console.WriteLine(baseItem.id + " is selected as " + i.title + " baseitem");

                    baseItem.media_item_count = dupeItems.Count;
                    //plexDB.Update(baseItem);

                    foreach (var d in dupeItems)
                    {
                        if (d.id == baseItem.id)
                        {

                        }
                        else
                        {

                            if (d.tags_country.Contains(baseItem.tags_country))
                            {



                                if (!doneIds.Contains(d.id.ToString()))
                                {
                                    if (!d.guid.Contains(prefferedAgent))
                                    {



                                        Console.WriteLine("Need To UnMatch " + d.title + " on item id " + d.id);


                                        doPlexUnmatchItem(d.id.ToString());


                                        System.Threading.Thread.Sleep(1000);

                                        try
                                        {
                                            var matches = getPlexMatchesByMetadataIdX(i.id.ToString()).Result;

                                            if (matches != null)
                                            {
                                                if (matches.MediaContainer.SearchResult != null)
                                                {
                                                    var bestMatch = matches.MediaContainer.SearchResult.FirstOrDefault();

                                                    if (bestMatch != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(bestMatch.Name))
                                                        {
                                                            doPlexMovieMatchItem(i.id.ToString(), WebUtility.UrlEncode(bestMatch.Guid), WebUtility.UrlEncode(bestMatch.Name), WebUtility.UrlEncode(bestMatch.Year.ToString()));
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteLog("ERROR:" + ex.Message.ToString());
                                        }

                                        doneIds.Add(d.id.ToString());
                                    }

                                }
                            }

                        }
                    }
                    Console.WriteLine();
                }
                else
                {
                    if (i.guid.Contains(prefferedAgent))
                    {

                    }
                    else
                    {

                        try
                        {
                            var matches = getPlexMatchesByMetadataIdX(i.id.ToString()).Result;

                            if (matches != null)
                            {
                                if (matches.MediaContainer.SearchResult != null)
                                {
                                    var bestMatch = matches.MediaContainer.SearchResult.FirstOrDefault();

                                    if (bestMatch != null)
                                    {
                                        if (!string.IsNullOrEmpty(bestMatch.Name))
                                        {
                                            doPlexUnmatchItem(i.id.ToString());
                                            //System.Threading.Thread.Sleep(10000);

                                            WriteLog("Using " + bestMatch.Guid + " to match " + bestMatch.Name + " " + bestMatch.Year.ToString());
                                            doPlexMovieMatchItem(i.id.ToString(), WebUtility.UrlEncode(bestMatch.Guid), WebUtility.UrlEncode(bestMatch.Name), WebUtility.UrlEncode(bestMatch.Year.ToString()));
                                            System.Threading.Thread.Sleep(1000);
                                            WriteLog("  https://zenjabba.digitalmonks.org/library/metadata/" + i.id.ToString() + "?checkFiles=0&includeAllConcerts=1&includeBandwidths=1&includeChapters=1&includeChildren=1&includeConcerts=1&includeExtras=1&includeFields=1&includeGeolocation=1&includeLoudnessRamps=1&includeMarkers=1&includeOnDeck=1&includePopularLeaves=1&includePreferences=1&includeRelated=1&includeRelatedCount=1&includeReviews=1&includeStations=1&X-Plex-Token=EtYC9xW_5g9Ht4sCfoac");

                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog("ERROR:" + ex.Message.ToString());
                        }

                    }




                }

            }


            plexDB.Close();
            Console.ReadLine();
        }


        private static void doGetRefreshPlexItemsNotPreferredAgent(long lib, string prefferedAgent)
        {

            Console.WriteLine("Processing Dupes For Section " + lib + " using " + prefferedAgent + " as the preffered agent");

            var items = plexDB.Table<metadata_items>().Where(x => x.library_section_id == lib && x.metadata_type == 1).OrderByDescending(x => x.year); //.Query<metadata_items>("select * from metadata_items where title <> '' and library_section_id = 7 and metadata_type = 1 order by title").ToList();

            List<metadata_items> itemsWithDupes = new List<metadata_items>();

            var targetId = 236224;

            var doIt = false;

            foreach (var i in items)
            {


                if (i.guid.Contains(prefferedAgent))
                {

                }
                else
                {

                    try
                    {
                        WriteLog("Refreshing Metadata For Id: " + i.id.ToString() + " " + i.title.ToString() + " " + i.year.ToString());

                        doPlexRefreshMetadataById(i.id.ToString());
                        System.Threading.Thread.Sleep(500);

                        WriteLog("  https://zenjabba.digitalmonks.org/library/metadata/" + i.id.ToString() + "?checkFiles=0&includeAllConcerts=1&includeBandwidths=1&includeChapters=1&includeChildren=1&includeConcerts=1&includeExtras=1&includeFields=1&includeGeolocation=1&includeLoudnessRamps=1&includeMarkers=1&includeOnDeck=1&includePopularLeaves=1&includePreferences=1&includeRelated=1&includeRelatedCount=1&includeReviews=1&includeStations=1&X-Plex-Token=EtYC9xW_5g9Ht4sCfoac");

                    }
                    catch (Exception ex)
                    {
                        WriteLog("ERROR:" + ex.Message.ToString());
                    }

                }

            }


            plexDB.Close();
            Console.ReadLine();
        }
        private static void doGetPlexDupeItemsNotPreferredAgentX(long lib, string prefferedAgent)
        {

            Console.WriteLine("Processing Dupes For Section " + lib + " using " + prefferedAgent + " as the preffered agent");

            var items = plexDB.Table<metadata_items>().Where(x => x.library_section_id == lib && x.metadata_type == 1).OrderBy(x => x.title); //.Query<metadata_items>("select * from metadata_items where title <> '' and library_section_id = 7 and metadata_type = 1 order by title").ToList();

            List<metadata_items> itemsWithDupes = new List<metadata_items>();

            var targetId = 236224;

            var doIt = false;

            foreach (var i in items)
            {

                var min1 = i.year - 1;
                var plus1 = i.year + 1;
                var dupeItems = items.Where(x => x.title == i.title && ((x.year == i.year) || (x.year == min1) || (x.year == plus1))).ToList();

                //var dupeItems = items.Where(x => x.title == i.title && ((x.year == i.year) ||(x.year == min1 ) || (x.year == plus1) )).ToList();
                //var dupeItems = items.Where(x => x.title == i.title && (x.year == i.year)).ToList();

                if (dupeItems.Count >= 2)
                {
                    Console.Write(i.title + " has " + dupeItems.Count + ": ");
                    var baseItem = items.First();
                    foreach (var d in dupeItems)
                    {
                        Console.Write(d.id + ", ");

                        if (d.guid.Contains(prefferedAgent))
                        {
                            baseItem = d;
                        }
                    }


                    Console.WriteLine();
                    Console.WriteLine(baseItem.id + " is selected as " + i.title + " baseitem");

                    baseItem.media_item_count = dupeItems.Count;
                    plexDB.Update(baseItem);

                    foreach (var d in dupeItems)
                    {
                        if (d.id == baseItem.id)
                        {

                        }
                        else
                        {

                            if (d.tags_country.Contains(baseItem.tags_country))
                            {



                                if (!doneIds.Contains(d.id.ToString()))
                                {
                                    if (!d.guid.Contains(prefferedAgent))
                                    {

                                        Console.WriteLine("Need To UnMatch " + d.title + " on item id " + d.id);
                                        var imdb = baseItem.guid.Replace("com.plexapp.agents.imdb://", "").Replace("?lang=en", "").ToString();

                                        var unMatch = File.ReadAllText("unmatch.txt");

                                        unMatch = unMatch.Replace("{ID}", d.id.ToString());

                                        //File.AppendAllText("plex-commands.sh", unMatch + Environment.NewLine);

                                        doPlexUnmatchItem(d.id.ToString());


                                        System.Threading.Thread.Sleep(10000);

                                        var match = File.ReadAllText("match.txt");

                                        match = match.Replace("{ID}", d.id.ToString()).Replace("{IMDB}", imdb).Replace("{NAME}", WebUtility.UrlEncode(d.title));

                                        //File.AppendAllText("plex-commands.sh", match + Environment.NewLine);

                                        doPlexMatchItem(d.id.ToString(), imdb, WebUtility.UrlEncode(d.title));

                                        doneIds.Add(d.id.ToString());
                                    }

                                }
                            }

                        }
                    }
                    Console.WriteLine();
                }

            }


            plexDB.Close();
            Console.ReadLine();
        }


        private static async Task doPlexUnmatchItem(string metadataItemId)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + "/library/metadata/" + metadataItemId + "/unmatch?X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("content-length", "0");
                    request.Headers.TryAddWithoutValidation("accept-language", "en");
                    request.Headers.TryAddWithoutValidation("accept", "text/plain, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");

                    var response = httpClient.SendAsync(request).Result;
                }
            }
        }


        public static PlexUser getPlexUser()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            using (var client = new BetterWebClient(cookieJar, true))
            {
                try
                {
                    var values = new NameValueCollection();
                    values["Authorization"] = "Basic " + auth;
                    values["X-Plex-Client-Identifier"] = "myAppID";
                    values["X-Plex-Product"] = "my App Name";
                    values["X-Plex-Version"] = "0.1.0";

                    client.Headers.Add(values);

                    var response = client.UploadString("https://plex.tv/users/sign_in.json", "");

                    WriteLog("Logging In As " + configuration.PlexUser.ToString());

                    /*
                    var json = new JavaScriptSerializer();
                    dynamic result = json.DeserializeObject(response);

                    var token = result["user"]["authentication_token"];
                    */
                    //Console.WriteLine(response);

                    var pu = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexUser>(response);

                    if (pu != null)
                    {
                        if (!string.IsNullOrEmpty(pu.User.AuthToken))
                        {
                            WriteLog("Plex Authentication Token: " + pu.User.AuthToken);
                        }
                    }

                    return pu;
                }
                catch (Exception ex)
                {
                    return null;
                    WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());

                }
            }
        }

        public static async Task<PlexUser> getPlexUserNew()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);




            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://plex.tv/users/sign_in.json"))
                {
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                    /*request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("content-length", "0");
                    request.Headers.TryAddWithoutValidation("accept-language", "en");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");*/

                    try
                    {
                        var response = await httpClient.SendAsync(request);


                        var pu = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexUser>(response.Content.ToString());

                        if (pu != null)
                        {
                            if (!string.IsNullOrEmpty(pu.User.AuthToken))
                            {
                                WriteLog("Plex Authentication Token: " + pu.User.AuthToken);
                            }
                        }

                        return pu;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return new PlexUser();
                    }
                }
            }
        }

        private static async void doPlexDeleteItemBroke(string metadataItemId, string mediaPartId)
        {
            /*
            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };*/


            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);
            var httpClient = new HttpClient(handler);
            //using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId))

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + "/library/metadata/" + metadataItemId + "/media/" + mediaPartId + "?includeExternalMedia=1&X-Plex-Product=Plex Web&X-Plex-Version=4.8.4&X-Plex-Client-Identifier=0aftwia9s9lrzb9za31c9hj0&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media&X-Plex-Model=bundled&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1592x1091%2C2560x1440&X-Plex-Token=");// + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3");

            // /library/metadata/11/media/15?
            //var request = new HttpRequestMessage(new HttpMethod("DELETE"), configuration.PlexProtocol + "://" + configuration.PlexHost + "/library/metadata/" + metadataItemId + "/media/" + mediaPartId + "?includeExternalMedia=1&X-Plex-Product=Plex Web&X-Plex-Version=4.8.4&X-Plex-Client-Identifier=0aftwia9s9lrzb9za31c9hj0&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media&X-Plex-Model=bundled&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1592x1091%2C2560x1440&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3");
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/media/" + mediaPartId + "?includeExternalMedia=1&X-Plex-Product=Plex Web&X-Plex-Version=4.8.4&X-Plex-Client-Identifier=0aftwia9s9lrzb9za31c9hj0&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media&X-Plex-Model=bundled&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1592x1091%2C2560x1440&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3");
            //using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), configuration.PlexProtocol + "://" + configuration.PlexHost+ ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId +"/media/" + mediaPartId + "?includeExternalMedia=1&X-Plex-Product=Plex Web&X-Plex-Version=4.8.4&X-Plex-Client-Identifier=0aftwia9s9lrzb9za31c9hj0&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media&X-Plex-Model=bundled&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1592x1091%2C2560x1440&X-Plex-Token=" + plexUser.User.AuthToken +"&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3"))


            try
            {
                request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);


                request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);

                request.Headers.TryAddWithoutValidation("accept-language", "en");
                request.Headers.TryAddWithoutValidation("accept", "xml, */*; q=0.01");
                request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");

                var response = await httpClient.SendAsync(request);

                //WriteLog(response.Content.ReadAsStringAsync().Result.ToString()); 
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
            }




        }

        private static async Task doPlexRefreshMetadataById(string metadataItemId)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?X-Plex-Product=Plex Web&X-Plex-Version=4.41.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=84.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x881%2C1792x1120&X-Plex-Token=EtYC9xW_5g9Ht4sCfoac&X-Plex-Language=en"))
                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                        request.Headers.TryAddWithoutValidation("Content-Length", "0");
                        request.Headers.TryAddWithoutValidation("Accept", "text/plain, */*; q=0.01");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                        var response = await httpClient.SendAsync(request);

                        //                //WriteLog(response.Content.ReadAsStringAsync().Result.ToString()); 
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                    }
                }
            }
        }

        private static async Task<PlexLibrary> doPlexGetLibraryEntries(string libraryId, int startingNumber)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/sections/" + libraryId + "/all?type=1&sort=addedAt%3Adesc&includeCollections=1&includeExternalMedia=1&includeAdvanced=1&includeMeta=1&X-Plex-Container-Start=" + startingNumber.ToString() + "&X-Plex-Container-Size=72&X-Plex-Product=Plex Web&X-Plex-Version=4.41.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=84.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x881%2C1792x1120&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Drm=none&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3"))
                    {
                        request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                        request.Headers.TryAddWithoutValidation("accept", "application/json");
                        request.Headers.TryAddWithoutValidation("accept-language", "en");
                        request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                        request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                        request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                        request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;

                        var json = response.Content.ReadAsStringAsync().Result.ToString();
                        //WriteLog(json);

                        var mc = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexLibrary>(json);

                        return mc;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());

                return null;

            }
        }

        private static async Task<PlexLibrary> doPlexGetLibraryEntries(string libraryId, string contentType, int startingNumber)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/sections/" + libraryId + "/all?type=" + contentType + "&sort=addedAt%3Adesc&includeCollections=1&includeExternalMedia=1&includeAdvanced=1&includeMeta=1&X-Plex-Container-Start=" + startingNumber.ToString() + "&X-Plex-Container-Size=72&X-Plex-Product=Plex Web&X-Plex-Version=4.41.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=84.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x881%2C1792x1120&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Drm=none&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3"))
                    {
                        request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                        request.Headers.TryAddWithoutValidation("accept", "application/json");
                        request.Headers.TryAddWithoutValidation("accept-language", "en");
                        request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                        request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                        request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                        request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;

                        var json = response.Content.ReadAsStringAsync().Result.ToString();
                        //WriteLog(json);

                        if(Debug)
                            WriteLog(json);

                        var mc = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexLibrary>(json);

                        return mc;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());

                return null;

            }
        }


        private static async Task doPlexRefreshMetadata(string metadataItemId)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            ////WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "?includeExternalMedia=1&skipRefresh=1&checkFiles=1&asyncCheckFiles=0&X-Plex-Product=Plex Web&X-Plex-Version=4.33.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x845%2C1792x1120&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3&X-Plex-Drm=none");

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "?includeExternalMedia=1&skipRefresh=1&checkFiles=1&asyncCheckFiles=0&X-Plex-Product=Plex Web&X-Plex-Version=4.33.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x845%2C1792x1120&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3&X-Plex-Drm=none"))
                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;

                        //                //WriteLog(response.Content.ReadAsStringAsync().Result.ToString()); 
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                    }

                }
            }

        }

        private static async Task doPlexMetadataDeleteUnavailableFiles(string metadataItemId)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?X-Plex-Token=" + plexUser.User.AuthToken + "");


            if(Debug)
                WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?includeExternalMedia=1&checkFiles=1&asyncCheckFiles=0&skipRefresh=1&X-Plex-Token=" + plexUser.User.AuthToken);

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?includeExternalMedia=1&checkFiles=1&asyncCheckFiles=0&skipRefresh=1&X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("accept", "/");
                    request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);

                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                    request.Headers.TryAddWithoutValidation("access-control-request-method", "PUT");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");

                    var response = httpClient.SendAsync(request).Result;

                    if(Debug)
                     WriteLog(response.Content.ReadAsStringAsync().Result.ToString());
                }
            }



        }

        private static async Task doPlexMetadataRefresh(string metadataItemId)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?X-Plex-Token=" + plexUser.User.AuthToken + "");



            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("OPTIONS"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/refresh?X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("accept", "/");
                    request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);

                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                    request.Headers.TryAddWithoutValidation("access-control-request-method", "PUT");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");

                    var response = httpClient.SendAsync(request).Result;
                }
            }



        }

        private static async Task doPlexMetadataAnalyze(string metadataItemId)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/analyze?X-Plex-Token=" + plexUser.User.AuthToken + "");



            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("OPTIONS"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/analyze?X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("accept", "/");
                    request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);

                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                    request.Headers.TryAddWithoutValidation("access-control-request-method", "PUT");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");

                    var response = httpClient.SendAsync(request).Result;
                }
            }



        }



        private static async Task<PlexMediaMatch> getPlexMatchesByMetadataId(string metadataItemId)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/matches?manual=1&X-Plex-Token=" + plexUser.User.AuthToken + "");

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/matches?manual=1&X-Plex-Token=" + plexUser.User.AuthToken + ""))
                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;


                        var json = response.Content.ReadAsStringAsync().Result.ToString();
                        //WriteLog(json);

                        var mc = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexMediaMatch>(json);

                        return mc;

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                        return null;
                    }

                }
            }

        }


        private static async Task<PlexMediaMatch> getPlexMatchesByMetadataIdX(string metadataItemId)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            ////WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/matches?manual=1&X-Plex-Token=" + plexUser.User.AuthToken + "");

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/matches?manual=1&X-Plex-Token=" + plexUser.User.AuthToken + ""))
                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;


                        var json = response.Content.ReadAsStringAsync().Result.ToString();
                        //WriteLog(json);

                        var mc = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexMediaMatch>(json);

                        return mc;

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                        return null;
                    }

                }
            }

        }

        private static async Task<PlexMediaContainer> getPlexMediaContainer(string metadataItemId)
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "?includeConcerts=1&includeExtras=1&includeOnDeck=1&includePopularLeaves=1&includePreferences=1&includeChapters=1&includeStations=1&includeExternalMedia=1&asyncAugmentMetadata=1&asyncCheckFiles=0&asyncRefreshAnalysis=1&asyncRefreshLocalMediaAgent=1&checkFiles=1&X-Plex-Product=Plex%20Web&X-Plex-Version=4.33.1&X-Plex-Client-Identifier=ycrthc40orsofnshpwmjgmzx&X-Plex-Platform=Microsoft%20Edge&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Microsoft%20Edge&X-Plex-Device-Screen-Resolution=1822x1106%2C2560x1440&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3&X-Plex-Drm=widevine");

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "?includeConcerts=1&includeExtras=1&includeOnDeck=1&includePopularLeaves=1&includePreferences=1&includeChapters=1&includeStations=1&includeExternalMedia=1&asyncAugmentMetadata=1&asyncCheckFiles=0&asyncRefreshAnalysis=1&asyncRefreshLocalMediaAgent=1&checkFiles=1&X-Plex-Product=Plex%20Web&X-Plex-Version=4.33.1&X-Plex-Client-Identifier=ycrthc40orsofnshpwmjgmzx&X-Plex-Platform=Microsoft%20Edge&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Microsoft%20Edge&X-Plex-Device-Screen-Resolution=1822x1106%2C2560x1440&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en&X-Plex-Text-Format=plain&X-Plex-Provider-Version=1.3&X-Plex-Drm=widevine"))
                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                        var response = httpClient.SendAsync(request).Result;


                        var json = response.Content.ReadAsStringAsync().Result.ToString();
                        //WriteLog(json);

                        var mc = Newtonsoft.Json.JsonConvert.DeserializeObject<PlexMediaContainer>(json);

                        return mc;

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                        return null;
                    }

                }
            }

        }

        private static async void doPlexEmptyTrashBySection(string sectionId)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);


            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/sections/" + sectionId + "/emptyTrash?X-Plex-Product=Plex Web&X-Plex-Version=4.33.1&X-Plex-Client-Identifier=kfowlv9lv8su6i8ds01fbqdm&X-Plex-Platform=Chrome&X-Plex-Platform-Version=81.0&X-Plex-Sync-Version=2&X-Plex-Features=external-media%2Cindirect-media&X-Plex-Model=hosted&X-Plex-Device=OSX&X-Plex-Device-Name=Chrome&X-Plex-Device-Screen-Resolution=1792x894%2C1792x1120&X-Plex-Token=" + plexUser.User.AuthToken + "&X-Plex-Language=en"))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);


                    request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("Content-Length", "0");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                    request.Headers.TryAddWithoutValidation("Accept", "text/plain, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                    request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                    var response = httpClient.SendAsync(request).Result;
                }
            }
        }

        private static async Task doPlexDeleteItem(string metadataItemId, string mediaPartId, string urlPart = "")
        {


            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            var url = string.Empty;
            if (string.IsNullOrEmpty(urlPart))
                url = configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/media/" + mediaPartId + "?X-Plex-Token=" + plexUser.User.AuthToken + "";
            else
                url = configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + urlPart + "?X-Plex-Token=" + plexUser.User.AuthToken + "";


            if (Debug)
                WriteLog("PLEX ITEM URL: " + url);

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("OPTIONS"), url))
                {
                    try
                    {

                        request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                        request.Headers.TryAddWithoutValidation("Accept", "*/*");



                        request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);
                        request.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "DELETE");
                        request.Headers.TryAddWithoutValidation("Origin", "https://app.plex.tv");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                        request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                        request.Headers.TryAddWithoutValidation("Referer", "https://app.plex.tv/");
                        request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");

                        var response = await httpClient.SendAsync(request);

                        
                    }

                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                    }
                }
            }
            using (var httpClient = new HttpClient(handler))
            {


                using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), url))

                {
                    try
                    {
                        request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                        request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);


                        request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);

                        request.Headers.TryAddWithoutValidation("accept-language", "en");
                        request.Headers.TryAddWithoutValidation("accept", "xml, */*; q=0.01");
                        request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                        request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                        request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");

                        var response = httpClient.SendAsync(request).Result;

                        if (Debug)
                            WriteLog(response.Content.ReadAsStringAsync().Result.ToString());

                        //                //WriteLog(response.Content.ReadAsStringAsync().Result.ToString()); 
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message.ToString() + ex.InnerException.ToString() + ex.Source.ToString());
                    }
                }
            }


        }

        private static async Task doPlexMatchItemByGUID(string metadataItemId, string guid, string name)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(configuration.PlexUser + ":" + configuration.PlexPass);
            string auth = System.Convert.ToBase64String(plainTextBytes);

            //WriteLog(configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/match?guid=" + WebUtility.UrlEncode(guid) + "&name=" + WebUtility.UrlEncode(name) + "&X-Plex-Token=" + plexUser.User.AuthToken);

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + "/library/metadata/" + metadataItemId + "/match?guid=" + WebUtility.UrlEncode(guid) + "&name=" + WebUtility.UrlEncode(name) + "&X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + auth);
                    request.Headers.TryAddWithoutValidation("X-Plex-Token", plexUser.User.AuthToken);
                    request.Headers.TryAddWithoutValidation("accept", "*/*");
                    request.Headers.TryAddWithoutValidation("access-control-request-method", "PUT");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");

                    var response = httpClient.SendAsync(request).Result;
                }
            }

        }

        private static async void doPlexMovieMatchItem(string metadataItemId, string guid, string Name, string Year)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            //WriteLog("" + configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/match?guid=" + guid + "&name=" + Name + "&year=" + Year + "&X-Plex-Language=en&X-Plex-Token=" + plexUser.User.AuthToken);

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/match?guid=" + guid + "&name=" + Name + "&year=" + Year + "&X-Plex-Language=en&X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("Content-Length", "0");
                    request.Headers.TryAddWithoutValidation("Accept", "text/plain, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "en");
                    request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("Origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                    request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                    request.Headers.TryAddWithoutValidation("Referer", "http://app.plex.tv/");

                    var response = await httpClient.SendAsync(request);
                }
            }
        }

        private static async void doPlexMatchItem(string metadataItemId, string tt, string name)
        {

            handler = new HttpClientHandler
            {
                CookieContainer = cookieJar,
                UseCookies = true,
                UseDefaultCredentials = false
            };


            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/match?guid=com.plexapp.agents.imdb%3A%2F%2F" + tt + "%3Flang%3Den&name=" + name + "&X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("content-length", "0");
                    request.Headers.TryAddWithoutValidation("accept-language", "en");
                    request.Headers.TryAddWithoutValidation("accept", "text/plain, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");

                    var response = httpClient.SendAsync(request).Result;
                }
            }

        }

        private static async void doPlexMatchItemOld(string metadataItemId, string guid, string name)
        {

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), configuration.PlexProtocol + "://" + configuration.PlexHost + ":" + configuration.PlexPort + "/library/metadata/" + metadataItemId + "/match?guid=" + guid + "&name=" + name + "&X-Plex-Token=" + plexUser.User.AuthToken))
                {
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("authority", configuration.PlexHost);
                    request.Headers.TryAddWithoutValidation("content-length", "0");
                    request.Headers.TryAddWithoutValidation("accept-language", "en");
                    request.Headers.TryAddWithoutValidation("accept", "text/plain, */*; q=0.01");
                    request.Headers.TryAddWithoutValidation("origin", "http://app.plex.tv");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("referer", "http://app.plex.tv/");

                    var response = httpClient.SendAsync(request).Result;
                }
            }

        }

        private static async void doGetPlexDupeItemsNotPreferredAgent(long lib, string prefferedAgent)
        {

            Console.WriteLine("Processing Dupes For Section " + lib + " using " + prefferedAgent + " as the preffered agent");

            var items = plexDB.Table<metadata_items>().Where(x => x.library_section_id == lib && x.metadata_type == 1).OrderBy(x => x.title); //.Query<metadata_items>("select * from metadata_items where title <> '' and library_section_id = 7 and metadata_type = 1 order by title").ToList();

            List<metadata_items> itemsWithDupes = new List<metadata_items>();


            var doIt = false;

            foreach (var i in items)
            {


                var min1 = i.year - 1;
                var plus1 = i.year + 1;
                var dupeItems = items.Where(x => x.title == i.title && x.tags_country == i.tags_country && ((x.year == i.year) || (x.year == min1) || (x.year == plus1))).ToList();
                //var dupeItems = items.Where(x => x.title == i.title && (x.year == i.year)).ToList();

                if (dupeItems.Count >= 2)
                {
                    WriteLog(i.title + " has " + dupeItems.Count + ": ");
                    var baseItem = items.First();
                    foreach (var d in dupeItems)
                    {
                        //Console.Write(d.id + ", ");

                        if (d.guid.Contains(prefferedAgent))
                        {
                            baseItem = d;
                        }
                    }



                    WriteLog(baseItem.id + " is selected as " + i.title + " baseitem");

                    baseItem.media_item_count = dupeItems.Count;
                    plexDB.Update(baseItem);

                    foreach (var d in dupeItems)
                    {
                        if (d.id == baseItem.id)
                        {

                        }
                        else
                        {
                            if (!doneIds.Contains(d.id.ToString()))
                            {
                                if (!d.guid.Contains(prefferedAgent))
                                {

                                    WriteLog("Need To UnMatch " + d.title + " on item id " + d.id);
                                    var imdb = baseItem.guid.Replace("com.plexapp.agents.imdb://", "").Replace("?lang=en", "").ToString();

                                    var unMatch = File.ReadAllText("unmatch.txt");

                                    unMatch = unMatch.Replace("{ID}", d.id.ToString());

                                    File.AppendAllText("plex-commands.sh", unMatch + Environment.NewLine);

                                    await doPlexUnmatchItem(d.id.ToString());


                                    System.Threading.Thread.Sleep(10000);

                                    var match = File.ReadAllText("match.txt");

                                    match = match.Replace("{ID}", d.id.ToString()).Replace("{IMDB}", imdb).Replace("{NAME}", WebUtility.UrlEncode(d.title));

                                    File.AppendAllText("plex-commands.sh", match + Environment.NewLine);

                                    doPlexMatchItem(d.id.ToString(), imdb, WebUtility.UrlEncode(d.title));

                                    doneIds.Add(d.id.ToString());
                                }

                            }


                        }
                    }

                }

            }

            plexDB.Close();
            WriteLog("Complete.");
        }

        public static void FixPlexDBSort()
        {
            try
            {
                plexDB.Query("DROP index 'index_title_sort_naturalsort'");
                plexDB.Query("DELETE from schema_migrations where version='20180501000000'");
            }
            catch (Exception ex)
            {

            }
        }



        public static void WriteConfiguration(bool silent = true)
        {
            try
            {
                if (!silent)
                    WriteLog("Serializing Settings");

                var configJson = Newtonsoft.Json.JsonConvert.SerializeObject(configuration);

                if (!silent)
                    WriteLog("Writing Settings To " + configFile);

                File.WriteAllText(configFile, configJson);
            }
            catch (Exception ex)
            {
                WriteLog("Failed To Write Config File. " + ex.Message.ToString());
            }
        }

        public static Configuration GetConfiguration()
        {

            try
            {
                var settingsJson = File.ReadAllText(configFile);

                Console.WriteLine("Deserializing Settings");

                var c = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(settingsJson);

                return c;
            }
            catch (Exception ex)
            {
                return new Configuration();
            }

        }

        public static async void addToPlexAutoScanQueue(string path)
        {
            var handler = new HttpClientHandler();

            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), ""))
                {
                    request.Headers.TryAddWithoutValidation("authority", "plexautoscan.zenjabba.com");
                    request.Headers.TryAddWithoutValidation("cache-control", "max-age=0");
                    request.Headers.TryAddWithoutValidation("upgrade-insecure-requests", "1");
                    request.Headers.TryAddWithoutValidation("origin", "https://plexautoscan.zenjabba.com");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.129 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "navigate");
                    request.Headers.TryAddWithoutValidation("sec-fetch-user", "?1");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "document");
                    request.Headers.TryAddWithoutValidation("referer", "https://plexautoscan.zenjabba.com/a6db7a48bb1b4d23b4ff2654ef611699");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
                    request.Headers.TryAddWithoutValidation("cookie", "_ga=GA1.2.10313295.1580341610");

                    request.Content = new StringContent("filepath=%2Fdata%2Fmovies%2FMurder+to+Mercy+The+Cyntoia+Brown+Story+%282020%29+WEBDL-1080p.mkv&eventType=Manual");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                }
            }
        }
    }
}
