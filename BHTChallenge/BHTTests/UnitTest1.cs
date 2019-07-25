using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BHTChallenge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace BHTTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string json = "{'users': [{'id': 1,'name': 'Jack ONiell','created_at': '2019-06-04 20:40:26','active':'true'},		{'id': 2,'name': 'Daniel Jackson','created_at': '2019-06-19 18:19:33','active': 'true'}],'scores': [{'user_id': 5,	'score': 8},{'user_id': 1,'score': 6}]}";

            JObject jObj = JObject.Parse(json);

            JArray usersObj = (JArray)jObj["users"];
            BHTChallenge._Default.DeserializeUsers(usersObj);
            JArray scoresObj = (JArray)jObj["scores"];
            BHTChallenge._Default.DeserializeScores(scoresObj);

            var users = BHTChallenge._Default.studentUsers;
            var scores = BHTChallenge._Default.studentScores;

            Assert.IsNotNull(users[1]);
            Assert.IsNotNull(scores[1]);

        }
    }
}
