using System.CommandLine;
using System.Linq;

static string Deleteline(string f)
{
    var arr = f.Split("\n");
    var x = arr.Where(z => !String.IsNullOrWhiteSpace(z));
    return String.Join("\n", x);

}


var rootCommand = new RootCommand("root command for file bundel CLI");
var bundelCommand = new Command("bundle", "bundle code files to a single file");
var createRspCommand = new Command("create-rsp", "Operating Instructions");
var bundelOption = new Option<FileInfo>("--output", "file to single path") { Name = "--o" };
var languageOption = new Option<string>("--language", "list of language") { IsRequired = true, Name = "--l" };
var noteOption = new Option<bool>("--note", "file to single path note") { Name = "--n" };
var sortOption = new Option<string>("--sort", "sort language accorging a-b") { Name = "--s" };
var removeEmptyLinesOption = new Option<bool>("--removeEmptyLines", "remove empty line") { Name = "--rel" };
var authorOption = new Option<string>("--author", "write name of user") { Name = "--a" };

sortOption.SetDefaultValue("latter");
rootCommand.AddCommand(bundelCommand);
rootCommand.AddCommand(createRspCommand);
bundelCommand.AddOption(bundelOption);
bundelCommand.AddOption(languageOption);
bundelCommand.AddOption(noteOption);
bundelCommand.AddOption(sortOption);
bundelCommand.AddOption(removeEmptyLinesOption);
bundelCommand.AddOption(authorOption);
Dictionary<string, string> languages = new Dictionary<string, string>();
languages.Add("c#", ".cs");
languages.Add("java", ".java");
languages.Add("c", ".cpp");
languages.Add("html", ".html");
languages.Add("c++", ".cpp");
languages.Add("css", ".css");
DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory());
List<FileInfo> listFile = new List<FileInfo>();


bundelCommand.SetHandler((arryLang, output, note, sort, empty, nameUser) =>
{
    try
    {
        var lang = arryLang.Split();
        List<string> listnew = new List<string>();
        foreach (var language in lang)
        {
            var f = false;
            if (language == "all")
            {
                foreach (var item in languages)
                {
                    listnew.Add(item.Value);
                    f = true;
                }
                break;
            }
            foreach (var item in languages)
            {
                if (item.Key == language)
                {
                    listnew.Add(item.Value);
                    f = true;
                }
            }
            if (!f)
            {
                throw new Exception("ERROR: language invalid");
            }


            listFile = directory.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Directory.Name != "bin" && x.Directory.Name != "Debug" && listnew.Contains(x.Extension)).ToList();


            if (output != null)
            {
                FileStream fs = new FileStream(output.FullName, FileMode.Append, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs);
                if (nameUser != null)
                {
                    writer.Write(nameUser);
                }
                if (note)
                {
                    writer.Write(Directory.GetCurrentDirectory());
                }

                if (sort != null)
                {
                    if (sort == "type")
                    {
                        listFile = listFile.OrderBy(x => x.Extension).ToList();
                    }
                    else
                        listFile = listFile.OrderBy(x => x.DirectoryName).ToList();
                }

                foreach (var inputFilePath in listFile)
                {
                    var filel = File.ReadAllText(inputFilePath.FullName);
                    if (empty)
                        filel = Deleteline(filel);
                    writer.Write(filel);
                }
                writer.Close();
            }
        }
    }


    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("error: the path is not validddd");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

}, languageOption, bundelOption, noteOption, sortOption, removeEmptyLinesOption, authorOption);



createRspCommand.SetHandler(() =>
{
    string c = "bundle --l ";
    Console.WriteLine("enter language to bundel");
    c += Console.ReadLine();
    Console.WriteLine("enter outpot ");
    string s = Console.ReadLine();
    if (s != "")
        c += " --o " + s;
    Console.WriteLine("enter note t/f ");
    bool b = bool.Parse(Console.ReadLine());
    if (b)
        c += " --n ";
    Console.WriteLine("enter sort according type or latter  ");
    s = Console.ReadLine();
    if (s != "")
        c += " --s " + s;
    Console.WriteLine("remove empty line t/f ");
    b = bool.Parse(Console.ReadLine());
    if (b)
        c += " --rel ";
    Console.WriteLine("enter name usere ");
    s = Console.ReadLine();
    if (s != "")
        c += " --a " + s;
    File.WriteAllText("responseFile.rsp", c);
});


rootCommand.InvokeAsync(args);