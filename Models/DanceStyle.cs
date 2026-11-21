using System.Collections.Generic;

namespace DanceLearning.Models
{
    /// <summary>Represents a dance style/course in the catalog.</summary>
    public class DanceStyle
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int LessonsCount { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string AccentColor { get; set; } = "#6C3CE1";
        public List<Lesson> Lessons { get; set; } = new();
    }

    /// <summary>Represents a single lesson inside a dance style.</summary>
    public class Lesson
    {
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoPath { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public List<DanceStep> Steps { get; set; } = new();
    }

    /// <summary>A single step/move within a lesson.</summary>
    public class DanceStep
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    /// <summary>Tracks weekly training schedule.</summary>
    public class TrainingDay
    {
        public string DayName { get; set; } = string.Empty;
        public bool IsScheduled { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>User profile and gamification data.</summary>
    public class UserProfile
    {
        public string Name { get; set; } = "Dancer";
        public int TotalPoints { get; set; }
        public int LessonsCompleted { get; set; }
        public int BundlesLearned { get; set; }
        public int TrainingStreak { get; set; }
        public double OverallProgress { get; set; }
        public List<DanceStyle> PlaylistStyles { get; set; } = new();
        public List<TrainingDay> WeekSchedule { get; set; } = new();

        public UserProfile()
        {
            WeekSchedule = new List<TrainingDay>
            {
                new() { DayName = "Пн" }, new() { DayName = "Вт" },
                new() { DayName = "Ср" }, new() { DayName = "Чт" },
                new() { DayName = "Пт" }, new() { DayName = "Сб" },
                new() { DayName = "Вс" }
            };
        }
    }
}
