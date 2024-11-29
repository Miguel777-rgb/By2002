using System.Data;
using Mono.Data.SQLite;
using System.Data.SQLite;
using UnityEngine;
using TMPro; // Necesario para trabajar con TMP_InputField y TMP_Text
using UnityEngine.UI; // Necesario para trabajar con Button
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager _instance;
    private SQLiteConnection connection;
    private string databasePath;

    // Referencias a los TMP_InputFields y el botón para enviar el formulario
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button submitButton;

    // Referencia al TMP_Text de la UI
    public TMP_Text debugText;

    public static DatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("DatabaseManagerRoot");
                _instance = singletonObject.AddComponent<DatabaseManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        databasePath = Application.persistentDataPath + "/userby2002.db"; // Ruta a la base de datos
        CreateDatabase();
        ConnectToDatabase();

        // Verificación de asignación de submitButton
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmit);
        }
        else
        {
            ShowMessage("SubmitButton no está asignado.");
        }
    }

    void CreateDatabase()
    {
        if (!System.IO.File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
            Debug.Log("Base de datos creada en: " + databasePath);
        }
    }

    void ConnectToDatabase()
    {
        string connectionString = "Data Source=" + databasePath + ";Version=3;";
        connection = new SQLiteConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Conexión exitosa a SQLite");

            string createTableQuery = "CREATE TABLE IF NOT EXISTS usuarios_by2002 (" +
                                      "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "username TEXT NOT NULL, " +
                                      "password TEXT NOT NULL)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }
        catch (SQLiteException ex)
        {
            Debug.LogError($"Error de conexión o creación de tabla: {ex.Message}");
        }
    }

    // Método que se ejecuta cuando se hace clic en el botón de enviar
    public void OnSubmit()
    {
        string pass = passwordInput.text;
        string username = usernameInput.text;

        if (string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(username))
        {
            ShowMessage("Todos los campos deben ser llenados.");
            return;
        }

        InsertData(pass, username);
    }

    // Método para insertar datos en la base de datos
    public void InsertData(string password, string username)
    {
        string hashedPassword = HashPassword(password);
        string query = "INSERT INTO usuarios_by2002 (password, username) VALUES (@password, @username)";
        SQLiteCommand cmd = new SQLiteCommand(query, connection);
        cmd.Parameters.AddWithValue("@password", hashedPassword);
        cmd.Parameters.AddWithValue("@username", username);

        try
        {
            cmd.ExecuteNonQuery();
            ShowMessage("Logeo exitoso");
        }
        catch (SQLiteException ex)
        {
            ShowMessage($"Error al insertar datos: {ex.Message}");
        }
    }

    // Método para recuperar datos (si lo necesitas en algún momento)
    public void RetrieveData()
    {
        string query = "SELECT * FROM usuarios_by2002";
        SQLiteCommand cmd = new SQLiteCommand(query, connection);

        try
        {
            SQLiteDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                ShowMessage($"Password: {dataReader["password"]}, Username: {dataReader["username"]}");
            }
            dataReader.Close();
        }
        catch (SQLiteException ex)
        {
            ShowMessage($"Error al recuperar datos: {ex.Message}");
        }
    }

    // Método para hashear la contraseña
    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Cerrar conexión al salir de la aplicación
    void OnApplicationQuit()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            ShowMessage("Conexión cerrada");
        }
    }

    // Método para mostrar mensajes en el TMP_Text de la UI
    private void ShowMessage(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;
        }
        else
        {
            Debug.LogWarning("debugText no está asignado en el Inspector.");
        }
    }
}
