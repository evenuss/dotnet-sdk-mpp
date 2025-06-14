﻿using System;
using System.IO;
using MPXJ;
using MPXJ.Net;
using MpxjTask = MPXJ.Net.Task;


class Program
{
    static void Main(string[] args)
    {
        if(args.Length < 1)
        {
            Console.WriteLine("Usage: dotnet run <file.mpp>");
            return;
        }
        string mppFile = args[0];
        try
        {
            using var stream = new FileStream(mppFile, FileMode.Open, FileAccess.Read);
            var reader = new UniversalProjectReader();
            var projectFile = reader.Read(stream);
            var tasks = projectFile.Tasks
                .Where(t => !string.IsNullOrEmpty(t.Name))
                .Select(t =>
                {
                    var assignedTo = t.ResourceAssignments?.FirstOrDefault()?.EffectiveCalendar?.Name ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(assignedTo))
                    {
                        return null; // Skip if AssignedTo is empty
                    }

                    var duration = 0;
                    if (t.Finish.HasValue && t.Start.HasValue)
                    {
                        duration = (int)(t.Finish.Value.Date - t.Start.Value.Date).TotalDays + 1;
                    }

                    return new
                    {
                        Name = t.Name,
                        Start = t.Start?.ToString("yyyy-MM-ddTHH:mm:ss:msZ") ?? string.Empty,
                        Finish = t.Finish?.ToString("yyyy-MM-ddTHH:mm:ss:msZ") ?? string.Empty,
                        AssignedTo = assignedTo,
                        AssignedBy = t.OutlineLevel == 1 ? "Project Manager" : "Supervisor",
                        DurationInDays = duration.ToString()
                    };

                })
                .Where(t => t != null)
                .ToList();

            var totalManDaysPerResource = tasks
                .GroupBy(t => t.AssignedTo)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => int.TryParse(t.DurationInDays, out var d) ? d : 0)
                );

            var totalManDayAll = totalManDaysPerResource.Values.Sum();

            var totalManDaysList = totalManDaysPerResource
                .Select(kvp => new { email = kvp.Key, TotalManDays = kvp.Value })
                .ToList();

            var response = new
            {
                FileName = Path.GetFileName(mppFile),
                ProjectName = projectFile.ProjectProperties?.ProjectTitle,
                ProjectStart = projectFile.ProjectProperties?.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                ProjectFinish = projectFile.ProjectProperties?.FinishDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                Tasks = tasks!,
                TotalManDaysPerResource = totalManDaysList,
                TotalManDayAll = totalManDayAll
            };
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(response, options));

           
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error reading MPP file: {ex.Message}");
        }
    }
}
