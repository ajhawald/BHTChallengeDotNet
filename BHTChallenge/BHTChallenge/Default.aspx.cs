using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BHTChallenge
{
    public partial class _Default : Page
    {
        //This is the final data list.
        public static List<DisplayUser> displayUsers = new List<DisplayUser>();

        protected void Page_Load(object sender, EventArgs e)
        {
            displayUsers.Clear();

            //Lists for aggregating data
            List<User> studentUsers = new List<User>();
            List<Score> studentScores = new List<Score>();

            //Read in json file.
            string pathToJson = ConfigurationSettings.AppSettings["jsonFilePath"];
            using (StreamReader r = new StreamReader(pathToJson))
            {
                string json = r.ReadToEnd();
                JObject studentObj = JObject.Parse(json);

                //TODO: use linqToJson instead of loops.
                foreach (var topLevelNode in studentObj.Children())
                {
                    var collection = topLevelNode.Children();
                    foreach (var dataNode in collection)
                    {
                        foreach (var dataItem in dataNode.Children())
                        {
                            JObject studentObj3 = JObject.Parse(dataItem.ToString());
                            string userJason = dataItem.ToString();
                            var name = studentObj3["name"];
                            
                            if (name != null)
                            {
                                bool isActive = (bool)studentObj3["active"];
                                if (isActive)
                                {
                                    User usr = new JavaScriptSerializer().Deserialize<User>(userJason);
                                    studentUsers.Add(usr);
                                } 
                            }
                            else //this is a score
                            {
                                Score s = new JavaScriptSerializer().Deserialize<Score>(userJason);
                                studentScores.Add(s);
                            }
                        }
                    }
                }

            }
            AssembleDisplayUsers(studentUsers, studentScores);
            GridView1.DataSource = ConvertToDatatable(displayUsers);
            GridView1.Attributes.Add("bordercolor", "808080");
            GridView1.DataBind();          
        }

        protected void GridView1_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //add thead for clientside table sorting.
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }
            //hide html on list of scores.
            string htmlColumn = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = htmlColumn;
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
                var intScores = scores.Where(s => s.user_id == User.id).Select(s => s.score).ToList();
                du.scoreList = intScores;
                du.scoreListString = convertScoresToHtmlList(intScores);
                du.scoreAverage = System.Math.Round(du.scoreList.Average(), 2); 
                displayUsers.Add(du);
            }
        }

        //to have the scores in one cell listed vertically.
        private static string convertScoresToHtmlList(List<int> scores)
        {
            string scoreList = string.Empty;
            foreach (var item in scores)
            {
                scoreList += item.ToString() + "</br>";
            }
            return scoreList;
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

        public class Score
        {
            [JsonProperty("user_id")]
            public int user_id;

            [JsonProperty("score")]
            public int score;

        }

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