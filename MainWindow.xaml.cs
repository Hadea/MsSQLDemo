using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient; // Wird über das Package "System.Data.SqlClient" bereitgestellt.
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MsSQLDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<string> ResponseList { get; set; } // Liste welche die Daten vom Datenbankserver zwischenspeichern soll

        public MainWindow()
        {
            InitializeComponent();
            ResponseList = new ObservableCollection<string>(); // Neue Liste kreieren
            lvResponse.ItemsSource = ResponseList; // Liste an das GUI Element binden sodass aktualisierungen (Add, Delete, Clear, ...) automatisch auch im GUI sichtbar sind
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(); // Eine Hilfsklasse welche uns beim erstellen eines connectionstrings unterstützt.
            builder.UserID = "NorthwindUser"; //Nutzer des Datenbankservers, hier einen Nutzer eintragen welcher möglichst wenig Rechte hat. Idealerweise nur Prozeduren ausführen.
            builder.Password = "NorthwindPass"; //Passwort zum Datenbankserver
            builder.InitialCatalog = "Northwind"; // Datenbank zu der sich standardmässig verbunden wird
            builder.DataSource = "localhost"; // Server IP und Port auf dem sich die Datenbank befindet. Die Trennung erfolgt mit einem Komma und nicht wie gewohnt mit einem Doppelpunkt.

            using (SqlConnection sqlConnection = new SqlConnection(builder.ToString())) // Bereitet die Verbindung zum Server anhand des connectionstrings vor
            {
                sqlConnection.Open(); // öffnet die Verbindung zum Datenbankserver
                SqlCommand sqlCommand = sqlConnection.CreateCommand(); // erstellt eine neues Kommando passend zu der Verbindung
                sqlCommand.CommandText = "select LastName from Employees;"; //Der SQL-Befehl. Nur zu demozwecken. Idealerweise nur Aufrufe von Prozeduren.

                using (SqlDataReader sqlResponse = sqlCommand.ExecuteReader()) //Befehl an SQL-Server senden und die Antwort in einem Reader ablegen
                {
                    ResponseList.Clear(); //Liste, welche die daten aufnehmen soll, wird geleert
                    while (sqlResponse.Read()) // solange noch Zeilen in der Rückgabe vom SQL-Server stehen wird gelesen
                    {
                        ResponseList.Add(sqlResponse.GetString(0)); // Liesst einen Text aus der Spalte 0 der Rückgabe
                    }
                }

            }
        }

        /// <summary>
        /// Eventhandler welcher einen übergebenen URI im entsprechenden Standardprogramm öffnet.
        /// </summary>
        /// <param name="sender">Hyperlink Objekt welches die Zieladresse enthält</param>
        /// <param name="e">Parameter des Navigations-Events</param>
        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try // Da der Prozessstart durchaus fehlschlagen kann werden mögliche Fehler abgefangen.
            {
                Process.Start(new ProcessStartInfo { FileName = ((Hyperlink)sender).NavigateUri.ToString(), UseShellExecute = true }); // konvertiert die URI aus dem Hyperlink in einen Text und sendet ihn an die Shell. Diese wiederum erkennt das https und startet daraufhin den eingestellten Standardbrowser.
                e.Handled = true;
            }
            catch (Exception ex) // Pokemon Exception handling, "gonna catch 'em all". Bietet sich an wenn Fehler das eigendliche Programm nicht beeinflussen.
            {
                //falls etwas schief geht (z.B. Datei nicht gefunden) dann wird eine Nachricht an den Nutzer geschickt in welcher der Link auch nochmal nachlesbar ist.
                MessageBox.Show($" {ex.Message} \n {e.Uri.ToString()} ", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

