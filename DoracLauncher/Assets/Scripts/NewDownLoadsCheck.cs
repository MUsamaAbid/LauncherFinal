using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using System.Windows.Forms;
using UnityEngine.SceneManagement;

public class NewDownLoadsCheck : MonoBehaviour
{
    [Header("Objects")]
    public Text versionText;
    public GameObject browserBtn;
    public Text restartTxt;
    public GameObject playBtn;
    public GameObject mainProgressBar;
    public Image progressBar;
    public Text downloadingMB;
    public Text totalDownloadSize;
    public Text DownloadingStatus;
    public Text FileLocaitonNotSet;
    public Text DownloadingPercentage;

    [Header("For Checking Updates")]
    string gameName = "\\Build\\Build.exe";
    string versionLink = "https://drive.google.com/uc?export=download&id=1WHxJEZPBpdjLKimT3PiIvRl2FjD7MlYo";
    string buildLink = "https://github.com/DoRacOfficial/GameBuildForLauncher/raw/main/Build.zip";
    //string buildLink = "https://github.com/Shais24/NewGame/archive/refs/heads/main.zip";
    private string rootPath;
    private string versionFile;
    private string gameZip;
    private string gameExe;

    int DownloadedPercentage;

    private void Start()
    {
       
        progressBar.fillAmount = 0;
        browserBtn.SetActive(false);
        playBtn.SetActive(false);
        restartTxt.gameObject.SetActive(false);

        //PlayerPrefs.SetInt("IsFirst", 0);
        if (PlayerPrefs.GetString("StoreFolder", null) == "")
        {
            rootPath = Directory.GetCurrentDirectory();

            UnityEngine.Debug.Log("if rootPah: " + rootPath);
        }
        else
        {
            rootPath = PlayerPrefs.GetString("StoreFolder");
            UnityEngine.Debug.Log("else rootPah: " + rootPath);
        }
        //File.Delete(gameZip);


        versionFile = Path.Combine(rootPath, "Version.txt");
        gameZip = Path.Combine(rootPath, "Build.zip");
        gameExe = Path.Combine(rootPath, "Build", gameName);
        mainProgressBar.SetActive(false);

        UnityEngine.Debug.Log("test: "+ rootPath + "\\Build\\");
        if (!Directory.Exists(rootPath + "\\Build\\") /*&& PlayerPrefs.GetInt("IsFirst", 0) == 0*/)
        {
            PlayerPrefs.SetInt("IsFirst", 0);
            browserBtn.SetActive(true);

            if(File.Exists(rootPath + "\\Version.txt"))
            {
                File.Delete(rootPath + "\\Version.txt");
            }
            
            if(File.Exists(rootPath + "Version.txt"))
            {
                File.Delete(rootPath + "Version.txt");
            }
           
            FileLocaitonNotSet.gameObject.SetActive(true);

        }

        if (PlayerPrefs.GetInt("IsFirst") == 1 || (Directory.Exists(rootPath + "\\Build") && PlayerPrefs.GetInt("IsFirst", 0) == 0))
        {
            CheckForUpdates();
        }
        if (PlayerPrefs.GetInt("IsFirst", 0) == 0)
        {
            FileLocaitonNotSet.gameObject.SetActive(true);
        }
        else
        {
            FileLocaitonNotSet.gameObject.SetActive(false);
        }
    }



    LauncherStatus _status;
    internal LauncherStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case LauncherStatus.ready:
                    playBtn.SetActive(true);
                    restartTxt.gameObject.SetActive(false);
                    DownloadingStatus.text = "Play";

                    break;
                case LauncherStatus.failed:
                    DownloadingStatus.text = "Update Failed - Retry";
                    restartTxt.gameObject.SetActive(true);
                    restartTxt.text = "Update Failed - Retry";
                    playBtn.SetActive(true);
                    break;
                case LauncherStatus.downloadingGame:
                    playBtn.SetActive(false);
                    DownloadingStatus.text = "Downloading Game";
                    restartTxt.gameObject.SetActive(true);
                    restartTxt.text = "Downloading Game";

                    break;
                case LauncherStatus.downloadingUpdate:
                    playBtn.SetActive(false);
                    DownloadingStatus.text = "Downloading Update";
                    restartTxt.gameObject.SetActive(true);
                    restartTxt.text = "Downloading Update";
                    break;
                default:
                    break;
            }
        }
    }

    public object ShowDiaog { get; private set; }

    public void OpenFileBrowser()
    {
        FolderBrowserDialog dlg = new FolderBrowserDialog();
        //OpenFileDialog dlg = new OpenFileDialog();

        /*dlg.InitialDirectory = rootPath;
        dlg.ValidateNames = false;
        dlg.CheckFileExists = false;
        
        dlg.FileName = "Folder Selection.";

        PlayerPrefs.SetInt("IsFirst", 1);
         if (dlg.ShowDialog() == DialogResult.OK)
         {
             UnityEngine.Debug.Log("ok");
             //rootPath = dlg.SelectedPath;
             PlayerPrefs.SetString("StoreFolder", dlg.InitialDirectory);
             rootPath = PlayerPrefs.GetString("StoreFolder");
            PlayerPrefs.SetInt("IsFirst", 1);
             UnityEngine.Debug.Log(dlg.InitialDirectory);
            browserBtn.SetActive(false);
            
            restartTxt.gameObject.SetActive(true);
            restartTxt.text = "Restart Required";
        }
        else
         {
             UnityEngine.Debug.Log("cancel");
             if (PlayerPrefs.GetString("StoreFolder", null) == "")
             {
                 rootPath = Directory.GetCurrentDirectory();
                 UnityEngine.Debug.Log("if rootPah: " + rootPath);
                PlayerPrefs.SetInt("IsFirst", 1);
            }
             else
             {
                 rootPath = PlayerPrefs.GetString("StoreFolder");
                 UnityEngine.Debug.Log("else rootPah: " + rootPath);
                PlayerPrefs.SetInt("IsFirst", 1);
            }
         }*/

        dlg.SelectedPath = rootPath;

        UnityEngine.Debug.Log("RootFoolder: " + Environment.SpecialFolder.ApplicationData);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            FileLocaitonNotSet.gameObject.SetActive(false);

            PlayerPrefs.SetString("StoreFolder", dlg.SelectedPath);
            rootPath = PlayerPrefs.GetString("StoreFolder");

            if (!Directory.Exists(rootPath + "\\Build\\"))
            {
                Directory.CreateDirectory(rootPath + "\\Build\\");
            }

            UnityEngine.Debug.Log("ok");
            //rootPath = dlg.SelectedPath;
            PlayerPrefs.SetString("StoreFolder", dlg.SelectedPath);
            rootPath = PlayerPrefs.GetString("StoreFolder");

            PlayerPrefs.SetInt("IsFirst", 1);
            UnityEngine.Debug.Log(dlg.SelectedPath);
            browserBtn.SetActive(false);

            restartTxt.gameObject.SetActive(true);
            restartTxt.text = "Please CLOSE & ReOPEN LAUNCHER!";
        }
        else
        {
            UnityEngine.Debug.Log("cancel");
            if (PlayerPrefs.GetString("StoreFolder", null) == "")
            {
                rootPath = Directory.GetCurrentDirectory();
                UnityEngine.Debug.Log("if rootPah: " + rootPath);
                PlayerPrefs.SetInt("IsFirst", 1);
            }
            else
            {
                rootPath = PlayerPrefs.GetString("StoreFolder");
                UnityEngine.Debug.Log("else rootPah: " + rootPath);
                PlayerPrefs.SetInt("IsFirst", 1);
            }
        }
    }


    void CheckForUpdates()
    {
        if (File.Exists(versionFile))
        {
            Version localVersion = new Version(File.ReadAllText(versionFile));
            versionText.text = localVersion.ToString();

            try
            {
                WebClient webClient = new WebClient();
                Version onlineVersion = new Version(webClient.DownloadString(versionLink));

                if (onlineVersion.IsDifferentThan(localVersion))
                {
                    InstallGameFiles(true, onlineVersion);
                }
                else
                {
                    Status = LauncherStatus.ready;
                }
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                UnityEngine.Debug.Log($"Error checking for game updates: {ex}");
            }
        }
        else
        {
            InstallGameFiles(false, Version.zero);
        }

        
    }

    void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
    {
        try
        {
            WebClient webClient = new WebClient();
            if (_isUpdate)
            {
                Status = LauncherStatus.downloadingUpdate;
            }
            else
            {
                Status = LauncherStatus.downloadingGame;
                _onlineVersion = new Version(webClient.DownloadString(versionLink));
            }

            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
            webClient.DownloadFileAsync(new Uri(buildLink), gameZip, _onlineVersion);

            //[Get File Total Lenght
            WebClient fileSize = new WebClient();
            fileSize.OpenRead(buildLink);
            Int64 bytes_total = Convert.ToInt64(fileSize.ResponseHeaders["Content-Length"]);

            webClient.DownloadProgressChanged += (s, e) =>
            {
                int i = 0;
                if (Directory.Exists(rootPath + "\\VBuild") && i == 0)
                {
                    i++;
                    Directory.Delete(rootPath + "\\Build", true);
                    UnityEngine.Debug.Log("File Exits" + rootPath + "\\Build");
                }

                double totalMB = (bytes_total / 1000000);
                double recivedMB = (e.BytesReceived / 1000000);
                mainProgressBar.SetActive(true);


                UnityEngine.Debug.Log("Total Size: " + (totalMB));
                UnityEngine.Debug.Log("Progress " + recivedMB + " MB");
                //ProgressBar
                downloadingMB.text = recivedMB + " MB";
                totalDownloadSize.text = totalMB + " MB";
                DownloadingPercentage.text = (Math.Round((recivedMB / totalMB) * 100)).ToString() + "%";
                progressBar.fillAmount = (float)(recivedMB / totalMB);
            };
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            UnityEngine.Debug.Log($"Error installing game files: {ex}");
        }

        //yield return new WaitForSeconds(1f);
    }

    private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        try
        {
            string onlineVersion = ((Version)e.UserState).ToString();
            ZipFile.ExtractToDirectory(gameZip, rootPath);
            File.Delete(gameZip);

            File.WriteAllText(versionFile, onlineVersion);
            UnityEngine.Debug.Log("Dowload Completed..");
            versionText.text = onlineVersion;
            Status = LauncherStatus.ready;
            mainProgressBar.SetActive(false);
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            UnityEngine.Debug.Log($"Error finishing download: {ex}");
        }
    }


    public void PlayButton_Click()
    {
        gameExe = Path.Combine(rootPath, "Build", gameName);
        UnityEngine.Debug.Log("Game " + gameExe);
        if (File.Exists(rootPath + "\\" + gameExe) && Status == LauncherStatus.ready)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(rootPath + "\\" + gameExe);
            startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
            UnityEngine.Debug.Log("Play Btn: " + rootPath);
            Process.Start(startInfo);


        }
        else if (Status == LauncherStatus.failed)
        {
            CheckForUpdates();
        }
    }

    public void RestartGame()
    {
        if (restartTxt.text == "Restart Required")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }

    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }

    public void RestGame()
    {
        if(Directory.Exists(rootPath + "\\Build\\"))
        {
            Directory.Delete(rootPath + "\\Build\\");
            PlayerPrefs.SetInt("IsFirst", 1);

            UnityEngine.Debug.Log("Rest Successfully " + rootPath);
        }
    }
}
struct Version
{
    internal static Version zero = new Version(0, 0, 0);

    private short major;
    private short minor;
    private short subMinor;

    internal Version(short _major, short _minor, short _subMinor)
    {
        major = _major;
        minor = _minor;
        subMinor = _subMinor;
    }
    internal Version(string _version)
    {
        string[] versionStrings = _version.Split('.');
        if (versionStrings.Length != 3)
        {
            major = 0;
            minor = 0;
            subMinor = 0;
            return;
        }

        major = short.Parse(versionStrings[0]);
        minor = short.Parse(versionStrings[1]);
        subMinor = short.Parse(versionStrings[2]);
    }

    internal bool IsDifferentThan(Version _otherVersion)
    {
        if (major != _otherVersion.major)
        {
            return true;
        }
        else
        {
            if (minor != _otherVersion.minor)
            {
                return true;
            }
            else
            {
                if (subMinor != _otherVersion.subMinor)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override string ToString()
    {
        return $"{major}.{minor}.{subMinor}";
    }
}

