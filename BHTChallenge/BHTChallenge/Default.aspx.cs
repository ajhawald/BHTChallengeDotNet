using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BHTChallenge
{
    public partial class _Default : Page
    {
        //This is the final data list.
        public static List<DisplayUser> displayUsers = new List<DisplayUser>();
        public static List<User> studentUsers = new List<User>();
        public static List<Score> studentScores = new List<Score>();

        public void Page_Load(object sender, EventArgs e)
        {
            displayUsers.Clear();

            //Read in json file.
            //THIS PATH MUST BE UPDATED IN WEB.CONFIG TO RUN ON DIFFERENT MACHINES!
            string pathToJson = ConfigurationSettings.AppSettings["jsonFilePath"];
            using (StreamReader r = new StreamReader(pathToJson))
            {
                try
                {
                    string json = r.ReadToEnd();
                    JObject jObj = JObject.Parse(json);

                    JArray usersObj = (JArray)jObj["users"];
                    DeserializeUsers(usersObj);
                    JArray scoresObj = (JArray)jObj["scores"];
                    DeserializeScores(scoresObj);
                }
                catch (Exception)
                {
                    throw new FileNotFoundException("could not find json file");
                }               
            }

            AssembleDisplayUsers(studentUsers, studentScores);
            GridView1.DataSource = ConvertToDatatable(displayUsers);
            GridView1.Attributes.Add("bordercolor", "808080");
            GridView1.DataBind();
        }

        public static void DeserializeScores(JArray scoresObj)
        {
            studentScores.Clear();
            foreach (var score in scoresObj)
            {
                JObject scoreObj = new JObject();
                try
                {
                    scoreObj = JObject.Parse(score.ToString());
                }
                catch (Exception)
                {
                    //TODO set up exception handling methods
                    //throw;
                }
                Score scr = JsonConvert.DeserializeObject<Score>(scoreObj.ToString());
                studentScores.Add(scr);
            }
        }

        public static void DeserializeUsers(JArray usersObj)
        {
            studentUsers.Clear();
            foreach (var user in usersObj)
            {
                JObject userObj = new JObject();
                try
                {
                    userObj = JObject.Parse(user.ToString());
                }
                catch (Exception)
                {
                    //TODO set up exception handling methods
                    //throw;
                }
                User usr = JsonConvert.DeserializeObject<User>(userObj.ToString());
                studentUsers.Add(usr);
            }
        }

        protected void GridView1_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //add thead for clientside table sorting.
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }
        }

        //this takes all the data converts it to the final list of display objects.
        private static void AssembleDisplayUsers(List<User> users, List<Score> scores)
        {
            foreach (var User in users)
            {
                DisplayUser du = new DisplayUser();
                du.id = User.id;
                du.name = User.name;
                du.created = User.created_at;
                du.scoreList = scores.Where(s => s.user_id == User.id).Select(s => s.score).ToList();
                du.scoreListString = string.Join(",", du.scoreList.Select(n => n.ToString()).ToArray());
                du.scoreAverage = System.Math.Round(du.scoreList.Average(), 2);
                displayUsers.Add(du);
            }
        }

        //create dt for gridview.
        static DataTable ConvertToDatatable(List<DisplayUser> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Created");
            dt.Columns.Add("Scores");
            dt.Columns.Add("Avg");
            foreach (var item in list)
            {
                var row = dt.NewRow();
                row["ID"] = item.id;
                row["Name"] = item.name;
                row["Created"] = item.created;
                row["Scores"] = item.scoreListString;
                row["Avg"] = item.scoreAverage;
                dt.Rows.Add(row);
            }
            return dt;
        }


        //OBJECTS
        [Serializable]
        public class Score
        {
            [JsonProperty("user_id")]
            public int user_id;

            [JsonProperty("score")]
            public int score;

        }

        [Serializable]
        public class User
        {
            [JsonProperty("id")]
            public int id;

            [JsonProperty("name")]
            public string name;

            [JsonProperty("created_at")]
            public DateTime created_at;

            [JsonProperty("active")]
            public bool active;
        }

        //These will be displayed in the grid
        public class DisplayUser
        {
            public int id;
            public string name;
            public DateTime created;
            public List<int> scoreList;
            public string scoreListString;
            public double scoreAverage;

        }
    }
}