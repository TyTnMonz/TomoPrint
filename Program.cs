using CommandLine;
using System.Diagnostics;

class Program
{
    // Class to handle keyword search parameters
    public class KeyWordSearch
    {
        public string Keyword { get; set; }
        public int Trigger { get; set; }
        public int Copies { get; set; }

        public KeyWordSearch(string keywordsearch)
        {
            // Split the keyword search input into keyword, trigger count, and number of copies
            string[] pars = keywordsearch.Split(",");
            this.Keyword = pars[0];
            this.Trigger = int.Parse(pars[1]);
            this.Copies = int.Parse(pars[2]);
        }
    }

    // Definition of command-line options using the CommandLineParser library
    public class Options
    {
        [Option('d', "document", HelpText = "Path to the PDF document to print.")]
        public string Document { get; set; }

        [Option('f', "folder", HelpText = "Path to the folder containing documents to print.")]
        public string Folder { get; set; }

        [Option('e', "extension", Default = "pdf", HelpText = "File extension filter (default: pdf).")]
        public string Extension { get; set; }

        [Option('p', "printer", HelpText = "Name of the printer (default: system default printer).")]
        public string Printer { get; set; }

        [Option('c', "copies", Default = 1, HelpText = "Number of copies to print (default: 1).")]
        public int Copies { get; set; }

        [Option('a', "size", Default = "A4", HelpText = "Paper size (default: A4).")]
        public string PaperSize { get; set; }

        [Option('l', "color", Default = false, HelpText = "Print in color (default: black and white).")]
        public bool Color { get; set; }

        [Option('t', "timer", Default = 0, HelpText = "Interval in seconds between repeated print jobs (default: 5).")]
        public int TimerInterval { get; set; }

        [Option('w', "watcher", Default = false, HelpText = "Set the Folder Watcher to print files when uploaded.")]
        public bool Watcher { get; set; }

        [Option('s', "silent", Default = true, HelpText = "Activate Silent Print mode.")]
        public bool Silent { get; set; }

        [Option('o', "output", Default = "", HelpText = "Folder to move printed files. Defaults to a 'printed' subfolder.")]
        public string PrintedFolder { get; set; }

        [Option('k', "keywordSearch", Default = "", HelpText = "Keyword to search in files, trigger count, and copies (format: <keyword,trigger,copies>).")]
        public string KeywordSearch { get; set; }
    }

    static System.Timers.Timer printTimer;
    static Options currentOptions;
    static KeyWordSearch? keyWordSearch;

    static void Main(string[] args)
    {
        try
        {
            // Parse command-line arguments
            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(options =>
              {
                  currentOptions = options;
              })
              .WithNotParsed(errors => throw new Exception("Error parsing command-line arguments."));

            // If keyword search option is provided, validate input format and initialize
            if (currentOptions.KeywordSearch != "")
            {
                if (currentOptions.KeywordSearch.Split(",").Length != 3)
                    throw new Exception("Keyword Search Parameters are not valid.");

                keyWordSearch = new(currentOptions.KeywordSearch);
            }

            // Execute the initial print job
            ExecutePrintJob(currentOptions);

            // If a folder is specified, set up either a timer or a folder watcher
            if (!string.IsNullOrEmpty(currentOptions.Folder))
            {
                if (currentOptions.TimerInterval > 0 && !currentOptions.Watcher)
                {
                    // Set up a timer to repeatedly print files at intervals
                    printTimer = new System.Timers.Timer(currentOptions.TimerInterval * 1000); // Convert to milliseconds
                    printTimer.Elapsed += (sender, e) => ExecutePrintJob(currentOptions);
                    printTimer.Start();

                    Console.WriteLine($"Timer started. Printing will repeat every {currentOptions.TimerInterval} seconds.");
                    Console.WriteLine("Press any key to stop the timer...");
                    Console.ReadKey();
                }

                if (currentOptions.Watcher && !(currentOptions.TimerInterval > 0))
                {
                    // Set up a folder watcher to monitor new files in the folder
                    using (FileSystemWatcher watcher = new FileSystemWatcher(currentOptions.Folder))
                    {
                        watcher.Filter = $"*.{currentOptions.Extension}";
                        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                        watcher.Created += (s, e) =>
                        {
                            Console.WriteLine($"New file detected: {e.FullPath}");
                            System.Threading.Thread.Sleep(1000); // Delay to ensure file is ready
                            PrintDocument(e.FullPath, currentOptions);
                        };

                        watcher.EnableRaisingEvents = true;
                        Console.WriteLine($"Watching folder: {currentOptions.Folder} for new .{currentOptions.Extension} files...");
                        Console.WriteLine("Press any key to stop the watcher...");
                        Console.ReadKey();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
        }
    }

    // Main print job handler
    static void ExecutePrintJob(Options options)
    {
        if (!string.IsNullOrEmpty(options.Document))
        {
            PrintDocument(options.Document, options);
        }

        if (!string.IsNullOrEmpty(options.Folder))
        {
            PrintAllDocumentsInFolder(options);
        }
    }

    // Print all documents in the specified folder
    static void PrintAllDocumentsInFolder(Options options)
    {
        if (!Directory.Exists(options.Folder))
        {
            Console.WriteLine($"Error: The folder '{options.Folder}' does not exist.");
            return;
        }

        string searchPattern = $"*.{options.Extension}";
        string[] files = Directory.GetFiles(options.Folder, searchPattern);

        foreach (string file in files)
        {
            PrintDocument(file, options);
        }
    }

    // Print a single document
    static void PrintDocument(string filePath, Options options)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: The file '{filePath}' does not exist.");
            return;
        }

        string sumatraPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SumatraPDF.exe");

        if (!File.Exists(sumatraPath))
        {
            Console.WriteLine($"Error: SumatraPDF.exe not found in the application directory.");
            return;
        }

        try
        {
            // Determine print settings based on options and keyword search results
            string printerArg = string.IsNullOrEmpty(options.Printer) ? "-print-to-default" : $"-print-to \"{options.Printer}\"";
            string colorMode = options.Color ? "color" : "monochrome";
            string paperSize = $"paper={options.PaperSize}";
            string copies = keyWordSearch != null && GetKeywordCount(filePath, keyWordSearch.Keyword) == keyWordSearch.Trigger
                               ? $"{keyWordSearch.Copies}x,"
                               : options.Copies > 1 ? $"{options.Copies}x," : "";
            string silent = options.Silent ? "-silent" : "";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = sumatraPath,
                Arguments = $"{printerArg} -print-settings \"{copies}{colorMode},{paperSize}\" \"{filePath}\" {silent}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Console.WriteLine($"Printing Job: {psi.Arguments}.");

            Process printProcess = Process.Start(psi);
            printProcess.WaitForExit();

            // Move printed file to the specified folder
            FileInfo file = new(filePath);
            string printedFilePath = options.PrintedFolder == "" ? Path.Combine(file.DirectoryName!, "printed") : options.PrintedFolder;
            Directory.CreateDirectory(printedFilePath);
            File.Move(filePath, Path.Combine(printedFilePath, file.Name));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }

    // Read Python executable path from a configuration file
    static string GetPythonExecutablePath()
    {
        string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg");
        try
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException("Configuration file not found.");

            foreach (var line in File.ReadLines(configFile))
            {
                if (line.StartsWith("python_path", StringComparison.OrdinalIgnoreCase))
                    return line.Split('=')[1].Trim();
            }

            throw new FileNotFoundException("Python path not found in configuration file.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return string.Empty;
    }

    // Execute the Python script to count keyword occurrences in a PDF
    static int GetKeywordCount(string pdfPath, string keyword)
    {
        // Define the path to the Python script and the Python executable
        string pythonExePath = GetPythonExecutablePath();
        if (pythonExePath == "") return 0;

        string pythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "search.py");
        if (!File.Exists(pythonScriptPath))
        {
            Console.WriteLine($"{pythonExePath} does not exist");
            return 0;
        }

        // Create the argument string for the Python script
        string arguments = $"\"{pythonScriptPath}\" \"{pdfPath}\" \"{keyword}\"";

        // Set up process start information
        ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = arguments,
                RedirectStandardOutput = true, // Capture the script output
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Run the Python script and capture its output
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine($"Search result: {result}");

                    // Attempt to parse the result as an integer
                    if (int.TryParse(result.Trim(), out int count))
                        return count;

                    return 0; // Return 0 if parsing fails
                }
            }
        }
    }
