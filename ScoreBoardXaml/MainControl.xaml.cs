using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Diagnostics;

using DigitsViewLib;

namespace ScoreboardV2Xaml
{
	public partial class MainControl
	{

        private string theScoreboardUrl;
        private string theTimeclockUrl;
        private string theShotclockUrl;
        private long theAttentionTime = 5;
        Brush theShotclockStdColor;

        private ScoreboardRetriever theScoreboardRetriever;
        private TimeclockRetriever theTimeclockRetriever;
        private ShotclockRetriever theShotclockRetriever;

        public MainControl()
		{

            long time = 50; 
            
            this.InitializeComponent();

            Logger.Source = "Scoreboard";

            string baseFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string configFileName = baseFolder + "\\Scoreboard.config";
            string backgroundImageFile = "";
          //  string zeroSubstitutionImageFile = "";
            string substitutionInfo = "yes";
   

            try
            {
                Configuration myConfig = new Configuration(configFileName);

                theScoreboardUrl = myConfig["ScoreboardURL"];
                theTimeclockUrl = myConfig["TimeclockURL"];
                theShotclockUrl = myConfig["ShotclockURL"];
                time = Int64.Parse(myConfig["refreshTime"]);
                theAttentionTime = Int64.Parse(myConfig["attentionTime"]);
                backgroundImageFile = myConfig["backgroundImageFile"];
              //  zeroSubstitutionImageFile = myConfig["zeroSubstitutionImageFile"];
                substitutionInfo = myConfig["substitutionInfo"];
            }
            catch(Exception e)
            {
                Logger.Log("Could not read config file or property not present'" + configFileName + "': " + e.Message, EventLogEntryType.Error, Logger.Type.InitFailure);
            }

            if (theScoreboardUrl != "")
            {
                theScoreboardRetriever = new ScoreboardRetriever(this.Dispatcher, ScoreboardUpdate, time, theScoreboardUrl);
            }

            if (theTimeclockUrl != "")
            {
                theTimeclockRetriever = new TimeclockRetriever(this.Dispatcher, TimeclockUpdate, time, theTimeclockUrl);
            }

            if (theShotclockUrl != "")
            {
                theShotclockRetriever = new ShotclockRetriever(this.Dispatcher, ShotclockUpdate, time, theShotclockUrl);
            }

            if (backgroundImageFile == "")
            {
                backgroundImageFile = baseFolder + @"/Images/scoreboard.png";
            }


            // Initialise source
            InitImageSource(BGIMG, backgroundImageFile );
            
            BitmapImage mySource = new BitmapImage();
            theShotclockStdColor = SHOTCLOCK.Fill;
       
        }

        private void InitImageSource(Image aImageSource, string aPath)
        {
            BitmapImage mySource = new BitmapImage();

            mySource.BeginInit();
            mySource.UriSource = new Uri(aPath);
            mySource.EndInit();

            aImageSource.Source = mySource;

        }

        private void ScoreboardUpdate(ScoreboardResponse aResponse)
        {

            if ( aResponse != null && aResponse.Status == "OK" )
            {
                HOMESCORE.Text = String.Format("{0}", aResponse.Home);
                GUESTSCORE.Text = String.Format("{0}", aResponse.Guest);
            }
            else 
            {
                HOMESCORE.Text = "-";
                GUESTSCORE.Text = "-";
            }		
		}

        private void TimeclockUpdate(TimeclockResponse aResponse)
        {

            if (aResponse != null && aResponse.Status == "OK")
            {
                TIMEMINUTE.Text = String.Format("{0}", aResponse.Minute);
                TIMESECOND.Text = String.Format("{0:00}", aResponse.Second);
            }
            else
            {
                TIMEMINUTE.Text = "-";
                TIMESECOND.Text = "-";
            }
        }
        private void ShotclockUpdate(ShotclockResponse aResponse)
        {

            if (aResponse != null && aResponse.Status == "OK")
            {
                SHOTCLOCK.Text = String.Format("{0:00}", aResponse.Time);
                if (aResponse.Time <= theAttentionTime)
                {
                    SHOTCLOCK.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEBEBEB"));
                }
                else
                {
                    SHOTCLOCK.Fill = theShotclockStdColor;
                }
            }
            else
            { 
                SHOTCLOCK.Text = "-";
            }
        }
    }
}


//