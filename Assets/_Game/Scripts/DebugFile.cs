using UnityEngine;

// Author : Renaudin Matteo
public class DebugFile : MonoBehaviour
{
    private const string LOG_FILE_PATH = "LogFile.txt";
    private const string SEPARATION = " | ";

    private const string STACK_TRACE = "StackTrace : ";
    private const string LOG_TYPE = "Log Type : ";
    private const string MESSAGE = "Message : ";

    #region Singleton
    private static DebugFile _Instance = null;

    public static DebugFile GetInstance()
    {
        if (_Instance == null) _Instance = new DebugFile();
        return _Instance;
    }

    private DebugFile() : base() { }
    #endregion

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }
    private void OnEnable() => Application.logMessageReceived += SetLogFile;

    public void SetLogFile(string pMessage, string pStackTrace, LogType pErrorType)
    {
        System.IO.File.AppendAllText(LOG_FILE_PATH, STACK_TRACE + pStackTrace + SEPARATION +
                                                    LOG_TYPE + pErrorType + SEPARATION +
                                                    MESSAGE + pMessage + "\n");
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= SetLogFile;
        if (_Instance != null) _Instance = null;
    }
}