/// <summary>
/// Class with tools to execute "command prompt" commands
/// </summary>
public class RunCommands {
    string appsYmlFile = "apps.yml";
    IDeserializer deserializer;
    AppsYmlAppModel sourceApp;
    List<AppsYmlAppModel> allApps = new List<AppsYmlAppModel>();
    SaveData savedata = new();
    /// <summary>
    /// constructive method to review all practices
    /// </summary>
    public RunCommands() {
        deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        var ymlFile = deserializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(appsYmlFile));
        var containers = ymlFile["containers"];
        foreach (var container in containers) {
            var containerApp = container["apps"];
            allApps.Add(new AppsYmlAppModel {
                appname = containerApp["appname"],
                runbefore = ((List<object>)containerApp["runbefore"]).Select(x => x?.ToString()).ToList(),
                runafter = ((List<object>)containerApp["runafter"]).Select(x => x?.ToString()).ToList()
            });
        }
    }
    /// <summary>
    /// constructor method overload
    /// </summary>
    /// <param name="appname">"appname" required parameter</param>
    public RunCommands(string appname) {
        deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        var ymlFile = deserializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText(appsYmlFile));
        var containers = ymlFile["containers"];
        foreach (var container in containers) {
            var containerApp = container["apps"];
            if (containerApp["appname"] == appname) {
                sourceApp = new AppsYmlAppModel {
                    appname = containerApp["appname"],
                    runbefore = ((List<object>)containerApp["runbefore"]).Select(x => x?.ToString()).ToList(),
                    runafter = ((List<object>)containerApp["runafter"]).Select(x => x?.ToString()).ToList()
                };
            }
        }
    }
    int beforeTries = 0;
    /// <summary>
    /// Runs the named "runbefore" fields in the apps.yml file in order
    /// </summary>
    public void Before() {
        Process p = new Process();
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = "cmd.exe";
        info.RedirectStandardInput = true;
        info.RedirectStandardError = true;
        info.UseShellExecute = false;

        p.StartInfo = info;
        p.Start();

        using (StreamWriter sw = p.StandardInput) {
            if (sw.BaseStream.CanWrite) {
                foreach (var cmdline in sourceApp.runbefore) {
                    sw.WriteLine(cmdline);
                }
            }
        }
        p.WaitForExit();
        p.Close();
        try {
            string errors = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors) && beforeTries < 3) {
                Before();
                beforeTries++;
            }
        }
        catch (Exception exc) {
            _ = savedata.ExceptionSave(exc.Message);
        }
    }
    int afterTries = 0;
    /// <summary>
    /// Runs the named "runafter" fields in the apps.yml file in order
    /// </summary>
    public void After() {
        Process p = new Process();
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = "cmd.exe";
        info.RedirectStandardInput = true;
        info.RedirectStandardError = true;
        info.UseShellExecute = false;

        p.StartInfo = info;
        p.Start();

        using (StreamWriter sw = p.StandardInput) {
            if (sw.BaseStream.CanWrite) {
                foreach (var cmdline in sourceApp.runafter) {
                    sw.WriteLine(cmdline);
                }
            }
        }
        p.WaitForExit();
        p.Close();
        try {
            string errors = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors) && afterTries < 3) {
                Before();
                afterTries++;
            }
        }
        catch (Exception exc) {
            _ = savedata.ExceptionSave(exc.Message);
        }
    }

    int allAfterTries = 0;
    /// <summary>
    /// Runs the "runafter" fields of all applications in the apps.yml file in order
    /// </summary>
    public void AllAfter() {
        Process p = new Process();
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = "cmd.exe";
        info.RedirectStandardInput = true;
        info.RedirectStandardError = true;
        info.UseShellExecute = false;

        p.StartInfo = info;
        p.Start();

        using (StreamWriter sw = p.StandardInput) {
            if (sw.BaseStream.CanWrite) {
                foreach (var _subapp in allApps) {
                    foreach (var cmdline in _subapp.runafter) {
                        sw.WriteLine(cmdline);
                    }
                }
            }
        }
        p.WaitForExit();
        p.Close();
        try {
            string errors = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors) && allAfterTries < 3) {
                Before();
                allAfterTries++;
            }
        }
        catch (Exception exc) {
            _ = savedata.ExceptionSave(exc.Message);
        }
    }
}

class AppsYmlAppModel {
    public string appname { get; set; }
    public List<string> runbefore { get; set; }
    public List<string> runafter { get; set; }
}
