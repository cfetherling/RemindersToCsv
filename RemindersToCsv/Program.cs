using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindersToCsv
{
  class Program
  {
    /// <summary>
    /// Parses the JSON returned from one of the endpoints when you open Reminders on www.icloud.com into CSV
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("2 args expected.");
        Console.WriteLine("RemindersToCsv SourceJsonFile DestinationCsvFile");
      }

      string raw;

      using (var sr = new StreamReader(args[0]))
      {
        raw = sr.ReadToEnd();
      }

      JObject data = JObject.Parse(raw);

      using (var sw = new StreamWriter(args[1], false))
      {
        sw.WriteLine("Title,CreateDate,AlarmDate");
        foreach (var reminder in data["Reminders"])
        {
          string reminderString = ParseReminder(reminder);
          sw.WriteLine(reminderString);
        }
        sw.Close();
      }
    }

    static string ParseReminder(JToken reminder)
    {
      string title = Csv.Escape(reminder["title"].Value<string>());
      DateTime createDate = ParseDate(reminder["createdDate"]);
      DateTime? alarmDate = ParseAlarm(reminder["alarms"]);

      return $"{title},{createDate},{alarmDate}";
    }

    static DateTime ParseDate(JToken date)
    {
      return new DateTime(date[1].Value<int>(), date[2].Value<int>(), date[3].Value<int>(), date[4].Value<int>(), date[5].Value<int>(), date[5].Value<int>());
    }

    static DateTime? ParseAlarm(JToken alarms)
    {
      JToken alarm = alarms.Count() == 0 ? null : alarms[0];

      if (alarm == null) return null;

      return ParseDate(alarm["onDate"]);
    }
  }
}
