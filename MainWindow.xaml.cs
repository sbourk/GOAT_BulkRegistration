using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mshtml;

namespace EventBriteBulkRegistration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ATTENDEE = "attendee_";
        private const string IdTag = "id_attendee_";
        private const string FirstNameTag = "_first_name";
        private const string LastNameTag = "_last_name";
        private const string EmailTag = "_email_address";
        private const string JobTitleTag = "_job_title";
        private const string CompanyTag = "_company";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".png";
                dlg.Filter = "CSV Files (*.csv)|*.csv";
                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    var attendees = readFile(filename);

                    var htmldoc = webBrowser1.Document as mshtml.HTMLDocument;
                    int i = 1;
                    foreach (var attendee in attendees)
                    {
                        var attendeeIdentifier = IdTag + i;
                        if (htmldoc.getElementById(attendeeIdentifier + FirstNameTag) == null)
                            continue;

                        var prefix = ATTENDEE + i;

                        var o = new object[2];
                        o[0] = attendee.DietaryRestriction;
                        o[1] = prefix;
                        webBrowser1.InvokeScript("ProcessDietaryInformation", o);
                        webBrowser1.InvokeScript("ClickMyEmployer", prefix);
                        webBrowser1.InvokeScript("AgreeWaiver", prefix);

                        htmldoc.getElementById(attendeeIdentifier + FirstNameTag).innerText = attendee.FirstName;
                        htmldoc.getElementById(attendeeIdentifier + LastNameTag).innerText = attendee.LastName;
                        htmldoc.getElementById(attendeeIdentifier + EmailTag).innerText = attendee.Email;
                        htmldoc.getElementById(attendeeIdentifier + JobTitleTag).innerText = attendee.JobTitle;
                        htmldoc.getElementById(attendeeIdentifier + CompanyTag).innerText = attendee.Company;
                        
                        i++;
                    }

                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "";
                MessageBox.Show("Unable to read or process File:", ex.Message + "\n" + innerMessage);
            }
        }

        private List<Attendee> readFile(string filename)
        {
            var contents = File.ReadAllText(filename).Split('\n');
            var csv = from line in contents select line.Split(',').ToArray();
            List<Attendee> attendees = new List<Attendee>();
            foreach (var row in csv.Skip(1).TakeWhile(r => r.Length > 1))
            {
                var a = new Attendee
                {
                    FirstName = row[0],
                    LastName = row[1],
                    Email = row[2],
                    JobTitle = row[3],
                    Company = row[4],
                    DietaryRestriction = row[5]
                };
                
                attendees.Add(a);
            }
            return attendees;
        }

        private void WebBrowser1_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            loadJS( @"\bulk.js");
            loadJS( @"\jquery-3.0.0.min.js");
        }

        private void loadJS( string scriptName)
        {
            HTMLDocument htmldoc = webBrowser1.Document as mshtml.HTMLDocument;

            var header = htmldoc.getElementsByTagName("head").Cast<HTMLHeadElement>().First();

            var script = (IHTMLScriptElement) htmldoc.createElement("script");
            script.type = "text/javascript";
            script.text = System.IO.File.ReadAllText(Environment.CurrentDirectory + scriptName);
            header.appendChild((IHTMLDOMNode) script);
        }
    }

    class Attendee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string DietaryRestriction { get; set; }
    }
}


